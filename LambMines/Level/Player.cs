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
    class Player : AnimatedObject
    {

        float speed;
        InputManager input;

        public Player(InputManager inputs, Vector2 Location, List<Animation> animationList, Texture2D tex)
            : base(Location, animationList, tex)
        {
            input = inputs;
            speed = 50.0f;
        }

        public override bool Update(float elapsedTime, ArrayList collisionList)
        {
                Vector2 movement = input.CheckLStick(0);
                if(input.IsKeyDown(Keys.W))
                {
                    movement.X -= speed * elapsedTime;
                    movement.Y -= speed * elapsedTime;
                }
                if(input.IsKeyDown(Keys.S))
                {
                    movement.X += speed * elapsedTime;
                    movement.Y += speed * elapsedTime;
                }
                if(input.IsKeyDown(Keys.A))
                {
                    movement.X -= speed * elapsedTime;
                    movement.Y += speed * elapsedTime;
                }
                if(input.IsKeyDown(Keys.D))
                {
                    movement.X += speed * elapsedTime;
                    movement.Y -= speed * elapsedTime;
                }

                if (movement != Vector2.Zero)
                {
                    int choose = GetAnimationDirection(movement);
                    AnimationPlay.PlayAnimation(Animations[choose + 8]);
                    if (checkMove(collisionList, Position + movement))
                    {
                        Position += movement;
                        movement.Normalize();
                    }
                }
                Error.Trace(Position.ToString());

                return base.Update(elapsedTime, collisionList);
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

		public override Object onCollision(Object collisionObject, Texture2D[] textureList)
        {
            if (input.IsKeyDown(Keys.Space))
            {          
                if (String.Compare(collisionObject.GetType().FullName, "LambMines.Sheep") == 0)
                {
                    ((Sheep)collisionObject).Seek(Position);
                }
            }

            if (input.IsKeyDown(Keys.B))
            {
                if (String.Compare(collisionObject.GetType().FullName, "LambMines.Sheep") == 0)
                {
                    ((Sheep)collisionObject).Repel(Position);
                }
            }
			return base.onCollision(collisionObject, textureList);
        }
        public override string WhatAmI() {
            return "Player";
        }
    }
}
