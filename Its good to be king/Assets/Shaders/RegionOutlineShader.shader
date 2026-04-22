Shader "Unlit/RegionOutlineShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _RegionTex ("Region Map", 2D) = "white" {}
        _KingdomTex ("Kingdom Lookup", 2D) = "white" {}
        _provinceIndexTex ("Province index Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _HighlightColor ("Highlight Color", Color) = (1,1,0,1)
        _SelectedRegion ("Selected Region", Color) = (0,0,0,0)
        
        _OutlineThickness ("Outline Thickness", Float) = 0.002
        _MapState ("Map State", Float) = 0
        //_SelectedKingdom ("Selected Kingdom", Float) = -1
    }
SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            sampler2D _RegionTex;
            sampler2D _KingdomTex;
            sampler2D _provinceIndexTex;
            float4 _OutlineColor;
            float4 _HighlightColor;
            float4 _SelectedRegion;
            float _OutlineThickness;
            float _KingdomCount;
            float _SelectedKingdom;
            float _MapState;
            sampler2D _ProvinceToKingdomTex;
            float _ProvinceCount;




            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float GetProvinceID(float2 uv)
            {
                float4 indexedColor = tex2D(_provinceIndexTex, uv);
                return indexedColor.r * 255;
            }

            float GetKingdomID(float provinceId)
            {
                float u = (provinceId + 0.5) / _ProvinceCount;
                float4 data = tex2D(_ProvinceToKingdomTex, float2(u, 0.5));
                return data.r * 255.0;
            }

            float4 GetKingdomColor(float kingdomId)
            {
                float u = (kingdomId + 0.5) / _KingdomCount;
                return tex2D(_KingdomTex, float2(u, 0.5));
            }


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                float4 baseColor = tex2D(_MainTex, uv);
                float4 region = tex2D(_RegionTex, uv);

                float provinceID = GetProvinceID(uv);
                float kingdomID = GetKingdomID(provinceID);
                float4 kingdomColor = GetKingdomColor(kingdomID);


                if (region.a < 0.1)
                {
                    return baseColor;
                }

                // if (abs(kingdomID - _SelectedKingdom) < 0.5)
                // {
                //     baseColor = lerp(baseColor, _HighlightColor, 0.3);
                // }


                float regionMatch = distance(region.rgb, _SelectedRegion.rgb);

                if (regionMatch < 0.01)
                {
                    return lerp(baseColor, _HighlightColor, 0.3);
                }
                float4 regionRight = tex2D(_RegionTex, uv + float2(_OutlineThickness, 0));
                float4 regionUp    = tex2D(_RegionTex, uv + float2(0, _OutlineThickness));
                float4 regionDown    = tex2D(_RegionTex, uv + float2(0, 0 - _OutlineThickness));
                float4 regionLeft    = tex2D(_RegionTex, uv + float2(0 - _OutlineThickness, 0));

                // Compare colors
                float diff = distance(region, regionRight) + distance(region, regionUp) + distance(region, regionLeft) + distance(region, regionDown);
                if (diff > 0.01)
                {
                    return _OutlineColor;
                }
                if(_MapState == 1)
                {
                    // Sample neighbors
                }
                if(_MapState == 0)
                {
                    return lerp(baseColor, kingdomColor, 0.7);

                }



                return baseColor;
            }
            ENDCG
        }
    }
}
