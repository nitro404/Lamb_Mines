using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags
{
    /** @class
     *  @brief  describes how to animate an animated texture
     *  @todo
     *          consider using an array of cell indices rather than a start and end cell...
     */
    public class Animation
    {
        private AnimatedTexture m_tex;                  ///< The texture to use
        private int             m_nStartCell;           ///< The first cell of the animation
        private int             m_nEndCell;             ///< The end cell of the animation
        private int             m_nCurrentCell;         ///< The active cell
                                               
        private int             m_nLoopCount;           ///< The number of times to loop, or -1 for infinite
        private bool            m_bIsRunning;           ///< is the anumation running?            
                            
        private float           m_fFrameTime;           ///< Time between frames in seconds
        private float           m_fFrameCountdown;      ///< Time elapsed on the current frame                            

        private int             m_nStopFrame;           ///< The frame to stop the animation on.

        
        public bool IsRunning
        {
            get
            {
                return m_bIsRunning;
            }
        }

        /// <summary>
        /// The current frame of the animation
        /// </summary>
        public int CurrentFrame
        {
            get
            {
                return m_nCurrentCell;
            }
            set
            {
                if( value >= 0 && value <= m_nEndCell )
                    m_nCurrentCell = value;
            }
        }

        /// <summary>
        /// the width of a cell of the animated texture
        /// </summary>
        public int CellWidth
        {
            get
            {
                return m_tex.CellWidth;
            }
        }

        /// <summary>
        /// the height of the cell of an animated texture
        /// </summary>
        public int CellHeight
        {
            get
            {
                return m_tex.CellHeight;
            }
        }

        /// <summary>
        /// The tint of the texture
        /// </summary>
        public Color Tint
        {
            set
            {
                m_tex.Tint = value;
            }
            get
            {
                return m_tex.Tint;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tex">the animated texture this animation will control</param>
        /// <param name="nStartCell"></param>
        /// <param name="nEndCell"></param>
        /// <param name="bLoop"></param>
        /// <param name="fFrameTime"></param>
        public Animation( AnimatedTexture tex, int nStartCell, int nEndCell, bool bLoop, float fFrameTime )
        {
            m_tex               = tex;
            m_nStartCell        = nStartCell;
            m_nEndCell          = nEndCell;

            if( bLoop )
                m_nLoopCount    = -1;
          
            m_bIsRunning        = false;
            m_fFrameTime        = fFrameTime;

            m_fFrameCountdown   = m_fFrameTime;
            m_nCurrentCell      = -1;
            m_nStopFrame        = -1;
        }

        /// <summary>
        /// Stop frame constructor
        /// </summary>
        /// <param name="tex">the animated texture</param>
        /// <param name="nStartCell">the cell to start the animation on</param>
        /// <param name="nEndCell">the cell to stop the animation on</param>
        /// <param name="fFrameTime">the time between frames</param>
        public Animation( AnimatedTexture tex, int nStartCell, int nStopCell, float fFrameTime )
        {
            m_tex               = tex;
            m_nStartCell        = nStartCell;
            m_nEndCell          = nStopCell;
            m_bIsRunning        = false;
            m_fFrameTime        = fFrameTime;
            m_fFrameCountdown   = m_fFrameTime;
            m_nCurrentCell      = -1;
            m_nStopFrame        = nStopCell;
        }

        /** @fn
         *  @brief  start the animation
         */
        public void Start()
        {
            m_nCurrentCell = m_nStartCell;
            m_bIsRunning = true;
        }

        /** @fn
         *  @brief  start the animation
         *  @param  nLoopCount [in] the number of times to play the animation
         */
        public void StartLoop( int nLoopCount )
        {
            m_nCurrentCell = m_nStartCell;
            m_bIsRunning = true;
            m_nLoopCount = nLoopCount;
        }

        /** @fn
         *  @brief  stop the animation
         */
        public void Stop()
        {
            //m_nCurrentCell = -1;
            m_bIsRunning = false;
        }

        /** @fn
         *  @brief  update the animation
         */
        public void Update(float fElapsedTime)
        {
            if( m_bIsRunning )
            {
                m_fFrameCountdown -= fElapsedTime;

                //Advance the frames as required
                if( m_fFrameCountdown <= 0 )
                {
                    ++m_nCurrentCell;

                    //Check if we've reached the stop frame
                    if( m_nStopFrame != -1 && m_nCurrentCell == m_nStopFrame )
                    {
                        Stop();
                    }
                    else if( m_nCurrentCell > m_nEndCell )
                    {
                        //check if the animation has ended, and if it should restart
                        m_nCurrentCell  = m_nStartCell;

                        //Update the loop count
                        switch( m_nLoopCount )
                        {
                            case 0:
                                m_bIsRunning = false;
                                break;
                            case 1:
                                m_nLoopCount = 0;
                                m_bIsRunning = false;
                                break;
                            case -1:
                                break;
                            default:
                                m_nLoopCount--;
                                m_bIsRunning = true;
                                break;
                        }                  
                    }

                    //reset the counter
                    m_fFrameCountdown = m_fFrameTime;
                }
            }
        }

        /** @fn
         *  @brief  draw the cell of the animation
         */
        public void Draw(Vector2 vScreenLoc, float fOrientation, SpriteBatch sb )
        {
            if( m_nCurrentCell != -1 )
                m_tex.Draw( m_nCurrentCell, vScreenLoc, fOrientation, sb );
        }
    }
}
