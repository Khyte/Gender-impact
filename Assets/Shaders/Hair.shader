Shader "Genesis/Character/gHairShaderNew"
{
	Properties
	{
		[HideInInspector] __dirty("", Int) = 1
		_MainTex("_MainTex", 2D) = "white" {}
	_NormalMap("_NormalMap", 2D) = "bump" {}
	_AlphaCutout("Mask Clip Value", Float) = 0.25
		_Specular("_Specular", Range(0 , 0.5)) = 0.35
		_Emission("_Emission", Range(0 , 0.2)) = 0.8
		_Smoothness("_Smoothness", Range(0 , 1)) = 0.8
		_Color("_Color", Color) = (1,1,1,0)
		[HideInInspector] _texcoord("", 2D) = "white" {}
	}

		SubShader
	{
		Tags{ "RenderType" = "Overlay"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" "IsEmissive" = "true" }
		LOD 200
		Cull Off
		Blend One Zero , OneMinusDstColor One
		BlendOp Add , Add
		CGPROGRAM
#pragma target 3.0
#pragma multi_compile_instancing
#pragma surface surf StandardSpecular keepalpha addshadow fullforwardshadows
		struct Input
	{
		float2 uv_texcoord;
	};

	uniform sampler2D _NormalMap;
	uniform float4 _NormalMap_ST;
	uniform sampler2D _MainTex;
	uniform float4 _MainTex_ST;
	uniform float4 _Color;
	uniform float _Emission;
	uniform float _Specular;
	uniform float _Smoothness;
	uniform float _AlphaCutout = 0.25;

	void surf(Input i , inout SurfaceOutputStandardSpecular o)
	{
		float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
		float3 txNormalMap69 = UnpackNormal(tex2D(_NormalMap, uv_NormalMap));
		o.Normal = txNormalMap69;
		float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
		float4 tex2DNode1 = tex2D(_MainTex, uv_MainTex);
		float4 txMainTex40 = tex2DNode1;
		float4 cColor39 = _Color;
		float4 blendOpSrc17 = txMainTex40;
		float4 blendOpDest17 = cColor39;
		float4 blendOpSrc56 = float4(0.1985294,0.1985294,0.1985294,1);
		float4 blendOpDest56 = (saturate((blendOpSrc17 * blendOpDest17)));
		float4 blendOpSrc25 = (saturate((blendOpDest56 > 0.5 ? (1.0 - (1.0 - 2.0 * (blendOpDest56 - 0.5)) * (1.0 - blendOpSrc56)) : (2.0 * blendOpDest56 * blendOpSrc56))));
		float4 blendOpDest25 = txMainTex40;
		float4 cFinalColor62 = (saturate(2.0f*blendOpSrc25*blendOpDest25 + blendOpSrc25*blendOpSrc25*(1.0f - 2.0f*blendOpDest25)));
		o.Albedo = cFinalColor62.xyz;
		o.Emission = (cFinalColor62 * _Emission).xyz;
		o.Specular = (cFinalColor62 * _Specular).xyz;
		o.Smoothness = (txMainTex40 * _Smoothness).x;
		float3 desaturateVar26 = lerp(txMainTex40.xyz,dot(txMainTex40.xyz,float3(0.299,0.587,0.114)),0.0);
		o.Occlusion = desaturateVar26.x;
		o.Alpha = 1;
		float txMainTexAlpha67 = tex2DNode1.a;
		clip(txMainTexAlpha67 - _AlphaCutout);
	}

	ENDCG
	}
		Fallback "Standard"
		Fallback "Diffuse"
}
