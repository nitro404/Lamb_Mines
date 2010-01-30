using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Scallywags 
{
    public class CharSelectMenu : MainMenuScreen 
    {
        const float           START_DELAY = 0.5f;           ///< Delay 1 second to show any randomly selected 
        const float           BACK_DELAY  = 0.1f;
        MenuModule            m_parent;                     ///< The menu module
                                                           
        CharacterPortrait[]   m_vCharacterPortraits;        ///< The 4 character protraits
        CharacterSelector     m_characterSelector;          ///< The item to select characters with 
        
        XNALabel              m_lblTitle;                   ///< The 
       
        GameMenuID            m_retID;                      ///< the ID that will be returned in the update function
        
        bool                  m_bSaveState;                 ///< Are we saving the character selection states when we leave the screen?        
        float                 m_fStartDelayTimer;           ///< Short delay before starting the game
        float                 m_fBackDelayTimer;            ///< Add a short delay if the user hits the back button
        Texture2D             m_texFade;                    ///< Texture to fade the menu out as the game starts

        bool[]                m_vAIFlags;                   ///< Check if particular players are set to be AI 
        Texture2D             m_tStartButton;
        SpriteFont            m_sfPirateFont;
        SoundManager          m_smSounds;
        bool m_bReady;

        ReferenceableInt m_WhoPaused;

        public SoundManager Sounds
        {
            get
            {
                return m_smSounds;
            }
        }

        /** @fn     CharSelectMenu
         *  @brief  the constructor
         */
        public CharSelectMenu(MenuModule parent, ReferenceableInt whoPaused)
        {
            m_parent = parent;

            m_WhoPaused = whoPaused;

            m_vCharacterPortraits   = null;
            m_characterSelector     = null;

            m_retID                 = GameMenuID.GMN_THIS;

            m_fStartDelayTimer      = 0;
            m_fBackDelayTimer       = 0;
            m_bSaveState            = false;
            m_bReady = false;

            m_vAIFlags = null;

            m_smSounds = parent.ParentApp.SoundPlayer;
        }

        /** @fn     void Init
         *  @param  device [in] the active rendering device
         *  @param  font [in] the font the menu items will use
         *  @param  drawRect [in] the area of the screen this item can draw to
         *  @brief  initialize the menu screen
         */
        public override void Init(GraphicsDevice device, Dictionary<string, SpriteFont> fonts, Rectangle drawRect, Dictionary<String, Texture2D> dtTextures)
        {
            if( m_bSaveState == false )
            {
                m_vAIFlags = new bool[4];
                m_vAIFlags[0] = false;
                m_vAIFlags[1] = false;
                m_vAIFlags[2] = false;
                m_vAIFlags[3] = false;

                //Set up fade out states
                m_fStartDelayTimer  = 0;
                m_texFade           = new Texture2D(device, 1, 1);
                Color[] clrs        = { Color.Black };
                m_texFade.SetData<Color>(clrs);

                ScallyWagsApp app = ((ScallyWagsApp)m_parent.ParentApp);
               
                ///////////////////////////////////
                //Get all the required resources and information from the app/module
                List< CharacterInfo > vCharacterInfo    = new List<CharacterInfo>();
                List< Texture2D > vPortraits            = new List<Texture2D>();
               
                //build the character list
                for( int i = 0; i < ScallyWagsApp.NUM_CHARACTERS; ++i )
                    vCharacterInfo.Add( app.GetCharacter( i, false ) );

                //build the portrait list
                vPortraits.Add( dtTextures[ "CharSelPortrait0" ] );
                vPortraits.Add( dtTextures[ "CharSelPortrait1" ] );
                vPortraits.Add( dtTextures[ "CharSelPortrait2" ] );
                vPortraits.Add( dtTextures[ "CharSelPortrait3" ] );
                vPortraits.Add( dtTextures[ "CharSelPortrait4" ] );
                vPortraits.Add( dtTextures[ "CharSelPortrait5" ] );

                vPortraits.Add( dtTextures[ "CharSelPortraitUnknown" ] );

                //////////////////////////////
                //Create the character portraits
                float fPortraitY            = drawRect.Y + ( drawRect.Height * 0.2f );//- dtTextures[ "CharSelectBG" ].Height ) / 1.75f;

                float fHorizontalSpacing    = 5.0f;
                float fHorizontalStride     = dtTextures[ "CharSelectBG" ].Width + fHorizontalSpacing;
                float fTotalWidth           = dtTextures[ "CharSelectBG" ].Width * 4.0f + fHorizontalSpacing * 3.0f;
                float fStartX               = drawRect.X + ( drawRect.Width - fTotalWidth ) / 2.0f;

                m_vCharacterPortraits = new CharacterPortrait[4];
                for (int i = 0; i < m_vCharacterPortraits.Length; ++i)
                {
                    m_vCharacterPortraits[i] = new CharacterPortrait( i );

                    m_vCharacterPortraits[i].Init( 
                        dtTextures,
                        //dtTextures[ "CharSelectBG" ],       //The background texture
                        //dtTextures[ "CharSelectRing" ],       //The portrait border
                        fonts["PirateFont"],                //The font to display the name with
                        vPortraits,                         //The character portraits
                        vCharacterInfo,                     //The character information 
                        6 );                                //The starting index ( 6 ) is out of the valid range, so it should appear as unknown
                
                    m_vCharacterPortraits[i].X = fStartX + i * fHorizontalStride;
                    m_vCharacterPortraits[i].Y = fPortraitY;

                    AddMenuItem( m_vCharacterPortraits[i] );
                }

                //The first player should always start active on the first character portrait, so default to that here
                m_vCharacterPortraits[ 0 ].CurrentCharacter = 0;

                /////////////////////////////
                //Create the labels
                m_lblTitle = new XNALabel();
                m_lblTitle.Init(fonts["PirateFontLarge"], "Select a Character");
                m_lblTitle.X = drawRect.X + (drawRect.Width - m_lblTitle.Width) / 2.0f;
                m_lblTitle.Y = drawRect.Y + m_lblTitle.Height;// * 2.0f;

                AddMenuItem( m_lblTitle );

                //Disable the play option at first
                //m_lstLabels[0].Enabled = false;

                //////////////////////////////////
                //Create the character selector
                m_characterSelector = new CharacterSelector();

                Texture2D[] vCursors = { 
                    dtTextures[ "CharSelectCursor1" ],
                    dtTextures[ "CharSelectCursor2" ],
                    dtTextures[ "CharSelectCursor3" ],
                    dtTextures[ "CharSelectCursor4" ],
                    dtTextures[ "CharSelectCursor1NoGlow" ],
                    dtTextures[ "CharSelectCursor2NoGlow" ],
                    dtTextures[ "CharSelectCursor3NoGlow" ],
                    dtTextures[ "CharSelectCursor4NoGlow" ]  };

                m_characterSelector.Init( dtTextures[ "CharSelectMiniPortraits" ], vCursors );

                m_characterSelector.X = drawRect.X + ( drawRect.Width - m_characterSelector.Width ) / 2.0f;
                m_characterSelector.Y = drawRect.Y +  drawRect.Height - m_characterSelector.Height * 2.5f;

                AddMenuItem( m_characterSelector );

                m_tStartButton = dtTextures["xboxControllerButtonStartCrop"];
                m_sfPirateFont = fonts["PirateFontLarge"];
            }
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
            m_retID = GameMenuID.GMN_THIS;
            int NumActivePlayers = 0;

            ///////////////////////////////////////////////
            //Delay the module switch with a fade out
            if ( m_fStartDelayTimer > 0 )
            {
                m_fStartDelayTimer -= fElapsedTime;

                if(  m_fStartDelayTimer > 0 )
                    return GameMenuID.GMN_THIS;
                else 
                {   
                    ScallyWagsApp app = (ScallyWagsApp)m_parent.ParentApp;

                    //Apply all the selected characters to the game settings
                    for (int i = 0; i < 4; i++ )
                    {
                        m_parent.TheGameSettings.Characters[ i ] = app.GetCharacter(m_vCharacterPortraits[i].CurrentCharacter, m_vAIFlags[i]);
                        if (m_vAIFlags[i] == false)
                        {
                            NumActivePlayers++;
                        }
                    }
                    if (Settings.DEBUG_PLAYER_COUNT == false)
                    {
                        m_parent.TheGameSettings.NumPlayers = NumActivePlayers;
                    }
                    else
                    {
                        m_parent.TheGameSettings.NumPlayers = Settings.DEBUG_NUM_PLAYERS;
                    }
                    return GameMenuID.GMN_STARTGAME;
                }
            }

            //Player one hit back on the character select screen.. go back to the previous menu
            if( m_fBackDelayTimer > 0 )
            {
                m_fBackDelayTimer -= fElapsedTime;

                if( m_fBackDelayTimer > 0 )
                    return GameMenuID.GMN_THIS;
                else
                    return GameMenuID.GMN_MAINMENU;
            }

            base.Update(fTotalTime, fElapsedTime, input, sounds, Control);

            //handle selector inputs while there are active selections
            if (m_characterSelector.ActiveSelectors > 0)
            {
                for( int i = 0; i < 4; ++i )
                    HandlePlayerInput( i, fElapsedTime, input );
            }
            else
            {
                //Handle menu option inputs otherwise
                HandleMenuInput(fElapsedTime, input);
            }

            int NumPlayers = 4;
            for( int i = 0; i < 4; ++i )
            {
                if (m_vCharacterPortraits[i].CurrentCharacter == 6)
                {
                    NumPlayers--;
                }
            }

            if (m_characterSelector.ActiveSelectors == 0 && NumPlayers > 1)
                m_bReady = true;
            else if (m_characterSelector.ActiveSelectors == 0 && (Settings.DEBUG_PLAYER_COUNT == true || Settings.DISABLE_AI == false))
                m_bReady = true;
            else
                m_bReady = false;

            /////////////////
            //Update the state of the character portraits
            for( int i = 0; i < m_vCharacterPortraits.Length; ++i )
            {
                if( m_characterSelector.Cursors[ i ].Enabled )
                {
                    m_vCharacterPortraits[ i ].CurrentCharacter = m_characterSelector.Cursors[ i ].CharacterIndex;
                }
            }

            //don't quit right away
            if( m_fStartDelayTimer > 0 )
                return GameMenuID.GMN_THIS;

            return m_retID;
        }

        /** @fn  
         *  @brief  draw the screen
         */
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            if (m_bReady == true)
            {
                Vector2 vSizeOne    = m_sfPirateFont.MeasureString( "Press" );
                vSizeOne.Y = 0;
                Vector2 vSizeTwo    = m_sfPirateFont.MeasureString( "to Play" );
                vSizeTwo.Y = 0;
                Vector2 vSizeThree  = new Vector2(m_tStartButton.Width, m_tStartButton.Height);
                vSizeThree.Y = 0;

                //Center
                Vector2 vDrawPos = new Vector2(
                    m_characterSelector.X + m_characterSelector.Width * 0.5f,
                    m_characterSelector.Y + m_characterSelector.Height * 1.25f);

                Vector2 vDimensions = vSizeOne + vSizeTwo + vSizeThree;
                vDrawPos.X -= vDimensions.X * 0.5f;

                
                sb.DrawString(m_sfPirateFont, "to Play", vDrawPos + vSizeOne + vSizeTwo, Color.Black);
                sb.DrawString(m_sfPirateFont, "Press", vDrawPos, Color.Black);
                vSizeOne.X += vDimensions.X * 0.1f;
                sb.Draw(m_tStartButton, vDrawPos + vSizeOne, Color.White);
            }

            if( m_fStartDelayTimer > 0 )
            {
                float fFadePercent = 1.0f - m_fStartDelayTimer / START_DELAY;
                Color colFade = new Color( 0, 0, 0, (byte)( 255 * fFadePercent) );
                sb.Draw( m_texFade, new Rectangle( 0, 0, sb.GraphicsDevice.Viewport.Width, sb.GraphicsDevice.Viewport.Height ), colFade );
            }

        }

        /** @fn     void HandleInput( float fElapsedTime, InputManager input
         *  @brief  handle player input...
         *  @param  fElapsedTime [in] the time since the last frame, in seconds
         *  @param  input [in] the input manager class
         */
        private void HandlePlayerInput( int nPlayerIndex, float fElapsedTime, InputManager input )
        {
            ////////////////////////////////////////
            //Controller input
            if (input.IsButtonPressed(nPlayerIndex, Buttons.A))
            {
                m_characterSelector.HandleSelection( nPlayerIndex );
                m_smSounds.PlayCue("Flip");
            }
            if( input.IsButtonPressed( nPlayerIndex, Buttons.B ) )
            {
                if( m_characterSelector.Cursors[nPlayerIndex].Selectable == true )
                {
                    if (m_characterSelector.Cursors[nPlayerIndex].Selected == true)
                    {
                        m_characterSelector.HandleUnselection(nPlayerIndex);
                    }
                    else
                    {
                        //For first player, return to previous menu.
                        m_fBackDelayTimer = BACK_DELAY;
                        return;
                    }
                }
                else
                {
                    //For every other player, remove remove their cursor and force the portrait to unkonwn
                    m_characterSelector.HandleUnselection( nPlayerIndex );
                    m_smSounds.PlayCue("Flip");
                    m_vCharacterPortraits[ nPlayerIndex ].CurrentCharacter = 6;
                }
            }
            if (input.IsButtonPressed(nPlayerIndex, Buttons.Y))
            {
                m_retID = GameMenuID.GMN_GAMEOPTIONS;
                m_WhoPaused.Integer = nPlayerIndex;
            }

            /////////////////////////
            //Handle thumbstick/dpad input

            if (input.IsDirectionPressed(nPlayerIndex, Direction.Right))
            {
                m_smSounds.PlayCue("Slide");
                m_characterSelector.MoveSelectionRight(nPlayerIndex);
            }
            if (input.IsDirectionPressed(nPlayerIndex, Direction.Left))
            {
                m_smSounds.PlayCue("Slide");
                m_characterSelector.MoveSelectionLeft(nPlayerIndex);
            }

            //////////////////////////////
            //Check player 1 input on the keybaord as well
            if (nPlayerIndex == 0)
            {
                if( input.IsKeyPressed( Keys.Enter ) )
                {
                    m_smSounds.PlayCue("Flip");
                    m_characterSelector.HandleSelection(nPlayerIndex);
                }

                if( input.IsKeyPressed(Keys.Back ) )
                {
                    //For first player, return to previous menu.
                    m_fBackDelayTimer = BACK_DELAY;
                    return;
                }

                if (input.IsKeyPressed(Keys.Right))
                {
                    m_characterSelector.MoveSelectionRight(nPlayerIndex);
                    m_smSounds.PlayCue("Slide");
                }
                if (input.IsKeyPressed(Keys.Left))
                {
                    m_characterSelector.MoveSelectionLeft(nPlayerIndex);
                    m_smSounds.PlayCue("Slide");
                }
            }
        }

        /** @fn     void HandleMenuInput( float fElapsedTime, InputManager input )
         *  @brief  handle input when no selectors are active - essentially player one controls the menu.
         *  @param  fElapsedTime [in] 
         *  @param  input [in]
         */
        void HandleMenuInput( float fElapsedTime, InputManager input )
        {
            //Enable the play button
            //m_lstLabels[0].Enabled = true;

            //go back to character selection
            for (int Player = 0; Player < 4; Player++)
            {
                if (input.IsKeyPressed(Keys.Back) || input.IsButtonPressed(Player, Buttons.B))
                {
                    m_characterSelector.HandleUnselection(Player);
                    m_smSounds.PlayCue("Flip");

                    return;
                }

                if (input.IsButtonPressed(Player, Buttons.Y) || input.IsKeyPressed(Keys.Y))
                {
                    m_retID = GameMenuID.GMN_GAMEOPTIONS;
                    m_WhoPaused.Integer = Player;
                }
                if (Player != 0)
                {
                    if (input.IsButtonPressed(Player, Buttons.A) || input.IsKeyPressed(Keys.A))
                    {
                        m_characterSelector.HandleSelection(Player);
                        m_smSounds.PlayCue("Flip");
                    }
                }
            }
                if (input.IsButtonPressed(0, Buttons.Start) || input.IsKeyPressed(Keys.Enter))
                {
                    if (m_bReady == true)
                    {
                        m_fStartDelayTimer = START_DELAY;

                        List<int> lstIDs = m_characterSelector.GetUnselectedCharacterIDs();
                        int nUsedIndex = 0;

                        //Randomly select the unchosen characters and force them to be AI
                        for (int i = 0; i < m_vCharacterPortraits.Length; ++i)
                        {
                            if (m_vCharacterPortraits[i].CurrentCharacter == 6)
                            {
                                if (Settings.DEBUG_PLAYER_COUNT == true)
                                {
                                    m_vCharacterPortraits[i].CurrentCharacter = lstIDs[nUsedIndex];
                                    ++nUsedIndex;
                                }

                                m_vAIFlags[i] = true;
                            }
                            else
                                m_vAIFlags[i] = false;

                        }
                    }
                }
        }

        public override void ShutDown()
        {
            if( m_retID == GameMenuID.GMN_GAMEOPTIONS )
            {
                m_bSaveState = true;
                return;
            }

            m_bSaveState = false;
            base.ShutDown();
        }
    }
}
