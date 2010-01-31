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

namespace LambMines
{
    class Animation
    {
        #region Members

        Texture2D texture;
        float frameTime;
        bool isLooping;
        Vector2 dimensions;
        Vector2 startLocation;

        #endregion

        #region Properties

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public float FrameTime
        {
            get { return frameTime; }
            set { frameTime = value; }
        }

        public bool IsLooping
        {
            get { return isLooping; }
            set { isLooping = value; }
        }

        public int FrameCount
        {
            get { return (int)((texture.Width - startLocation.X) / dimensions.X);
            }
        }

        public Vector2 Dimensions
        {
            get{ return dimensions; }
            set{ dimensions = value; }
        }

        public Vector2 StartLocation
        {
            get{ return startLocation; }
            set{ startLocation = value; }
        }

        #endregion

        #region Functions

        public Animation(Texture2D tex, float frametime, bool looping, Vector2 size, int row)
        {
            texture = tex;
            frameTime = frametime;
            isLooping = looping;
            startLocation = new Vector2(0, size.Y * row);
            dimensions = size;
        }

        #endregion
    }
}
