using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags
{
    public class SparkEmitter
    {
        private Random      m_random;       //Random number generator
        private List<Spark> m_lstSparks;    //The sparks to fire from the
        private Vector2     m_vLocation;    //The emitter location
        private Vector2     m_vDirection;   //The emitter direction
        private Vector2     m_vVariance;    //The variance of the direction

        private float       m_fDecayRate;   //The rate of decay of the sparks
        private Vector2     m_vDecayDir;    //The direction at which to decay the spark velocity

        private float       m_fMaxSpeed;    //The max speed of the sparks
        private float       m_fMaxLife;     //The max life of the sparks

        private Texture2D   m_texSpark;     //The texture to use to draw the sparks
        private float       m_fTotalLife;   //The total duration of the emitter, -1 for infinite?
        private Color[]     m_vColors;      //The possible colours
        public SparkEmitter()
        {
            m_random = new Random();
        }

        /// <summary>
        /// Initialize the emitter parameters
        /// </summary>
        /// <param name="vLocation">the emitter origin</param>
        /// <param name="nNumSparks">the number of possible sparks</param>
        /// <param name="fMaxLife">the lifespan of the sparks</param>
        /// <param name="vEmitDir">the direction the sparks will emit</param>
        /// <param name="vEmitVariance">the variance of the emission direction</param>
        /// <param name="vDecayDir">the direction the sparks will accelerate towards</param>
        /// <param name="fMaxLaunchSpeed">the max speed at which the sparks will launch</param>
        /// <param name="fDecayRate">the negative acceleration rate of the sparks in the decay direction</param>
        /// <param name="fTotalLife">the total life of the emitter</param>
        /// <param name="tex">the texture to use for the sparks</param>
        /// <param name="colTints">the potential tints of the sparks</param>
        public void Init( Vector2 vLocation, 
            int nNumSparks, 
            float fMaxLife, 
            Vector2 vEmitDir, 
            Vector2 vEmitVariance, 
            Vector2 vDecayDir, 
            float fMaxLaunchSpeed, 
            float fDecayRate,
            float fTotalLife,
            Texture2D tex,
            Color[] colTints )
        {
            //Set up initial properties
            m_vLocation     = vLocation;
            m_fMaxLife      = fMaxLife;
            m_vDirection    = vEmitDir;
            m_vVariance     = vEmitVariance;
            m_vDecayDir     = vDecayDir;
            m_fMaxSpeed     = fMaxLaunchSpeed;
            m_fDecayRate    = fDecayRate;
            m_fTotalLife    = fTotalLife;
            m_texSpark      = tex;
            m_vColors       = colTints;

            m_vDirection.Normalize();

            m_lstSparks = new List<Spark>();

            //Pregenerate the sparks
            for( int i = 0; i < nNumSparks; ++i )
            {
                Spark newSpark = new Spark();

                newSpark.fLife = (float)(m_random.NextDouble()) * m_fMaxLife;
                m_lstSparks.Add(newSpark);
            }
        }

        /// <summary>
        /// Draw the sparks
        /// </summary>
        /// <param name="sb">the active sprite batch</param>
        public void Draw( SpriteBatch sb )
        {
            foreach (Spark spark in m_lstSparks)
            {
                if (spark.fLife > 0)
                {
                    sb.Draw(m_texSpark, spark.vPosition, spark.color);
                }
            }
        }

        /// <summary>
        /// Update the emitter
        /// </summary>
        /// <param name="fElapsedTime">the time elapsed since the last frame</param>
        public void Update( float fElapsedTime )
        {
            if( m_fTotalLife > 0 )
                m_fTotalLife -= fElapsedTime;

            //Let fly the sparks!
            foreach (Spark spark in m_lstSparks)
            {
                if (spark.fLife <= 0 && m_fTotalLife > 0)
                {
                    //Randomize the spark location
                    float fSparkY = m_vLocation.Y;//m_vTitlePosition.Y + m_vTitleDimensions.Y * 0.6f;
                    float fSparkX = m_vLocation.X;//m_vTitlePosition.X + m_vTitleDimensions.X * 0.5f;

                    //calculate the direction to fire the spark
                    Vector2 vDir = m_vDirection * (float)( m_random.NextDouble() ) * m_fMaxSpeed;
                    
                    //calculate the variance from the standard direction in both negative and positive directions
                    Vector2 vVariance   = m_vVariance;
                    float fVarLength    = vVariance.Length();
                    float fRandomLength = (float)( m_random.NextDouble() ) * fVarLength - fVarLength * 0.5f;
                    
                    vVariance.Normalize();
                    vVariance = vVariance * fRandomLength;

                    spark.vPosition = new Vector2(fSparkX, fSparkY);
                    spark.vVelocity = vDir + vVariance;
                    spark.fLife = (float)(m_random.NextDouble()) * m_fMaxLife;
                    spark.color = Color.White;

                    if( m_vColors != null )//.Length > 0 )
                    {
                        int nRand = m_random.Next( m_vColors.Length );
                        spark.color = m_vColors[ nRand ];
                    }
                }
                else
                {
                    spark.fLife     -= fElapsedTime;
                    spark.vVelocity += m_vDecayDir * m_fDecayRate * fElapsedTime;
                    spark.vPosition += spark.vVelocity * fElapsedTime;

                    spark.color.A = (byte)(spark.fLife / m_fMaxLife * 255);
                }
            }
        }
    }
}
