using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Scallywags 
{
    class OptionsMenu:MainMenuScreen 
    {
        XNALabel   m_lblTitle;          ///< The menu screen title

        XNALabel[] m_vLabels;           ///< The menu labels
                                    
        XNASlideBar m_slideSound;       ///< The sound effect volume slide bar
        XNASlideBar m_slideMusic;       ///< The music volume slide bar
                                    
        int m_nCurrentSelection;        ///< The current selected item

        SoundManager m_soundManager;    ///< The app's sound manager.. for volume control

        /** @fn     OptionsMenu
         *  @brief  constructor
         */
        public OptionsMenu( SoundManager soundManager )
        {
            m_soundManager = soundManager;
        }

        /** @fn     void Init
         *  @param  device [in] the active rendering device
         *  @param  font [in] the font the menu items will use
         *  @param  drawRect [in] the area of the screen this item can draw to
         *  @brief  initialize the menu screen
         */
        public override void Init(GraphicsDevice device, Dictionary<string, SpriteFont> fonts, Rectangle drawRect, Dictionary<String, Texture2D> dtTextures)
        {
            m_nCurrentSelection = 0;
            
            /////////////////////
            //Slide bars
            m_slideSound = new XNASlideBar( 100 );
            m_slideMusic = new XNASlideBar( 100 );

            m_slideSound.Init( dtTextures[ "SlideBack" ], dtTextures[ "SlideCursor" ] );
            m_slideMusic.Init( dtTextures[ "SlideBack" ], dtTextures[ "SlideCursor" ] );
            
            /////////////////////////////
            //Labels
            m_vLabels = new XNALabel[ 3 ];

            m_vLabels[ 0 ] = new XNALabel();  
            m_vLabels[ 1 ] = new XNALabel();  
            m_vLabels[ 2 ] = new XNALabel();    
 
            m_lblTitle = new XNALabel();
            m_lblTitle.Init( fonts[ "PirateFontLarge" ], "Options" );
            m_lblTitle.X = drawRect.X + ( drawRect.Width - m_lblTitle.Width ) / 2.0f;
            m_lblTitle.Y = drawRect.Y + m_lblTitle.Height;// * 2.0f;


            // Label to show when sounds are selected
            m_vLabels[0].Init(fonts["PirateFontLarge"], "Sound Volume");

            // Label to show when music is selected
            m_vLabels[1].Init(fonts["PirateFontLarge"], "Music Volume");

            // The back option            
            float fHorizontalGap = 100.0f;   ///< Distance from the right of the draw area to draw items
            float fVerticalGap = 100.0f;   ///< Distance from the bottom of the draw area to draw items

            m_vLabels[ 2 ].Init(fonts["PirateFontLarge"], "OK");
            m_vLabels[ 2 ].X = drawRect.X + drawRect.Width - fHorizontalGap -  m_vLabels[ 2 ].Width;
            m_vLabels[ 2 ].Y = drawRect.Y + drawRect.Height - fVerticalGap;// -  m_vLabels[ 2 ].Height;
            //m_vLabels[ 2 ].Selected = false;

            //////////////////////////////////////
            //Position the slide bars and headers
            float fSoundY = drawRect.Y + 0.3f * drawRect.Height;
            float fMusicY = drawRect.Y + 0.5f * drawRect.Height;

            float fLabelSlideGap = 40.0f;   //Gap between the label and the slider

            float fSoundXStart  = drawRect.X + (drawRect.Width - (m_vLabels[0].Width + fLabelSlideGap + m_slideSound.Width)) / 2;

            m_vLabels[ 0 ].X    = fSoundXStart;
            m_vLabels[ 0 ].Y    = fSoundY;
            m_slideSound.X      = fSoundXStart + m_vLabels[0].Width + fLabelSlideGap;
            m_slideSound.Y      = fSoundY;

            //m_slideSound.Selected = false;

            float fMusicXStart = drawRect.X + (drawRect.Width - (m_vLabels[1].Width + fLabelSlideGap + m_slideMusic.Width)) / 2;

            m_vLabels[1].X = fMusicXStart;
            m_vLabels[1].Y = fMusicY;
            m_slideMusic.X = fMusicXStart + m_vLabels[1].Width + fLabelSlideGap;
            m_slideMusic.Y = fMusicY;

            ////////////////////////////
            //Add all the items to the draw list
            AddMenuItem( m_vLabels[ 0 ] );
            AddMenuItem( m_vLabels[ 1 ] );
            AddMenuItem( m_vLabels[ 2 ] );

            AddMenuItem( m_slideSound );
            AddMenuItem( m_slideMusic );

            AddMenuItem( m_lblTitle );


            m_vLabels[ 0 ].Selected = true;
            m_slideSound.Selected   = true;
            
            ////////////////////////
            //Align the sound manager with the slide bars
            m_slideSound.PositionPercent = m_soundManager.SoundEffectVolume;
            m_slideMusic.PositionPercent = m_soundManager.MusicVolume;
        }

        /** @fn     GameMenuID Update( double fTotalTime, float fElapsedTIme, InputManager input )
         *  @brief  the update function
         *  @return the ID of the menu screen to switch to, or GMN_THIS if this screen is to continue to run
         *  @param  fTotalTime [in] the total time the application has been running
         *  @param  fElapsedTime [in] the time, in seconds, since the previous frame
         *  @param  input [in] the application's input manager
         */
        public override GameMenuID Update(double fTotalTime, float fElapsedTime, InputManager input, SoundManager sounds, int Control)
        {
            base.Update(fTotalTime, fElapsedTime, input, sounds, Control);

            m_soundManager.SoundEffectVolume = m_slideSound.PositionPercent;
            m_soundManager.MusicVolume = m_slideMusic.PositionPercent;

            //////////////////////////////////
            //Process input
            if (input.IsKeyPressed(Keys.Enter) || input.IsButtonPressed(Control, Buttons.A))
            {
                switch( m_nCurrentSelection )
                {
                    case 0:
                        //m_slideSound.Selected       = !m_slideSound.Selected;
                        //m_vLabels [ 0 ].Selected    = !m_slideSound.Selected;
                        break;
                    case 1:
                        //m_slideMusic.Selected   = !m_slideMusic.Selected;
                        //m_vLabels[ 1 ].Selected = !m_slideMusic.Selected;
                        break;
                    case 2: 
                        return GameMenuID.GMN_MAINMENU;
                    default:
                        break;
                }
            }

            if (input.IsKeyPressed(Keys.Down) || input.IsDirectionPressed(Control, Direction.Down))
                OnDownPress();
            if (input.IsKeyPressed(Keys.Up) || input.IsDirectionPressed(Control, Direction.Up))
                OnUpPress();
            if (input.IsKeyDown(Keys.Right) || input.IsDirectionHeld(Control, Direction.Right))
                OnRightPress();
            if (input.IsKeyDown(Keys.Left) || input.IsDirectionHeld(Control, Direction.Left))
                OnLeftPress();

            //Return without changes
            if (input.IsKeyPressed(Keys.Back) || input.IsButtonPressed(Control, Buttons.B))
                return GameMenuID.GMN_MAINMENU;

            return GameMenuID.GMN_THIS;
        }

        
        /** @fn 
         *  @brief  handle the user pressing down
         */
        private void OnDownPress()
        {
            //Only move down if the slide bars aren't active
            //if( m_slideMusic.Selected == false && m_slideSound.Selected == false )
            {
                if( m_nCurrentSelection == 0 )
                    m_slideSound.Selected = false;
                if( m_nCurrentSelection == 1 )
                    m_slideMusic.Selected = false;

                m_vLabels[ m_nCurrentSelection ].Selected = false;
                m_nCurrentSelection = ( m_nCurrentSelection + 1 ) % m_vLabels.Length;
                m_vLabels[m_nCurrentSelection].Selected = true;

                if (m_nCurrentSelection == 0)
                    m_slideSound.Selected = true;
                if (m_nCurrentSelection == 1)
                    m_slideMusic.Selected = true;
            }
        }

        /** @fn     
         *  @brief  handle the user pressing up
         */
        private void OnUpPress()
        {
            //Only move up if the slide bars aren't active
            //if (m_slideMusic.Selected == false && m_slideSound.Selected == false)
            {
                if (m_nCurrentSelection == 0)
                    m_slideSound.Selected = false;
                if (m_nCurrentSelection == 1)
                    m_slideMusic.Selected = false;

                m_vLabels[m_nCurrentSelection].Selected = false;
                m_nCurrentSelection = (m_nCurrentSelection + (m_vLabels.Length - 1)) % m_vLabels.Length;
                m_vLabels[m_nCurrentSelection].Selected = true;

                if (m_nCurrentSelection == 0)
                    m_slideSound.Selected = true;
                if (m_nCurrentSelection == 1)
                    m_slideMusic.Selected = true;
            }
        }

        /** @fn     
         *  @brief  handle the user pressing right
         */
        private void OnRightPress()
        {
            if( m_slideSound.Selected )
                m_slideSound.SlideRight();
            else if( m_slideMusic.Selected )
                m_slideMusic.SlideRight();            
        }

        /** @fn
         *  @brief  handle the user pressing left
         */
        private void OnLeftPress()
        {
            if (m_slideSound.Selected)
                m_slideSound.SlideLeft();
            else if (m_slideMusic.Selected)
                m_slideMusic.SlideLeft();  
        }
    }
}
