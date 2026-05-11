

float4x4 WorldViewProjection;
texture Texture;
sampler2D TextureSampler = sampler_state
{
    Texture = <Texture>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct VSInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

struct VSOutput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

VSOutput VS(VSInput v)
{
    VSOutput o;
    o.Position = mul(v.Position, WorldViewProjection);
    o.Color = v.Color;
    o.TexCoord = v.TexCoord;
    return o;
}

float4 PS(VSOutput i) : COLOR0
{
    return tex2D(TextureSampler, i.TexCoord) * i.Color;
}

technique SkinnedMesh
{
    pass P0
    {
        VertexShader = compile vs_2_0 VS();
        PixelShader = compile ps_2_0 PS();
    }
}
