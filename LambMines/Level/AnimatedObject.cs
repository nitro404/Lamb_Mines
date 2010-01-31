﻿using System;
using System.Collections;
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
    class AnimatedObject : Object
    {

        AnimationPlayer animationPlayer;
        List<Animation> animations;


        public AnimatedObject(Vector2 Location, List<Animation> animationList, Texture2D tex) : base(Location, tex) 
        {
            animationPlayer = new AnimationPlayer();
            animations = animationList;
            animationPlayer.PlayAnimation(animations[0]);
        }

        public List<Animation> Animations
        {
            get { return animations; }
        }

        public AnimationPlayer AnimationPlay
        {
            get { return animationPlayer; }
        }

        public override Vector2 Centre
        {
            get
            {
                return new Vector2((animations[0].Dimensions.X/2) + Position.X, (animations[0].Dimensions.Y - 22.5f) + Position.Y);
            }
        }

        /// <summary>
        /// This is the trigger function that is called when there is a collision on this object. 
        /// Only an object that has been set with an associated onCollisionEvent can be called.
        /// </summary>
        /// <param name="collisionObject">This is a reference to the object that has collided with this event</param>
        /// <returns>Returns FALSE only if this object needs to be destroyed.</returns>
        public override Object onCollision(Object collisionObject,Texture2D[] textureList)
        {
            return null;
        }

        public void PlayAnimation(int value)
        {
            animationPlayer.PlayAnimation(animations[value]);
        }

        /// <summary>
        /// General update loop
        /// </summary>
        public override bool Update(float elapsedTime, ArrayList collisionList)
        {
			return isAlive;
        }

        /// <summary>
        /// General draw loop
        /// </summary>
        public override void Draw(SpriteBatch sb, GameTime gameTime, Vector2 Offset)
        {
            animationPlayer.Draw(gameTime, sb, GlobalHelpers.GetScreenCoords(Position + Offset), new SpriteEffects());
        }

        /// <summary>
        /// Kill this unit.
        /// </summary>
        public override void Kill()
        {
			KillMe();
        }

    }
}
