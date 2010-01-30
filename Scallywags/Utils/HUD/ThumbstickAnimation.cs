using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags
{
    public class ThumbstickAnimation
    {
        private Animation m_aniThumbstick;
        private Vector2   m_vLocation;

        public ThumbstickAnimation()
        {
            m_aniThumbstick = null;
        }

        public void Init( float fX, float fY, Dictionary< string, Texture2D > dtTextures )
        {
            AnimatedTexture aniThumb = new AnimatedTexture( dtTextures[ "thubstickAni2" ], 72, 67 );
            m_aniThumbstick          = new Animation( aniThumb, 0, 59, true, 0.005f );

            m_vLocation              = new Vector2( fX, fY );

            m_aniThumbstick.Start();
        }

        public void Draw( SpriteBatch spriteBatch )
        {
            m_aniThumbstick.Draw( new Vector2( m_vLocation.X, m_vLocation.Y ), 0.0f, spriteBatch );
        }

        public void Update( float fElapsedTime )
        {
            m_aniThumbstick.Update( fElapsedTime );
        }
    }
}
