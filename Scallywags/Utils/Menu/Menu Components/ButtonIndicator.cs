using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags
{
    /** @class  ButtonIndicator
     *  @brief  menu item that displays what actions are available and through which buttons
     */
    public class ButtonIndicator : MenuItem
    {
        //String constants to identify where the button images should go in the text
        public const string BUTTON_A = "/A";
        public const string BUTTON_B = "/B";
        public const string BUTTON_Y = "/Y";
        public const string BUTTON_S = "/S";

        private Dictionary< string, Texture2D >     m_dtButtonTextures;   ///< The dictionary of textures
        private List< ButtonPos >                   m_lstButtons;         ///< The list of button positions
        private Color                               m_colFont;                                                                         

        /** @fn 
         *  @brief  constructor
         */
        public ButtonIndicator( Dictionary< string, Texture2D > dtTextures )
        {
            m_dtButtonTextures = new Dictionary<string,Texture2D>();

            m_dtButtonTextures.Add( BUTTON_A, dtTextures[ "xboxButtonsAcrop" ] );
            m_dtButtonTextures.Add( BUTTON_B, dtTextures[ "xboxButtonsBcrop" ] );
            m_dtButtonTextures.Add( BUTTON_Y, dtTextures[ "xboxButtonsYcrop" ] );
            m_dtButtonTextures.Add( BUTTON_S, dtTextures[ "xboxControllerButtonStartCrop" ] );
   
            m_colFont    = Color.White;
            m_lstButtons = null;
        }

        /** @fn 
         *  @brief  initialize the text
         */
        public void SetText( string strText, SpriteFont font, Rectangle drawRect, Color colFont )
        {
            Font            = font;
            m_lstButtons    = new List<ButtonPos>();
            m_colFont       = colFont;

            //Parse any button image information out of the string
            string strTemp = "";

            for( int i = 0; i < strText.Length; ++i )
            {
                //add characters normally
                if( strText[i] != '/' )
                {
                    strTemp += strText[ i ];
                }
                else
                {
                    string strButtonImg = strText.Substring(i, 2);    //Get the button constant

                    ButtonPos btnPos    = new ButtonPos();
                    int nCurrentXPos    = (int)font.MeasureString( strTemp ).X;
                    int nHorizontalGap  = 0; //Space between the last character and the button image

                    btnPos.position.X = nCurrentXPos + nHorizontalGap;
                    btnPos.position.Y = 0;
                    btnPos.tex        = m_dtButtonTextures[ strButtonImg ];

                    //Determine how wide a gap the string needs to make room for the image
                    string strImageBuffer = GetStringGap( btnPos.tex.Width + 2 * nHorizontalGap );
                  
                    //add the gap to the string
                    strTemp += strImageBuffer;

                    //Skip the second character in the button string
                    ++i;

                    m_lstButtons.Add(btnPos);
                }
            }

            Text = strTemp;     
       
            //Calculate the dimensions of the menu item
            Vector2 vDimensions = font.MeasureString( Text );

            Width   = vDimensions.X;
            Height  = vDimensions.Y;

            X = drawRect.X + (drawRect.Width - Width) / 2;
            Y = drawRect.Y + drawRect.Height - Height * 2.5f;

            //Center the button images vertically
            foreach( ButtonPos btnPos in m_lstButtons )
                btnPos.position.Y = ( Height - btnPos.tex.Height ) / 2.0f;
        }

        /** @fn     
         *  @brief  get a string that is wide enough to fill a specific pixel gap
         */
        private string GetStringGap( int nPixelWidth )
        {
            string strRet = " ";

            while( Font.MeasureString( strRet ).X < nPixelWidth )
                strRet += " ";

            return strRet;
        }

        /** @fn     void Draw( SpriteBatch sb )
         *  @brief  draw the UIobject
         *  @param  sb [in] the active sprite batch
         */
        public override void Draw(SpriteBatch sb)
        {
            sb.DrawString( Font, Text, new Vector2( X, Y ), m_colFont );

            //Draw the button images
            foreach( ButtonPos btn in m_lstButtons )
                sb.Draw( btn.tex, this.Position + btn.position, Color.White );
        }

        /** @fn     void Update( float fElapsedTime, InputManager inputs  )
         *  @brief  update the UIObject - in general, only use this for animations.
         *  @param  fElapsedTime [in] the time since the last frame, in seconds.
         *  @param  inputs [in] the state of the input
         */
        public override void Update(float fElapsedTime, InputManager inputs)
        {
            
        }

        /** @class     
         *  @brief  stores the position and image of a button texture
         */
        private class ButtonPos
        {
            public Vector2     position;    ///< The offset from the X,Y of the menu item
            public Texture2D   tex;         ///< The button texture
            public float       scale;       ///< The scale of the image.
                                            
            public ButtonPos()
            {
                position    = Vector2.Zero;
                tex         = null;
                scale       = 1.0f;
            }
        }
    }
}
