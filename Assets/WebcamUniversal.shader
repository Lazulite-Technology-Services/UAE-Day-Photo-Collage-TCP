Shader "Unlit/WebcamUniversal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white"
        _CropRect ("Crop Rect", Vector) = (0,0,1,1)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _CropRect;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 minUV = _CropRect.xy;
                float2 maxUV = _CropRect.xy + _CropRect.zw;

                if(i.uv.x < minUV.x || i.uv.x > maxUV.x || i.uv.y < minUV.y || i.uv.y > maxUV.y)
                    discard;

                float2 croppedUV = (i.uv - minUV) / _CropRect.zw;
                return tex2D(_MainTex, croppedUV);
            }
            ENDCG
        }
    }
}
