using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags
{
    public abstract class MenuItem
    {
        private SpriteFont  m_font;             ///< The font the UIObject will use
        private string      m_strText;          ///< The text value of the UI object, the context will depend on the object.                        

        private Vector2     m_vScreenPosition;  ///< The Screen pixel position of the UIObject - generally the top left corner, but this isn't enforced
        private Vector2     m_vDimensions;      ///< The width/height of the UI object

        bool                m_bEnabled;         ///< Is this item enabled?
        bool                m_bSelectable;      ///< Is this item selectable?                                        
        bool                m_bSelected;        ///< Is this item selected?
                                                
        #region PROPERTIES

        /** @prop   Text
         *  @brief  the text value of the UIObject
         */
        public string Text
        {
            get
            {
                return m_strText;
            }
            set
            {
                m_strText = value;
            }
        }

        /** @prop   Font
         *  @brief  the font the UI object will use for any text it will draw
         */
        public SpriteFont Font
        {
            get
            {
                return m_font;
            }
            set
            {
                m_font = value;
            }
        }

        /** @prop   X
         *  @brief  the X position of the UI object
         */
        public float X
        {
            get
            {
                return m_vScreenPosition.X;
            }
            set
            {
                m_vScreenPosition.X = value;
            }
        }

        /** @prop   Y
         *  @brief  the Y position of the UI Object
         */
        public float Y
        {
            get
            {
                return m_vScreenPosition.Y;
            }
            set
            {
                m_vScreenPosition.Y = value;
            }
        }
        
        /** @prop    Position
         *  @brief  the screen position of the UIObject
         */
        public Vector2 Position
        {
            get
            {
                return m_vScreenPosition;
            }
            set
            {
                m_vScreenPosition = value;
            }
        }

        /** @prop   Dimensions
         *  @brief  the width and height of the UIObject
         */
        public Vector2 Dimensions
        {
            get
            {
                return m_vDimensions;
            }
            set
            {
                m_vDimensions = value;
            }
        }

        /** @prop   Width
         *  @brief  the width of the UIObject
         */
        public float Width 
        {
            get
            {
                return m_vDimensions.X;
            }
            set
            {
                m_vDimensions.X = value;
            }
        }

        /** @prop   Height
         *  @brief  the height of the UI Object
         */
        public float Height
        {
            get
            {
                return m_vDimensions.Y;
            }
            set
            {
                m_vDimensions.Y = value;
            }
        }

        /** @prop   Selected
         *  @brief  is this label selected?
         */
        public bool Selected
        {
            set
            {
                m_bSelected = value;
            }
            get
            {
                return m_bSelected;
            }
        }

        /** @prop   Enabled
         *  @brief  is this label enabled?
         */
        public bool Enabled
        {
            set
            {
                m_bEnabled = value;
            }
            get
            {
                return m_bEnabled;
            }
        }

        /** @prop   Selectable
         *  @brief  is this menuitem selectable?
         */
        public bool Selectable
        {
            get
            {
                return m_bSelectable;
            }
            set
            {
                m_bSelectable = value;
            }
        }

        #endregion

        /** @fn     UIObject
         *  @brief  constructor
         */
        public MenuItem()
        {
            m_font = null;
            m_strText = "";

            m_vScreenPosition = Vector2.Zero;
            m_vDimensions = Vector2.One;

            m_bSelectable   = false;
            m_bEnabled      = true;
            m_bSelected     = false;
        }

        /** @fn     void Draw( SpriteBatch sb )
         *  @brief  draw the UIobject
         *  @param  sb [in] the active sprite batch
         */
        public abstract void Draw( SpriteBatch sb );

        /** @fn     void Update( float fElapsedTime, InputManager inputs  )
         *  @brief  update the UIObject - in general, only use this for animations.
         *  @param  fElapsedTime [in] the time since the last frame, in seconds.
         *  @param  inputs [in] the state of the input
         */
        public abstract void Update( float fElapsedTime, InputManager inputs );

    }
}
