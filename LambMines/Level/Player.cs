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
        int direction;
        InputManager input;
        bool isWolf;

        public Player(InputManager inputs, Vector2 Location, List<Animation> animationList, Texture2D tex)
            : base(Location, animationList, tex)
        {
            input = inputs;
            speed = 50.0f;
            isWolf = false;
            direction = 4;
        }

        public override bool Update(float elapsedTime, ArrayList collisionList)
        {
            if (input.IsKeyPressed(Keys.Z))
            {
                isWolf = !isWolf;
            }
            if (input.IsKeyPressed(Keys.X))
            {
                SpawnMine();
            }

                Vector2 movement = input.CheckLStick(0);
                if(input.IsKeyDown(Keys.Up))
                {
                    movement.X -= speed * elapsedTime;
                    movement.Y -= speed * elapsedTime;
                }
                if(input.IsKeyDown(Keys.Down))
                {
                    movement.X += speed * elapsedTime;
                    movement.Y += speed * elapsedTime;
                }
                if(input.IsKeyDown(Keys.Left))
                {
                    movement.X -= speed * elapsedTime;
                    movement.Y += speed * elapsedTime;
                }
                if(input.IsKeyDown(Keys.Right))
                {
                    movement.X += speed * elapsedTime;
                    movement.Y -= speed * elapsedTime;
                }

                int choose = direction;
                if (isWolf)
                {
                    choose += 8;
                }

                AnimationPlay.PlayAnimation(Animations[choose]);

                AnimationPlay.Paused = true;

                if (movement != Vector2.Zero)
                {
                    AnimationPlay.Paused = false;
                    direction = GetAnimationDirection(movement);
                    if (checkMove(collisionList, Position + movement))
                    {
                        Position += movement;
                        movement.Normalize();
                    }
                    Error.Trace(Position.ToString());
                }

			//HACK
			/*
				if (parent.m_ParentApp.Inputs.IsKeyPressed(Keys.H))
				{
					List<Animation> animList = new List<Animation>();
					for (int j = 0; j < 8; j++)
					{
						Animation anim = new Animation(parent.textureList[18], 1, true, new Vector2(35, 35), j);
						animList.Add(anim);
					}
					Vector2 tempVec = new Vector2(Position.X,Position.Y);
					Sheep tempSheep = new Sheep(tempVec, animList, parent.textureList[18]);
					//((ArrayList)parent.AllObjects[(int)Level.RenderLevel.RL_OBJECTS]).Add(tempSheep);
					parent.Spawn(Level.RenderLevel.RL_OBJECTS, tempSheep);
					Clutter tempShadow = new Clutter(tempSheep.Position, parent.textureList[21]);
					tempShadow.AddShadow(ref tempSheep, parent.textureList[21]);
					//((ArrayList)parent.AllObjects[(int)Level.RenderLevel.RL_SHADOWS]).Add(tempShadow);
					parent.Spawn(Level.RenderLevel.RL_SHADOWS, tempShadow);
					tempSheep.parent = parent;
					tempShadow.parent = parent;
					//tempSheep.AddShadow(textureList[21]);

				}
			*/	
                //return base.Update(elapsedTime, collisionList);
				return true;
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
		public override void Kill()
		{
			base.Kill();
			parent.isGameOver = true;
			parent.playerWon = false;
		}

		public override Object[] onCollision(Object collisionObject, Texture2D[] textureList)
        {

            if (String.Compare(collisionObject.GetType().FullName, "LambMines.Sheep") == 0)
            {
                if (isWolf)
                {
                    ((Sheep)collisionObject).Repel(Position);
                }
                else
                {
                    ((Sheep)collisionObject).Seek(Position);
                }
            }


			return base.onCollision(collisionObject, textureList);
        }
        public override string WhatAmI() {
            return "Player";
        }

        public void SpawnMine()
        {
            List<Animation> animList = new List<Animation>();
            animList.Add(new Animation(parent.TextureList[16], 0.25f, true, new Vector2(90,54), 0));
            Mine mine = new Mine(this.Position, animList, parent.TextureList[16]);
            mine.DisarmMine(2);
            mine.parent = parent;
            parent.SpawnMine(Level.RenderLevel.RL_MINES, mine);
            parent.AddTrigger(45.0f, mine);
        }
    }
}
