Shader "Unlit/Combine" {
    Properties
    {
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        ZTest Always
        ZWrite Off
        Cull Back

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float4 screen_pos : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = v.vertex;
                o.screen_pos = o.vertex;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                float2 coord = (i.screen_pos.xy / i.screen_pos.w + 1.0) * 0.5;
                return tex2D(_MainTex, coord);
            }
            ENDCG
        }
    }
}
