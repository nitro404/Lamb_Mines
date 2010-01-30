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

        public override void Update(float elapsedTime)
        {
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

                if (movement != Vector2.Zero)
                {
                    Position += movement;
                    movement.Normalize();
                    int choose = GetAnimationDirection(movement);
                    AnimationPlay.PlayAnimation(Animations[choose]);
                }

            base.Update(elapsedTime);
        }
    }
}
