using Microsoft.Xna.Framework.Audio;

namespace Scallywags
{
    /** @class  SoundCue
     *  @brief  a wrapper class to help managing sound cues
     */
    public class SoundCue
    {
        private Cue m_cue;              ///< The XACT sound cue
        private int m_nLoopCount;       ///< The number of times this cue is to be looped

        /** @fn     SoundCue( Cue cue )
         *  @brief  constructor
         *  @param  cue [in] the cue this instance will represent
         */
        public SoundCue(Cue cue)
        {
            m_cue        = cue;
            m_nLoopCount = 0;
        }

        #region PROPERTIES

        /** @prop   CueName
         *  @brief  the name of the cue in the XACT audio project
         */
        public string CueName
        {
            get
            {
                return m_cue.Name;
            }
        }

        /** @prop   LoopCount
         *  @brief  the number of times this cue is supposed to loop
         */
        public int LoopCount
        {
            get
            {
                return m_nLoopCount;
            }
            set
            {
                if( value < 0 )
                    m_nLoopCount = -1;
                else
                    m_nLoopCount = value;
            }
        }

        /** @prop   IsPlaying
         *  @brief  is this cue currently playing?
         */
        public bool IsPlaying
        {
            get
            {
                return m_cue.IsPlaying;
            }
        }

        #endregion

        #region INTERFACE

        /** @fn     void Play()
         *  @brief  play the cue
         */
        public void Play()
        {
            m_cue.Play();
        }

        /** @fn     void Stop()
         *  @brief  stop the cue
         */
        public void Stop()
        {
            m_cue.Stop(AudioStopOptions.Immediate);
        }

        #endregion

    }
}
