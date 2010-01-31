using System;
using System.Collections;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

namespace LambMines
{
    class GlobalHelpers
    {

        //TEMP TESTS
        public static Vector2 GetScreenCoords(Vector2 iso)
        {
            Vector2 result;
            result.X = GetScreenX(iso);
            result.Y = GetScreenY(iso);
            return result;
        }

        public static float GetScreenX(Vector2 iso)
        {
            float xBasic = (float)(iso.X - iso.Y) * (float)Math.Cos(0.46365);
			//float xBasic = ((float)Math.Cos(0.46365) * iso.X) + ((-(float)Math.Sin(0.46365)) * iso.Y);
			//float xBasic = ((float)Math.Cos(0.610865238) * iso.X) + ((-(float)Math.Sin(0.610865238)) * iso.Y);
            return xBasic;
        }

        public static float GetScreenY(Vector2 iso)
        {
            float yBasic = (float)(iso.X + iso.Y) * (float)Math.Sin(0.46365);
			//float yBasic = ((float)Math.Sin(0.46365) * iso.X) + ((float)Math.Cos(0.46365) * iso.Y);
			//float yBasic = ((float)Math.Sin(0.610865238) * iso.X) + ((float)Math.Cos(0.610865238) * iso.Y);
            return yBasic;
        }
    }
}
