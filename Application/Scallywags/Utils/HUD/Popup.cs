using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags
{
    /** @class  Popup
     *  @brief  a messagebox type popup
     */
    public class Popup : HudItem
    {
        private List< PopupMessage >    m_lstMessages;          ///< The message queue                                                                 
        private Vector2                 m_vMessageLocation;     ///< The location of the message on the screen.
                                                                
        /** @class  Popup
         *  @brief  constructor
         */
        public Popup()
        {
            m_lstMessages       = new List<PopupMessage>();
            m_vMessageLocation  = Vector2.Zero;
        }

   
        /** @fn     Init( fX, fY )
         *  @brief  intiialize the popup 
         *  @param  fX [in] the default X position
         *  @param  fY [in] the default Y position
         */
        public void Init( )
        {           
            base.Init(  0, 0 );
        }

        /** @fn     void ShowMessage( string strMessage, Color colText )
         *  @brief  show a message, or add it to the queue if a message is showing already
         *  @param  strMessage [in] the message to show
         *  @param  colText [in] the color of the text
         */
        public void ShowMessage( string strMessage, Color colText )
        {
            PopupMessage msg = new PopupMessage();
            msg.Message      = strMessage;
            msg.Time         = 7.5f;
            msg.TextColor    = colText;

            m_lstMessages.Add( msg );
        }

        /**  @fn     void Draw( float fX, float fY )
          *  @brief  draw the hud item
          *  @param  spriteBatch [in] a handle to the drawing device
          *  @param  font [in] a sprite font to draw debug output with
          */
        public override void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            int i = 0;  

            foreach( PopupMessage msg in m_lstMessages )
            {
                Vector2 vStringDimensions = font.MeasureString(msg.Message);
             
                m_vMessageLocation.X = X - vStringDimensions.X / 2.0f;
                m_vMessageLocation.Y = Y + vStringDimensions.Y * i;

                Color colOutLine = Color.White;

                if( msg.TextColor == Color.White || msg.TextColor == Color.Yellow )
                    colOutLine = Color.Black;
                else
                {
                    //Apply a slight tint to the outline so the contrast doesn't burn one's eyes.
                    colOutLine.R -=  (byte)( (255 - msg.TextColor.R  ) / 5 );
                    colOutLine.G -=  (byte)( (255 - msg.TextColor.G  ) / 5 );
                    colOutLine.B -=  (byte)( (255 - msg.TextColor.B  ) / 5 );
                }

                colOutLine.A = msg.TextColor.A;

                //White outline
                spriteBatch.DrawString(font,
                                        msg.Message,
                                        new Vector2(m_vMessageLocation.X + 1.0f, m_vMessageLocation.Y + 1.0f),
                                        colOutLine);
                spriteBatch.DrawString(font,
                                        msg.Message,
                                        new Vector2(m_vMessageLocation.X - 1.0f, m_vMessageLocation.Y - 1.0f),
                                        colOutLine);
                spriteBatch.DrawString(font,
                                        msg.Message,
                                        new Vector2(m_vMessageLocation.X - 1.0f, m_vMessageLocation.Y + 1.0f),
                                        colOutLine);
                spriteBatch.DrawString(font,
                                        msg.Message,
                                        new Vector2(m_vMessageLocation.X + 1.0f, m_vMessageLocation.Y - 1.0f),
                                        colOutLine);
                
                //Foreground
                spriteBatch.DrawString( font,
                                        msg.Message,
                                        new Vector2( m_vMessageLocation.X, m_vMessageLocation.Y ),
                                        msg.TextColor );
                i++;
            }

        }

        /** @fn     void Update( float fElapsedTime )
         *  @brief  update the hud item
         *  @param  fElapsedTime [in] time elapsed since the last frame in seconds
         */
        public override void Update( float fElapsedTime )
        {

            for( int i = 0; i < m_lstMessages.Count; ++i )//PopupMessage msg in m_lstMessages )
            {
                PopupMessage msg = m_lstMessages[i];

                msg.ElapsedTime += fElapsedTime;

                ///////////////////////////////
                //If 75% of the message time has elapsed, start fading out
                float fElapsedPercent       = msg.ElapsedTime / msg.Time;
                float fOpaquePercent        = 0.75f;
                float fTransparentPercent   = 1.0f - fOpaquePercent;
                
                if ( fElapsedPercent > fOpaquePercent )
                {
                    Color colCurrent = msg.TextColor;
                    colCurrent.A = (byte)(255 * ( (1.0f - fElapsedPercent) / fTransparentPercent) );
                    msg.TextColor = colCurrent;
                }
                

                ////////////////////////////////////////////
                //If the message has expired, replace it with the next in the queue, if there is one.
                if (msg.ElapsedTime >= msg.Time)
                {
                    m_lstMessages.RemoveAt( i );
                }
            }
        }

        /** @fn     void Clear()
         *  @brief  remove any messages from the popup queue
         */
        public void Clear()
        {
            m_lstMessages.Clear();
        }

        #region POPUP_MESSAGE_CLASS

        /** @class  PopupMessage
         *  @brief  a popup message
         */
        private class PopupMessage
        {
            private string  m_strMessage;   ///< The message string
            private float   m_fMsgTime;     ///< time the message will exist on the screen
            private float   m_fElapsedTime; ///< The elapsed time the message has been on the screen
            private Color   m_colText;      ///< The text colour     

            /** @fn     PopupMessage
             *  @brief  the message to display, and how long to display it
             */
            public PopupMessage()
            {
                m_strMessage    = "";
                m_fMsgTime      = 0;
                m_fElapsedTime  = 0;
                m_colText       = Color.White;
            }

            /** @prop   string Message
             *  @brief  the message to display
             */
            public string Message
            {
                get
                {
                    return m_strMessage;
                }
                set
                {
                    m_strMessage = value;
                }
            }

            /** @prop   float Time
             *  @brief  the time the message will display for
             */
            public float Time
            {
                get
                {
                    return m_fMsgTime;
                }
                set
                {
                    m_fMsgTime = value;
                }
            }

            /** @prop   float ElapsedTime
             *  @brief  the elapsed time
             */
            public float ElapsedTime
            {
                get
                {
                    return m_fElapsedTime;
                }
                set
                {
                    m_fElapsedTime = value;
                }
            }

            /** @prop   TextColor
             *  @brief  the color of the text
             */
            public Color TextColor
            {
                get
                {
                    return m_colText;
                }
                set
                {
                    m_colText = value;
                }
            }
        };
        #endregion
    }
}
