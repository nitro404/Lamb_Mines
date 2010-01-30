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

namespace Scallywags
{
    class Sheep : AnimatedObject
    {

        public static int randomizer = 0;
        //static const SPEED = 25;
        float speed;
        float maxSpeed;
        Vector2 goal;
        Random ai;

        public Sheep(Vector2 Location, List<Animation> animationList, Texture2D tex)
            : base(Location, animationList, tex)
        {
            goal = Location;
            speed = 0.5f;
            maxSpeed = 150.0f;
            ai = new Random(randomizer);
            randomizer++;
        }

        public override void Update(float elapsedTime)
        {
            if (Position == goal)
            {
                if (ai.Next(0, 100) > 98)
                {
                    GetNewGoal();
                }
            }
            else
            {
                Vector2 dir = ((goal - Position) / (goal - Position).Length());
                float dist = Math.Min(maxSpeed, (goal - Position).Length() * speed);
                Vector2 travel = dir * dist;
                int choose = GetAnimationDirection(dir);
                AnimationPlay.PlayAnimation(Animations[choose]);
                if (dist > elapsedTime)
                {
                    Position += (travel * elapsedTime);
                }
                else
                {
                    Position = goal;
                }
            }
            base.Update(elapsedTime);
        }

        public override void Draw(SpriteBatch sb, GameTime gameTime)
        {

            base.Draw(sb, gameTime);
        }

        public void GetNewGoal()
        {
            goal = new Vector2(ai.Next(-20, 20) + Position.X, ai.Next(-20, 20) + Position.Y);
        }

        public int GetAnimationDirection(Vector2 direction)
        {
            double angle = Math.Atan2(direction.Y, direction.X) + (Math.PI * 7/8);
            double fraction = angle * 0.5 / Math.PI;
            fraction += 1.0 / 16.0;
            if (fraction >= 1.0)
            {
                fraction -= 1.0;
            }
            int index = (int)(fraction * 8.0);
            //float angle = (float)Math.Atan2(direction.Y, direction.X) - (float)Math.Atan2(1.0f, 0.0f);
            //return (int)((angle/(Math.PI/4)) + 8) % 8;
            return index;
        }

    }
}
