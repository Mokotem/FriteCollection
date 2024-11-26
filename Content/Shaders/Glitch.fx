#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
float ofx;
float ofy;
int fact1, fact2, fact3;
float volume;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 col = tex2D(SpriteTextureSampler,input.TextureCoordinates);
    float2 posr = input.TextureCoordinates;
    posr.r += ofx * fact1;
    posr.g += ofy * fact1;
	
    float2 posg = input.TextureCoordinates;
    posg.r += ofx * fact2;
    posg.g += ofy * fact2;
	
    float2 posb = input.TextureCoordinates;
    posb.r += ofx * fact3;
    posb.g += ofy * fact3;
	
    float4 final;
    final.r = tex2D(SpriteTextureSampler, posr).r;
    final.g = tex2D(SpriteTextureSampler, posg).g;
    final.b = tex2D(SpriteTextureSampler, posb).b;
    final.a = 1;
	
    return (final * volume)
	+ (tex2D(SpriteTextureSampler, input.TextureCoordinates) * (1 - volume));

}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};