sampler2D SpriteTexture : register(s0);

float2 Time;
float2 Frequency;
float2 Amplitude;

float4 MainPS(float2 uv : TEXCOORD0) : COLOR0
{
    float offsetX = sin((uv.y + Time.y) * Frequency.y) * Amplitude.x;
    float offsetY = cos((uv.x + Time.x) * Frequency.x) * Amplitude.y;

    float2 distortedUV = uv + float2(offsetX, offsetY);
    float4 baseColor = tex2D(SpriteTexture, distortedUV);

    float luminance = dot(baseColor.rgb, float3(0.299, 0.587, 0.114));
    return float4(luminance, luminance, luminance, baseColor.a);
}

technique WavyDeform
{
    pass WavyPass
    {
        PixelShader = compile ps_2_0 MainPS();
    }
}
