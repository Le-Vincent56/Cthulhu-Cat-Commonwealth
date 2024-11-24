Shader "Hidden/Heartbeat"
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
            float _Flow;

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

			//#define _HigherFidelity
			float4 frag(Varyings i) : SV_Target
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				
				// Get Noise
				float2 warp = 2*(i.uv - 0.5f);
				float dist = sqrt(warp.x * warp.x + warp.y * warp.y);
				float angle = atan2(warp.y, warp.x);

				dist *=  (1 - _Intensity) + _Intensity * smoothstep(0, 1, abs(-distance(_Flow, dist)));

				//dist = smoothstep(0, 1, dist);

				//warp += dist * sign(warp.x) * sign(warp.y);

				warp.x = dist * cos(angle);
				warp.y = dist * sin(angle);

				warp = (warp * 0.5f) + 0.5f;

				// Get Main Texture and blend in
				float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, warp);
				
				
				return color;
			}
			
			ENDHLSL
		}
	} 
	FallBack "Diffuse"
}