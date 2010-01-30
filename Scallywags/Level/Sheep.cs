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
            speed = 5f;
            maxSpeed = 15.0f;
            ai = new Random(randomizer);
            randomizer++;
        }

        public override bool Update(float elapsedTime)
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

			return isAlive;

        }

        public override void Draw(SpriteBatch sb, GameTime gameTime, Vector2 Offset)
        {

            base.Draw(sb, gameTime, Offset);
        }

        public void GetNewGoal()
        {
            goal = new Vector2(ai.Next(-200, 200) + Position.X, ai.Next(-200, 200) + Position.Y);
        }
		public override void Kill()
		{
			KillMe();
		}

    }
}
