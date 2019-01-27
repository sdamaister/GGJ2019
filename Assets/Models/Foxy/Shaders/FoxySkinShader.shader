Shader "Custom/FoxySkinShader"
{
    Properties
    {
        _NormalAlbedo ("Albedo (RGB)", 2D) = "white" {}
		_NormalDiedAlbedo("Albedo (RGB)", 2D) = "white" {}
		_StunAlbedo("Albedo (RGB)", 2D) = "white" {}
		_StunDiedAlbedo("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		_Stunned("Stunned", Range(0,1)) = 0
		_FreezeCoef("FreezeCoef", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

		sampler2D _NormalAlbedo;
		sampler2D _NormalDiedAlbedo;
		sampler2D _StunAlbedo;
		sampler2D _StunDiedAlbedo;

        struct Input
        {
			float2 uv_NormalAlbedo;
			float2 uv_NormalDiedAlbedo;
			float2 uv_StunAlbedo;
			float2 uv_StunDiedAlbedo;
        };

        half _Glossiness;
        half _Metallic;
		half _Stunned;
		half _FreezeCoef;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			fixed4 color;
            // Albedo comes from a texture tinted by color
			if (_Stunned == 0.0f)
			{
				fixed4 normalColor = tex2D(_NormalAlbedo, IN.uv_NormalAlbedo);
				fixed4 diedColor   = tex2D(_NormalDiedAlbedo, IN.uv_NormalDiedAlbedo);

				color = lerp(diedColor, normalColor, _FreezeCoef);
			}
			else
			{
				fixed4 normalColor = tex2D(_StunAlbedo, IN.uv_StunAlbedo);
				fixed4 diedColor = tex2D(_StunDiedAlbedo, IN.uv_StunDiedAlbedo);

				color = lerp(diedColor, normalColor, _FreezeCoef);
			}

            o.Albedo = color.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = color.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
