using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Scallywags
{
    public class GameOptions : MainMenuScreen
    {   
        XNALabel        m_lblTitle;             ///< The menu title
        XNALabel[]      m_vLabels;              ///< The menu screen labels
        XNALabel        m_lblDescription;       ///< The description of the selected option                                        

        int             m_nSelectedOption;      ///< What menu item is selected?  
        int             m_nSelectedGoldOption;  ///< The selected amount 
        int             m_nSelectedTurnOption;  ///< The selected amount of turns
        GameSettings    m_gameSettings;         ///< The active game settings

        string [] m_vGoldAmounts;               ///< The text options for gold
        string [] m_vTurnLimits;                ///< The text options for turns

        string [] m_vGoldDescriptions;          ///< The text to describe the gold options
        string [] m_vTurnDescriptions;          ///< The text to describe the turn options
                                                
        Rectangle m_drawRect;                   ///< The area we can draw to
        SoundManager m_smSounds;

        public GameOptions( GameSettings settings, SoundManager Sounds)
        {
            m_gameSettings = settings;

            m_vGoldAmounts = new string[4];
            m_vGoldAmounts[0] = "3";
            m_vGoldAmounts[1] = "4";
            m_vGoldAmounts[2] = "5";
            m_vGoldAmounts[3] = "Total Pillage";

            m_vTurnLimits  = new string[4];
            m_vTurnLimits[0] = "10";
            m_vTurnLimits[1] = "20";
            m_vTurnLimits[2] = "30";
            m_vTurnLimits[3] = "Infinite";

            m_vGoldDescriptions = new string [4];
            m_vGoldDescriptions[0] = "The first player to gather 3 gold coins at their home port wins!";
            m_vGoldDescriptions[1] = "The first player to gather 4 gold coins at their home port wins!";
            m_vGoldDescriptions[2] = "The first player to gather 5 gold coins at their home port wins!";
            m_vGoldDescriptions[3] = "The player with the most gold coins when the final coin is pillaged wins!";

            m_vTurnDescriptions = new string[4];
            m_vTurnDescriptions[0] = "The player with the most gold after 10 turns wins!";
            m_vTurnDescriptions[1] = "The player with the most gold after 20 turns wins!";
            m_vTurnDescriptions[2] = "The player with the most gold after 30 turns wins!";
            m_vTurnDescriptions[3] = "Play will continue until the gold limit is reached.";

            m_smSounds = Sounds;
        }

        /** @fn     void Init
         *  @param  device [in] the active rendering device
         *  @param  font [in] the font the menu items will use
         *  @param  drawRect [in] the area of the screen this item can draw to
         *  @brief  initialize the menu screen
         */
        public override void Init(GraphicsDevice device, Dictionary<string, SpriteFont> fonts, Rectangle drawRect, Dictionary<String, Texture2D> dtTextures)
        {
            m_nSelectedOption       = 0;
            m_nSelectedGoldOption   = 0;
            m_nSelectedTurnOption   = 0;
            m_drawRect              = drawRect;

            //Create the labels
            m_vLabels = new XNALabel[5];

            for( int i = 0; i < m_vLabels.Length; ++i )
                m_vLabels[ i ] = new XNALabel();

            m_vLabels[0].Init(fonts["PirateFontLarge"], "Gold Amount - ");
            m_vLabels[2].Init(fonts["PirateFontLarge"], "TurnLimit   - ");

            m_lblTitle = new XNALabel();
            m_lblTitle.Init(fonts["PirateFontLarge"], "Game Options");
            m_lblTitle.X = drawRect.X + (drawRect.Width - m_lblTitle.Width) / 2.0f;
            m_lblTitle.Y = drawRect.Y + m_lblTitle.Height;// * 2.0f;

            if( m_gameSettings.GoldLimit == - 1 )
                m_nSelectedGoldOption = 3;
            else
                m_nSelectedGoldOption = m_gameSettings.GoldLimit - 3;//( m_gameSettings.GoldLimit / 3 ) - 1;

            if( m_gameSettings.TurnLimit == -1 )
                m_nSelectedTurnOption = 3;
            else
                m_nSelectedTurnOption = ( m_gameSettings.TurnLimit / 10 ) - 1;

            m_vLabels[1].Init(fonts["PirateFontLarge"], m_vGoldAmounts[m_nSelectedGoldOption]);
            m_vLabels[3].Init(fonts["PirateFontLarge"], m_vTurnLimits[m_nSelectedTurnOption]);

            /////////////////////////////
            //Position the items
            float fHeadingStartX      = drawRect.X + 200;
            float fOptionStart        = drawRect.X + 450;

            float fGoldY = drawRect.Y + drawRect.Height * 0.25f;
            float fTurnY = drawRect.Y + drawRect.Height * 0.45f;

            m_vLabels[ 0 ].Selected = true;

            m_vLabels[ 0 ].X = fHeadingStartX;
            m_vLabels[ 0 ].Y = fGoldY;

            m_vLabels[ 1 ].X = fOptionStart;
            m_vLabels[ 1 ].Y = fGoldY;            

            m_vLabels[ 2 ].X = fHeadingStartX;
            m_vLabels[ 2 ].Y = fTurnY;

            m_vLabels[ 3 ].X = fOptionStart;
            m_vLabels[ 3 ].Y = fTurnY;

            /////////////////////////
            //Back option
            float fHorizontalGap = 100.0f;   ///< Distance from the right of the draw area to draw items
            float fVerticalGap   = 100.0f;   ///< Distance from the bottom of the draw area to draw items

            m_vLabels[4].Init(fonts["PirateFontLarge"], "OK");
            m_vLabels[4].X = drawRect.X + drawRect.Width - fHorizontalGap - m_vLabels[4].Width;
            m_vLabels[4].Y = drawRect.Y + drawRect.Height - fVerticalGap; ;

            //////////////////////////
            //Description
            m_lblDescription = new XNALabel();
            m_lblDescription.Init( fonts[ "PirateFontMedium" ], "" );

            //Add the menu items
            AddMenuItem(m_vLabels[0]);
            AddMenuItem(m_vLabels[1]);
            AddMenuItem(m_vLabels[2]);
            AddMenuItem(m_vLabels[3]);
            AddMenuItem(m_vLabels[4]);

            AddMenuItem( m_lblTitle );
            AddMenuItem( m_lblDescription );
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

            /////////////////////////////////
            //Handle enter key press
            if (input.IsKeyPressed(Keys.Enter))
            {
                switch (m_nSelectedOption)
                {
                    case 0:
                        break;
                    case 1:
                        break;
                    case 2:
                        {
                            //Apply the game settings
                            switch (m_nSelectedGoldOption)
                            {
                                case 0:
                                    {
                                        m_gameSettings.GoldLimit = 3;
                                        m_gameSettings.EndOnTotalPillage = false;

                                        break;
                                    }
                                case 1:
                                    {
                                        m_gameSettings.GoldLimit = 4;
                                        m_gameSettings.EndOnTotalPillage = false;
                                        break;
                                    }
                                case 2:
                                    {
                                        m_gameSettings.GoldLimit = 5;
                                        m_gameSettings.EndOnTotalPillage = false;
                                        break;
                                    }
                                default:
                                    {
                                        m_gameSettings.GoldLimit = -1;
                                        m_gameSettings.EndOnTotalPillage = true;
                                        break;
                                    }
                            }

                            switch (m_nSelectedTurnOption)
                            {
                                case 0:
                                    {
                                        m_gameSettings.TurnLimit = 10;
                                        break;
                                    }
                                case 1:
                                    {
                                        m_gameSettings.TurnLimit = 20;
                                        break;
                                    }
                                case 2:
                                    {
                                        m_gameSettings.TurnLimit = 30;
                                        break;
                                    }
                                default:
                                    {
                                        m_gameSettings.TurnLimit = -1;
                                        break;
                                    }
                            }

                            return GameMenuID.GMN_PLAYERSELECT;
                        }
                }
            }

            if (input.IsButtonPressed(Control, Buttons.A))
            {
                switch (m_nSelectedOption)
                {
                    case 0:
                        break;
                    case 1:
                        break;
                    case 2:
                        {
                            //Apply the game settings
                            switch (m_nSelectedGoldOption)
                            {
                                case 0:
                                    {
                                        m_gameSettings.GoldLimit = 3;
                                        m_gameSettings.EndOnTotalPillage = false;

                                        break;
                                    }
                                case 1:
                                    {
                                        m_gameSettings.GoldLimit = 4;
                                        m_gameSettings.EndOnTotalPillage = false;
                                        break;
                                    }
                                case 2:
                                    {
                                        m_gameSettings.GoldLimit = 5;
                                        m_gameSettings.EndOnTotalPillage = false;
                                        break;
                                    }
                                default:
                                    {
                                        m_gameSettings.GoldLimit = -1;
                                        m_gameSettings.EndOnTotalPillage = true;
                                        break;
                                    }
                            }

                            switch (m_nSelectedTurnOption)
                            {
                                case 0:
                                    {
                                        m_gameSettings.TurnLimit = 10;
                                        break;
                                    }
                                case 1:
                                    {
                                        m_gameSettings.TurnLimit = 20;
                                        break;
                                    }
                                case 2:
                                    {
                                        m_gameSettings.TurnLimit = 30;
                                        break;
                                    }
                                default:
                                    {
                                        m_gameSettings.TurnLimit = -1;
                                        break;
                                    }
                            }

                            return GameMenuID.GMN_PLAYERSELECT;
                        }
                }
            }
            if (input.IsKeyPressed(Keys.Down) || input.IsDirectionPressed(Control, Direction.Down))
            {
                HandleDownPress();
                m_smSounds.PlayCue("Flip");
            }
            if (input.IsKeyPressed(Keys.Up) || input.IsDirectionPressed(Control, Direction.Up))
            {
                HandleUpPress();
                m_smSounds.PlayCue("Flip");
            }
            if (input.IsKeyPressed(Keys.Right) || input.IsDirectionPressed(Control, Direction.Right))
            {
                HandleRightPress();
                m_smSounds.PlayCue("Slide");
            }
            if (input.IsKeyPressed(Keys.Left) || input.IsDirectionPressed(Control, Direction.Left))
            {
                HandleLeftPress();
                m_smSounds.PlayCue("Slide");
            }

            //Back without saving
            if (input.IsButtonPressed(Control, Buttons.B))
                return GameMenuID.GMN_PLAYERSELECT;

            if (input.IsKeyPressed(Keys.Down))
            {
                HandleDownPress();
                m_smSounds.PlayCue("Flip");
            }
            if (input.IsKeyPressed(Keys.Up))
            {
                HandleUpPress();
                m_smSounds.PlayCue("Flip");
            }
            if (input.IsKeyPressed(Keys.Right))
            {
                HandleRightPress();
                m_smSounds.PlayCue("Slide");
            }
            if (input.IsKeyPressed(Keys.Left))
            {
                HandleLeftPress();
                m_smSounds.PlayCue("Slide");
            }

            //Back without saving
            if (input.IsKeyPressed(Keys.Back))
                return GameMenuID.GMN_PLAYERSELECT;

            /////////////////////////////
            //Set the correct description
            switch (m_nSelectedOption)
            {
                case 0:
                    m_lblDescription.Text = m_vGoldDescriptions[m_nSelectedGoldOption];
                    PositionDescription();
                    break;
                case 1:
                    m_lblDescription.Text = m_vTurnDescriptions[m_nSelectedTurnOption];
                    PositionDescription();
                    break;
                default:
                    m_lblDescription.Text = "";
                    break;
            }


            return GameMenuID.GMN_THIS;
        }

        /** @fn
         *  @brief  position the item description in the draw area
         */
        private void PositionDescription()
        {
            Vector2 vDim = m_lblDescription.Font.MeasureString( m_lblDescription.Text );

            if( vDim.X < m_drawRect.Width )
            {
                float fStartX = m_drawRect.X + (m_drawRect.Width - vDim.X) * 0.5f;
                float fStartY = m_drawRect.Y + m_drawRect.Height - vDim.Y * 6.0f;

                m_lblDescription.X = fStartX;
                m_lblDescription.Y = fStartY;
            }
        }
        
        /** @fn     void HandleDownPress()
         *  @brief  handle the user pressing down
         */
        private void HandleDownPress()
        {
            //Every other label is an option
            int nLabelIndex = m_nSelectedOption * 2;

            m_vLabels[ nLabelIndex ].Selected = false;
            m_nSelectedOption = (m_nSelectedOption + 1) % 3;

            nLabelIndex = m_nSelectedOption * 2;
            m_vLabels[nLabelIndex].Selected = true;
        }

        /** @fn     void HandleUpPress()
         *  @brief  handle the user pressing up
         */
        private void HandleUpPress()
        {
            int nLabelIndex = m_nSelectedOption * 2;

            m_vLabels[nLabelIndex].Selected = false;
            m_nSelectedOption = (m_nSelectedOption + 2) % 3;

            nLabelIndex = m_nSelectedOption * 2;
            m_vLabels[nLabelIndex].Selected = true;
        }

        /** @fn     void HandleRightPress()
         *  @brief  handle the user pressing right
         */
        private void HandleRightPress()
        {
            if( m_nSelectedOption == 0 )
            {
                //Switch gold amount sub option
                m_nSelectedGoldOption = ( m_nSelectedGoldOption + 1 ) % 4;
                m_vLabels[ 1 ].Text = m_vGoldAmounts[ m_nSelectedGoldOption ];
            }
            else if (m_nSelectedOption == 1)
            {
                m_nSelectedTurnOption = (m_nSelectedTurnOption + 1) % 4;
                m_vLabels[3].Text = m_vTurnLimits[m_nSelectedTurnOption];
            }
        }

        /** @fn     void HandleLeftPress()
         *  @brief  handle the user pressing left
         */
        private void HandleLeftPress()
        {
            if( m_nSelectedOption == 0 )
            {
                //Switch gold amount sub option
                m_nSelectedGoldOption = ( m_nSelectedGoldOption + 3 ) % 4;
                m_vLabels[1].Text = m_vGoldAmounts[m_nSelectedGoldOption];
            }
            else if( m_nSelectedOption == 1 )
            {
                m_nSelectedTurnOption = ( m_nSelectedTurnOption + 3 ) % 4;
                m_vLabels[3].Text = m_vTurnLimits[ m_nSelectedTurnOption ];
            }
        }
    }
}
