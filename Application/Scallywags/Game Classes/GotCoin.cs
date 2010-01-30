using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags
{
    class GotCoin : Object3D
    {
        Vector3 m_BaseLocation;
        float m_MaxTime;
        float m_Timer;
        bool m_Enabled;

        public GotCoin(Model mdl)
            : base(mdl)
        {
            m_Enabled = false;
        }

        public void Launch(Vector3 LaunchSpot, float Duration)
        {
            Location = LaunchSpot;
            m_BaseLocation = LaunchSpot;
            m_MaxTime = Duration;
            m_Enabled = true;
            m_Timer = 0;
        }

        public override void Update(float fElapsedTime)
        {
            m_Timer += fElapsedTime;
            Roll += fElapsedTime * 5;

            Alpha = 1.0f - m_Timer / m_MaxTime;

            Y = m_BaseLocation.Y + (Settings.TOWER_COIN_HEIGHT * (m_Timer / m_MaxTime));

            if (m_Timer > m_MaxTime)
                m_Enabled = false;

            base.Update(fElapsedTime);
        }

        public override void Draw(GraphicsDevice device, Matrix matView, Matrix matProjection, Vector3 CameraPosition)
        {
            if (m_Enabled == true)
                base.Draw(device, matView, matProjection, CameraPosition);
        }


    }
}
