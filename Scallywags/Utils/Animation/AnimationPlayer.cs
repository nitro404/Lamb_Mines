using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace Scallywags
{
    class AnimationPlayer
    {
        Animation tileSet;
        int frameIndex;
        float time;


        public Animation TileSet
        {
            get { return tileSet; }
            set { tileSet = value; }
        }

        public int FrameIndex
        {
            get { return frameIndex; }
            set { frameIndex = value; }
        }

        public Vector2 Origin
        {
            get { return new Vector2(tileSet.Dimensions.X / 2, tileSet.Dimensions.Y - 45); }
        }

        public void PlayAnimation(Animation animation)
        {
            if (tileSet == animation)
                return;

            tileSet = animation;
            frameIndex = 0;
            time = 0.0f;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects)
        {

            if (tileSet == null)
                throw new NotSupportedException("No animation is currently Playing");

            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (time > tileSet.FrameTime)
            {
                time -= tileSet.FrameTime;

                if (tileSet.IsLooping)
                {
                    frameIndex = (frameIndex + 1) % tileSet.FrameCount;
                }
                else
                {
                    frameIndex = Math.Min(frameIndex + 1, tileSet.FrameCount - 1);
                }
            }

            Rectangle source = new Rectangle((int)(FrameIndex * tileSet.Dimensions.X) + (int)tileSet.StartLocation.X, (int)tileSet.StartLocation.Y, (int)tileSet.Dimensions.X, (int)tileSet.Dimensions.Y);

            spriteBatch.Draw(tileSet.Texture, position, source, Color.White, 0.0f, Vector2.Zero, 1.0f, spriteEffects, 0.0f);
        }
    }
}
