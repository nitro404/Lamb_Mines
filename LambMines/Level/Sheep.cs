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
    class Sheep : AnimatedObject
    {

        public static int randomizer = 0;
        //static const SPEED = 25;
        int interest;
        float speed;
        float maxSpeed;
        Vector2 goal;
        Random ai;

        public Sheep(Vector2 Location, List<Animation> animationList, Texture2D tex)
            : base(Location, animationList, tex)
        {
            goal = Location;
            speed = 0.3f;
            maxSpeed = 60.0f;
            ai = new Random(randomizer);
            randomizer++;
            interest = 0;
        }

        public override bool Update(float elapsedTime, ArrayList collisionList)
        {
            float decision = ai.Next(0, 1000);
            if (decision > 925 + interest)
            {
                Wander();
            }
            else if (Position != goal)
            {
                Vector2 dir = ((goal - Position) / (goal - Position).Length());
                float dist = Math.Min(maxSpeed, (goal - Position).Length() * speed);
                Vector2 travel = dir * dist;
                int choose = GetAnimationDirection(dir);
                AnimationPlay.PlayAnimation(Animations[choose]);
                if (dist > elapsedTime)
                {
                    if (checkMove(collisionList, Position + travel * elapsedTime))
                    {
                        Position += (travel * elapsedTime);
                    }
                }
                else
                {
                    if (checkMove(collisionList, goal))
                    {
                        Position = goal;
                    }
                }
            }

            if (ai.Next(0, 10) > 5)
            {
                interest--;
            }

            return base.Update(elapsedTime, collisionList);

        }

        public override void Draw(SpriteBatch sb, GameTime gameTime, Vector2 Offset)
        {
            base.Draw(sb, gameTime, Offset);
            if (shadowTexture != null)
                sb.Draw(shadowTexture, GlobalHelpers.GetScreenCoords(Position + Offset), Color.White);
            //sb.Draw(myTexture, Position + Offset, Color.White);

        }

        public bool checkMove(ArrayList collisionList, Vector2 Target)
        {
            foreach (Edge edge in collisionList)
            {
                Vector2 intersection = new Vector2();
                if (CollisionHandler.CheckLineIntersection(edge.pointA, edge.pointB, Position, Target, intersection))
                {
                    return false;
                }
            }
            return true;
        }

        public void Wander()
        {
            goal = new Vector2(ai.Next(-50, 50) + Position.X, ai.Next(-50, 50) + Position.Y);
            interest += 25;
        }

        public void SetNewGoal(Vector2 value)
        {
            goal = value;
        }

        public void Seek(Vector2 value)
        {
            if (interest < 55)
            {
                float distance = (value - Position).Length();
                Vector2 direction = value - Position;
                direction.Normalize();
                float difference = (value - goal).Length();
                if (difference > 10)
                {
                    goal = new Vector2(ai.Next((int)(-distance / 10.0f), (int)(distance / 10.0f)) + value.X, ai.Next((int)(-distance / 10.0f), (int)(distance / 10.0f)) + value.Y);
                    interest = (int)Math.Min(100, interest + distance / 4);
                }
            }
        }

        public void Repel(Vector2 value)
        {
            if (interest < 85)
            {
                float distance = (value - Position).Length();
                Vector2 direction = (value - Position) * -1;
                direction.Normalize();
                float difference = (value - goal).Length();
                Vector2 newLoc = (direction * (200.0f - distance) * 4.0f) + Position;
                if (difference > 10)
                {
                    goal = new Vector2(ai.Next((int)(-distance / 2.0f), (int)(distance / 2.0f)) + newLoc.X, ai.Next((int)(-distance / 2.0f), (int)(distance / 2.0f)) + newLoc.Y);
                    interest = 100;
                }
            }
        }

        public override void Kill()
        {
            KillMe();
        }

    }
}