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
    public class Seaweed : Object3D
    {
        string m_strTech;           ///< The name of the technique to use for the water
        //float m_Phase;              ///< To alter the wave phase for each seaweed strand in the shader

        public string TechniqueName 
        {
            set
            {
                m_strTech = value;
            }
        } 
        
        public Seaweed(Model mdl)
            : base(mdl)
        {   
            Location    = new Vector3(0, 0, 0);
            Roll = 0.0f;
            Pitch = 0.0f;
            Yaw = 0.0f;
            Scale = 1.0f;
            //m_Phase = 1.0f;            
        }

        public Seaweed()
            : base()
        {
            Location    = new Vector3(0, 0, 0);
            Roll = 0.0f;
            Pitch = 0.0f;
            Yaw = 0.0f;
            Scale = 1.0f;
            //m_Phase = 1.0f;
        }  

        /** @fn     void Draw(Matrix matView, Matrix matProjection)
         *  @brief  draw the game object
         *  @param  device [in] the rendering device
         *  @param  matView [in] the active view matrix
         *  @param  matProjection [in] the active projection
         */
        public override void Draw(GraphicsDevice device, Matrix matView, Matrix matProjection, Vector3 CameraPosition)
        {
            Matrix matWorld = Matrix.Identity *                 //clear
            Matrix.CreateScale(ScaleVec) *                      //scale
            Matrix.CreateFromYawPitchRoll(Yaw, Pitch, Roll) *   //Rotation
            Matrix.CreateTranslation(Location);
            //device.RenderState.FillMode = FillMode.WireFrame;

            //Store the previous effect
            EffectTechnique prevEffect = Shaders[0].CurrentTechnique;

            //Set the active effect
            if (Settings.DETECT_EDGES == true)
                Shaders[0].CurrentTechnique = Shaders[0].Techniques["NormalDepthWater"];
            else
                Shaders[0].CurrentTechnique = Shaders[0].Techniques[m_strTech];

            foreach (Effect effect in Shaders)
            {
               /* if (effect.Parameters["g_vWaveLocation1"] != null)
                    effect.Parameters["g_vWaveLocation1"].SetValue(WaveLocations[0]);
                if (effect.Parameters["g_vWaveLocation2"] != null)
                    effect.Parameters["g_vWaveLocation2"].SetValue(WaveLocations[1]);
                if (effect.Parameters["g_vWaveLocation3"] != null)
                    effect.Parameters["g_vWaveLocation3"].SetValue(WaveLocations[2]);
                if (effect.Parameters["g_vWaveLocation4"] != null)
                    effect.Parameters["g_vWaveLocation4"].SetValue(WaveLocations[3]);
                */
                //hacks here...
                if (effect.Parameters["g_matTransform"] != null)
                {
                    Matrix matTransform = matWorld * matView * matProjection;
                    effect.Parameters["g_matTransform"].SetValue(matTransform);
                }


                ////////////////////////////
                //Basic Effect parameters
                if (effect.Parameters["World"] != null)
                    effect.Parameters["World"].SetValue(matWorld);

                if (effect.Parameters["View"] != null)
                    effect.Parameters["View"].SetValue(matView);

                if (effect.Parameters["Projection"] != null)
                    effect.Parameters["Projection"].SetValue(matProjection);

                //Material colour
                if (effect.Parameters["DiffuseColor"] != null && m_lstMaterials.Count > 0)
                    effect.Parameters["DiffuseColor"].SetValue(m_lstMaterials[0]);

                if (effect.Parameters["EyePosition"] != null)
                    effect.Parameters["EyePosition"].SetValue(CameraPosition);

                //Active texture
                if (effect.Parameters["BasicTexture"] != null && m_lstTextures.Count > 0)
                    effect.Parameters["BasicTexture"].SetValue(m_lstTextures[0]);

                ///////////////////////////////////////////
                //Render the mesh with the current effect

                effect.Begin();

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Begin();

                    //mesh.Draw();
                    DrawMesh(device);

                    pass.End();
                }

                effect.End();
            }//end shader loop

            //Replace the previous effect
            Shaders[0].CurrentTechnique = prevEffect;
        }
    }
}
