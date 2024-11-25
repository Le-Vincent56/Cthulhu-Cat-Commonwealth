Shader "Hidden/ChromaticAberration"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader 
	{
		Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
		
		Pass
		{
			HLSLPROGRAM
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            
			#pragma vertex vert
			#pragma fragment frag
			
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
			
            
			float4 _MainTex_TexelSize;

            float _Intensity;
            float _Angle;
            float _Distance;

            #define _SCREEN_SPACE_OCCLUSION
            
            struct Attributes
            {
                float4 vertex       : POSITION;
                float2 uv               : TEXCOORD0;
            };

            struct Varyings
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
            	UNITY_VERTEX_OUTPUT_STEREO
			};

            Varyings vert(Attributes v)
			{
				Varyings o;
            	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = TransformObjectToHClip(v.vertex);
				o.uv = v.uv;
				

				return o;
			}

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
			// Combines the top and bottom colors using normal blending.
			// https://en.wikipedia.org/wiki/Blend_modes#Normal_blend_mode
			// This performs the same operation as Blend SrcAlpha OneMinusSrcAlpha.
			float4 alphaBlend(float4 top, float4 bottom)
			{
				float3 color = (top.rgb * top.a) + (bottom.rgb * (1 - top.a));
				float alpha = top.a + bottom.a * (1 - top.a);

				return float4(color, alpha);
			}

            float2 UVFromPolar(float angle, float radius)
            {
	            float2 uv;
            	uv.x = sin(angle) * radius;
            	uv.y = cos(angle) * radius;
            	return uv;
            }

			//#define _HigherFidelity
			float4 frag(Varyings i) : SV_Target
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				

				float r = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + UVFromPolar(_Angle, _Distance)).r;
				float g = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv).rgb;
				float b = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv - UVFromPolar(_Angle, _Distance)).b;

				float4 ca = float4(r, g, b, _Intensity);
				
				float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
				
				return alphaBlend(ca, color);
			}
			
			ENDHLSL
		}
	} 
	FallBack "Diffuse"
}