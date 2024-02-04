Shader "Custom/SkyboxRotation"
{
    Properties
    {
        _RotationX("X Rotation", Range(0, 360)) = 0
        _RotationY("Y Rotation", Range(0, 360)) = 0
        _RotationZ("Z Rotation", Range(0, 360)) = 0
        _MainTex("Cubemap", Cube) = "" {}
    }

    SubShader
    {
        Tags { "Queue" = "Background" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float3 texCoord : TEXCOORD0;
            };

            float _RotationX;
            float _RotationY;
            float _RotationZ;
            samplerCUBE _MainTex;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                // Apply rotations
                float3 rotatedVertex = v.vertex.xyz;
                float sinX = sin(_RotationX * 3.14159265358979323846 / 180);
                float cosX = cos(_RotationX * 3.14159265358979323846 / 180);
                float sinY = sin(_RotationY * 3.14159265358979323846 / 180);
                float cosY = cos(_RotationY * 3.14159265358979323846 / 180);
                float sinZ = sin(_RotationZ * 3.14159265358979323846 / 180);
                float cosZ = cos(_RotationZ * 3.14159265358979323846 / 180);

                rotatedVertex = float3(
                    rotatedVertex.x * cosY * cosZ - rotatedVertex.y * (cosX * sinZ + sinX * sinY * cosZ) + rotatedVertex.z * (sinX * sinZ - cosX * sinY * cosZ),
                    rotatedVertex.x * cosY * sinZ + rotatedVertex.y * (cosX * cosZ - sinX * sinY * sinZ) + rotatedVertex.z * (sinX * cosZ + cosX * sinY * sinZ),
                    -rotatedVertex.x * sinY + rotatedVertex.y * sinX * cosY + rotatedVertex.z * cosX * cosY
                );

                o.texCoord = rotatedVertex;
                return o;
            }

            half4 frag(v2f i) : COLOR
            {
                return texCUBE(_MainTex, i.texCoord);
            }
            ENDCG
        }
    }
}
