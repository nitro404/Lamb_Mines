/**	@file	Basic.fx
	@brief	a basic shader to get this rolling
*/
uniform extern float4x4	g_matTransform;

struct VertexOut
{
	float4	vPosition	: POSITION;
	float4	vColor		: COLOR0;
};

VertexOut VertexShader( float4 vPos : POSITION0 )
{
	VertexOut vert = (VertexOut)0;
	
	vert.vPosition	= mul( vPos, g_matTransform );
	vert.vColor		= 0.5f;
	
	return vert;
}

float4 PixelShader( VertexOut vert ) : COLOR
{
	return vert.vColor;
}

technique BasicTech
{
	pass P0
	{
		vertexShader = compile vs_2_0 VertexShader();
		pixelShader  = compile ps_2_0 PixelShader();
	}
}