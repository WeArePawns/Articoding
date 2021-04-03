Shader "Unlit/PopupHighlightShader"
{
    Properties
    {
        [HideInInspector] _MainTex("Base (RGB)", 2D) = "white" {}
        _PositionSize ("Position and Size", Vector) = (0.0, 0.0, 0.0, 0.0)
        _BackgroundColor("Background Color", Color) = (0.0, 0.0, 0.0, 0.55)
        _HighlightColor("Highlight Color", Color) = (1.0, 1.0, 1.0, 0.1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct MeshData
            {
                float4 vertex : POSITION;
            };

            struct VertexData
            {
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD0;
            };

            float4 _PositionSize;
            float4 _BackgroundColor;
            float4 _HighlightColor;

            VertexData vert (MeshData v)
            {
                VertexData o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            float4 frag(VertexData i) : SV_Target
            {
                float2 coords = i.screenPos.xy / i.screenPos.w * _ScreenParams.xy;
                float4 finalColor = _BackgroundColor;
                
                if (coords.x >= _PositionSize.x && coords.x < _PositionSize.x + _PositionSize.z &&
                    coords.y >= _PositionSize.y && coords.y < _PositionSize.y + _PositionSize.w)
                    finalColor = _HighlightColor;

                return finalColor;
            }
            ENDCG
        }
    }
}
