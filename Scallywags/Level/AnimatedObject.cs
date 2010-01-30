using System;
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

namespace Scallywags
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

        /// <summary>
        /// This is the trigger function that is called when there is a collision on this object. 
        /// Only an object that has been set with an associated onCollisionEvent can be called.
        /// </summary>
        /// <param name="collisionObject">This is a reference to the object that has collided with this event</param>
        /// <returns>Returns FALSE only if this object needs to be destroyed.</returns>
        public override bool onCollision(Object collisionObject)
        {
            return true;
        }

        public void PlayAnimation(int value)
        {
            animationPlayer.PlayAnimation(animations[value]);
        }

        /// <summary>
        /// General update loop
        /// </summary>
        public override void Update(float elapsedTime)
        {

        }

        /// <summary>
        /// General draw loop
        /// </summary>
        public override void Draw(SpriteBatch sb, GameTime gameTime)
        {
            animationPlayer.Draw(gameTime, sb, GlobalHelpers.GetScreenCoords(Position), new SpriteEffects());
        }

        /// <summary>
        /// Kill this unit.
        /// </summary>
        public override void Kill()
        {

        }

    }
}
