using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags
{
    /** @class  Coin
     *  @brief  the item the players are attempting to collect
     */
    public class Coin : Object3D
    {
        bool m_Draw;
        bool m_Exists;

        public bool Exists
        {
            set
            {
                m_Exists = value;
            }
        }

        public bool Drawable
        {
            set
            {
                m_Draw = value;
            }
        }
        /** @fn     Coin( Model mdl )
         *  @brief  Constructor
         *  @param  mdl [in] the model to use to display the Coin
         */
        public Coin( Model mdl ) : base(mdl)
        {
            m_Draw = true;
            m_Exists = true;
            Roll = (float)Math.PI / 2.0f;
        }

        /** @fn     void Update( float fElapsedTime )
         *  @brief  Updates the coin rotation every frame
         */
        public override void Update( float fElapsedTime )
        {
            Yaw += fElapsedTime;
            if (Yaw >= ((float)Math.PI / 180) * 360.0f)         //keeps the coin under 360.0f
            {
                Yaw = 0.0f;             
            }
        }

        /** @fn     bool checkCollide( Ship ship )
         *  @brief  check if a ship has collided with this coin
         *  @return true if the ship has collided, false otherwise
         *  @param  ship [in] the ship to check
         */
        public bool CheckCollide(Ship ship)
        {
            float Distance = (float)Math.Sqrt(Math.Pow(ship.Location.X - Location.X, 2) + Math.Pow(ship.Location.Z - Location.Z, 2));

            if (Distance < Settings.TARGET_RADIUS)
            {
                return true;
            }

            return false;
        }

        public override void Draw(GraphicsDevice device, Matrix matView, Matrix matProjection, Vector3 CameraPosition)
        {
            if (m_Exists)
            {
                if (m_Draw == true)
                    Alpha = 1.0f;
                else
                    Alpha = 0.0f;
                base.Draw(device, matView, matProjection, CameraPosition);
            }
        }
        
    }
}