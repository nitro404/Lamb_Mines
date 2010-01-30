extern float4x4	g_matWorld;
extern float4x4	g_matView;
extern float4x4	g_matProj;

extern float	g_fTime;
extern float4	g_vDiffuse;

struct VertexInfo
{
    float4 pos : POSITION0;
};

struct VertexOut
{
	float4 pos : POSITION0;
	float4 col : COLOR0;
};

VertexOut VertexShaderFunction( VertexInfo vert )
{
    VertexOut output = (VertexOut)0;
    
    //vert.pos.y = 1.0;	//Why would this mess up water rendering, but not targeters?
    
    float4 pos_ws = mul( vert.pos, g_matWorld );
	float4 pos_vs = mul( pos_ws, g_matView );
	float4 pos_ps = mul( pos_vs, g_matProj );
	
    output.pos = pos_ps;
    
    //radial wave sorta thing
    float fDistance = length( pos_ws );
	output.pos.y += 3.25 * sin( 0.05 * fDistance + ( g_fTime * 2.0 ) );
	
	output.col = g_vDiffuse;

    return output;
}

float4 PixelShaderFunction(VertexOut vert ) : COLOR0
{
    return vert.col;
}

technique WaterTech
{
    pass Pass0
    {
		CullMode = CCW;
		
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
