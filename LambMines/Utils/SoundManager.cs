using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Scallywags
{
    public class SoundManager
    {
        #region DATA_MEMBERS

        const string SOUND_DIRECTORY = "..\\..\\..\\Sound\\";

        AudioEngine m_audioEngine;
        WaveBank    m_waveBank;
        SoundBank   m_soundBank;
        Cue         m_currentCuePlayer;
        
        float       m_fVolume;

        #endregion

        #region PROPERTIES

        public AudioEngine AudioEngine
        {
            get
            {
                return m_audioEngine;
            }
            set
            {
                m_audioEngine = value;
            }
        }

        public WaveBank WaveBank
        {
            get
            {
                return m_waveBank;
            }
            set
            {
                m_waveBank = value;
            }
        }

        public SoundBank SoundBank
        {
            get
            {
                return m_soundBank;
            }
            set
            {
                m_soundBank = value;
            }
        }

        public float Volume
        {
            set
            {
                m_fVolume = value;
            }
        }

       

        #endregion

        #region CLASS_CONSTRUCTER

        public SoundManager()
        {
            m_fVolume = 1.0f;
        }

        #endregion

        #region MODIFIRES

        public void Init()
        {
            m_audioEngine = new AudioEngine(SOUND_DIRECTORY + "GameSound.xgs");
            m_soundBank = new SoundBank( m_audioEngine, SOUND_DIRECTORY + "Sound Bank.xsb" );
            m_waveBank = new WaveBank( m_audioEngine, SOUND_DIRECTORY + "Wave Bank.xwb" );   
        }

        public void PlayCue( string strCueName )
        {
            if ( m_waveBank.IsPrepared && m_soundBank.IsInUse == false)
            {
                m_soundBank.PlayCue(strCueName);
                m_currentCuePlayer = m_soundBank.GetCue( strCueName );
            }
        }

        public void StopCue()
        {
            m_currentCuePlayer.Stop( AudioStopOptions.AsAuthored );
            this.Update();
        }

        public void Update()
        {
            m_audioEngine.Update();
        }

        public void CleanUp()
        {
            m_audioEngine.Dispose();
            m_soundBank.Dispose();
            m_waveBank.Dispose();
        }

        #endregion
    }
}
