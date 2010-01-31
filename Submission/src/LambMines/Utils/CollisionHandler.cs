using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

namespace LambMines
{
    class CollisionHandler
    {

        public static bool CheckLineIntersection(Vector2 pA, Vector2 pB, Vector2 pC, Vector2 pD, Vector2 PoI)
        {
            Vector2 a = pA; Vector2 b = pB; Vector2 c = pC; Vector2 d = pD;
            //Is Line Undefined
            if (a.X == b.X && a.Y == b.Y || c.X == d.X && c.Y == d.Y)
                return false;

            //Do lines share an End Point
            if (a.X == c.X && a.Y == c.Y || b.X == c.X && b.Y == c.Y ||
                a.X == d.X && a.Y == d.Y || b.X == d.X && b.Y == d.Y)
                return false;

            //Translate so that PointA is on the Origin
            b.X -= a.X; b.Y -= a.Y;
            c.X -= a.X; c.Y -= a.Y;
            d.X -= a.X; d.Y -= a.Y;

            //Get the Length of AB
            float distAB = (float)Math.Sqrt(b.X * b.X + b.Y * b.Y);

            //Rotate the system so that Point B is on the Positive X Axis
            float theCos = b.X / distAB;
            float theSin = b.Y / distAB;
            float newX = c.X * theCos + c.Y * theSin;
            c.Y = c.Y * theCos - c.X * theSin; c.X = newX;
            newX = d.X * theCos + d.Y * theSin;
            d.Y = d.Y * theCos - d.X * theSin; d.X = newX;

            //Fail if CD does not cross AB
            if (c.Y < 0 && d.Y < 0 || c.Y >= 0 && d.Y >= 0)
                return false;

            //Discover the position of the intersection point along line A-B
            float ABpos = d.X + (c.X - d.X) * d.Y / (d.Y - c.Y);

            //Fail if segment CD crosses line AB outside segment AB
            if (ABpos < 0 || ABpos > distAB)
                return false;

            // Apply the discovered position to line A-B in the originial 
            PoI.X = a.X + ABpos * theCos;
            PoI.Y = a.Y + ABpos * theSin;

            //Success
            return true;
        }

    }
}
