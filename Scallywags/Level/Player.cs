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

        public override bool Update(float elapsedTime)
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
                    Position += movement;
                    movement.Normalize();
                    int choose = GetAnimationDirection(movement);
                    AnimationPlay.PlayAnimation(Animations[choose]);
                }

            return base.Update(elapsedTime);
        }

        public override bool onCollision(Object collisionObject)
        {
            if (input.IsKeyDown(Keys.Space))
            {          
                if (String.Compare(collisionObject.GetType().FullName, "Scallywags.Sheep") == 0)
                {
                    ((Sheep)collisionObject).Seek(Position);
                }
            }
            return base.onCollision(collisionObject);
        }
    }
}
