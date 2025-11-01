// Copyright (c) 2015-2021, bacondither
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions
// are met:
// 1. Redistributions of source code must retain the above copyright
//    notice, this list of conditions and the following disclaimer
//    in this position and unchanged.
// 2. Redistributions in binary form must reproduce the above copyright
//    notice, this list of conditions and the following disclaimer in the
//    documentation and/or other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE AUTHORS ``AS IS'' AND ANY EXPRESS OR
// IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
// OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
// IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
// INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
// NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

// Adaptive sharpen - version 2021-10-17
// Tuned for use post-resize

//!HOOK OUTPUT
//!BIND HOOKED
//!DESC adaptive-sharpen

//--------------------------------------- Settings ------------------------------------------------

#define curve_height    0.5                  // Main control of sharpening strength [>0]
                                             // 0.3 <-> 2.0 is a reasonable range of values

#define overshoot_ctrl  false                // Allow for higher overshoot if the current edge pixel
                                             // is surrounded by similar edge pixels

// Defined values under this row are "optimal" DO NOT CHANGE IF YOU DO NOT KNOW WHAT YOU ARE DOING!

#define curveslope      0.5                  // Sharpening curve slope, high edge values

#define L_compr_low     0.167                // Light compression, default (0.167=~6x)
#define L_compr_high    0.334                // Light compression, surrounded by edges (0.334=~3x)

#define D_compr_low     0.250                // Dark compression, default (0.250=4x)
#define D_compr_high    0.500                // Dark compression, surrounded by edges (0.500=2x)

#define scale_lim       0.1                  // Abs max change before compression [>0.01]
#define scale_cs        0.056                // Compression slope above scale_lim

#define pm_p            1.0                  // Power mean p-value [>0-1.0]
//-------------------------------------------------------------------------------------------------

#define max4(a,b,c,d)  ( max(max(a, b), max(c, d)) )

// Soft if, fast linear approx
#define soft_if(a,b,c) ( sat((a + b + c + 0.056/2.5)/(maxedge + 0.03/2.5) - 0.85) )

// Soft limit, modified tanh approx
#define soft_lim(v,s)  ( sat(abs(v/s)*(27.0 + pow(v/s, 2.0))/(27.0 + 9.0*pow(v/s, 2.0)))*s )

// Weighted power mean
#define wpmean(a,b,w)  ( pow(w*pow(abs(a), pm_p) + abs(1.0-w)*pow(abs(b), pm_p), (1.0/pm_p)) )

// Get destination pixel values
#define get(x,y)       ( HOOKED_texOff(vec2(x, y)).rgb )
#define sat(x)         ( clamp(x, 0.0, 1.0) )
#define dxdy(val)      ( length(fwidth(val)) ) // =~1/2.5 hq edge without c_comp

#ifdef LUMA_tex
#define CtL(RGB)       RGB.x
#else
#define CtL(RGB)       ( sqrt(dot(sat(RGB)*sat(RGB), vec3(0.2126, 0.7152, 0.0722))) )
#endif

#define b_diff(pix)    ( (blur-luma[pix])*(blur-luma[pix]) )

vec4 hook() {

    // [                c22               ]
    // [           c24, c9,  c23          ]
    // [      c21, c1,  c2,  c3, c18      ]
    // [ c19, c10, c4,  c0,  c5, c11, c16 ]
    // [      c20, c6,  c7,  c8, c17      ]
    // [           c15, c12, c14          ]
    // [                c13               ]
    vec3 c[25] = vec3[](get( 0, 0), get(-1,-1), get( 0,-1), get( 1,-1), get(-1, 0),
                        get( 1, 0), get(-1, 1), get( 0, 1), get( 1, 1), get( 0,-2),
                        get(-2, 0), get( 2, 0), get( 0, 2), get( 0, 3), get( 1, 2),
                        get(-1, 2), get( 3, 0), get( 2, 1), get( 2,-1), get(-3, 0),
                        get(-2, 1), get(-2,-1), get( 0,-3), get( 1,-2), get(-1,-2));

    float e[13] = float[](dxdy(c[0]),  dxdy(c[1]),  dxdy(c[2]),  dxdy(c[3]),  dxdy(c[4]),
                          dxdy(c[5]),  dxdy(c[6]),  dxdy(c[7]),  dxdy(c[8]),  dxdy(c[9]),
                          dxdy(c[10]), dxdy(c[11]), dxdy(c[12]));

    // RGB to luma
    float luma[25] = float[](CtL(c[0]), CtL(c[1]), CtL(c[2]), CtL(c[3]), CtL(c[4]), CtL(c[5]), CtL(c[6]),
                             CtL(c[7]),  CtL(c[8]),  CtL(c[9]),  CtL(c[10]), CtL(c[11]), CtL(c[12]),
                             CtL(c[13]), CtL(c[14]), CtL(c[15]), CtL(c[16]), CtL(c[17]), CtL(c[18]),
                             CtL(c[19]), CtL(c[20]), CtL(c[21]), CtL(c[22]), CtL(c[23]), CtL(c[24]));

    float c0_Y = luma[0];

    // Blur, gauss 3x3
    float  blur   = (2.0 * (luma[2]+luma[4]+luma[5]+luma[7]) + (luma[1]+luma[3]+luma[6]+luma[8]) + 4.0 * luma[0]) / 16.0;

    // Contrast compression, center = 0.5
    float c_comp = sat(0.266666681f + 0.9*exp2(blur * blur * -7.4));

    // Edge detection
    // Relative matrix weights
    // [          1          ]
    // [      4,  5,  4      ]
    // [  1,  5,  6,  5,  1  ]
    // [      4,  5,  4      ]
    // [          1          ]
    float edge = ( 1.38*b_diff(0)
                 + 1.15*(b_diff(2) + b_diff(4) + b_diff(5) + b_diff(7))
                 + 0.92*(b_diff(1) + b_diff(3) + b_diff(6) + b_diff(8))
                 + 0.23*(b_diff(9) + b_diff(10) + b_diff(11) + b_diff(12)) ) * c_comp;

    vec2 cs = vec2(L_compr_low,  D_compr_low);

    if (overshoot_ctrl) {
        float maxedge = max4( max4(e[1],e[2],e[3],e[4]), max4(e[5],e[6],e[7],e[8]),
                              max4(e[9],e[10],e[11],e[12]), e[0] );

        // [          x          ]
        // [       z, x, w       ]
        // [    z, z, x, w, w    ]
        // [ y, y, y, 0, y, y, y ]
        // [    w, w, x, z, z    ]
        // [       w, x, z       ]
        // [          x          ]
        float sbe = soft_if(e[2],e[9], dxdy(c[22]))*soft_if(e[7],e[12],dxdy(c[13]))  // x dir
                  + soft_if(e[4],e[10],dxdy(c[19]))*soft_if(e[5],e[11],dxdy(c[16]))  // y dir
                  + soft_if(e[1],dxdy(c[24]),dxdy(c[21]))*soft_if(e[8],dxdy(c[14]),dxdy(c[17]))  // z dir
                  + soft_if(e[3],dxdy(c[23]),dxdy(c[18]))*soft_if(e[6],dxdy(c[20]),dxdy(c[15])); // w dir

        cs = mix(cs, vec2(L_compr_high, D_compr_high), sat(2.4002*sbe - 2.282));
    }

    // Precalculated default squared kernel weights
    const vec3 w1 = vec3(0.5,           1.0, 1.41421356237); // 0.25, 1.0, 2.0
    const vec3 w2 = vec3(0.86602540378, 1.0, 0.54772255751); // 0.75, 1.0, 0.3

    // Transition to a concave kernel if the center edge val is above thr
    vec3 dW = pow(mix( w1, w2, sat(2.4*edge - 0.82)), vec3(2.0));

    // Use lower weights for pixels in a more active area relative to center pixel area
    // This results in narrower and less visible overshoots around sharp edges
    float modif_e0 = 3.0 * e[0] + 0.02/2.5;

    float weights[12]  = float[](( min(modif_e0/e[1],  dW.y) ),
                                 ( dW.x ),
                                 ( min(modif_e0/e[3],  dW.y) ),
                                 ( dW.x ),
                                 ( dW.x ),
                                 ( min(modif_e0/e[6],  dW.y) ),
                                 ( dW.x ),
                                 ( min(modif_e0/e[8],  dW.y) ),
                                 ( min(modif_e0/e[9],  dW.z) ),
                                 ( min(modif_e0/e[10], dW.z) ),
                                 ( min(modif_e0/e[11], dW.z) ),
                                 ( min(modif_e0/e[12], dW.z) ));

    weights[0] = (max(max((weights[8]  + weights[9])/4.0,  weights[0]), 0.25) + weights[0])/2.0;
    weights[2] = (max(max((weights[8]  + weights[10])/4.0, weights[2]), 0.25) + weights[2])/2.0;
    weights[5] = (max(max((weights[9]  + weights[11])/4.0, weights[5]), 0.25) + weights[5])/2.0;
    weights[7] = (max(max((weights[10] + weights[11])/4.0, weights[7]), 0.25) + weights[7])/2.0;

    // Calculate the negative part of the laplace kernel and the low threshold weight
    float lowthrsum   = 0.0;
    float weightsum   = 0.0;
    float neg_laplace = 0.0;

    for (int pix = 0; pix < 12; ++pix)
    {
        float lowthr = sat((20.*4.5*c_comp*e[pix + 1] - 0.221));

        neg_laplace += luma[pix+1] * luma[pix+1] * weights[pix] * lowthr;
        weightsum   += weights[pix] * lowthr;
        lowthrsum   += lowthr / 12.0;
    }

    neg_laplace = sqrt(neg_laplace / weightsum);

    // Compute sharpening magnitude function
    float sharpen_val = curve_height/(curve_height*curveslope*edge + 0.625);

    // Calculate sharpening diff and scale
    float sharpdiff = (c0_Y - neg_laplace)*(lowthrsum*sharpen_val + 0.01);

    // Calculate local near min & max, partial sort
    float temp;

    for (int i1 = 0; i1 < 24; i1 += 2)
    {
        temp = luma[i1];
        luma[i1]   = min(luma[i1], luma[i1+1]);
        luma[i1+1] = max(temp, luma[i1+1]);
    }

    for (int i2 = 24; i2 > 0; i2 -= 2)
    {
        temp = luma[0];
        luma[0]    = min(luma[0], luma[i2]);
        luma[i2]   = max(temp, luma[i2]);

        temp = luma[24];
        luma[24] = max(luma[24], luma[i2-1]);
        luma[i2-1] = min(temp, luma[i2-1]);
    }

    float min_dist  = min(abs(luma[24] - c0_Y), abs(c0_Y - luma[0]));
    min_dist = min(min_dist, scale_lim*(1.0 - scale_cs) + min_dist*scale_cs);

    // Soft limited anti-ringing with tanh, wpmean to control compression slope
    sharpdiff = wpmean(max(sharpdiff, 0.0), soft_lim( max(sharpdiff, 0.0), min_dist ), cs.x )
              - wpmean(min(sharpdiff, 0.0), soft_lim( min(sharpdiff, 0.0), min_dist ), cs.y );
    
    float sharpdiff_lim = sat(c0_Y + sharpdiff) - c0_Y;
    /*float satmul = (c0_Y + max(sharpdiff_lim*0.9, sharpdiff_lim)*1.03 + 0.03)/(c0_Y + 0.03);
    vec3 res = c0_Y + sharpdiff_lim + (c[0] - c0_Y)*satmul;
    */
    return vec4(sharpdiff_lim + c[0], HOOKED_texOff(0).a);
}
