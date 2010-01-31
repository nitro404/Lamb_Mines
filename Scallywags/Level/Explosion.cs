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
using Microsoft.Xna.Framework.GamerServices;

namespace LambMines
{
	class Explosion:AnimatedObject
	{


		public Explosion(Vector2 Location, List<Animation> animationList, Texture2D tex)
            : base(Location, animationList, tex)
        {

        }

		public override bool Update(float elapsedTime)
		{

			AnimationPlay.PlayAnimation(Animations[0]);

            if (!AnimationPlay.IsPlaying)
            {
                Kill();
            }

			base.Update(elapsedTime);

			if (AnimationPlay.IsPlaying)
				return true;
			return false;

		}

		public override void Draw(SpriteBatch sb, GameTime gameTime, Vector2 Offset)
        {
            base.Draw(sb, gameTime, Offset);
        }

		public override void Kill()
		{
			KillMe();
		}
		public override string WhatAmI()
		{
			return "Explosion";
		}
	}
}
