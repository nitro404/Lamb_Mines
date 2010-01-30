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
    class MainMenu:MainMenuScreen 
    {
        Texture2D m_texBanner;
        Color m_colBanner;
        Vector2 m_vBannerPos;
        ReferenceableInt m_WhoPaused;  
     
        GameMenuID          m_gmidNextMenu;    ///< The ID of the menu to switch to
        List< XNALabel >    m_lstLabels;       ///< The labels in the main menu

        public MainMenu(ReferenceableInt whoPaused)
        {
            m_WhoPaused = whoPaused;
        }

        /** @fn     void Init
         *  @param  device [in] the active rendering device
         *  @param  font [in] the font the menu items will use
         *  @param  drawRect [in] the area of the screen this item can draw to
         *  @brief  initialize the menu screen
         */
        public override void Init( GraphicsDevice device, Dictionary< string, SpriteFont > fonts, Rectangle drawRect, Dictionary< String, Texture2D > dtTextures )
        {
            m_gmidNextMenu = GameMenuID.GMN_THIS;

            ////////////////////////////////////
            //Set banner properties ( position and opacity )
            m_texBanner = dtTextures[ "Banner" ];
            m_colBanner = new Color( 255, 255, 255, 200 );
            m_vBannerPos = new Vector2( drawRect.X + ( drawRect.Width * 0.5f - m_texBanner.Width * 0.5f ),
                drawRect.Y + drawRect.Height * 0.1f );

            //////////////////////
            //Create the items
            m_lstLabels = new List<XNALabel>();

            m_lstLabels.Add( new XNALabel() );
            m_lstLabels.Add( new XNALabel() );
            m_lstLabels.Add( new XNALabel() );
            m_lstLabels.Add( new XNALabel() );

            string[] vLabelTexts = { "New Game", "Options", "Controls/How To Play", "Credits" };

            ///////////////////////////
            //Measure the items
            int nDrawCenterX = drawRect.X + drawRect.Width / 2;
            int nDrawCenterY = drawRect.Y + drawRect.Height / 2;

            int nVerticalSpacing    = 5;   //how far apart the items are vertically
            int nTotalHeight        = 0;

            for( int i = 0; i < m_lstLabels.Count; ++i )
            {
                m_lstLabels[i].Init(fonts["PirateFontLarge"], vLabelTexts[i]);
                m_lstLabels[i].Selectable = true;

                //Tally the total height
                nTotalHeight += (int)m_lstLabels[i].Height + nVerticalSpacing;   
            }

            ///////////////////////
            //Place the items
            int nTopY           = nDrawCenterY - ( nTotalHeight / 3 ); //Where the items will begin to draw
            int nElapsedHeight  = 0;
            
            foreach( XNALabel label in m_lstLabels )
            {
                //Center the items in the draw rect     
                label.Y     = nTopY + nElapsedHeight;
                label.X     = nDrawCenterX - ( label.Width / 2 );
                
                AddMenuItem( label );

                nElapsedHeight += (int)label.Height + nVerticalSpacing;
            }
         
            m_lstLabels[0].Selected = true;
        }

        /** @fn     void Draw( SpriteBatch sb )
         *  @brief  draw the sprite banner after the base draw
         */
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            sb.Draw( m_texBanner, m_vBannerPos, m_colBanner );
        }

        /** @fn     GameMenuID Update( double fTotalTime, float fElapsedTIme, InputManager input )
         *  @brief  the update function
         *  @return the ID of the menu screen to switch to, or GMN_THIS if this screen is to continue to run
         *  @param  fTotalTime [in] the total time the application has been running
         *  @param  fElapsedTime [in] the time, in seconds, since the previous frame
         *  @param  input [in] the application's input manager
         */
        public override GameMenuID Update(double fTotalTime, float fElapsedTime, InputManager input, SoundManager sounds, int Control ) 
        {
            base.Update( fTotalTime, fElapsedTime, input, sounds, Control );

            HandleInput( fElapsedTime, input, sounds );

            return m_gmidNextMenu;
        }

        /** @fn     void NextItem
         *  @brief  select the next menu item
         */
        private void NextItem()
        {
            int nCurrentIndex = GetSelectedIndex();

            m_lstLabels[nCurrentIndex].Selected = false;

            //Select the next label                    
            int nNextIndex = (nCurrentIndex + 1) % m_lstLabels.Count;
            m_lstLabels[nNextIndex].Selected = true; 
        }

        /** @fn     void PrevItem
         *  @brief  select the previous menu item
         */
        private void PrevItem()
        {
            int nCurrentIndex = GetSelectedIndex();

            m_lstLabels[ nCurrentIndex ].Selected = false;

            //Select the previous label                    
            int nNextIndex = (nCurrentIndex + (m_lstLabels.Count - 1)) % m_lstLabels.Count;
            m_lstLabels[nNextIndex].Selected = true;     
        }

        /**     @fn     
         *      @brief  Get the index of the currently selected label
         */
        private int GetSelectedIndex()
        {
            for (int i = 0; i < m_lstLabels.Count; ++i)
            {
                if (m_lstLabels[i].Selected)
                    return i;
            }

            return -1;
        }

        /** @fn     void ActivateSelection()
         *  @brief  perform the current selections action
         */
        private void ActivateSelection()
        {
            int nSelectedIndex = GetSelectedIndex();

            switch( nSelectedIndex )
            {
                case 0:
                    m_gmidNextMenu = GameMenuID.GMN_PLAYERSELECT;
                    break;
                case 1:
                    m_gmidNextMenu = GameMenuID.GMN_OPTIONS;
                    break;
                case 2:
                    m_gmidNextMenu = GameMenuID.GMN_CONTROLS;
                    break;
                case 3:
                    m_gmidNextMenu = GameMenuID.GMN_CREDITS;
                    break;
                default:
                    m_gmidNextMenu = GameMenuID.GMN_THIS;       //Unknown selection
                    break;
            }
        }
        
        /** @fn     void HandleInput( float fElapsedTime, InputManager input )
         *  @brief  handle user input
         *  @param  fElapsedTime [in] time since the previous frame
         *  @param  input [in] the input manager
         */
        private void HandleInput( float fElapsedTime, InputManager input, SoundManager sounds )
        {
            //Only use player one for now

            if (input.IsKeyPressed(Keys.Down))
            {
                sounds.PlayCue("Flip");
                NextItem();
            }
            if (input.IsKeyPressed(Keys.Up))
            {
                sounds.PlayCue("Flip");
                PrevItem();
            }
            
            if( input.IsKeyDown(Keys.Enter) )
                ActivateSelection();

            for (int i = 0; i < 4; i++)
            {
                if (input.IsDirectionPressed(i, Direction.Down))
                {
                    sounds.PlayCue("Flip");
                    NextItem();
                }
                if (input.IsDirectionPressed(i, Direction.Up))
                {
                    sounds.PlayCue("Flip");
                    PrevItem();
                }

                if (input.IsButtonPressed(i, Buttons.A))
                {
                    m_WhoPaused.Integer = i;
                    ActivateSelection();
                }
            }
        }

        /** @fn     void ShutDown()
         *  @brief  cleanup the menu screen
         */
        public override void ShutDown()
        {
            m_lstLabels.Clear();

            base.ShutDown();
        }
    }
}