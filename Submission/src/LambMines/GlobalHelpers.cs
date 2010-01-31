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

        public static Vector2 getTilePositionOffset(Vector2 location, float Height)
        {
            Vector2 newPos = new Vector2(location.X, location.Y - Height + 45);
            return newPos;
        }
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

			double constCos = Math.Cos(Math.PI / 4.0);
			double constSin = Math.Sin(Math.PI / 4.0);

			double xPos = (constCos * iso.X) + ((-constSin) * iso.Y);
			//double yPos = (constSin * iso.X) + (constCos * iso.Y);

			xPos *= (45.0 / 32.0);
			//yPos *= (45.0 / 64.0);

			return xBasic;//(float)xPos;//
        }

        public static float GetScreenY(Vector2 iso)
        {
            float yBasic = (float)(iso.X + iso.Y) * (float)Math.Sin(0.46365);

			double constCos = Math.Cos(Math.PI / 4.0);
			double constSin = Math.Sin(Math.PI / 4.0);
			double yPos = (constSin * iso.X) + (constCos * iso.Y);
			yPos *= (45.0 / 64.0);

			return yBasic;//(float)yPos;//
        }
    }
}
