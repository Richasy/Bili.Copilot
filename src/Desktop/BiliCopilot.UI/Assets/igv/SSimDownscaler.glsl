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
//!BIND PREKERNEL
//!BIND HOOKED
//!SAVE L2
//!WIDTH NATIVE_CROPPED.w
//!WHEN NATIVE_CROPPED.h POSTKERNEL.h >
//!COMPONENTS 3
//!DESC SSimDownscaler L2 pass 1

#define axis 1

#define offset      vec2(0,0)

#define MN(B,C,x)   (x < 1.0 ? ((2.-1.5*B-(C))*x + (-3.+2.*B+C))*x*x + (1.-(B)/3.) : (((-(B)/6.-(C))*x + (B+5.*C))*x + (-2.*B-8.*C))*x+((4./3.)*B+4.*C))
#define Kernel(x)   MN(.0, .5, abs(x))
#define taps        2.0

vec4 hook() {
    vec2 base = PREKERNEL_pt * (PREKERNEL_pos * input_size + tex_offset);

    float low  = ceil((PREKERNEL_pos - taps*POSTKERNEL_pt) * input_size - offset + tex_offset - 0.5)[axis];
    float high = floor((PREKERNEL_pos + taps*POSTKERNEL_pt) * input_size - offset + tex_offset - 0.5)[axis];

    float W = 0.0;
    vec4 avg = vec4(0);
    vec2 pos = base;

    for (float k = low; k <= high; k++) {
        pos[axis] = PREKERNEL_pt[axis] * (k - offset[axis] + 0.5);
        float rel = (pos[axis] - base[axis])*POSTKERNEL_size[axis];
        float w = Kernel(rel);

        vec4 tex = textureLod(PREKERNEL_raw, pos, 0.0) * PREKERNEL_mul;
        avg += w * tex * tex;
        W += w;
    }
    avg /= W;

    return avg;
}

//!HOOK POSTKERNEL
//!BIND L2
//!BIND HOOKED
//!SAVE L2
//!WHEN NATIVE_CROPPED.w POSTKERNEL.w >
//!COMPONENTS 3
//!DESC SSimDownscaler L2 pass 2

#define axis 0

#define offset      vec2(0,0)

#define MN(B,C,x)   (x < 1.0 ? ((2.-1.5*B-(C))*x + (-3.+2.*B+C))*x*x + (1.-(B)/3.) : (((-(B)/6.-(C))*x + (B+5.*C))*x + (-2.*B-8.*C))*x+((4./3.)*B+4.*C))
#define Kernel(x)   MN(.0, .5, abs(x))
#define taps        2.0

vec4 hook() {
    float low  = ceil((L2_pos - taps*POSTKERNEL_pt) * L2_size - offset - 0.5)[axis];
    float high = floor((L2_pos + taps*POSTKERNEL_pt) * L2_size - offset - 0.5)[axis];

    float W = 0.0;
    vec4 avg = vec4(0);
    vec2 pos = L2_pos;

    for (float k = low; k <= high; k++) {
        pos[axis] = L2_pt[axis] * (k - offset[axis] + 0.5);
        float rel = (pos[axis] - L2_pos[axis])*POSTKERNEL_size[axis];
        float w = Kernel(rel);

        avg += w * textureLod(L2_raw, pos, 0.0) * L2_mul;
        W += w;
    }
    avg /= W;

    return avg;
}

//!HOOK POSTKERNEL
//!BIND HOOKED
//!BIND L2
//!SAVE MR
//!WHEN NATIVE_CROPPED.h POSTKERNEL.h >
//!COMPONENTS 4
//!DESC SSimDownscaler mean & R

#define oversharp   0.0

#define sigma_nsq   10. / (255.*255.)
#define locality    2.0

#define offset      vec2(0,0)

#define Kernel(x)   pow(1.0 / locality, abs(x))
#define taps        3.0

#define Luma(rgb)   ( dot(rgb, vec3(0.2126, 0.7152, 0.0722)) )

mat3x3 ScaleH(vec2 pos) {
    float low  = ceil(-0.5*taps - offset)[0];
    float high = floor(0.5*taps - offset)[0];

    float W = 0.0;
    mat3x3 avg = mat3x3(0);

    for (float k = low; k <= high; k++) {
        pos[0] = HOOKED_pos[0] + HOOKED_pt[0] * k;
        float rel = k + offset[0];
        float w = Kernel(rel);

        vec3 L = POSTKERNEL_tex(pos).rgb;
        avg += w * mat3x3(L, L*L, L2_tex(pos).rgb);
        W += w;
    }
    avg /= W;

    return avg;
}

vec4 hook() {
    vec2 pos = HOOKED_pos;

    float low  = ceil(-0.5*taps - offset)[1];
    float high = floor(0.5*taps - offset)[1];

    float W = 0.0;
    mat3x3 avg = mat3x3(0);

    for (float k = low; k <= high; k++) {
        pos[1] = HOOKED_pos[1] + HOOKED_pt[1] * k;
        float rel = k + offset[1];
        float w = Kernel(rel);

        avg += w * ScaleH(pos);
        W += w;
    }
    avg /= W;

    float Sl = Luma(max(avg[1] - avg[0] * avg[0], 0.));
    float Sh = Luma(max(avg[2] - avg[0] * avg[0], 0.));
    return vec4(avg[0], mix(sqrt((Sh + sigma_nsq) / (Sl + sigma_nsq)) * (1. + oversharp), clamp(Sh / Sl, 0., 1.), float(Sl > Sh)));
}

//!HOOK POSTKERNEL
//!BIND HOOKED
//!BIND MR
//!WHEN NATIVE_CROPPED.h POSTKERNEL.h >
//!DESC SSimDownscaler final pass

#define locality    2.0

#define offset      vec2(0,0)

#define Kernel(x)   pow(1.0 / locality, abs(x))
#define taps        3.0

#define Gamma(x)    ( pow(x, vec3(1.0/2.0)) )
#define GammaInv(x) ( pow(clamp(x, 0.0, 1.0), vec3(2.0)) )

mat3x3 ScaleH(vec2 pos) {
    float low  = ceil(-0.5*taps - offset)[0];
    float high = floor(0.5*taps - offset)[0];

    float W = 0.0;
    mat3x3 avg = mat3x3(0);

    for (float k = low; k <= high; k++) {
        pos[0] = HOOKED_pos[0] + HOOKED_pt[0] * k;
        float rel = k + offset[0];
        float w = Kernel(rel);

        vec4 MR = MR_tex(pos);
        avg += w * mat3x3(MR.a*MR.rgb, MR.rgb, MR.aaa);
        W += w;
    }
    avg /= W;

    return avg;
}

vec4 hook() {
    vec2 pos = HOOKED_pos;

    float low  = ceil(-0.5*taps - offset)[1];
    float high = floor(0.5*taps - offset)[1];

    float W = 0.0;
    mat3x3 avg = mat3x3(0);

    for (float k = low; k <= high; k++) {
        pos[1] = HOOKED_pos[1] + HOOKED_pt[1] * k;
        float rel = k + offset[1];
        float w = Kernel(rel);

        avg += w * ScaleH(pos);
        W += w;
    }
    avg /= W;
    vec4 L = POSTKERNEL_texOff(0);
    return vec4(avg[1] + avg[2] * L.rgb - avg[0], L.a);
}
