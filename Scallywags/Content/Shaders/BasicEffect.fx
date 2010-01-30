//-----------------------------------------------------------------------------
// BasicEffect.fx
//
// This is a simple shader that supports 1 ambient and 3 directional lights.
// All lighting computations happen in world space.
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

extern float3	g_vBoatPos;
extern float	g_fBoatRotation;
extern float	g_fTime;
extern float3	g_vWaveLocation1;
extern float3	g_vWaveLocation2;
extern float3	g_vWaveLocation3;
extern float3	g_vWaveLocation4;
extern float	g_Alpha;

//-----------------------------------------------------------------------------
// Texture sampler
//-----------------------------------------------------------------------------

uniform const texture BasicTexture;

uniform const sampler TextureSampler : register(s0) = sampler_state
{
	Texture = (BasicTexture);
	MipFilter = Linear;
	MinFilter = Linear;
	MagFilter = Linear;
	
	AddressU  = WRAP;
    AddressV  = WRAP;
};

//-----------------------------------------------------------------------------
// Diffuse Map
//-----------------------------------------------------------------------------

uniform const texture DiffuseMap;

uniform const sampler DiffuseSampler : register(s1) = sampler_state
{
    Texture = (DiffuseMap);
    MipFilter = Linear;
    MinFilter = Linear;
    MagFilter = Linear;
    
    AddressU = WRAP;
    AddressV = WRAP;
};


//-----------------------------------------------------------------------------
// Fog settings
//-----------------------------------------------------------------------------

uniform const float		FogEnabled		: register(c0);
uniform const float		FogStart		: register(c1);
uniform const float		FogEnd			: register(c2);
uniform const float3	FogColor		: register(c3);

uniform const float3	EyePosition		: register(c4);		// in world space


//-----------------------------------------------------------------------------
// Material settings
//-----------------------------------------------------------------------------

uniform const float3	DiffuseColor	: register(c5) = 1;
uniform const float		Alpha			: register(c6) = 1;
uniform const float3	EmissiveColor	: register(c7) = 0;
uniform const float3	SpecularColor	: register(c8) = 1;
uniform const float		SpecularPower	: register(c9) = 16;


//-----------------------------------------------------------------------------
// Lights
// All directions and positions are in world space and must be unit vectors
//-----------------------------------------------------------------------------

uniform const float3	AmbientLightColor		: register(c10);

uniform const float3	DirLight0Direction		: register(c11);
uniform const float3	DirLight0DiffuseColor	: register(c12);
uniform const float3	DirLight0SpecularColor	: register(c13);

uniform const float3	DirLight1Direction		: register(c14);
uniform const float3	DirLight1DiffuseColor	: register(c15);
uniform const float3	DirLight1SpecularColor	: register(c16);

uniform const float3	DirLight2Direction		: register(c17);
uniform const float3	DirLight2DiffuseColor	: register(c18);
uniform const float3	DirLight2SpecularColor	: register(c19);




//-----------------------------------------------------------------------------
// Matrices
//-----------------------------------------------------------------------------

uniform const float4x4	World		: register(vs, c20);	// 20 - 23
uniform const float4x4	View		: register(vs, c24);	// 24 - 27
uniform const float4x4	Projection	: register(vs, c28);	// 28 - 31


//-----------------------------------------------------------------------------
// Structure definitions
//-----------------------------------------------------------------------------

struct ColorPair
{
	float3 Diffuse;
	float3 Specular;
};

struct CommonVSOutput
{
	float4	Pos_ws;
	float4	Pos_ps;
	float4	Diffuse;
	float3	Specular;
	float	FogFactor;
};


//-----------------------------------------------------------------------------
// Shader I/O structures
// Nm: Normal
// Tx: Texture
// Vc: Vertex color
//
// Nm Tx Vc
//  0  0  0	VSInput
//  0  0  1 VSInputVc
//  0  1  0 VSInputTx
//  0  1  1 VSInputTxVc
//  1  0  0 VSInputNm
//  1  0  1 VSInputNmVc
//  1  1  0 VSInputNmTx
//  1  1  1 VSInputNmTxVc


//-----------------------------------------------------------------------------
// Vertex shader inputs
//-----------------------------------------------------------------------------

struct VSInput
{
	float4	Position	: POSITION;
};

struct VSInputVc
{
	float4	Position	: POSITION;
	float4	Color		: COLOR;
};

struct VSInputNm
{
	float4	Position	: POSITION;
	float3	Normal		: NORMAL;
};

struct VSInputNmVc
{
	float4	Position	: POSITION;
	float3	Normal		: NORMAL;
	float4	Color		: COLOR;
};

struct VSInputTx
{
	float4	Position	: POSITION;
	float2	TexCoord	: TEXCOORD0;
};

struct VSInputTxVc
{
	float4	Position	: POSITION;
	float2	TexCoord	: TEXCOORD0;
	float4	Color		: COLOR;
	float4  Normal		: NORMAL;
};

struct VSInputNmTx
{
	float4	Position	: POSITION;
	float2	TexCoord	: TEXCOORD0;
	float3	Normal		: NORMAL;
};

struct VSInputNmTxVc
{
	float4	Position	: POSITION;
	float2	TexCoord	: TEXCOORD0;
	float3	Normal		: NORMAL;
	float4	Color		: COLOR;
};


//-----------------------------------------------------------------------------
// Vertex shader outputs
//-----------------------------------------------------------------------------

struct VertexLightingVSOutput
{
	float4	PositionPS	: POSITION;		// Position in projection space
	float4	Diffuse		: COLOR0;
	float4	Specular	: COLOR1;		// Specular.rgb and fog factor
};

struct VertexLightingVSOutputTx
{
	float4	PositionPS	: POSITION;		// Position in projection space
	float4	Diffuse		: COLOR0;
	float4	Specular	: COLOR1;
	float2	TexCoord	: TEXCOORD0;
};

struct PixelLightingVSOutput
{
	float4	PositionPS	: POSITION;		// Position in projection space
	float4	PositionWS	: TEXCOORD0;
	float3	NormalWS	: TEXCOORD1;
	float4	Diffuse		: COLOR0;		// diffuse.rgb and alpha
};

struct PixelLightingVSOutputTx
{
	float4	PositionPS	: POSITION;		// Position in projection space
	float2	TexCoord	: TEXCOORD0;
	float4	PositionWS	: TEXCOORD1;
	float3	NormalWS	: TEXCOORD2;
	float4	Diffuse		: COLOR0;		// diffuse.rgb and alpha
};


//-----------------------------------------------------------------------------
// Pixel shader inputs
//-----------------------------------------------------------------------------

struct VertexLightingPSInput
{
	float4	PositionWS	: TEXCOORD0;
	float4	Diffuse		: COLOR0;
	float4	Specular	: COLOR1;
};

struct VertexLightingPSInputTx
{
	float4	Diffuse		: COLOR0;
	float4	Specular	: COLOR1;
	float2	TexCoord	: TEXCOORD0;
};

struct PixelLightingPSInput
{
	float4	PositionWS	: TEXCOORD0;
	float3	NormalWS	: TEXCOORD1;
	float4	Diffuse		: COLOR0;		// diffuse.rgb and alpha
};

struct PixelLightingPSInputTx
{
	float2	TexCoord	: TEXCOORD0;
	float4	PositionWS	: TEXCOORD1;
	float3	NormalWS	: TEXCOORD2;
	float4	Diffuse		: COLOR0;		// diffuse.rgb and alpha
};

// Output structure for the vertex shader that renders normal and depth information.
struct NormalDepthVertexShaderOutput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
};


//-----------------------------------------------------------------------------
// Compute lighting
// E: Eye-Vector
// N: Unit vector normal in world space
//-----------------------------------------------------------------------------
ColorPair ComputeLights(float3 E, float3 N)
{
	ColorPair result;
	
	result.Diffuse = AmbientLightColor;
	result.Specular = 0;

	// Directional Light 0
	float3 L = -DirLight0Direction;
	float3 H = normalize(E + L);
	float2 ret = lit(dot(N, L), dot(N, H), SpecularPower).yz;
	result.Diffuse += DirLight0DiffuseColor * ret.x;
	result.Specular += DirLight0SpecularColor * ret.y;
	
	// Directional Light 1
	L = -DirLight1Direction;
	H = normalize(E + L);
	ret = lit(dot(N, L), dot(N, H), SpecularPower).yz;
	result.Diffuse += DirLight1DiffuseColor * ret.x;
	result.Specular += DirLight1SpecularColor * ret.y;
	
	// Directional Light 2
	L = -DirLight2Direction;
	H = normalize(E + L);
	ret = lit(dot(N, L), dot(N, H), SpecularPower).yz;
	result.Diffuse += DirLight2DiffuseColor * ret.x;
	result.Specular += DirLight2SpecularColor * ret.y;
		
	result.Diffuse *= DiffuseColor;
	result.Diffuse	+= EmissiveColor;
	result.Specular	*= SpecularColor;
		
	return result;
}


//-----------------------------------------------------------------------------
// Compute per-pixel lighting.
// When compiling for pixel shader 2.0, the lit intrinsic uses more slots
// than doing this directly ourselves, so we don't use the intrinsic.
// E: Eye-Vector
// N: Unit vector normal in world space
//-----------------------------------------------------------------------------
ColorPair ComputePerPixelLights(float3 E, float3 N)
{
	ColorPair result;
	
	result.Diffuse = AmbientLightColor;
	result.Specular = 0;
	
	// Light0
	float3 L = -DirLight0Direction;
	float3 H = normalize(E + L);
	float dt = max(0,dot(L,N));
    result.Diffuse += DirLight0DiffuseColor * dt;
    if (dt != 0)
		result.Specular += DirLight0SpecularColor * pow(max(0,dot(H,N)), SpecularPower);

	// Light1
	L = -DirLight1Direction;
	H = normalize(E + L);
	dt = max(0,dot(L,N));
    result.Diffuse += DirLight1DiffuseColor * dt;
    if (dt != 0)
	    result.Specular += DirLight1SpecularColor * pow(max(0,dot(H,N)), SpecularPower);
    
	// Light2
	L = -DirLight2Direction;
	H = normalize(E + L);
	dt = max(0,dot(L,N));
    result.Diffuse += DirLight2DiffuseColor * dt;
    if (dt != 0)
	    result.Specular += DirLight2SpecularColor * pow(max(0,dot(H,N)), SpecularPower);
    
    //Commenting out Creates White Light, leaving in creates coloured light
    result.Diffuse *= float4(1.0,1.0,1.0,1.0);//DiffuseColor;
    result.Diffuse += EmissiveColor;
    result.Specular *= SpecularColor;
		
	return result;
}

//-----------------------------------------------------------------------------
// Compute fog factor
//-----------------------------------------------------------------------------
float ComputeFogFactor(float d)
{
    return clamp((d - FogStart) / (FogEnd - FogStart), 0, 1) * FogEnabled;
}

CommonVSOutput ComputeCommonVSOutput(float4 position)
{
	CommonVSOutput vout;
	
	float4 pos_ws = mul(position, World);
	float4 pos_vs = mul(pos_ws, View);
	float4 pos_ps = mul(pos_vs, Projection);
	vout.Pos_ws = pos_ws;
	vout.Pos_ps = pos_ps;
	
	vout.Diffuse	= float4(DiffuseColor.rgb, Alpha);
	vout.Specular	= 0;
	vout.FogFactor	= 0;//ComputeFogFactor(length(EyePosition - pos_ws ));
	
	return vout;
}

CommonVSOutput ComputeCommonVSOutputWithLighting(float4 position, float3 normal)
{
	CommonVSOutput vout;
	
	float4 pos_ws = mul(position, World);
	float4 pos_vs = mul(pos_ws, View);
	float4 pos_ps = mul(pos_vs, Projection);
	vout.Pos_ws = pos_ws;
	vout.Pos_ps = pos_ps;
	
	float3 N = normalize(mul(normal, World));
	float3 posToEye = EyePosition - pos_ws;
	float3 E = normalize(posToEye);
	ColorPair lightResult = ComputeLights(E, N);
	
	vout.Diffuse	= float4(lightResult.Diffuse.rgb, Alpha);
	vout.Specular	= lightResult.Specular;
	vout.FogFactor	= ComputeFogFactor(length(posToEye));
	
	return vout;
}

//-----------------------------------------------------------------------------
// Wave Value
//-----------------------------------------------------------------------------

float ComputeWaveValue(float Time, float3 Position)
{

	float fDistance1 = length( g_vWaveLocation1 - Position);
	float fDistance2 = length( g_vWaveLocation2.x - Position.x );
	float fDistance3 = length( g_vWaveLocation3 - Position );
	//float fDistance4 = length( g_vWaveLocation4 - Position );

    float WaveVal = 3.0 * sin( 0.075 * fDistance1 - ( Time ));
	WaveVal += 2.0 * sin( 0.05 * fDistance2 + ( (Time) * 0.5 ) );
	WaveVal += 2.0 * sin( 0.1 * fDistance3 + ( (Time) * 2 ) );
	//WaveVal += 3.0 * sin( 0.075 * fDistance4 - ( (Time) * 1.0 ) );
	
	return WaveVal;

}

float3 ComputeWaveNormal(float Time, float3 Position)
{

	float fDistance1 = length( g_vWaveLocation1 - Position);
	float fDistance2 = length( g_vWaveLocation2.x - Position.x );
	float fDistance3 = length( g_vWaveLocation3.x - Position.z );
	//float fDistance4 = length( g_vWaveLocation4 - Position );

    float WaveNorm1 =  3.0 * cos( 0.075 * fDistance1 - ( Time ));
	float WaveNorm2 =  2.0 * cos( 0.05 * fDistance2 + ( (Time) * 0.5 ) );
	float WaveNorm3 = 2.0 * cos( 0.1 * fDistance3 + ( (Time) * 2 ) );
	//WaveVal += 3.0 * cos( 0.075 * fDistance4 - ( (Time) * 1.0 ) );
	
	float3 Normal1 = WaveNorm1 * normalize(g_vWaveLocation1 - Position);
	float3 Normal2 = WaveNorm2 * normalize(float3(0.0,g_vWaveLocation2.x - Position.x,0.0));
	float3 Normal3 = WaveNorm3 * normalize(g_vWaveLocation3 - Position);
	
	return (Normal1 + Normal2 + Normal3)/7;

}

//-----------------------------------------------------------------------------
// Vertex shaders
//-----------------------------------------------------------------------------

PixelLightingVSOutputTx VSBasicTxVc(VSInputTxVc vin)
{
	PixelLightingVSOutputTx vout;
	
	CommonVSOutput cout = ComputeCommonVSOutput(vin.Position);
	//CommonVSOutput cout = ComputeCommonVSOutputWithLighting( vin.Position, vin.Normal );

	vout.PositionPS	= cout.Pos_ps;
	vout.Diffuse	= cout.Diffuse;
	vout.Diffuse.a  = 1.0;
	vout.PositionWS	= float4(cout.Specular, cout.FogFactor);
	vout.TexCoord	= vin.TexCoord;
	vout.NormalWS	= vin.Normal;
	
	return vout;
}

PixelLightingVSOutputTx VSDiffuseMapTxVc(VSInputTxVc vin)
{
	PixelLightingVSOutputTx vout;
	
	CommonVSOutput cout = ComputeCommonVSOutput(vin.Position);
	//CommonVSOutput cout = ComputeCommonVSOutputWithLighting( vin.Position, vin.Normal );

	vout.PositionPS	= cout.Pos_ps;
	vout.Diffuse	= cout.Diffuse;
	vout.Diffuse.a  = 1.0;
	vout.PositionWS	= float4(cout.Specular, cout.FogFactor);
	vout.TexCoord	= vin.TexCoord;
	vout.NormalWS	= vin.Normal;
	
	return vout;
}

PixelLightingVSOutputTx VertexWobbleFunction( VSInputTxVc vin )
{
    PixelLightingVSOutputTx output;
    
    CommonVSOutput cout = ComputeCommonVSOutput( vin.Position );
    
    //radial wave sorta thing
    //float fDistance1 = length( g_vWaveLocation1 - g_vBoatPos );
	//float fDistance2 = length( g_vWaveLocation2 - g_vBoatPos );
	//float fDistance3 = length( g_vWaveLocation3 - g_vBoatPos );
	//float fDistance4 = length( g_vWaveLocation4 - g_vBoatPos );
    //float fDistance = length( g_vBoatPos );
    
    //NOTE: adding a small increment to time so it looks less static
    //float waveVal = 3.25 * sin( 0.05 * fDistance + ( (g_fTime + 0.25) * 2.0 ) );
    
	output.PositionPS = cout.Pos_ps;
	output.PositionWS = cout.Pos_ws;
	output.Diffuse	= cout.Diffuse;
	output.Diffuse.a  = 1.0;
	output.NormalWS	= vin.Normal;
	output.TexCoord	= vin.TexCoord;
	output.PositionPS.y += ComputeWaveValue(g_fTime + 0.1, g_vBoatPos);

    return output;
}

PixelLightingVSOutput VSWaterNmVc(VSInputNmVc vin)
{
	PixelLightingVSOutput vout;
	
	//CommonVSOutput cout = ComputeCommonVSOutput(vin.Position);
	CommonVSOutput cout = ComputeCommonVSOutputWithLighting( vin.Position, vin.Normal );
	
	vout.PositionWS = mul( vin.Position, World );
	float4 pos_vs = mul( vout.PositionWS, View );
	vout.PositionPS	= mul( pos_vs, Projection );
	vout.NormalWS = vin.Normal;
	vout.Diffuse = cout.Diffuse;
	
	//float WaveVal = sin( 0.05 * fDistance1 - g_fTime*2.0);
	
	float WaveVal = ComputeWaveValue(g_fTime, vout.PositionWS);
		
	vout.PositionPS.y += ComputeWaveValue(g_fTime, vout.PositionWS);
	//Temp Hackz
	vout.NormalWS += ComputeWaveNormal(g_fTime, vout.PositionWS);
	vout.NormalWS = normalize(vout.NormalWS);
	
	return vout;
}

PixelLightingVSOutputTx VSWaterNmVcTx(VSInputNmTxVc vin)
{
	PixelLightingVSOutputTx vout;
	
	//CommonVSOutput cout = ComputeCommonVSOutput(vin.Position);
	CommonVSOutput cout = ComputeCommonVSOutputWithLighting( vin.Position, vin.Normal );
	
	vout.PositionWS = mul( vin.Position, World );
	float4 pos_vs = mul( vout.PositionWS, View );
	vout.PositionPS	= mul( pos_vs, Projection );
	vout.TexCoord	= vin.TexCoord;
	vout.NormalWS = vin.Normal;
	vout.Diffuse = cout.Diffuse;
	
	//float WaveVal = sin( 0.05 * fDistance1 - g_fTime*2.0);
	
	float WaveVal = ComputeWaveValue(g_fTime, vout.PositionWS);
		
	vout.PositionPS.y += ComputeWaveValue(g_fTime, vout.PositionWS);
	//Temp Hackz
	vout.NormalWS += ComputeWaveNormal(g_fTime, vout.PositionWS);
	vout.NormalWS = normalize(vout.NormalWS);
	
	return vout;
}

// Alternative vertex shader outputs normal and depth values, which are then
// used as an input for the edge detection filter in PostprocessEffect.fx.
NormalDepthVertexShaderOutput NormalDepthVertexShader(VSInputNmTx input)
{
    NormalDepthVertexShaderOutput output;

    // Apply camera matrices to the input position.
    output.Position = mul(mul(mul(input.Position, World), View), Projection);
    
    float3 worldNormal = mul(input.Normal, World);

    // The output color holds the normal, scaled to fit into a 0 to 1 range.
    output.Color.rgb = (worldNormal + 1) / 2;

    // The output alpha holds the depth, scaled to fit into a 0 to 1 range.
    output.Color.a = output.Position.z / output.Position.w;
    
    return output;    
}

NormalDepthVertexShaderOutput NormalDepthWobbleVertexShader(VSInputNmTx input)
{
    NormalDepthVertexShaderOutput output;

    // Apply camera matrices to the input position.
    output.Position = mul(mul(mul(input.Position, World), View), Projection);
    output.Position.y += ComputeWaveValue(g_fTime + 0.1, g_vBoatPos);
    
    float3 worldNormal = mul(input.Normal, World);

    // The output color holds the normal, scaled to fit into a 0 to 1 range.
    output.Color.rgb = (worldNormal + 1) / 2;

    // The output alpha holds the depth, scaled to fit into a 0 to 1 range.
    output.Color.a = output.Position.z / output.Position.w;
    
    return output;    
}

NormalDepthVertexShaderOutput NormalDepthWaterVertexShader(VSInputNmTx input)
{
    NormalDepthVertexShaderOutput output;

    // Apply camera matrices to the input position.
    output.Position = mul(mul(mul(input.Position, World), View), Projection);
    
    float3 FixedNormals = input.Normal + ComputeWaveNormal(g_fTime + 0.1, mul(input.Position, World));
    FixedNormals.y *= 1;
	FixedNormals = normalize(FixedNormals);
	
    output.Position.y += ComputeWaveValue(g_fTime + 0.1, mul(input.Position, World));  
    
    float3 worldNormal = mul(FixedNormals, World);

    // The output color holds the normal, scaled to fit into a 0 to 1 range.
    output.Color.rgb = (worldNormal + 1) / 2;

    // The output alpha holds the depth, scaled to fit into a 0 to 1 range.
    output.Color.a = output.Position.z / output.Position.w;
    
    return output;    
}

//-----------------------------------------------------------------------------
// Pixel shaders
//-----------------------------------------------------------------------------

float4 PSBasicTx(PixelLightingPSInputTx pin) : COLOR
{
	
	ColorPair Colors = ComputePerPixelLights( EyePosition, mul(pin.NormalWS, World));
	
	float4 color;
	color.rgb = tex2D( TextureSampler, pin.TexCoord ) * (Colors.Diffuse + Colors.Specular.rgb);
	color.a = g_Alpha;
		
	//color.rgb = lerp(color.rgb, FogColor, pin.Specular.w);
	return color;
	
}

float4 PSDiffuseMapTx(PixelLightingPSInputTx pin) : COLOR
{
	
	ColorPair Colors = ComputePerPixelLights( EyePosition, mul(pin.NormalWS, World));
	
	float3 DiffuseMap;
	DiffuseMap.rgb = tex2D( DiffuseSampler, pin.TexCoord ) * (Colors.Diffuse + Colors.Specular.rgb);
	
	float4 TestColor = float4(1.0, 0.0, 0.0, 1.0);
	
	float4 color;
	color.rgb = tex2D( TextureSampler, pin.TexCoord ) * (Colors.Diffuse + Colors.Specular.rgb);
	color.a = g_Alpha;
	if (DiffuseMap.r > 0.1)
	{
	color.rgb = DiffuseColor.rgb * DiffuseMap.rgb;
	}
	//lerp(color.rgb, DiffuseColor.rgb, DiffuseMap.r);
	
		
	//color.rgb = lerp(color.rgb, FogColor, pin.Specular.w);
	return color;
	
}

float4 PSBasicTxNoLight(PixelLightingPSInput pin) : COLOR
{
	
	float4 color;
	color.rgb = DiffuseColor.rgb;
	color.a = g_Alpha;
		
	//color.rgb = lerp(color.rgb, FogColor, pin.Specular.w);
	return color;
	
}

float4 PSWater(PixelLightingPSInputTx pin) : COLOR
{

	float Comparison = dot(normalize(mul(EyePosition, World) - pin.PositionWS), pin.NormalWS);

	ColorPair Colors = ComputePerPixelLights( EyePosition, pin.NormalWS);
	
	float4 color;
	//color.rgb = float3( 0.5, 0.8, 0.95 );;
	color.rgb = tex2D( TextureSampler, pin.TexCoord );

	
	//color.rgb = tex2D( TextureSampler, pin.TexCoord ) * (Colors.Diffuse + Colors.Specular.rgb);
	color.a = 0.8 - Comparison/10;
		
	//color.rgb = lerp(color.rgb, FogColor, pin.Specular.w);
	return color;
}

float4 PSWaterNoTex( PixelLightingPSInputTx pin) : COLOR
{
	return float4( 0.5, 0.8, 0.95, 0.5 );
}

// Simple pixel shader for rendering the normal and depth information.
float4 NormalDepthPixelShader(float4 color: COLOR0) : COLOR0
{
    return color;
}

//-----------------------------------------------------------------------------
// Techniques
//-----------------------------------------------------------------------------

Technique BasicTech
{
	Pass
	{
		CullMode = CCW;
		
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		
		VertexShader = compile vs_2_0 VSBasicTxVc();
		PixelShader	 = compile ps_2_0 PSBasicTx();
		
		
	}
}

Technique DiffuseTech
{
	Pass
	{
		CullMode = CCW;
		
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		
		VertexShader = compile vs_2_0 VSDiffuseMapTxVc();
		PixelShader	 = compile ps_2_0 PSDiffuseMapTx();
		
		
	}
}

Technique BasicTechNoTex
{
	Pass
	{
		CullMode = CCW;
		
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		
		VertexShader = compile vs_2_0 VSBasicTxVc();
		PixelShader	 = compile ps_2_0 PSBasicTxNoLight();
		
		
	}
}

technique WobbleTech
{
    pass Pass0
    {
		CullMode = None;
		
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		
        VertexShader = compile vs_2_0 VertexWobbleFunction();
        PixelShader	 = compile ps_2_0 PSBasicTx();
    }
}

technique WobbleNoTexTech
{
    pass Pass0
    {
		CullMode = None;
		
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		
        VertexShader = compile vs_2_0 VertexWobbleFunction();
        PixelShader	 = compile ps_2_0 PSBasicTxNoLight();
    }
}

technique DiffuseWobbleTech
{
    pass Pass0
    {
		CullMode = None;
		
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		
        VertexShader = compile vs_2_0 VertexWobbleFunction();
        PixelShader	 = compile ps_2_0 PSDiffuseMapTx();
    }
}

technique WaterTech
{
    pass Pass0
    {
		CullMode = None;
		
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		
        VertexShader = compile vs_2_0 VSWaterNmVcTx();
        PixelShader	 = compile ps_2_0 PSWater();
    }
}

technique WaterTechNoTex
{
    pass Pass0
    {
		CullMode = None;
		
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		
        VertexShader = compile vs_2_0 VSWaterNmVcTx();
        PixelShader	 = compile ps_2_0 PSWaterNoTex();
    }
}

technique OnWaterTech
{
    pass Pass0
    {
		CullMode = None;
		
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		
        VertexShader = compile vs_2_0 VSWaterNmVc();
        PixelShader	 = compile ps_2_0 PSBasicTxNoLight();
    }
}

technique OnLandTech
{
    pass Pass0
    {
		CullMode = None;
		
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		
        VertexShader = compile vs_2_0 VSBasicTxVc();
        PixelShader	 = compile ps_2_0 PSBasicTxNoLight();
    }
}

// Technique draws the object as normal and depth values.
technique NormalDepth
{
    pass P0
    {
		CullMode = None;
		
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
    
        VertexShader = compile vs_1_1 NormalDepthVertexShader();
        PixelShader = compile ps_1_1 NormalDepthPixelShader();
    }
}

technique NormalDepthWobble
{
    pass P0
    {
		CullMode = None;
		
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
    
        VertexShader = compile vs_1_1 NormalDepthWobbleVertexShader();
        PixelShader = compile ps_1_1 NormalDepthPixelShader();
    }
}

technique NormalDepthWater
{
    pass P0
    {
		CullMode = None;
		
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
    
        VertexShader = compile vs_1_1 NormalDepthWaterVertexShader();
        PixelShader = compile ps_1_1 NormalDepthPixelShader();
    }
}
