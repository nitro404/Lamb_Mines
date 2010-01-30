using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags
{
    public class Fireworks
    {
        int m_WinnerPlayerNum = 0;

        Particles.ParticleSystem m_Firework0_0;
       Particles.ParticleSystem m_Firework1_1;
       Particles.ParticleSystem m_Firework1_2;
       Particles.ParticleSystem m_Firework2_1;
       Particles.ParticleSystem m_Firework2_2;
       Particles.ParticleSystem m_Firework3_1;
       Particles.ParticleSystem m_Firework3_2;
       Particles.ParticleSystem m_Firework4_1;
       Particles.ParticleSystem m_Firework4_2;

       Vector3 m_Firework0_0Loc = new Vector3(0f, -5000f, 0f);
       Vector3 m_Firework1_1Loc = new Vector3(263.573f, 1.495f, 259.727f);
       Vector3 m_Firework1_2Loc = new Vector3(271.545f, 1.949f, 224.578f);
       Vector3 m_Firework2_1Loc = new Vector3(-275.595f, 3.038f, 235.0f);
       Vector3 m_Firework2_2Loc = new Vector3(-254.017f, 2.462f, 259.664f);        
       Vector3 m_Firework3_1Loc = new Vector3(-259.785f, 1.847f, -261.579f);
       Vector3 m_Firework3_2Loc = new Vector3(-277.205f, 2.267f, -235.785f);
       Vector3 m_Firework4_1Loc = new Vector3(279.498f, 2.674f, -230.824f);
       Vector3 m_Firework4_2Loc = new Vector3(258.912f, 1.881f, -260.459f);

        /// <summary>
        /// Prepares the sets of particles for each cove
        /// </summary>
        /// <param name="ParentModule"></param>
       public Fireworks(GameModule ParentModule)
       { 
            m_WinnerPlayerNum = -1; 
           
           m_Firework1_1 = new Particles.ParticleSystem(ParentModule.ParentApp, ParentModule.ParentApp.Content, "FireworkStars");
           m_Firework1_2 = new Particles.ParticleSystem(ParentModule.ParentApp, ParentModule.ParentApp.Content, "FireworkStars");
           m_Firework2_1 = new Particles.ParticleSystem(ParentModule.ParentApp, ParentModule.ParentApp.Content, "FireworkStars");
           m_Firework2_2 = new Particles.ParticleSystem(ParentModule.ParentApp, ParentModule.ParentApp.Content, "FireworkStars");
           m_Firework3_1 = new Particles.ParticleSystem(ParentModule.ParentApp, ParentModule.ParentApp.Content, "FireworkStars");
           m_Firework3_2 = new Particles.ParticleSystem(ParentModule.ParentApp, ParentModule.ParentApp.Content, "FireworkStars");
           m_Firework4_1 = new Particles.ParticleSystem(ParentModule.ParentApp, ParentModule.ParentApp.Content, "FireworkStars");
           m_Firework4_2 = new Particles.ParticleSystem(ParentModule.ParentApp, ParentModule.ParentApp.Content, "FireworkStars");
           m_Firework0_0 = new Particles.ParticleSystem(ParentModule.ParentApp, ParentModule.ParentApp.Content, "FireworkStars");

           m_Firework0_0.Location = m_Firework0_0Loc;
           m_Firework1_1.Location = m_Firework1_1Loc;
           m_Firework1_2.Location = m_Firework1_2Loc;
           m_Firework2_1.Location = m_Firework2_1Loc;
           m_Firework2_2.Location = m_Firework2_2Loc;
           m_Firework3_1.Location = m_Firework3_1Loc;
           m_Firework3_2.Location = m_Firework3_2Loc;
           m_Firework4_1.Location = m_Firework4_1Loc;
           m_Firework4_2.Location = m_Firework4_2Loc;
       }  

        /// <summary>
        /// Set the Winner player to set the fireworks to draw in that area.
        /// </summary>
        /// <param name="WinningPlayerNum"></param>
        public void BONZAI(int WinningPlayerNum)
        {
            m_WinnerPlayerNum = WinningPlayerNum;
        }

        /// <summary>
        /// Returns the cove's first Firework
        /// </summary>
        /// <returns></returns>
        public Particles.ParticleSystem getWinnerFireworks1()
        {
            switch (m_WinnerPlayerNum)
            {
                case 0:
                    return m_Firework1_1;
                case 1:
                    return m_Firework2_1;
                case 2:
                    return m_Firework3_1;
                case 3:
                    return m_Firework4_1;
            }
            return m_Firework0_0;
        }
            
        /// <summary>
        /// Returns the cove's second Firework
        /// </summary>
        /// <returns></returns>
        public Particles.ParticleSystem getWinnerFireworks2()
        {
            switch (m_WinnerPlayerNum)
            {
                case 0:
                    return m_Firework1_2;
                case 1:
                    return m_Firework2_2;
                case 2:
                    return m_Firework3_2;
                case 3:
                    return m_Firework4_2;
            }
            return m_Firework0_0;
        }     
    }
}
