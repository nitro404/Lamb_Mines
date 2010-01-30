#include "BasicEffect.fx"

/*extern float4x4	g_matWorld;
extern float4x4	g_matView;
extern float4x4	g_matProj;

/*
struct VertexInfo
{
    float4 pos : POSITION0;
};

struct VertexOut
{
	float4 pos : POSITION0;
	float4 col : COLOR0;
};
*/

VertexLightingVSOutputTx VertexWobbleFunction( VSInputNmTx vert )
{
    VertexLightingVSOutputTx output = (VertexLightingVSOutputTx)0;
    
    //vert.pos.y = 1.0;	//Why would this mess up water rendering, but not targeters?
    
    float4 pos_ws = mul( vert.Position, g_matWorld );
	float4 pos_vs = mul( pos_ws, g_matView );
	float4 pos_ps = mul( pos_vs, g_matProj );
	
    output.PositionPS = pos_ps;
    
    //radial wave sorta thing
    float fDistance = length( g_vBoatPos );
    float waveVal = 3.25 * sin( 0.05 * fDistance + ( g_fTime * 1.0 ) );
	//output.pos.y;
	/*
	//find the length of the vertex position
	float3 positionVec;
	positionVec.x = output.pos.x - g_vBoatPos.x;
	positionVec.y = output.pos.y - g_vBoatPos.y;
	positionVec.z = output.pos.z - g_vBoatPos.z;
	float dist = abs(length(positionVec));
	
	//now find the current rotation radius
	float L = asin(positionVec.y/dist);
	//increase the radius by the wave value
	L += waveVal;
	
	//now find the new position of the vertex
	positionVec.y = dist*sin(L);
	float P = sqrt( dist*dist - positionVec.y*positionVec.y );
	positionVec.z = P*sin(g_fBoatRotation);
	positionVec.x = sqrt( positionVec.z*positionVec.z - P*P );
	
	output.pos.x = g_vBoatPos.x + positionVec.x;
	output.pos.y = g_vBoatPos.y + positionVec.y;
	output.pos.z = g_vBoatPos.z + positionVec.z;
	*/
	
	output.PositionPS.y += waveVal;
	
	output.Diffuse.rgb = Diffuse;
	output.Diffuse.a = 1.0f;
	output.TexCoord = vert.TexCoord;

    return output;
}

/*
float4 PixelShaderFunction(VertexOut vert ) : COLOR0
{
    return vert.col;
}
*/

technique WobbleTech
{
    pass Pass0
    {
		CullMode = None;
		
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader	 = compile ps_2_0 PSBasicTx();
    }
}

*/
