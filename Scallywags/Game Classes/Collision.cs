//#define COLLISION_MODE_ON
//#define VIEW_COLLISION_LINES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

using System.IO;

namespace Scallywags {
    public class Collision {

#region Data Members
       
        BasicEffect eff;
        private float[,] m_terrainMap;
        Object3D m_anObject;
        XNAModularApp m_ParentApp;
        collisionInfo theInfo;
        const int SCALE_VAL = 12;
        Player[] m_AllPlayers;

#endregion

#region Functionality

        public Collision() {
        }
        /*  @func Init
         *  @brief Initializes the collision lines
         *  @param GraphicsDevice device The current Graphics Device used to create a BasicEffect
         */
        public void Init(GraphicsDevice device, Object3D anObject, Object3D anObject2, string terrainFileName, XNAModularApp parentApp, Player[] allPlayers, Texture2D terrainHM)
        {
            m_ParentApp = parentApp;
            m_anObject = anObject2;
            //Texture2D wha = terrainHM;//new Texture(terrainFileName);

            m_terrainMap = new float[51,51];

            //float maxVal = 0;//221-78=94.121
            //float minVal = 255;//78-48.897
            m_AllPlayers = allPlayers;
            //VertexPositionTexture[] buffer = new VertexPositionTexture[anObject.TheModel.Meshes[0].VertexBuffer.SizeInBytes / VertexPositionTexture.SizeInBytes];
            //anObject.TheModel.Meshes[0].VertexBuffer.GetData(buffer);
            int[] allinfo = new int[anObject.TheModel.Meshes[0].MeshParts[0].NumVertices];
            float[] what = new float[anObject.TheModel.Meshes[0].VertexBuffer.SizeInBytes/sizeof(float)];
            anObject.TheModel.Meshes[0].VertexBuffer.GetData(what);

            //float[,] terrainMap = new float[51,51];
            float xVal = 0;
            float yVal = 0;
            float zVal = 0;
            int increment = anObject.TheModel.Meshes[0].MeshParts[0].VertexStride/4;
            for (int i = 0; i < what.Length; i += increment)
            {
                xVal = what[i];
                yVal = what[i + 1];
                zVal = what[i + 2];
                xVal += 300; xVal /= 12;
                zVal += 300; zVal /= 12;
                if ((xVal < 51) && (zVal  < 51)) {
                    if (xVal > 0 && zVal > 0) {
                        m_terrainMap[(int)xVal, (int)zVal] = yVal;
                    }
                }
            }

            /*
            //Color[] pixels = new Color[wha.Width * wha.Height];
            wha.GetData<Color>(pixels);
            
            for (int x = 0; x < wha.Width; x++) {
                for (int y = 0; y < wha.Height; y++) {
                    Color colPixel = pixels[y * wha.Width + x];
                    m_terrainMap[x, y] = colPixel.R;//(((float)(colPixel.R) - 79) * 100 / 150) - 54;
                    maxVal = Math.Max(maxVal, colPixel.R);
                    minVal = Math.Min(minVal, colPixel.R);
                }
            }
            for (int x = 0; x < wha.Width; x++)
            {
                for (int y = 0; y < wha.Height; y++)
                {
                    //heightVal-minimum number = V
                    //v/(143/96.018) = N
                    //N-lowestHeightVal
                    // EQUATION:  I take the Color from the heightmap, offset the lowest R value to 0, multiply by a ratio of the difference in Color and Height, and subtract to place in the correct height 
                    //231,94
                    // m_terrainMap[x, y] = (((float)(wha.GetPixel(x, y).R) - 78) / (143f / 96.018f)) - 48.879f;//(((float)(wha.GetPixel(x, y).R)) / 255.0f) * (47.139f + 48.879f) -48.879f ;
                    //m_terrainMap[x, y] = (m_terrainMap[x, y] - minVal) / (43.627f / -63.847f);
                    //Color colPixel = pixels[y * wha.Width + x];

                    m_terrainMap[x, y] = ((m_terrainMap[x, y] - minVal) / (maxVal - minVal) * (43.627f + 63.847f)) - 60.847f;


                    //maxVal = Math.Max(maxVal, colPixel.R);
                    //minVal = Math.Min(minVal, colPixel.R);
                }
            }
            */
            eff = new BasicEffect(device, null);//Create a ne BasicEfect

        }
        /*  @func DoesCollide
         *  @brief Returns a vector2 position object for the correct location of the ship.
         *  The position changes of the ship has passes through one of the collision lines
         *  @param Vector2 oldLocation The previous location of the ship
         *  @param Vector2 newLocation The desired location of the ship
         *  @return The correct location of the ship
         */
        public Vector2 DoesCollide(Vector2 oldLocation, Vector2 newLocation) {
            if (oldLocation == newLocation) {
                return newLocation;
            }

            theInfo = findHeight(newLocation);//get the hight at the current loction on the terrain
            if (theInfo.heightAtPoint >= -2.0f) {//if the terrain is above the water
                
                
                return oldLocation;
                //}
            }
            else {
                //Check the other players ships here.
                bool bubbleCollide = false;
                Vector2 theNewerPos = newLocation;
                for(int i = 0; i < m_AllPlayers.Length; i++){
                    for (int c = 0; c < m_AllPlayers[i].totalShips; c++){
                        if (m_AllPlayers[i].Ships[c].isDisabled) { continue; }
                        Vector2 diff = new Vector2(m_AllPlayers[i].Ships[c].Location.X, m_AllPlayers[i].Ships[c].Location.Z);
                        diff = diff - newLocation;
                        if (diff.X == 0 && diff.Y == 0) { continue; }
                        if (diff.Length() < Settings.TARGET_RADIUS) {
                            diff.Normalize();
                            diff *= Settings.TARGET_RADIUS;
                            if (!bubbleCollide) {//if the bubble has not alredy collided once
                                bubbleCollide = true;
                                theNewerPos = new Vector2(m_AllPlayers[i].Ships[c].Location.X - diff.X, m_AllPlayers[i].Ships[c].Location.Z - diff.Y);
                            }
                            else {//if the bubble has collided twice then just return it to it's old poition, it can't move.
                                return oldLocation;
                            }
                        }
                    }
                }
                if (bubbleCollide) {
                    return theNewerPos;//move the ship
                }
                return newLocation;
            }
        }
        /*  @func Draw
         *  @brief This is the standard draw function
         */
        public void Draw(GraphicsDevice device, Matrix matView, Matrix matProj) {
#if COLLISION_MODE_ON
            //create the model's world transform matrix
            Matrix matWorld = Matrix.Identity *                         //clear
                Matrix.CreateScale(1.0f) *                        //scale
                Matrix.CreateFromYawPitchRoll(0.0f, 0.0f, 0.0f) *      //Rotation
                Matrix.CreateTranslation(0.0f, 0.0f, 0.0f);
            //set the maticies to the shader
            eff.Parameters["World"].SetValue(matWorld);
            eff.Parameters["View"].SetValue(matView);
            eff.Parameters["Projection"].SetValue(matProj);
            eff.Begin(SaveStateMode.SaveState);//begin the shader rendering
            m_anObject.Scale = 0.001f;

            Vector3 oldPos = m_anObject.Location;
            foreach (EffectPass pass in eff.CurrentTechnique.Passes) {
                pass.Begin();//begin this pass of the shader
                //declare the vertexes and then draw them
                device.VertexDeclaration = new VertexDeclaration(device, VertexPositionColor.VertexElements);
                for (int i = 0; i < 51; i++) {
                    for (int c = 0; c < 51; c++) {
                        //-18 to 47
                        //((height/256)*(47+18))-18
                        m_anObject.Location = new Vector3(i * SCALE_VAL - 300, m_terrainMap[i, c], c * SCALE_VAL - 300);

                        //if (m_terrainMap[i, c] >= 0) {
                        //device.DrawUserPrimitives(PrimitiveType.LineList, m_lineList, 0, m_collisionList.Count);
                        m_anObject.Draw(device, matView, matProj, Vector3.Zero);
                        //}

                    }
                }
                pass.End();
            }

            m_anObject.Location = oldPos;
            m_anObject.Scale = 1.0f;
            eff.End();
            //m_ParentApp.DebugFont.DrawFont(new SpriteBatch(device), "Height Here : " + theInfo.heightAtPoint.ToString(), 0.0f, 0.65f, Microsoft.Xna.Framework.Graphics.Color.White);
#endif
#if VIEW_COLLISION_LINES
            int interpPoints = 36;
            
            float step = MathHelper.TwoPi / interpPoints;
            VertexPositionColor[] theBuff = new VertexPositionColor[5 + interpPoints*(2+4)];


            theBuff[0].Position = new Vector3(-230, 7, -230); theBuff[0].Color = Color.Red;
            theBuff[1].Position = new Vector3(-230, 7, 230); theBuff[1].Color = Color.Red;
            theBuff[2].Position = new Vector3(230, 7, 230); theBuff[2].Color = Color.Red;
            theBuff[3].Position = new Vector3(230, 7, -230); theBuff[3].Color = Color.Red;
            theBuff[4].Position = new Vector3(-230, 7, -230); theBuff[4].Color = Color.Red;

            float Radius = 75;
            for (int i = 0; i < interpPoints; i++){
                float x = Radius * (float)Math.Cos(step*i );
                float z = Radius * (float)Math.Sin(step*i );
                theBuff[5+i].Position = new Vector3(x, 7, z); theBuff[5+i].Color = Color.Red;
            }
            Radius = 270;
            for (int i = 0; i < interpPoints; i++) {
                float x = Radius * (float)Math.Cos(step * i);
                float z = Radius * (float)Math.Sin(step * i);
                theBuff[5 + i + interpPoints].Position = new Vector3(x, 7, z); theBuff[5 + i + interpPoints].Color = Color.Red;
            }

            Radius = 30;
            for (int i = 0; i < interpPoints; i++) {
                float x = Radius * (float)Math.Cos(step * i);
                float z = Radius * (float)Math.Sin(step * i);
                theBuff[5 + i + interpPoints*2].Position = new Vector3(x+75, 7, z); theBuff[5 + i + interpPoints*2].Color = Color.Red;
            }
            for (int i = 0; i < interpPoints; i++) {
                float x = Radius * (float)Math.Cos(step * i);
                float z = Radius * (float)Math.Sin(step * i);
                theBuff[5 + i + interpPoints*3].Position = new Vector3(x-75, 7, z); theBuff[5 + i + interpPoints*3].Color = Color.Red;
            }
            for (int i = 0; i < interpPoints; i++) {
                float x = Radius * (float)Math.Cos(step * i);
                float z = Radius * (float)Math.Sin(step * i);
                theBuff[5 + i + interpPoints*4].Position = new Vector3(x, 7, z+75); theBuff[5 + i + interpPoints*4].Color = Color.Red;
            }
            for (int i = 0; i < interpPoints; i++) {
                float x = Radius * (float)Math.Cos(step * i);
                float z = Radius * (float)Math.Sin(step * i);
                theBuff[5 + i + interpPoints*5].Position = new Vector3(x, 7, z-75); theBuff[5 + i + interpPoints*5].Color = Color.Red;
            }




            device.DrawUserPrimitives(PrimitiveType.LineStrip, theBuff, 0, 4+interpPoints*(2+4));
#endif
        }

        #region Support
        //This functon will calculate the height on the terrain at the specified location
        public collisionInfo findHeight(Vector2 objectPos) {
            collisionInfo returnVal;

            //Avoiding a first pass exception here... m_terrainMap doesn't exist when this is first called. - Mike.
            if( m_terrainMap == null )
                return new collisionInfo();

            try
            {
                Vector2 gridPos = Vector2.Zero;
                //float tempLoc;

                //find the xPosition on the grid
                gridPos.X = (int)Math.Floor(objectPos.X + 300) - ((int)Math.Floor(objectPos.X + 300) % SCALE_VAL);
                gridPos.Y = (int)Math.Floor(objectPos.Y + 300) - ((int)Math.Floor(objectPos.Y + 300) % SCALE_VAL);
                //find the three points on the plane;
                Vector3 A = Vector3.Zero, B = Vector3.Zero, C = Vector3.Zero;

                //if ((int)(gridPos.X / SCALE_VAL) > m_BoundsX || (int)(gridPos.Y / SCALE_VAL) > m_BoundsY || (int)(gridPos.X / SCALE_VAL) < 0 || (int)(gridPos.Y / SCALE_VAL) < 0)
                //{

                //}

                A.X = gridPos.X;
                A.Y = gridPos.Y;
                A.Z = m_terrainMap[(int)(gridPos.X / SCALE_VAL), (int)(gridPos.Y / SCALE_VAL)];
                B.X = gridPos.X + SCALE_VAL;
                B.Y = gridPos.Y;
                B.Z = m_terrainMap[(int)(B.X / SCALE_VAL), (int)(B.Y / SCALE_VAL)];
                C.X = gridPos.X;
                C.Y = gridPos.Y + SCALE_VAL;
                C.Z = m_terrainMap[(int)(C.X / SCALE_VAL), (int)(C.Y / SCALE_VAL)];

                //Calculate the Vectors product
                Vector3 AB = Vector3.Zero, AC = Vector3.Zero;

                AB.X = A.X - B.X;
                AB.Y = A.Y - B.Y;
                AB.Z = A.Z - B.Z;

                AC.X = A.X - C.X;
                AC.Y = A.Y - C.Y;
                AC.Z = A.Z - C.Z;

                //Calculate the Normal
                Vector3 normal = Vector3.Zero;
                normal.X = (AB.Y * AC.Z) - (AB.Z * AC.Y);
                normal.Y = (AB.Z * AC.X) - (AB.X * AC.Z);
                normal.Z = (AB.X * AC.Y) - (AB.Y * AC.X);
                //Find D using Ax+By+Cz +D = 0
                //D = -Ax-By-Cz
                float D = 0;
                D = -(normal.X * B.X) - (normal.Y * B.Y) - (normal.Z * B.Z);

                //use the finished equasion to find the height of the position in the terrain
                returnVal = new collisionInfo(B, C, normal,
                ((-(normal.X * (objectPos.X + 300)) - (normal.Y * (objectPos.Y + 300)) - D) / normal.Z));
            }
            catch//If out of Bounds
            {
                returnVal = new collisionInfo(Vector3.Zero, Vector3.Zero, Vector3.Zero,
                0.0f);

                //Error.Trace( "Error in find height: " + ex.Message );
                //Error.Trace( "Stack trace:\n\n" + ex.StackTrace );
            }

            return returnVal;
        }

        #endregion

    }
        #endregion

    #region Struct

    public struct collisionInfo {
        public collisionInfo(Vector3 point1, Vector3 point2,Vector3 normalVec, float height) {
            collisionPoint1 = new Vector2(point1.X, point1.Z);
            collisionPoint2 = new Vector2(point2.X, point2.Z);
            heightAtPoint = height;
            collisionNormal = normalVec;
            isInfiniteSlope = false;
            if (collisionPoint2.X - collisionPoint1.X == 0) {
                isInfiniteSlope = true;
                M = 0;
                B = 0;
            }
            else {
                //slope = rise/run
                M = (collisionPoint2.Y - collisionPoint1.Y) / (collisionPoint2.X - collisionPoint1.X);
                //B = Y-M*X
                B = collisionPoint2.Y - M * collisionPoint2.X;
                //Equation of the line is Y = MX + B
            }

        }
        public Vector2 collisionPoint1;
        public Vector2 collisionPoint2;
        public Vector3 collisionNormal;
        public float heightAtPoint;
        public bool isInfiniteSlope;
        public float M;
        public float B;
    }
    #endregion

}
