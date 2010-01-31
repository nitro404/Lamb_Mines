using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;


namespace LambMines
{
    /** @class  SoundManager
     *  @brief  the class to manage sounds
     */
    public class SoundManager
    {
        #region DATA_MEMBERS

#if WINDOWS
        const string SOUND_DIRECTORY = "Content/AudioFiles/Win/";   ///< The directory the sound files are located in
#endif
#if XBOX
        const string SOUND_DIRECTORY = "Content/AudioFiles/Xbox/";   ///< The directory the sound files are located in
#endif

        AudioEngine m_audioEngine;                              ///< The XACT audio engine
        WaveBank    m_waveBank;                                 ///< The XACT wave bank
        SoundBank   m_soundBank;                                ///< The XACT sound bank
        
        List<SoundCue> m_lstSoundsPlaying;                      ///< THe list of currently playing sound clips
        
        AudioCategory m_acMusic;                                ///< Controls sound cues in the music category
        AudioCategory m_acSounds;                               ///< control sound cues in the Sounds category

        float       m_fTotalVolume;                             ///< The total volume
        float       m_fMusicVolume;                             ///< The play volume
        float       m_fSoundVolume;                             ///< The volume of the sound
                                                                
        #endregion

        #region PROPERTIES

        /** @prop   float Volume
         *  @brief  the volume level
         */
        public float Volume
        {
            set
            {
                m_fTotalVolume = value;
            }
        }

        /** @prop   float SoundEffectVolume
         *  @brief  the volume of the sound effects
         */
        public float SoundEffectVolume
        {
            get
            {
                return m_fSoundVolume;
            }
            set
            {
                m_fSoundVolume = value;
            }
        }

        /** @prop   float MusicVolume
         *  @brief  the volume of the music
         */
        public float MusicVolume
        {
            get
            {
                return m_fMusicVolume;
            }
            set
            {
                m_fMusicVolume = value;
            }
        }

        #endregion

        #region CLASS_CONSTRUCTER

        /** @fn     SoundManager()
         *  @brief  constructor
         */
        public SoundManager()
        {
            m_fTotalVolume = 1.0f;
            m_fSoundVolume = 1.0f;
            m_fMusicVolume = 1.0f;

            m_lstSoundsPlaying = new List<SoundCue>();
        }

        #endregion

        #region MODIFIRES

        /** @fn     void Init()
         *  @brief  initialize the sounds
         */
        public void Init( ContentManager content )
        {
            //m_audioEngine = content.Load< AudioEngine >( SOUND_DIRECTORY + "GameSound" );
            m_audioEngine = new AudioEngine( SOUND_DIRECTORY + "GameSound.xgs" );
            m_soundBank = new SoundBank( m_audioEngine, SOUND_DIRECTORY + "Sound Bank.xsb" );
            m_waveBank = new WaveBank( m_audioEngine, SOUND_DIRECTORY + "Wave Bank.xwb" );   

            m_acMusic = m_audioEngine.GetCategory( "Music" );
            m_acSounds = m_audioEngine.GetCategory( "Sounds" );

            m_acMusic.SetVolume(m_fMusicVolume * m_fTotalVolume);
            m_acSounds.SetVolume(m_fSoundVolume * m_fTotalVolume);
        }

        /** @fn     void PlayCue( string strCueName )
         *  @brief  play a cue, one time
         *  @param  strCueName [in] the name of the cue to play
         */
        public void PlayCue( string strCueName )
        {
            if ( m_waveBank.IsPrepared )
            {
                try
                {
                    SoundCue tempCue = new SoundCue( m_soundBank.GetCue( strCueName ) );
                    tempCue.Play();

                    m_lstSoundsPlaying.Add( tempCue );
                }
                catch( Exception e )
                {
                    Error.Trace( "PlayCue Error: " + e.Message );
                }
            }
        }

        /** @fn     void PlayLoop( string strCueName, int nLoopCount )
         *  @brief  play a sound cue a specified number of times
         *  @param  strCueName [in] the name of the cue to play
         *  @param  nLoopCount [in] the number of times to play the cue, or -1 to play infinitely
         */
        public void PlayLoop( string strCueName, int nLoopCount )
        {
            if( m_waveBank.IsPrepared )
            {
                SoundCue tempCue = new SoundCue( m_soundBank.GetCue( strCueName ) );
                tempCue.LoopCount = nLoopCount - 1;
                tempCue.Play();

                m_lstSoundsPlaying.Add( tempCue );
            }
        }

        public bool CheckPlaying(string strCuename)
        {
            foreach (SoundCue sound in m_lstSoundsPlaying)
            {
                if (sound.CueName == strCuename)
                    return true;
            }
            return false;
        }

        /** @fn     void StopAll()
         *  @brief  stop all playing sounds
         */
        public void StopAll()
        {   
            for( int i = 0; i < m_lstSoundsPlaying.Count; )
            {
                m_lstSoundsPlaying[ i ].Stop();
                m_lstSoundsPlaying.RemoveAt( i );
            }
        }

        /** @fn     void StopCue( string strName )
         *  @brief  stop all playing occurences of a specific sound cue
         *  @param  strName [in] the name of the cue(s) to stop
         */
        public void StopCue( string strName )
        {
            for (int i = 0; i < m_lstSoundsPlaying.Count; ++i)
            {
                SoundCue cue = m_lstSoundsPlaying[i];
                if( cue.CueName.Equals( strName ) )
                {
                    cue.Stop();

                    m_lstSoundsPlaying.RemoveAt( i );
                    i--;
                }
            } 
        }

        /** @fn     void Update()
         *  @brief  update the sound manager - should be done once per frame
         */
        public void Update()
        {
            m_audioEngine.Update();

            ////////////////////////
            //Set the volumes
            m_acMusic.SetVolume( m_fMusicVolume * m_fTotalVolume );
            m_acSounds.SetVolume( m_fSoundVolume * m_fTotalVolume );

            //////////////////////
            //Replay looping sounds and delete finished sounds
            for (int i = 0; i < m_lstSoundsPlaying.Count; ++i)
            {
                SoundCue cue = m_lstSoundsPlaying[ i ];

                if( cue.IsPlaying == false )
                {
                    if( cue.LoopCount != 0 )
                    {
                        SoundCue newCue = new SoundCue( m_soundBank.GetCue( cue.CueName ) );
                        newCue.LoopCount = cue.LoopCount - 1;

                        newCue.Play();

                        m_lstSoundsPlaying.RemoveAt( i );
                        i--;
                        m_lstSoundsPlaying.Add( newCue );
                    }
                    else //loop count IS 0
                    {
                        m_lstSoundsPlaying.RemoveAt(i);
                        --i;  
                    }
                }
            }
        }

        /** @fn     void CleanUp()
         *  @brief  clean up the XACT objects
         */
        public void CleanUp()
        {
            m_audioEngine.Dispose();
            m_soundBank.Dispose();
            m_waveBank.Dispose();
        }

        #endregion
    }
}
