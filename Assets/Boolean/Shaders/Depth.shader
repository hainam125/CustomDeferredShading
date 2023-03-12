Shader "Ist/Boolean/Depth"
{

SubShader
{
CGINCLUDE
#include "UnityCG.cginc"

float ComputeDepth(float4 clippos)
{
#if defined(SHADER_TARGET_GLSL) || defined(SHADER_API_GLES) || defined(SHADER_API_GLES3)
    return (clippos.z / clippos.w) * 0.5 + 0.5;
#else
    return clippos.z / clippos.w;
#endif
}


struct ia_out
{
    float4 vertex : POSITION;
};

struct vs_out
{
    float4 vertex : SV_POSITION;
};


vs_out vert(ia_out v)
{
    vs_out o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    return o;
}

float4 frag(vs_out i) : SV_Target
{
    return 1;
}
ENDCG

    // back
    Pass {
        Cull Front
        ZTest Greater
        ZWrite On

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        ENDCG
    }

    // front
    Pass {
        Cull Back
        ZTest Less
        ZWrite On

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        ENDCG
    }

}
}
