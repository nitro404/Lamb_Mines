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
    class PauseHandler
    {
        bool m_bIsPaused;
        float m_fTotalTime;

        int m_iWhoPaused;

        PauseMenu PauseMenu;
        ControlsMenu contMenu;
        HowToPlay howPlayMenu;
        OptionsMenu optMenu;

        Texture2D m_texMenuBG;        ///< The 2D menu Background
        Texture2D m_texDisolve;       ///< The 2d texture for the screen switch effect

        ButtonIndicator m_btnIndicator;     ///< to show what buttons are available
        GameModule m_gmParentModule;
        Rectangle           m_rectScreenDraw;   ///< The area of the window the menu screens can draw to
        Dictionary<GameMenuID, MainMenuScreen> m_dtMenuScreens;    ///< The map of menu screens
        MainMenuScreen m_currentScreen;    ///< The current menu screen
                                           ///
        MenuModule.SwitchInfo m_switchInfo;       ///< Information about the active menu switch

        public bool IsPaused
        {
            get
            {
                return m_bIsPaused;
            }
            set
            {
                m_bIsPaused = value;
                if (m_bIsPaused == false)
                {
                    m_currentScreen = PauseMenu;
                    SwitchScreen(GameMenuID.GMN_PAUSEMENU);

                }
            }
        }

        public int WhoPaused
        {
            get
            {
                return m_iWhoPaused;
            }
            set
            {
                m_iWhoPaused = value;
            }
        }

        public PauseHandler()
        {
            m_switchInfo = null;
            m_rectScreenDraw = Rectangle.Empty;
            m_currentScreen = null;
            m_dtMenuScreens = new Dictionary<GameMenuID, MainMenuScreen>();
            m_fTotalTime = 0;
            m_iWhoPaused = -1;
        }

        public void Initialize(SoundManager Sounds, GameModule ParentModule)
        {
            m_fTotalTime = 0;
            m_gmParentModule = ParentModule;

            m_iWhoPaused = -1;

            int nViewWidth = ParentModule.ParentApp.Device.Viewport.Width;
            int nViewHeight = ParentModule.ParentApp.Device.Viewport.Height;

            /////////////////////////////////////////////////////////////
            //Create the and initialize 2D Screens and assets
            m_texMenuBG = ParentModule.Textures["MenuBack"];
            m_texDisolve = ParentModule.Textures["MenuBack"];

            int nBGScreenX = (nViewWidth - m_texMenuBG.Width) / 2;
            int nBGScreenY = (nViewHeight - m_texMenuBG.Height) / 2;

            m_rectScreenDraw = new Rectangle(nBGScreenX, nBGScreenY, m_texMenuBG.Width, m_texMenuBG.Height);

            PauseMenu = new PauseMenu(this);
            contMenu = new ControlsMenu();
            optMenu = new OptionsMenu(Sounds);
            howPlayMenu = new HowToPlay();

            m_currentScreen = PauseMenu;

            m_dtMenuScreens.Add(GameMenuID.GMN_PAUSEMENU, PauseMenu);
            m_dtMenuScreens.Add(GameMenuID.GMN_OPTIONS, optMenu);
            m_dtMenuScreens.Add(GameMenuID.GMN_CONTROLS, contMenu);
            m_dtMenuScreens.Add(GameMenuID.GMN_HOWTOPLAY, howPlayMenu);

            m_currentScreen.Init(ParentModule.ParentApp.Device, ParentModule.Fonts, m_rectScreenDraw, ParentModule.Textures);

            m_btnIndicator = new ButtonIndicator(ParentModule.Textures);
            m_btnIndicator.SetText("/A Select", ParentModule.Fonts["PirateFont"], m_rectScreenDraw, Color.Black);

        }

        /** @fn     MODULE_IDENTIFIER Update( GameTime gameTime )
         *  @brief  Update the module scene
         *  @return the ID of the module to switch to, or ( MID_THIS {0} ) if this module is to continue executing
         *  @param  fTotalTime [in] the total elapsed time since the app began running, in seconds
         *  @param  fElapsedTime [in] the time elapsed since the last frame
         */
        public bool Update(float fElapsedTime)
        {
            m_fTotalTime += fElapsedTime;

            if (m_switchInfo != null)
            {
                //Switching screens
                m_switchInfo.fSwitchTimer -= fElapsedTime;

                if (m_switchInfo.fSwitchTimer <= 0)
                {
                    if (m_switchInfo.bFadeOut)
                    {
                        //Perform the switch
                        m_currentScreen.ShutDown();
                        m_currentScreen = m_dtMenuScreens[m_switchInfo.gmidNext];
                        m_currentScreen.Init(m_gmParentModule.ParentApp.Device, m_gmParentModule.Fonts, m_rectScreenDraw, m_gmParentModule.Textures);

                        //Set the button indicator properties for the new screen
                        switch (m_switchInfo.gmidNext)
                        {
                            case GameMenuID.GMN_PAUSEMENU:
                                m_btnIndicator.SetText("/A Select", m_gmParentModule.Fonts["PirateFont"], m_rectScreenDraw, Color.Black);
                                break;
                            case GameMenuID.GMN_CONTROLS:
                                m_btnIndicator.SetText("/A How To Play   /B Back To Menu", m_gmParentModule.Fonts["PirateFont"], m_rectScreenDraw, Color.Black);
                                break;
                            case GameMenuID.GMN_OPTIONS:
                                m_btnIndicator.SetText("/A Select   /B Cancel", m_gmParentModule.Fonts["PirateFont"], m_rectScreenDraw, Color.Black);
                                break;
                            case GameMenuID.GMN_HOWTOPLAY:
                                m_btnIndicator.SetText("/A Back To Menu   /B Controls", m_gmParentModule.Fonts["PirateFont"], m_rectScreenDraw, Color.Black);
                                break;
                            default:
                                break;

                        }

                        //Reset the switch info
                        m_switchInfo.fSwitchTimer = MenuModule.SwitchInfo.FADE_IN_TIME;
                        m_switchInfo.bFadeOut = false;
                    }
                    else
                    {
                        //Fade sequence is complete
                        m_switchInfo = null;
                    }
                }
            }
            else
            {
                /////////////////////////////////////
                //Update the current screen and Handle any requested state change
                GameMenuID menuID = m_currentScreen.Update(m_fTotalTime, fElapsedTime, m_gmParentModule.ParentApp.Inputs, m_gmParentModule.ParentApp.SoundPlayer, m_iWhoPaused);

                if (menuID == GameMenuID.GMN_UNPAUSE)
                {
                    SwitchScreen(GameMenuID.GMN_PAUSEMENU);
                    m_bIsPaused = false;
                    menuID = GameMenuID.GMN_PAUSEMENU;
                    m_gmParentModule.SetCameraToTargeter();
                }

                switch (menuID)
                {
                    case GameMenuID.GMN_CONTROLS:
                        SwitchScreen(GameMenuID.GMN_CONTROLS);
                        break;
                    case GameMenuID.GMN_MAINMENU:
                        SwitchScreen(GameMenuID.GMN_PAUSEMENU);
                        break;
                    case GameMenuID.GMN_OPTIONS:
                        SwitchScreen(GameMenuID.GMN_OPTIONS);
                        break;
                    case GameMenuID.GMN_HOWTOPLAY:
                        SwitchScreen(GameMenuID.GMN_HOWTOPLAY);
                        break;
                    case GameMenuID.GMN_UNPAUSE:
                        m_bIsPaused = false;
                        break;
                    case GameMenuID.GMN_CREDITS:
                        return false;
                    default:
                        //GameMenuID.GMN_THIS
                        break;
                }
            }
            return true;
        }

        /** @fn     void Draw( GameTime gameTime )
        *  @brief  Draw the module's scene
        *  @param  device [in] the active graphics device
        *  @param  gameTime [in] information about the time between frames
        */
        public void Draw(SpriteBatch sb)
        {
            if (m_bIsPaused == true)
            {
                //Draw the menu background
                sb.Draw(m_texMenuBG, m_rectScreenDraw, Color.White);

                //Draw the button indicator
                m_btnIndicator.Draw(sb);

                //Draw the current menu
                m_currentScreen.Draw(sb);

                //Draw the transition effect if it's switching
                if (m_switchInfo != null)
                {
                    float fPercentComplete = 1.0f - m_switchInfo.fSwitchTimer / MenuModule.SwitchInfo.FADE_OUT_TIME;

                    if (m_switchInfo.bFadeOut == false)
                        fPercentComplete = (m_switchInfo.fSwitchTimer / MenuModule.SwitchInfo.FADE_IN_TIME);

                    Color colFade = Color.White;
                    colFade.A = (byte)(255.0f * fPercentComplete);

                    sb.Draw(m_texMenuBG, m_rectScreenDraw, colFade);
                }
            }
        }

        /** @fn     void SwitchScreen( GameMenuID gmid )
        *   @brief  switch the active menu screen
            @param  gmid [in] the ID of the screen to switch to
        */
        private void SwitchScreen(GameMenuID gmid)
        {
            m_gmParentModule.ParentApp.SoundPlayer.PlayCue("pageTurn");
            //ParentApp.SoundPlayer.PlayCue("Menu_Transition");
            //ParentApp.SoundPlayer.PlayCue( "wearepirates" );
            m_switchInfo = new MenuModule.SwitchInfo(gmid);
        }

    }
}
