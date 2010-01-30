using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags
{
    public class SeaweededLake : Object3D
    {
        private Seaweed[] m_Seaweed;

        public Seaweed[] Seaweed
        {
            get
            {
                return m_Seaweed;
            }
        }

        public SeaweededLake(Model mdlSeaweed, int nNumSeaweed, Effect underwaterEffect)
            : base(mdlSeaweed)
        {
            m_Seaweed = new Seaweed[nNumSeaweed];

            for (int i = 0; i < nNumSeaweed; ++i)
            {
                m_Seaweed[i] = new Seaweed(mdlSeaweed);
                m_Seaweed[i].Shaders.Add(underwaterEffect);                
            }
        }

        public void InitSeaweed()
        {
            //The Seaweed starting locations
            Vector3[] vSeaweedLocations = {
                new Vector3(-60.0f, 0.0f,   5.0f ),    
                new Vector3(   0.0f, 0.0f, 60.0f ),    
                new Vector3( 60.0f, 0.0f,   5.0f ),    
                new Vector3(   0.0f, 0.0f, -60.0f ),     
                new Vector3(   0.0f, 0.0f, -60.0f ),     
                new Vector3(   0.0f, 0.0f, -60.0f )     
                 };
            
            float[] vSeaweedYaw = {
                  0.0f,  0.0f,  0.0f ,
                  0.0f,  0.0f,  0.0f 
                };
            float[] vSeaweedPitch = {
                  0.0f,  0.0f,  0.0f ,
                  0.0f,  0.0f,  0.0f 
                };
            float[] vSeaweedRoll = {
                  0.0f,  0.0f,  0.0f ,
                  0.0f,  0.0f,  0.0f 
                };
            float[] vSeaweedScale = {
                  0.0f,  0.0f,  0.0f ,
                  0.0f,  0.0f,  0.0f 
                };
            //Init the objects
            for (int i = 0; i < m_Seaweed.Length; ++i)
            {
                m_Seaweed[i].Location = vSeaweedLocations[i];                
                m_Seaweed[i].Yaw = vSeaweedYaw[i];
                m_Seaweed[i].Pitch = vSeaweedPitch[i];
                m_Seaweed[i].Roll = vSeaweedRoll[i];
                m_Seaweed[i].Scale = vSeaweedScale[i];               
            }
        }

        public override void Draw(GraphicsDevice device, Matrix matView, Matrix matProjection, Vector3 CameraPosition)
        {
            base.Draw(device, matView, matProjection, CameraPosition);

            //Draw the Seaweed
            for (int i = 0; i < m_Seaweed.Length; ++i)
            {
                m_Seaweed[i].Draw(device, matView, matProjection, CameraPosition);              
            }
        }

        public override void Update(float fElapsedTime)
        {
            foreach (Seaweed seaweeds in m_Seaweed)
            { 
                seaweeds.Update(fElapsedTime);
                //seaweeds.Yaw += fElapsedTime * 4.0f;

                //if (seaweeds.Yaw > MathHelper.TwoPi)
                //    seaweeds.Yaw -= MathHelper.TwoPi;
            }
            base.Update(fElapsedTime);
        }

    }
}
