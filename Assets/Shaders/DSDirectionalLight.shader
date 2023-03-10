Shader "DeferredShading/DSDirectionalLight" {
	Properties {
		_NormalBuffer ("Normal", 2D) = "white" {}
		_PositionBuffer ("Position", 2D) = "white" {}
		_ColorBuffer ("Color", 2D) = "white" {}
		_GlowBuffer ("Glow", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		Blend One One
		ZTest Always
		ZWrite Off
	    Cull Back

		CGINCLUDE

		sampler2D _NormalBuffer;
		sampler2D _PositionBuffer;
		sampler2D _ColorBuffer;
		sampler2D _GlowBuffer;
		float4 _LightColor;
		float4 _LightDir;
		float4 _ShadowParams; // [0]: 0=disabled, [1]: steps


		struct vs_in {
			float4 vertex : POSITION;
			float4 normal : NORMAL;
		};

		struct ps_in {
			float4 vertex : SV_POSITION;
			float4 screen_pos : TEXCOORD0;
		};

		struct ps_out {
			float4 color : COLOR0;
		};


		ps_in vert (vs_in v) {
			ps_in o;
			o.vertex = v.vertex;
			o.screen_pos = v.vertex;
			return o;
		}

		ps_out frag (ps_in i) {
			float2 coord = (i.screen_pos.xy / i.screen_pos.w + 1.0) * 0.5;
#if UNITY_UV_STARTS_AT_TOP
			coord.y = 1.0 - coord.y;
#endif

			float4 FragPos4		= tex2D(_PositionBuffer, coord);
			if (FragPos4.w == 0.0) { discard; }
			float4 AS		= tex2D(_ColorBuffer, coord);
			float4 NS		= tex2D(_NormalBuffer, coord);
			if(dot(AS.xyz,AS.xyz)==0.0) { discard; }

			float3 FragPos		= FragPos4.xyz;
			float3 LightColor	= _LightColor.rgb;
			float3 LightDir	= _LightDir;

			float3 Albedo = AS.rgb;
			float Shininess = 50;
			float3 Normal = NS.xyz;
			float3 EyePos = _WorldSpaceCameraPos.xyz;
			float3 EyeDir = normalize(EyePos - FragPos);

			float3 h = normalize(EyeDir + LightDir);
			float nh = max(dot(Normal, h), 0.0);
			float Specular = pow(nh, Shininess);
			float Intensity = max(dot(Normal, LightDir), 0.0);
            //Intensity = smoothstep(0, 0.01, Intensity);
            //Specular = smoothstep(0.005, 0.01, Specular);

			float4 Result = 1;
			Result.rgb = LightColor * (Albedo * Intensity + Specular);

			ps_out r = { Result };
			return r;
		}
		ENDCG

	    Pass {
		    CGPROGRAM
		    #pragma vertex vert
		    #pragma fragment frag
		    #pragma target 3.0
		    ENDCG
	    }
    } 
	FallBack "Diffuse"
}