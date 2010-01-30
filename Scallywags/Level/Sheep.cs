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
        float interest;
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
			if (shadowTexture != null)
				sb.Draw(shadowTexture, GlobalHelpers.GetScreenCoords(Position) + Offset, Color.White);
				//sb.Draw(myTexture, Position + Offset, Color.White);
				
        }

        public void GetNewGoal()
        {
            goal = new Vector2(ai.Next(-200, 200) + Position.X, ai.Next(-200, 200) + Position.Y);
        }

        public void SetNewGoal(Vector2 value)
        {
            goal = value;
        }

        public void Seek(Vector2 value)
        {
            float distance = (value - Position).Length();
            Vector2 direction = value - Position;
            direction.Normalize();
            goal = new Vector2(ai.Next((int)(-distance / 10.0f), (int)(distance / 10.0f)) + value.X, ai.Next((int)(-distance / 10.0f), (int)(distance / 10.0f)) + value.Y);
        }

        public void Repel(Vector2 value)
        {
            float distance = (value - Position).Length();
            Vector2 direction = value - Position;
            direction.Normalize();
            goal = new Vector2(ai.Next((int)(-distance / 10.0f), (int)(distance / 10.0f)) + value.X, ai.Next((int)(-distance / 10.0f), (int)(distance / 10.0f)) + value.Y);
        
        }

		public override void Kill()
		{
			KillMe();
		}

    }
}
