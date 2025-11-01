// SSimSuperRes by Shiandow
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3.0 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library.

//!HOOK POSTKERNEL
//!BIND HOOKED
//!SAVE LOWRES
//!HEIGHT NATIVE_CROPPED.h
//!WHEN NATIVE_CROPPED.h OUTPUT.h <
//!COMPONENTS 4
//!DESC SSSR Downscaling I

#define axis        1

#define offset      vec2(0,0)

#define MN(B,C,x)   (x < 1.0 ? ((2.-1.5*B-(C))*x + (-3.+2.*B+C))*x*x + (1.-(B)/3.) : (((-(B)/6.-(C))*x + (B+5.*C))*x + (-2.*B-8.*C))*x+((4./3.)*B+4.*C))
#define Kernel(x)   MN(0.334, 0.333, abs(x))
#define taps        2.0

#define Luma(rgb)   dot(rgb*rgb, vec3(0.2126, 0.7152, 0.0722))

vec4 hook() {
    float low  = ceil((HOOKED_pos - taps/input_size) * HOOKED_size - offset - 0.5)[axis];
    float high = floor((HOOKED_pos + taps/input_size) * HOOKED_size - offset - 0.5)[axis];

    float W = 0.0;
    vec4 avg = vec4(0);
    vec2 pos = HOOKED_pos;
    vec4 tex;

    for (float k = low; k <= high; k++) {
        pos[axis] = HOOKED_pt[axis] * (k - offset[axis] + 0.5);
        float rel = (pos[axis] - HOOKED_pos[axis])*input_size[axis];
        float w = Kernel(rel);

        tex.rgb = textureLod(HOOKED_raw, pos, 0.0).rgb * HOOKED_mul;
        tex.a = Luma(tex.rgb);
        avg += w * tex;
        W += w;
    }
    avg /= W;

    return vec4(avg.rgb, max(abs(avg.a - Luma(avg.rgb)), 5e-7));
}

//!HOOK POSTKERNEL
//!BIND LOWRES
//!SAVE LOWRES
//!WIDTH NATIVE_CROPPED.w
//!HEIGHT NATIVE_CROPPED.h
//!WHEN NATIVE_CROPPED.w OUTPUT.w <
//!COMPONENTS 4
//!DESC SSSR Downscaling II

#define axis        0

#define offset      vec2(0,0)

#define MN(B,C,x)   (x < 1.0 ? ((2.-1.5*B-(C))*x + (-3.+2.*B+C))*x*x + (1.-(B)/3.) : (((-(B)/6.-(C))*x + (B+5.*C))*x + (-2.*B-8.*C))*x+((4./3.)*B+4.*C))
#define Kernel(x)   MN(0.334, 0.333, abs(x))
#define taps        2.0

#define Luma(rgb)   dot(rgb*rgb, vec3(0.2126, 0.7152, 0.0722))

vec4 hook() {
    float low  = ceil((LOWRES_pos - taps/input_size) * LOWRES_size - offset - 0.5)[axis];
    float high = floor((LOWRES_pos + taps/input_size) * LOWRES_size - offset - 0.5)[axis];

    float W = 0.0;
    vec4 avg = vec4(0);
    vec2 pos = LOWRES_pos;
    vec4 tex;

    for (float k = low; k <= high; k++) {
        pos[axis] = LOWRES_pt[axis] * (k - offset[axis] + 0.5);
        float rel = (pos[axis] - LOWRES_pos[axis])*input_size[axis];
        float w = Kernel(rel);

        tex.rgb = textureLod(LOWRES_raw, pos, 0.0).rgb * LOWRES_mul;
        tex.a = Luma(tex.rgb);
        avg += w * tex;
        W += w;
    }
    avg /= W;

    return vec4(avg.rgb, max(abs(avg.a - Luma(avg.rgb)), 5e-7) + LOWRES_texOff(0).a);
}

//!HOOK POSTKERNEL
//!BIND PREKERNEL
//!BIND LOWRES
//!SAVE var
//!WIDTH NATIVE_CROPPED.w
//!HEIGHT NATIVE_CROPPED.h
//!WHEN NATIVE_CROPPED.h OUTPUT.h <
//!COMPONENTS 2
//!DESC SSSR var

#define spread      1.0 / 4.0

#define GetL(x,y)   PREKERNEL_tex(PREKERNEL_pt * (PREKERNEL_pos * input_size + tex_offset + vec2(x,y))).rgb
#define GetH(x,y)   LOWRES_texOff(vec2(x,y)).rgb

#define Luma(rgb)   dot(rgb*rgb, vec3(0.2126, 0.7152, 0.0722))
#define diff(x,y)   vec2(Luma((GetL(x,y) - meanL)), Luma((GetH(x,y) - meanH)))

vec4 hook() {
    vec3 meanL = GetL(0,0);
    vec3 meanH = GetH(0,0);
    for (int X=-1; X<=1; X+=2) {
        meanL += GetL(X,0) * spread;
        meanH += GetH(X,0) * spread;
    }
    for (int Y=-1; Y<=1; Y+=2) {
        meanL += GetL(0,Y) * spread;
        meanH += GetH(0,Y) * spread;
    }
    meanL /= (1.0 + 4.0*spread);
    meanH /= (1.0 + 4.0*spread);

    vec2 var = diff(0,0);
    for (int X=-1; X<=1; X+=2)
        var += diff(X,0) * spread;

    for (int Y=-1; Y<=1; Y+=2)
        var += diff(0,Y) * spread;

    return vec4(max(var / (1.0 + 4.0*spread), vec2(1e-6)), 0, 0);
}

//!HOOK POSTKERNEL
//!BIND HOOKED
//!BIND PREKERNEL
//!BIND LOWRES
//!BIND var
//!WHEN NATIVE_CROPPED.h OUTPUT.h <
//!DESC SSSR final pass

#define oversharp   0.5

// -- Window Size --
#define taps        3.0
#define even        (taps - 2.0 * floor(taps / 2.0) == 0.0)
#define minX        int(1.0-ceil(taps/2.0))
#define maxX        int(floor(taps/2.0))

#define Kernel(x)   cos(acos(-1.0)*(x)/taps) // Hann kernel

// -- Input processing --
#define var(x,y)    var_tex(var_pt * (pos + vec2(x,y) + 0.5)).rg
#define GetL(x,y)   PREKERNEL_tex(PREKERNEL_pt * (pos + tex_offset + vec2(x,y) + 0.5)).rgb
#define GetH(x,y)   LOWRES_tex(LOWRES_pt * (pos + vec2(x,y) + 0.5))

#define Luma(rgb)   dot(rgb*rgb, vec3(0.2126, 0.7152, 0.0722))

vec4 hook() {
    vec4 c0 = HOOKED_texOff(0);

    vec2 pos = HOOKED_pos * LOWRES_size - vec2(0.5);
    vec2 offset = pos - (even ? floor(pos) : round(pos));
    pos -= offset;

    vec2 mVar = vec2(0.0);
    for (int X=-1; X<=1; X++)
    for (int Y=-1; Y<=1; Y++) {
        vec2 w = clamp(1.5 - abs(vec2(X,Y)), 0.0, 1.0);
        mVar += w.r * w.g * vec2(GetH(X,Y).a, 1.0);
    }
    mVar.r /= mVar.g;

    // Calculate faithfulness force
    float weightSum = 0.0;
    vec3 diff = vec3(0);

    for (int X = minX; X <= maxX; X++)
    for (int Y = minX; Y <= maxX; Y++)
    {
        float R = (-1.0 - oversharp) * sqrt(var(X,Y).r / (var(X,Y).g + mVar.r));

        vec2 krnl = Kernel(vec2(X,Y) - offset);
        float weight = krnl.r * krnl.g / (Luma((c0.rgb - GetH(X,Y).rgb)) + GetH(X,Y).a);

        diff += weight * (GetL(X,Y) + GetH(X,Y).rgb * R + (-1.0 - R) * (c0.rgb));
        weightSum += weight;
    }
    diff /= weightSum;

    c0.rgb = ((c0.rgb) + diff);

    return c0;
}
