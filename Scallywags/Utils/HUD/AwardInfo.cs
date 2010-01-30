using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags
{
    /// <summary>
    /// class to describe a line of the scoreboard
    /// </summary>
    public class AwardInfo
    {
        private string      m_strInfoText;      //The text of the info
        private Texture2D   m_texPortrait;      //The character portrait
        private bool        m_bSlideComplete;   //Has the entrance animation completed
        private Vector2     m_vPosition;        //the top left corner of the drawing area

        private int         m_nVisibleGold;     //The gold that is showing
        private int         m_nTotalGold;       //The total gold the player has

        private Animation   m_coinAnim;         //The coin animation

        private float       m_fPlaceScale;      //The scale of the place text

        /// <summary>
        /// The text of the award
        /// </summary>
        public string Text
        {
            set
            {
                m_strInfoText = value;
            }
            get
            {
                return m_strInfoText;
            }
        }

        /// <summary>
        /// The portrait
        /// </summary>
        public Texture2D Portrait
        {
            set
            {
                m_texPortrait = value;
            }
            get
            {
                return m_texPortrait;
            }
        }

        /// <summary>
        /// Is the animation complete?
        /// </summary>
        public bool SlideComplete
        {
            set
            {
                m_bSlideComplete = true;
            }
            get
            {
                return m_bSlideComplete;
            }
        }

        /// <summary>
        /// the x position of the award
        /// </summary>
        public float X
        {
            get
            {
                return m_vPosition.X;
            }
            set
            {
                m_vPosition.X = value;
            }
        }

        /// <summary>
        /// the Y position of the award
        /// </summary>
        public float Y
        {
            get
            {
                return m_vPosition.Y;
            }
            set
            {
                m_vPosition.Y = value;
            }
        }

        /// <summary>
        /// The screen position of the award
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return m_vPosition;
            }
            set
            {
                m_vPosition = value;
            }
        }

        /// <summary>
        /// the total gold the player accumulated
        /// </summary>
        public int TotalGold
        {
            get
            {
                return m_nTotalGold;
            }
        }

        /// <summary>
        /// The gold that has been displayed
        /// </summary>
        public int VisibleGold
        {
            get
            {
                return m_nVisibleGold;
            }
            set
            {
                m_nVisibleGold = value;
            }
        }

        /// <summary>
        /// The coin award's coin animation
        /// </summary>
        public Animation CoinAnimation
        {
            get
            {
                return m_coinAnim;
            }
        }

        /// <summary>
        /// the scale of the place value (1st, 2nd...) text
        /// </summary>
        public float PlaceScale
        {
            get
            {
                return m_fPlaceScale;
            }
            set
            {
                m_fPlaceScale = value;
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        public AwardInfo(AnimatedTexture coinTex, int nCoinAmount )
        {
            m_strInfoText       = "";
            m_texPortrait       = null;
            m_bSlideComplete    = false;
            m_vPosition         = Vector2.Zero;
            m_nVisibleGold      = 0;
            m_nTotalGold        = nCoinAmount;
            m_coinAnim          = new Animation( coinTex, 0, m_nTotalGold, 0.2f );

            m_fPlaceScale       = 0.0f;
        }
    }
}