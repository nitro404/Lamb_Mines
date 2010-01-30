using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags
{
    /// <summary>
    /// victory screen
    /// </summary>
    public class WinScreen : HudItem
    {
        /// <summary>
        /// the states of the win screen
        /// </summary>
        private enum WinScreenState
        {
            WS_VICTORY_MSG,
            WS_AWARD_SLIDE,
            WS_GOLD_COUNT,
            WS_GAME_OVER
        };

        const int       WIN_SPARKS  = 5000;         //The number of sparks to use
        const float     SPARK_TIME  = 10.0f;        //Seconds to show sparks for
        const string    WIN_MESSAGE = "Victory!";   //the message to write when someone wins

        const float     GROW_RATE   = 0.5f;         //It takes this many seconds to reach the max message scale
        const float     MAX_SCALE   = 1.0f;         //the max message size

        private WinScreenState  m_state;            //The state of the win screen

        private float           m_fTitleScale;      //The scale of the win message text
        private Vector2         m_vTitleDimensions; //The string width/height of the title message
        private Vector2         m_vTitlePosition;   //The position of the top left corner of the win message

        private Texture2D       m_texBG;            //The background texture

        private List< AwardInfo >   m_lstAwards;    //The list of awards
        private float               m_fAwardLeft;   //The left margin X value

        private AnimatedTexture     m_texCoinAnim;  //The animated coin texture
        private SpriteFont          m_fontAwardText;//The font to use for award text

//        private Statistics[]        m_WinPlayerStats;//The statistics for the players (passed from turn manager)
        private string[]            m_Awards;        //The awards/titles each player recieves
        private float               m_fSparkTimer;  //Time to show sparks

        private SparkEmitter        m_emitterLeft;  //to launch sprites
        private SparkEmitter        m_emitterRight; //to launch sprites

        private ButtonIndicator     m_buttonIndicator;  //The UI hint to exit the game

        public bool WinComplete
        {
            get
            {
                return m_state == WinScreenState.WS_GAME_OVER;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public  WinScreen()
        {
            m_fTitleScale   = 0.1f;

            m_texBG         = null;
            m_texCoinAnim   = null;
            m_fontAwardText = null;
            m_fAwardLeft    = 0;
            m_fSparkTimer   = 0;

            m_vTitleDimensions = m_vTitlePosition = Vector2.Zero;
            m_state = WinScreenState.WS_VICTORY_MSG;
        }

        /// <summary>
        /// Initialize the win screen
        /// </summary>
        /// <param name="fX">the x location of the upper left hand corner of the draw area</param>
        /// <param name="fY">the y location of the upper left hand corner of the draw area</param>
        /// <param name="fWidth">the width of the draw area</param>
        /// <param name="fHeight">the height of the draw area</param>
        /// <param name="device">the active rendering device</param>
        /// <param name="dtTextures">the loaded textures</param>
        /// <param name="fontAward">the font to use for the award text</param>
        public void Init( float fX, float fY, float fWidth, float fHeight, 
            GraphicsDevice device, Dictionary< string, Texture2D > dtTextures,
            SpriteFont fontAward, SpriteFont fontStartOption )
        {
            //Start by showing the vicotry message
            m_state = WinScreenState.WS_VICTORY_MSG;

            m_fTitleScale   = 0.1f;
            m_texBG         = dtTextures[ "endTurnBG" ];
            m_fontAwardText = fontAward;

            ///////////////////////////////////////
            //Create the sparks
            Texture2D texSpark  = new Texture2D(device, 5, 5);
            Color colInvis      = new Color( 0, 0, 0, 0 );
            Color colHalfVis    = Color.White;
            colHalfVis.A        = 100;

            Color[] vTexColor = { 
                colInvis,    colHalfVis,  Color.White, colHalfVis,  colInvis,
                colHalfVis,  Color.White, Color.White, Color.White, colHalfVis,
                Color.White, Color.White, Color.White, Color.White, Color.White,
                colHalfVis,  Color.White, Color.White, Color.White, colHalfVis,
                colInvis,    colHalfVis,  Color.White, colHalfVis,  colInvis  };

            texSpark.SetData<Color>(vTexColor);

            //The left spark emitter
            float fLeftDir              = MathHelper.PiOver2;
            float fLeftVariance         = fLeftDir + MathHelper.PiOver2;
            Vector2 vLeftDirection      = new Vector2( ( float )Math.Cos( -fLeftDir ), (float)Math.Sin( -fLeftDir ) );
            Vector2 vLeftVarianceDir    = new Vector2( ( float )Math.Cos( -fLeftVariance ), ( float )Math.Sin( -fLeftVariance ) ); 
            
            m_emitterLeft = new SparkEmitter();
            m_emitterLeft.Init( 
                new Vector2( X + fWidth * 0.25f, Y + fHeight ),         //location
                WIN_SPARKS,                                             //number of sparks
                5.0f,                                                   //spark life
                vLeftDirection,                                         //spark direction
                vLeftVarianceDir * 300.0f,                              //Emission variance
                new Vector2( 0.0f, 1.0f ),                              //Direction of decay
                800.0f,                                                 //launch speed
                400.0f,                                                 //decay speed
                10.0f,                                                  //total life
                texSpark,                                               //texture to use
                new Color[] { Color.Purple, Color.Gold, Color.Green } );    //Colours

            //The right spark emitter
            m_emitterRight = new SparkEmitter();
            m_emitterRight.Init( 
                new Vector2( X + fWidth * 0.75f, Y + fHeight ),         //location
                WIN_SPARKS,                                             //number of sparks
                5.0f,                                                   //spark life
                new Vector2( 0, -1.0f ),                                //spark direction
                new Vector2( 300.0f, 0.0f ),                            //Emission variance
                new Vector2( 0.0f, 1.0f ),                              //Direction of decay
                800.0f,                                                 //launch speed
                400.0f,                                                 //decay speed
                10.0f,                                                  //total life
                texSpark,                                               //texture to use
                new Color[] { Color.Purple, Color.Gold, Color.Green } );    //Colours

            X               = fX;
            Y               = fY;

            base.Init( fWidth, fHeight );

            m_fAwardLeft = X + Width * 0.1f;

            /////////////////////////
            //Animation related
            m_texCoinAnim = new AnimatedTexture( dtTextures[ "coin_anim" ], 100, 50 );   
         
            m_buttonIndicator       = new ButtonIndicator( dtTextures );
            m_buttonIndicator.SetText( 
                "Press /S to return to the menu", 
                fontStartOption, 
                new Rectangle( (int)X, (int)(Y + Height * 0.9f), (int)Width, (int)( Height * 0.1f ) ), Color.White );
        }

        /// <summary>
        /// Determine the winner and stats
        /// </summary>
        /// <param name="vPlayers">the list of players</param>
        public void GenerateStatistics( Player[] vPlayers, Dictionary< string, Texture2D > dtTextures, int WinningPlayerIndex )
        {
            //modify here

            m_lstAwards = new List<AwardInfo>();

            //Get the ranked listing
            List< Player > lstCurrent   = new List<Player>( vPlayers );
            List< Player > lstRanked    = new List<Player>();
            
            int nTotalPlayers = lstCurrent.Count;

            m_Awards = new string[nTotalPlayers];

            for(int i = 0; i< nTotalPlayers; i++)
            {
                m_Awards[i] = string.Empty;
            }

            while (lstRanked.Count < nTotalPlayers )
            {
                //Find the next highest ranking player
                int nRankIndex = FindHighestRankingIndex( lstCurrent );
                
                lstRanked.Add( lstCurrent[ nRankIndex ] );
                lstCurrent.RemoveAt( nRankIndex );
            }

            //Create the award objects
            float fYPosition = Y + Height * 0.075f;
            float fYGap      = 5.0f;
            float fXPos      = X + Width * 1.5f;
            
            for( int i = 0; i < lstRanked.Count; ++i )
            {
                m_Awards[i] = AwardText(lstRanked, i, WinningPlayerIndex, nTotalPlayers, m_Awards);

                //set up the initial state of the award
                AwardInfo newAward      = new AwardInfo( m_texCoinAnim, lstRanked[i].totalGold );
                newAward.Text           = lstRanked[i].Name + "\n" + m_Awards[i];
                newAward.Position       = new Vector2( fXPos, fYPosition );
                
                newAward.VisibleGold    = 0;

                //pick the correct portrait
                switch( lstRanked[i].CharacterID )
                {
                    case 0:
                        newAward.Portrait   = dtTextures[ "win_duke" ];
                        break;
                    case 1:
                        newAward.Portrait   = dtTextures[ "win_irongut" ];
                        break;
                    case 2:
                        newAward.Portrait   = dtTextures[ "win_scarlet" ];
                        break;
                    case 3:
                        newAward.Portrait   = dtTextures[ "win_indigo" ];
                        break;
                    case 4:
                        newAward.Portrait   = dtTextures[ "win_amber" ];
                        break;
                    case 5:
                        newAward.Portrait   = dtTextures[ "win_moldy" ];
                        break;
                }

                m_lstAwards.Add( newAward );

                //advance the vertical position of the awards
                fYPosition += newAward.Portrait.Height + fYGap;
            }
        }

        /// <summary>
        /// Find the index of the highest ranking player in a player array
        /// </summary>
        /// <param name="lstPlayers">the list of game players</param>
        /// <returns>the index in the list of the player with the highest score</returns>
        private int FindHighestRankingIndex( List< Player > lstPlayers )
        {
            int nMaxGold = -1;
            int nMaxGoldIndex = -1;

            for (int i = 0; i < lstPlayers.Count; ++i)
            {
                if (lstPlayers[i].totalGold > nMaxGold)
                {
                    nMaxGold = lstPlayers[i].totalGold;
                    nMaxGoldIndex = i;
                }
            }

            return nMaxGoldIndex;
        }

        /// <summary>
        /// Draw the victory screen thing
        /// </summary>
        /// <param name="sb">the sprite batch to draw with</param>
        /// <param name="font">the font to draw text with</param>
        public override void Draw( SpriteBatch sb, SpriteFont font )
        {
            switch (m_state)
            {
                case WinScreenState.WS_VICTORY_MSG:
                {
                    DrawTitle(sb, font);
                    break;
                }
                case WinScreenState.WS_AWARD_SLIDE:
                {
                    DrawSparks(sb);
                    DrawTitle(sb, font);
                    DrawLeaderBoard(sb);
                    DrawCoins(sb);

                    DrawPlace( sb, m_fontAwardText );

                    break;
                }
                case WinScreenState.WS_GOLD_COUNT:
                {
                    DrawSparks(sb);
                    DrawTitle(sb, font);
                    DrawLeaderBoard(sb);
                    DrawCoins( sb );

                    DrawPlace(sb, m_fontAwardText );

                    break;
                }
                case WinScreenState.WS_GAME_OVER:
                {
                    DrawSparks(sb);
                    DrawTitle(sb, font);
                    DrawLeaderBoard(sb);
                    DrawCoins(sb);

                    DrawPlace(sb, m_fontAwardText);

                    m_buttonIndicator.Draw( sb );

                    break;   
                }
            }
         }

        /// <summary>
        /// update the victory message
        /// </summary>
        /// <param name="fElapsedTime">the time elapsed since the previous frame</param>
        public override void Update(float fElapsedTime)
        {
            switch( m_state )
            {
                case WinScreenState.WS_VICTORY_MSG:
                {
                    //Enlarge the message
                    if( MAX_SCALE > m_fTitleScale )
                        m_fTitleScale += fElapsedTime / GROW_RATE;

                    if (m_fTitleScale >= MAX_SCALE)
                    {
                        m_fSparkTimer   = SPARK_TIME; 
                        m_state         = WinScreenState.WS_AWARD_SLIDE;
                    }

                    break;
                }
                case WinScreenState.WS_AWARD_SLIDE:
                {
                    if( m_fSparkTimer > 0 )
                        m_fSparkTimer -= fElapsedTime;

                    UpdateSparks( fElapsedTime );
                    UpdateAwards( fElapsedTime );

                    break;
                }
                case WinScreenState.WS_GOLD_COUNT:
                {
                    UpdateSparks(fElapsedTime);
                    UpdateCoins( fElapsedTime );

                    break;
                }
                case WinScreenState.WS_GAME_OVER:
                {
                    UpdateSparks(fElapsedTime);
                    
                    break;
                }
            }
        }

        /// <summary>
        /// Draw the active sparks
        /// </summary>
        /// <param name="sb">the active sprite batch</param>
        private void DrawSparks( SpriteBatch sb ) 
        {
            m_emitterLeft.Draw( sb );
            m_emitterRight.Draw( sb );
        }

        /// <summary>
        /// Draw the victory title
        /// </summary>
        /// <param name="sb">the active sprite batch</param>
        /// <param name="font">the font to use to draw the message</param>
        private void DrawTitle( SpriteBatch sb, SpriteFont font )
        {
            float fTitleYPercent = 0.85f;

            Vector2 vBGPos = new Vector2(
                X + (Width * 0.5f),
                Y + (Height * fTitleYPercent));

            Vector2 vBGCenter = new Vector2(
                (m_texBG.Width) * 0.5f,
                (m_texBG.Height) * 0.5f);

            sb.Draw(m_texBG, vBGPos, null, Color.White, 0, vBGCenter, m_fTitleScale, SpriteEffects.None, 0.1f);

            //The center of the title position
            Vector2 vTitleCenter = new Vector2(
                X + Width * 0.5f,
                Y + Height * fTitleYPercent);

            m_vTitleDimensions = font.MeasureString(WIN_MESSAGE) * m_fTitleScale;
            m_vTitlePosition = vTitleCenter - m_vTitleDimensions * 0.5f;

            Vector2 vCaptionOffset = new Vector2(2.0f, 2.0f);

            sb.DrawString(font, WIN_MESSAGE, m_vTitlePosition + vCaptionOffset, Color.Black, 0, Vector2.Zero, m_fTitleScale, SpriteEffects.None, 0.1f);
            sb.DrawString(font, WIN_MESSAGE, m_vTitlePosition, Color.Gold, 0, Vector2.Zero, m_fTitleScale, SpriteEffects.None, 0.1f);
        }

        /// <summary>
        /// Draw the leaderboard
        /// </summary>
        /// <param name="sb">the active sprite batch</param>
        private void DrawLeaderBoard( SpriteBatch sb )
        {
            for( int i = 0; i < m_lstAwards.Count; ++i )
            {
                Vector2 vDim        = m_fontAwardText.MeasureString( m_lstAwards[i].Text );
                Vector2 vTextPos    = m_lstAwards[i].Position;
                vTextPos.X += m_lstAwards[i].Portrait.Width * 1.5f;
                vTextPos.Y += ( m_lstAwards[i].Portrait.Height - vDim.Y ) * 0.5f; //Center the text on the portrait
                
                //Draw portrait
                sb.Draw( m_lstAwards[i].Portrait, m_lstAwards[i].Position, Color.White );

                //////////////////////////
                //draw text Shadows
                Vector2[] vOffsets = {
                    new Vector2(  2.0f,  2.0f ),
                    new Vector2(  2.0f, -2.0f ),
                    new Vector2( -2.0f,  2.0f ),
                    new Vector2( -2.0f, -2.0f ) };

                for (int j = 0; j < vOffsets.Length; ++j)
                    sb.DrawString(m_fontAwardText, m_lstAwards[i].Text, vTextPos + vOffsets[j], Color.Black);

                //Draw the foreground text
                sb.DrawString( m_fontAwardText, m_lstAwards[i].Text, vTextPos, Color.White );
            }
        }

        /// <summary>
        /// Draw the coin animations
        /// </summary>
        /// <param name="sb">the active sprite batch</param>
        private void DrawCoins( SpriteBatch sb )
        {
            for( int i = 0; i < m_lstAwards.Count; ++i )
            {
                //Draw the gold pile for completed 
                if( m_lstAwards[i].SlideComplete )
                {
                    //Adjust for animated textures being drawn from their center points.
                    Vector2 vCellDim = new Vector2(
                        m_texCoinAnim.CellWidth,
                        m_texCoinAnim.CellHeight );

                    Vector2 vCoinLoc = m_lstAwards[i].Position + vCellDim * 0.5f;
                    vCoinLoc.X -= m_lstAwards[i].Portrait.Width * 0.25f;
                    vCoinLoc.Y += m_lstAwards[i].Portrait.Height - m_texCoinAnim.CellHeight;

                    m_lstAwards[i].CoinAnimation.Draw( vCoinLoc, 0, sb );
                }
            }
        }

        /// <summary>
        /// Draw the numeric place on the portrait
        /// </summary>
        /// <param name="sb">the active sprite batch</param>
        /// <param name="font">the font to use</param>
        private void DrawPlace(SpriteBatch sb, SpriteFont font)
        {
            for (int i = 0; i < m_lstAwards.Count; ++i)
            {
                Color colText = Color.White;

                if (m_lstAwards[i].SlideComplete)
                {
                    string strPlaceText = "";

                    switch (i)
                    {
                        case 0:
                            strPlaceText    = "1st";
                            colText         = Color.Gold;
                            break;
                        case 1:
                            strPlaceText    = "2nd";
                            colText         = Color.Silver;
                            break;
                        case 2:
                            strPlaceText    = "3rd";
                            colText         = new Color( 140, 120, 83 );//Color.Bronze;
                            break;
                        case 3:
                            strPlaceText    = "4th";
                            colText         = Color.DimGray;
                            break;
                    }

                    Vector2 vDim = font.MeasureString(strPlaceText);

                    //draw the number...
                    Vector2 vPos = m_lstAwards[i].Position;
                    vPos.Y += m_lstAwards[i].Portrait.Height - vDim.Y;
                    vPos.X += m_lstAwards[i].Portrait.Width * 0.75f;

                    Vector2 vOffset = new Vector2(
                        1.0f, 1.0f );

                    sb.DrawString(font, strPlaceText, vPos + vOffset, Color.Black);
                    sb.DrawString(font, strPlaceText, vPos, colText );//Color.Gold );
                }
            }
        }

        /// <summary>
        /// Update the awards slide ins
        /// </summary>
        /// <param name="fElapsedTime">time elapsed since the last frame</param>
        private void UpdateAwards( float fElapsedTime )
        {
            /////////////////////////////
            //Slide the player awards in
            float fSlideSpeed = 1000.0f;

            for( int i = 0; i < m_lstAwards.Count; ++i )
            {
                if( m_lstAwards[i].SlideComplete == false )
                {
                    m_lstAwards[i].X -= fSlideSpeed * fElapsedTime;

                    if (m_lstAwards[i].X <= m_fAwardLeft)
                    {
                        m_lstAwards[i].X                = m_fAwardLeft;
                        m_lstAwards[i].SlideComplete    = true;

                        //Start the animation and switch the screen state to gold counting
                        m_lstAwards[i].CoinAnimation.Start();
                        m_state = WinScreenState.WS_GOLD_COUNT;
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Update the active coin animation
        /// </summary>
        /// <param name="fElapsedTime"></param>
        private void UpdateCoins( float fElapsedTime )
        {
            for( int i = 0; i < m_lstAwards.Count; ++i )
            {
                if( m_lstAwards[i].SlideComplete == true && m_lstAwards[i].CoinAnimation.IsRunning )
                {
                    m_lstAwards[i].CoinAnimation.Update( fElapsedTime );

                    //Check if the coin animation is complete
                    if( m_lstAwards[i].CoinAnimation.IsRunning == false )
                    {
                        if( m_lstAwards.Count - 1 == i )
                        {
                            //The game is totally over
                            m_state = WinScreenState.WS_GAME_OVER;
                        }
                        else
                        {
                            //Slide in the next award
                            m_state = WinScreenState.WS_AWARD_SLIDE;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// update the sparks
        /// </summary>
        /// <param name="fElapsedTime">the time since the last frame, in seconds</param>
        private void UpdateSparks( float fElapsedTime )
        {
            m_emitterLeft.Update( fElapsedTime );
            m_emitterRight.Update(fElapsedTime);
        }
        /// <summary>
        /// This function sorts out the awards and gives the appropriate one to the player
        /// </summary>
        /// <param name="thePlayer">The list of players</param>
        /// <param name="playerIndex">The index of the player within the list that is currently being checked</param>
        /// <param name="WinningPlayerID">The index of the winning player</param>
        /// <param name="totalPlayers">The total amount of players in the game</param>
        /// <param name="Awards">The array of awards awarded to the player</param>
        /// <returns>Returns the award the player has earned</returns>
        private string AwardText(List<Player> thePlayer, int playerIndex, int WinningPlayerID, int totalPlayers, string[] Awards)
        {
            string theAwardText = string.Empty;
            int[] theStats = new int[ thePlayer.Count ];
            float[] theFloatStats = new float[ thePlayer.Count ];
            
            if(!(checkIsTitleAwarded(Awards, "One Pirate Armada")))
            {
                // One Pirate Armada - Win the game with one ship
                if (WinningPlayerID == playerIndex)
                {
                    if (thePlayer[playerIndex].PlayerStats.returnTotalShips() == 1)
                    {
                        theAwardText = "One Pirate Armada"; 
                        return theAwardText;
                    }
                }
            }

            // You're Doing It Wrong - Sink yourself the most times
            if (!(checkIsTitleAwarded(Awards, "You're Doing It Wrong")))
            {
                for (int i = 0; i < totalPlayers; i++)
                {
                    theStats[i] = thePlayer[i].PlayerStats.returnSelfInflicted();
                }
                
                theAwardText += SortItems(theStats, true, playerIndex, "You're Doing It Wrong");
                if (theAwardText != string.Empty)
                { return theAwardText; }
                
            }

            // Dread Pirate - Sink the most ships
            if (!(checkIsTitleAwarded(Awards, "Dread Pirate")))
            {
                for (int i = 0; i < totalPlayers; i++)
                {
                    theStats[i] = thePlayer[i].PlayerStats.returnShipHits();
                }
                
                theAwardText += SortItems(theStats, true, playerIndex, "Dread Pirate");
                if (theAwardText != string.Empty)
                { return theAwardText; }
            }

            // Drunken Pirate - Accuracy below 10% -- lowest accuracy
            // Sharpshooter - Accuracy above 80% -- highest accuracy
            for (int i = 0; i < totalPlayers; i++)
            {
                theFloatStats[i] = thePlayer[i].PlayerStats.returnHitPercentage();
            }
            if (!(checkIsTitleAwarded(Awards, "Drunken Pirate")))
            {
                if (theFloatStats[playerIndex] < 0.1f && thePlayer[ playerIndex ].PlayerStats.returnShots() > 0)
                { 
                    theAwardText = "Drunken Pirate";
                    return theAwardText; 
                }
            }
            if(!(checkIsTitleAwarded(Awards, "Sharpshooter")))
            {
                if( theFloatStats[ playerIndex ] > 0.8f )
                { 
                    theAwardText = "Sharpshooter";
                    return theAwardText; 
                }
            }

            // Town Raider - Destroy the most towers
            if (!(checkIsTitleAwarded(Awards, "Town Raider")))
            {
                for (int i = 0; i < totalPlayers; i++)
                {
                    theStats[i] = thePlayer[i].PlayerStats.returnTowerHits();
                }
                theAwardText += SortItems(theStats, true, playerIndex, "Town Raider");
                if (theAwardText != string.Empty)
                { return theAwardText; }
            }

            // Cannon Fodder - Lose the most ships
            if (!(checkIsTitleAwarded(Awards, "Cannon Fodder")))
            {
                for (int i = 0; i < totalPlayers; i++)
                {
                    theStats[i] = thePlayer[i].PlayerStats.returnShipLosses();
                }

                theAwardText += SortItems(theStats, true, playerIndex, "Cannon Fodder");
                if (theAwardText != string.Empty)
                { return theAwardText; }
            }

            // Traveller - longest distance travelled
            if(!(checkIsTitleAwarded(Awards, "Traveller")))
            {
                for (int i = 0; i < totalPlayers; i++)
                {
                    theFloatStats[i] = thePlayer[i].PlayerStats.returnDistance();
                }
                theAwardText += SortItems(theStats, true, playerIndex, "Traveller");
                if (theAwardText != string.Empty)
                { return theAwardText; }
            }

            if (theAwardText == string.Empty)
            {
                theAwardText = "Landlubber";
            }
            return theAwardText;
        }

        /// <summary>
        /// Checks to see whether or not the item is awarded
        /// </summary>
        /// <param name="Awards">The array of awards awarded to the player</param>
        /// <param name="text">The text of the award</param>
        /// <returns>Whether or not the parameters match</returns>
        private bool checkIsTitleAwarded(string[] Awards, string text)
        {
            for (int i = 0; i < Awards.Length; i++)
            {
                if (Awards[i] == text)
                 return true;
            }
            return false;
        }

        /// <summary>
        /// Checks whether or not all of the items are the exact same - int version
        /// </summary>
        /// <param name="items">The list of items</param>
        /// <returns>Whether or not all the items are the same</returns>
        private bool isEverythingSame(int[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != items.Max())
                { return false; }
            }
            return true;
        }

        /// <summary>
        /// Checks whether or not all of the items are the exact same - float version
        /// </summary>
        /// <param name="items">The list of items</param>
        /// <returns>Whether or not all the items are the same</returns>
        private bool isEverythingSame(float[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != items.Max())
                { return false; }
            }
            return true;
        }

        /// <summary>
        /// Sorts all of the award condition variables - int version
        /// </summary>
        /// <param name="items">The array of items to be sorted</param>
        /// <param name="descending">True to sort descending, false to sort ascending</param>
        /// <param name="playerIndex">The index of the player being checked</param>
        /// <param name="title">The name of the award</param>
        /// <returns>The title if it's rewarded, or blank if it's not</returns>
        private string SortItems(int[] items, bool descending, int playerIndex, string title)
        {
            //Don't bother sorting it they're all the same
            if (isEverythingSame(items))
                return "";

            
            int temp;

            if(descending)
            {
                //Find the highest value
                temp = 0;

                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i] > temp)
                    {
                        temp = items[i];
                    }
                }
            }
            else
            {
                //Find the lowest value
                temp = 1000;

                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i] < temp)
                    {
                        temp = items[i];
                    }
                }
            }

            
            //if the highest/lowest is the player being considered, return the title
            if (items[playerIndex] == temp)
            {
                return title;
            }

            return "";
        }

        /// <summary>
        /// Sorts all of the award condition variables - float version
        /// </summary>
        /// <param name="items">The array of items to be sorted</param>
        /// <param name="descending">True to sort descending, false to sort ascending</param>
        /// <param name="playerIndex">The index of the player being checked</param>
        /// <param name="title">The name of the award</param>
        /// <returns>The title if it's rewarded, or blank if it's not</returns>
        private string SortItems(float[] items, bool descending, int playerIndex, string title)
        {
            if (isEverythingSame(items))
            {
                return "";
            }

            float temp;

            if (descending)
            {
                temp = 0;

                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i] > temp)
                    {
                        temp = items[i];
                    }
                }
            }
            else
            {
                temp = 100;

                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i] < temp)
                    {
                        temp = items[i];
                    }
                }
            }
           
            if (items[playerIndex] == temp)
            {
                return title;
            }

            return "";
        }
    }
}
