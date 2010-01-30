using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags
{
    public class ChestOpen : Object3D
    {
        bool m_Opening;
        bool m_Closing;

        /// <summary>
        /// This Class is for the ChestLid.X
        /// </summary>
        /// <param name="mdl"></param>
        public ChestOpen(Model mdl)
            : base(mdl)
        {
            m_Opening = false;
            m_Closing = false;
        }

        /// <summary>
        /// If Openning send in true, and if closing send in false
        /// </summary>
        /// <param name="value"></param>
        public void Openning(bool value)
        {
            if (value)
            {
                m_Opening = true;
                m_Closing = false;
            }
            else
            {
                m_Closing = true;
                m_Opening = false;
            }
        }

        public override void Update(float fElapsedTime)
        {
            if (m_Opening)
            {
                
                if (Pitch >= ((float)Math.PI / 180) * 45.0f)
                {
                    Pitch = ((float)Math.PI / 180) * 45.0f;
                }
                else
                {
                    Pitch += fElapsedTime;
                }
            }
            else if (m_Closing)
            {
                
                if (Pitch <= ((float)Math.PI / 180) * 0.0f)
                {
                    Pitch = ((float)Math.PI / 180) * 0.0f;
                }
                else 
                { 
                    Pitch -= fElapsedTime;
                }
            }            
        }

        public override void Draw(GraphicsDevice device, Matrix matView, Matrix matProjection, Vector3 CameraPosition)
        {
            base.Draw(device, matView, matProjection, CameraPosition);            
        }
    }
}
