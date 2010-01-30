using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags
{
    public class ShipGold : Object3D
    {

        bool m_Enabled;

        public bool Enabled
        {
            get
            {
                return m_Enabled;
            }
            set
            {
                m_Enabled = value;
            }
        }

        public ShipGold(Model mdl)
            : base(mdl)
        {
            m_Enabled = false;
            Color = Color.Gold;
        }

        public override void Draw(GraphicsDevice device, Matrix matView, Matrix matProjection, Vector3 CameraPosition)
        {
            if (m_Enabled == true)
            {

                //create the model's world transform matrix
                Matrix matWorld = Matrix.Identity *                 //clear
                    Matrix.CreateScale(Scale) *                  //scale
                    Matrix.CreateFromYawPitchRoll(Yaw, Pitch, Roll) *   //Rotation
                    Matrix.CreateTranslation(Location);          //translate

                if (Settings.DETECT_EDGES == true)
                    Shaders[0].CurrentTechnique = m_lstEffects[0].Techniques["NormalDepthWobble"];
                else
                {
                    Shaders[0].CurrentTechnique = Shaders[0].Techniques["WobbleNoTexTech"];
                }

                foreach (Effect effect in m_lstEffects)
                {
                    if (effect.Parameters["g_vBoatPos"] != null)
                        effect.Parameters["g_vBoatPos"].SetValue(Location);

                    if (effect.Parameters["g_fBoatRotation"] != null)
                        effect.Parameters["g_fBoatRotation"].SetValue(Yaw);
                }


                foreach (Effect effect in m_lstEffects)
                {
                    ////////////////////////////
                    //Basic Effect parameters
                    if (effect.Parameters["World"] != null)
                        effect.Parameters["World"].SetValue(matWorld);

                    if (effect.Parameters["View"] != null)
                        effect.Parameters["View"].SetValue(matView);

                    if (effect.Parameters["Projection"] != null)
                        effect.Parameters["Projection"].SetValue(matProjection);

                    if (effect.Parameters["g_Alpha"] != null)
                        effect.Parameters["g_Alpha"].SetValue(m_Alpha);

                    //Material colour
                    if (effect.Parameters["DiffuseColor"] != null && m_Color != Color.TransparentWhite)
                        effect.Parameters["DiffuseColor"].SetValue(m_Color.ToVector3());
                    else if (effect.Parameters["DiffuseColor"] != null && m_lstMaterials.Count > 0)
                        effect.Parameters["DiffuseColor"].SetValue(m_lstMaterials[0]);

                    //Active texture
                    if (effect.Parameters["BasicTexture"] != null && m_lstTextures.Count > 0)
                        effect.Parameters["BasicTexture"].SetValue(m_lstTextures[0]);

                    if (effect.Parameters["DiffuseMap"] != null && m_tDiffuse != null)
                        effect.Parameters["DiffuseMap"].SetValue(m_tDiffuse);

                    if (effect.Parameters["EyePosition"] != null)
                        effect.Parameters["EyePosition"].SetValue(CameraPosition);

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

                }

                Shaders[0].CurrentTechnique = Shaders[0].Techniques["BasicTech"];

            }
        }

    }
}
