using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Scallywags
{
    class Clouds
    {

        Vector3 Test1 = new Vector3(4.763f, 30.22f, 146.203f);

        Vector3 Test2 = new Vector3(1079.63f, 91.183f, 59.568f);
        Vector3 Test3 = new Vector3(1148.859f, 180.782f, 241.055f);
        Vector3 Test4 = new Vector3(1079.63f, 248.593f, 59.568f);///1338.258f, 248.593f,31.037f 
        Vector3 Test5 = new Vector3(1338.258f, 409.216f, 31.037f);
        //Paths for clouds to loop through
        Vector3 Path1_01 = new Vector3(-1488.157f, CLOUDHEIGHT, 2469.581f);
        Vector3 Path1_02 = new Vector3(-1488.157f, CLOUDHEIGHT, -1734.539f);

        Vector3 Path2_01 = new Vector3(-1191.551f, CLOUDHEIGHT, 2837.05f);
        Vector3 Path2_02 = new Vector3(-1191.551f, CLOUDHEIGHT, -2156.327f);

        Vector3 Path3_01 = new Vector3(-850.572f, CLOUDHEIGHT, 3009.341f);
        Vector3 Path3_02 = new Vector3(-850.572f, CLOUDHEIGHT, -2430.551f);

        Vector3 Path4_01 = new Vector3(818.082f, CLOUDHEIGHT, 2424.892f);
        Vector3 Path4_02 = new Vector3(818.082f, CLOUDHEIGHT, -2317.446f);

        Vector3 Path5_01 = new Vector3(1141.629f, CLOUDHEIGHT, 2160.885f);
        Vector3 Path5_02 = new Vector3(1141.629f, CLOUDHEIGHT, -2035.634f);

        Vector3 Path6_01 = new Vector3(1482.608f, CLOUDHEIGHT, 1667.309f);
        Vector3 Path6_02 = new Vector3(1482.608f, CLOUDHEIGHT, -1657.593f);

        const float CLOUDHEIGHT = 251.345f - 20.0f;
        const float PATH1SPEED = -8.0f;
        const float PATH2SPEED = -12.0f;
        const float PATH3SPEED = -9.0f;
        const float PATH4SPEED = -11.0f;
        const float PATH5SPEED = -7.0f;
        const float PATH6SPEED = -10.0f;

        const float CLOUD1PERCENT = 1.1f;
        const float CLOUD2PERCENT = 0.9f;
        const float CLOUD3PERCENT = 0.8f;
        const float CLOUD4PERCENT = 0.75f;
        const float CLOUD5PERCENT = 0.85f;

        Particles.ParticleSystem m_Cloud01; // = new Particles.ParticleSystem(m_ParentModule.ParentApp, m_ParentModule.ParentApp.Content, "Cloud");
        Particles.ParticleSystem m_Cloud02; // = new Particles.ParticleSystem(m_ParentModule.ParentApp, m_ParentModule.ParentApp.Content, "Cloud");
        Particles.ParticleSystem m_Cloud03; // = new Particles.ParticleSystem(m_ParentModule.ParentApp, m_ParentModule.ParentApp.Content, "Cloud");
        Particles.ParticleSystem m_Cloud04; // = new Particles.ParticleSystem(m_ParentModule.ParentApp, m_ParentModule.ParentApp.Content, "Cloud");
        Particles.ParticleSystem m_Cloud05; // = new Particles.ParticleSystem(m_ParentModule.ParentApp, m_ParentModule.ParentApp.Content, "Cloud");

        public Clouds()
        {

        }

        public void Init(GameModule ParentModule)
        {
            m_Cloud01 = new Particles.ParticleSystem(ParentModule.ParentApp, ParentModule.ParentApp.Content, "Cloud");
            m_Cloud02 = new Particles.ParticleSystem(ParentModule.ParentApp, ParentModule.ParentApp.Content, "Cloud");
            m_Cloud03 = new Particles.ParticleSystem(ParentModule.ParentApp, ParentModule.ParentApp.Content, "Cloud");
            m_Cloud04 = new Particles.ParticleSystem(ParentModule.ParentApp, ParentModule.ParentApp.Content, "Cloud");
            m_Cloud05 = new Particles.ParticleSystem(ParentModule.ParentApp, ParentModule.ParentApp.Content, "Cloud");

            float Cloud1StartZ = -2710.0f;
            Path1_01.Z += Cloud1StartZ;
            float Cloud2StartZ = -2000.0f;
            Path2_01.Z += Cloud2StartZ;
            float Cloud3StartZ = -3900.0f;
            Path3_01.Z += Cloud3StartZ;
            float Cloud4StartZ = -3100.0f;
            Path4_01.Z += Cloud4StartZ;
            float Cloud5StartZ = -1300.0f;
            Path5_01.Z += Cloud5StartZ;

            m_Cloud01.Location = Path1_01;
            m_Cloud02.Location = Path2_01;
            m_Cloud03.Location = Path3_01;
            m_Cloud04.Location = Path4_01;
            m_Cloud05.Location = Path5_01;

        }


        //GenerateClouds(listParticles, fElapsedTime);

        /** @fn void GenerateClouds()
        *  @brief Checks and updates clouds positions to continue to loop
        */
        public void Update(float fElapsedTime)
        {

            //In Between Pt1 and Pt2 on Path1
            //Moves into Negative X direction
            if (m_Cloud01.Location.Z <= Path1_01.Z && m_Cloud01.Location.Z > Path1_02.Z && m_Cloud01.Location.X == Path1_01.X)
            {
                Vector3 path1 = new Vector3(0.0f, 0.0f, PATH1SPEED * CLOUD1PERCENT * fElapsedTime);
                m_Cloud01.Location += path1;
            }
            if (m_Cloud02.Location.Z <= Path1_01.Z && m_Cloud02.Location.Z > Path1_02.Z && m_Cloud02.Location.X == Path1_01.X)
            {
                Vector3 path1 = new Vector3(0.0f, 0.0f, PATH1SPEED * CLOUD2PERCENT * fElapsedTime);
                m_Cloud02.Location += path1;
            }
            if (m_Cloud03.Location.Z <= Path1_01.Z && m_Cloud03.Location.Z > Path1_02.Z && m_Cloud03.Location.X == Path1_01.X)
            {
                Vector3 path1 = new Vector3(0.0f, 0.0f, PATH1SPEED * CLOUD3PERCENT * fElapsedTime);
                m_Cloud03.Location += path1;
            }
            if (m_Cloud04.Location.Z <= Path1_01.Z && m_Cloud04.Location.Z > Path1_02.Z && m_Cloud04.Location.X == Path1_01.X)
            {
                Vector3 path1 = new Vector3(0.0f, 0.0f, PATH1SPEED * CLOUD4PERCENT * fElapsedTime);
                m_Cloud04.Location += path1;
            }
            if (m_Cloud05.Location.Z <= Path1_01.Z && m_Cloud05.Location.Z > Path1_02.Z && m_Cloud05.Location.X == Path1_01.X)
            {
                Vector3 path1 = new Vector3(0.0f, 0.0f, PATH1SPEED * CLOUD5PERCENT * fElapsedTime);
                m_Cloud05.Location += path1;
            }
            /////////////////////////////////
            //In between Pt1 and Pt2 of Path2
            if (m_Cloud01.Location.Z <= Path2_01.Z && m_Cloud01.Location.Z > Path2_02.Z && m_Cloud01.Location.X == Path2_01.X)
            {
                Vector3 path2 = new Vector3(0.0f, 0.0f, PATH2SPEED * CLOUD1PERCENT * fElapsedTime);
                m_Cloud01.Location += path2;
            }
            if (m_Cloud02.Location.Z <= Path2_01.Z && m_Cloud02.Location.Z > Path2_02.Z && m_Cloud02.Location.X == Path2_01.X)
            {
                Vector3 path2 = new Vector3(0.0f, 0.0f, PATH2SPEED * CLOUD2PERCENT * fElapsedTime);
                m_Cloud02.Location += path2;
            }
            if (m_Cloud03.Location.Z <= Path2_01.Z && m_Cloud03.Location.Z > Path2_02.Z && m_Cloud03.Location.X == Path2_01.X)
            {
                Vector3 path2 = new Vector3(0.0f, 0.0f, PATH2SPEED * CLOUD3PERCENT * fElapsedTime);
                m_Cloud03.Location += path2;
            }
            if (m_Cloud04.Location.Z <= Path2_01.Z && m_Cloud04.Location.Z > Path2_02.Z && m_Cloud04.Location.X == Path2_01.X)
            {
                Vector3 path2 = new Vector3(0.0f, 0.0f, PATH2SPEED * CLOUD4PERCENT * fElapsedTime);
                m_Cloud04.Location += path2;
            }
            if (m_Cloud05.Location.Z <= Path2_01.Z && m_Cloud05.Location.Z > Path2_02.Z && m_Cloud05.Location.X == Path2_01.X)
            {
                Vector3 path2 = new Vector3(0.0f, 0.0f, PATH2SPEED * CLOUD5PERCENT * fElapsedTime);
                m_Cloud05.Location += path2;
            }
            /////////////////////////////////
            //In between Pt1 and Pt2 of Path3
            if (m_Cloud01.Location.Z <= Path3_01.Z && m_Cloud01.Location.Z > Path3_02.Z && m_Cloud01.Location.X == Path3_01.X)
            {
                Vector3 path3 = new Vector3(0.0f, 0.0f, PATH3SPEED * CLOUD1PERCENT * fElapsedTime);
                m_Cloud01.Location += path3;
            }
            if (m_Cloud02.Location.Z <= Path3_01.Z && m_Cloud02.Location.Z > Path3_02.Z && m_Cloud02.Location.X == Path3_01.X)
            {
                Vector3 path3 = new Vector3(0.0f, 0.0f, PATH3SPEED * CLOUD2PERCENT * fElapsedTime);
                m_Cloud02.Location += path3;
            }
            if (m_Cloud03.Location.Z <= Path3_01.Z && m_Cloud03.Location.Z > Path3_02.Z && m_Cloud03.Location.X == Path3_01.X)
            {
                Vector3 path3 = new Vector3(0.0f, 0.0f, PATH3SPEED * CLOUD3PERCENT * fElapsedTime);
                m_Cloud03.Location += path3;
            }
            if (m_Cloud04.Location.Z <= Path3_01.Z && m_Cloud04.Location.Z > Path3_02.Z && m_Cloud04.Location.X == Path3_01.X)
            {
                Vector3 path3 = new Vector3(0.0f, 0.0f, PATH3SPEED * CLOUD4PERCENT * fElapsedTime);
                m_Cloud04.Location += path3;
            }
            if (m_Cloud05.Location.Z <= Path3_01.Z && m_Cloud05.Location.Z > Path3_02.Z && m_Cloud05.Location.X == Path3_01.X)
            {
                Vector3 path3 = new Vector3(0.0f, 0.0f, PATH3SPEED * CLOUD5PERCENT * fElapsedTime);
                m_Cloud05.Location += path3;
            }
            /////////////////////////////////
            //In between Pt1 and Pt2 of Path4
            if (m_Cloud01.Location.Z <= Path4_01.Z && m_Cloud01.Location.Z > Path4_02.Z && m_Cloud01.Location.X == Path4_01.X)
            {
                Vector3 path4 = new Vector3(0.0f, 0.0f, PATH4SPEED * CLOUD1PERCENT * fElapsedTime);
                m_Cloud01.Location += path4;
            }
            if (m_Cloud02.Location.Z <= Path4_01.Z && m_Cloud02.Location.Z > Path4_02.Z && m_Cloud02.Location.X == Path4_01.X)
            {
                Vector3 path4 = new Vector3(0.0f, 0.0f, PATH4SPEED * CLOUD2PERCENT * fElapsedTime);
                m_Cloud02.Location += path4;
            }
            if (m_Cloud03.Location.Z <= Path4_01.Z && m_Cloud03.Location.Z > Path4_02.Z && m_Cloud03.Location.X == Path4_01.X)
            {
                Vector3 path4 = new Vector3(0.0f, 0.0f, PATH4SPEED * CLOUD3PERCENT * fElapsedTime);
                m_Cloud03.Location += path4;
            }
            if (m_Cloud04.Location.Z <= Path4_01.Z && m_Cloud04.Location.Z > Path4_02.Z && m_Cloud04.Location.X == Path4_01.X)
            {
                Vector3 path4 = new Vector3(0.0f, 0.0f, PATH4SPEED * CLOUD4PERCENT * fElapsedTime);
                m_Cloud04.Location += path4;
            }
            if (m_Cloud05.Location.Z <= Path4_01.Z && m_Cloud05.Location.Z > Path4_02.Z && m_Cloud05.Location.X == Path4_01.X)
            {
                Vector3 path4 = new Vector3(0.0f, 0.0f, PATH4SPEED * CLOUD5PERCENT * fElapsedTime);
                m_Cloud05.Location += path4;
            }
            /////////////////////////////////
            //In between Pt1 and Pt2 of Path5
            if (m_Cloud01.Location.Z <= Path5_01.Z && m_Cloud01.Location.Z > Path5_02.Z && m_Cloud01.Location.X == Path5_01.X)
            {
                Vector3 path5 = new Vector3(0.0f, 0.0f, PATH5SPEED * CLOUD1PERCENT * fElapsedTime);
                m_Cloud01.Location += path5;
            }
            if (m_Cloud02.Location.Z <= Path5_01.Z && m_Cloud02.Location.Z > Path5_02.Z && m_Cloud02.Location.X == Path5_01.X)
            {
                Vector3 path5 = new Vector3(0.0f, 0.0f, PATH5SPEED * CLOUD2PERCENT * fElapsedTime);
                m_Cloud02.Location += path5;
            }
            if (m_Cloud03.Location.Z <= Path5_01.Z && m_Cloud03.Location.Z > Path5_02.Z && m_Cloud03.Location.X == Path5_01.X)
            {
                Vector3 path5 = new Vector3(0.0f, 0.0f, PATH5SPEED * CLOUD3PERCENT * fElapsedTime);
                m_Cloud03.Location += path5;
            }
            if (m_Cloud04.Location.Z <= Path5_01.Z && m_Cloud04.Location.Z > Path5_02.Z && m_Cloud04.Location.X == Path5_01.X)
            {
                Vector3 path5 = new Vector3(0.0f, 0.0f, PATH5SPEED * CLOUD4PERCENT * fElapsedTime);
                m_Cloud04.Location += path5;
            }
            if (m_Cloud05.Location.Z <= Path5_01.Z && m_Cloud05.Location.Z > Path5_02.Z && m_Cloud05.Location.X == Path5_01.X)
            {
                Vector3 path5 = new Vector3(0.0f, 0.0f, PATH5SPEED * CLOUD5PERCENT * fElapsedTime);
                m_Cloud05.Location += path5;
            }
            /////////////////////////////////
            //In between Pt1 and Pt2 of Path6
            if (m_Cloud01.Location.Z <= Path6_01.Z && m_Cloud01.Location.Z > Path6_02.Z && m_Cloud01.Location.X == Path6_01.X)
            {
                Vector3 path6 = new Vector3(0.0f, 0.0f, PATH6SPEED * CLOUD1PERCENT * fElapsedTime);
                m_Cloud01.Location += path6;
            }
            if (m_Cloud02.Location.Z <= Path6_01.Z && m_Cloud02.Location.Z > Path6_02.Z && m_Cloud02.Location.X == Path6_01.X)
            {
                Vector3 path6 = new Vector3(0.0f, 0.0f, PATH6SPEED * CLOUD2PERCENT * fElapsedTime);
                m_Cloud02.Location += path6;
            }
            if (m_Cloud03.Location.Z <= Path6_01.Z && m_Cloud03.Location.Z > Path6_02.Z && m_Cloud03.Location.X == Path6_01.X)
            {
                Vector3 path6 = new Vector3(0.0f, 0.0f, PATH6SPEED * CLOUD3PERCENT * fElapsedTime);
                m_Cloud03.Location += path6;
            }
            if (m_Cloud04.Location.Z <= Path6_01.Z && m_Cloud04.Location.Z > Path6_02.Z && m_Cloud04.Location.X == Path6_01.X)
            {
                Vector3 path6 = new Vector3(0.0f, 0.0f, PATH6SPEED * CLOUD4PERCENT * fElapsedTime);
                m_Cloud04.Location += path6;
            }
            if (m_Cloud05.Location.Z <= Path6_01.Z && m_Cloud05.Location.Z > Path6_02.Z && m_Cloud05.Location.X == Path6_01.X)
            {
                Vector3 path6 = new Vector3(0.0f, 0.0f, PATH6SPEED * CLOUD5PERCENT * fElapsedTime);
                m_Cloud05.Location += path6;
            }
            ////////////////////////////////////////////////////
            //Path1 -> Path4 -> Path2 -> Path5 -> Path3 -> Path6 -> Path1
            //Path1 -> Path4
            if (m_Cloud01.Location.Z <= Path1_02.Z && m_Cloud01.Location.X == Path1_01.X)
            {
                m_Cloud01.Location = Path4_01;
            }
            if (m_Cloud02.Location.Z <= Path1_02.Z && m_Cloud02.Location.X == Path1_01.X)
            {
                m_Cloud02.Location = Path4_01;
            }
            if (m_Cloud03.Location.Z <= Path1_02.Z && m_Cloud03.Location.X == Path1_01.X)
            {
                m_Cloud03.Location = Path4_01;
            }
            if (m_Cloud04.Location.Z <= Path1_02.Z && m_Cloud04.Location.X == Path1_01.X)
            {
                m_Cloud04.Location = Path4_01;
            }
            if (m_Cloud05.Location.Z <= Path1_02.Z && m_Cloud05.Location.X == Path1_01.X)
            {
                m_Cloud05.Location = Path4_01;
            }
            /////////////////////////////////
            //Path1 -> Path4 -> Path2 -> Path5 -> Path3 -> Path6 -> Path1
            //Path2 -> Path5
            if (m_Cloud01.Location.Z <= Path2_02.Z && m_Cloud01.Location.X == Path2_01.X)
            {
                m_Cloud01.Location = Path5_01;
            }
            if (m_Cloud02.Location.Z <= Path2_02.Z && m_Cloud02.Location.X == Path2_01.X)
            {
                m_Cloud02.Location = Path5_01;
            }
            if (m_Cloud03.Location.Z <= Path2_02.Z && m_Cloud03.Location.X == Path2_01.X)
            {
                m_Cloud03.Location = Path5_01;
            }
            if (m_Cloud04.Location.Z <= Path2_02.Z && m_Cloud04.Location.X == Path2_01.X)
            {
                m_Cloud04.Location = Path5_01;
            }
            if (m_Cloud05.Location.Z <= Path2_02.Z && m_Cloud05.Location.X == Path2_01.X)
            {
                m_Cloud05.Location = Path5_01;
            }
            /////////////////////////////////
            //Path1 -> Path4 -> Path2 -> Path5 -> Path3 -> Path6 -> Path1
            //Path3 -> Path6
            if (m_Cloud01.Location.Z <= Path3_02.Z && m_Cloud01.Location.X == Path3_01.X)
            {
                m_Cloud01.Location = Path6_01;
            }
            if (m_Cloud02.Location.Z <= Path3_02.Z && m_Cloud02.Location.X == Path3_01.X)
            {
                m_Cloud02.Location = Path6_01;
            }
            if (m_Cloud03.Location.Z <= Path3_02.Z && m_Cloud03.Location.X == Path3_01.X)
            {
                m_Cloud03.Location = Path6_01;
            }
            if (m_Cloud04.Location.Z <= Path3_02.Z && m_Cloud04.Location.X == Path3_01.X)
            {
                m_Cloud04.Location = Path6_01;
            }
            if (m_Cloud05.Location.Z <= Path3_02.Z && m_Cloud05.Location.X == Path3_01.X)
            {
                m_Cloud05.Location = Path6_01;
            }
            /////////////////////////////////
            //Path1 -> Path4 -> Path2 -> Path5 -> Path3 -> Path6 -> Path1
            //Path4 -> Path2
            if (m_Cloud01.Location.Z <= Path4_02.Z && m_Cloud01.Location.X == Path4_01.X)
            {
                m_Cloud01.Location = Path2_01;
            }
            if (m_Cloud02.Location.Z <= Path4_02.Z && m_Cloud02.Location.X == Path4_01.X)
            {
                m_Cloud02.Location = Path2_01;
            }
            if (m_Cloud03.Location.Z <= Path4_02.Z && m_Cloud03.Location.X == Path4_01.X)
            {
                m_Cloud03.Location = Path2_01;
            }
            if (m_Cloud04.Location.Z <= Path4_02.Z && m_Cloud04.Location.X == Path4_01.X)
            {
                m_Cloud04.Location = Path2_01;
            }
            if (m_Cloud05.Location.Z <= Path4_02.Z && m_Cloud05.Location.X == Path4_01.X)
            {
                m_Cloud05.Location = Path2_01;
            }
            /////////////////////////////////
            //Path1 -> Path4 -> Path2 -> Path5 -> Path3 -> Path6 -> Path1
            //Path5 -> Path3
            if (m_Cloud01.Location.Z <= Path5_02.Z && m_Cloud01.Location.X == Path5_01.X)
            {
                m_Cloud01.Location = Path3_01;
            }
            if (m_Cloud02.Location.Z <= Path5_02.Z && m_Cloud02.Location.X == Path5_01.X)
            {
                m_Cloud02.Location = Path3_01;
            }
            if (m_Cloud03.Location.Z <= Path5_02.Z && m_Cloud03.Location.X == Path5_01.X)
            {
                m_Cloud03.Location = Path3_01;
            }
            if (m_Cloud04.Location.Z <= Path5_02.Z && m_Cloud04.Location.X == Path5_01.X)
            {
                m_Cloud04.Location = Path3_01;
            }
            if (m_Cloud05.Location.Z <= Path5_02.Z && m_Cloud05.Location.X == Path5_01.X)
            {
                m_Cloud05.Location = Path3_01;
            }
            /////////////////////////////////
            //Path1 -> Path4 -> Path2 -> Path5 -> Path3 -> Path6 -> Path1
            //Path6 -> Path1
            if (m_Cloud01.Location.Z <= Path6_02.Z && m_Cloud01.Location.X == Path6_01.X)
            {
                m_Cloud01.Location = Path1_01;
            }
            if (m_Cloud02.Location.Z <= Path6_02.Z && m_Cloud02.Location.X == Path6_01.X)
            {
                m_Cloud02.Location = Path1_01;
            }
            if (m_Cloud03.Location.Z <= Path6_02.Z && m_Cloud03.Location.X == Path6_01.X)
            {
                m_Cloud03.Location = Path1_01;
            }
            if (m_Cloud04.Location.Z <= Path6_02.Z && m_Cloud04.Location.X == Path6_01.X)
            {
                m_Cloud04.Location = Path1_01;
            }
            if (m_Cloud05.Location.Z <= Path6_02.Z && m_Cloud05.Location.X == Path6_01.X)
            {
                m_Cloud05.Location = Path1_01;
            }

            m_Cloud01.UpdateEffect(Vector3.Zero, fElapsedTime);
            m_Cloud02.UpdateEffect(Vector3.Zero, fElapsedTime);
            m_Cloud03.UpdateEffect(Vector3.Zero, fElapsedTime);
            m_Cloud04.UpdateEffect(Vector3.Zero, fElapsedTime);
            m_Cloud05.UpdateEffect(Vector3.Zero, fElapsedTime);

        }

        public void Draw(Matrix View, Matrix Projection, GameTime gameTime)
        {
            m_Cloud01.Draw(View, Projection, gameTime);
            m_Cloud02.Draw(View, Projection, gameTime);
            m_Cloud03.Draw(View, Projection, gameTime);
            m_Cloud04.Draw(View, Projection, gameTime);
            m_Cloud05.Draw(View, Projection, gameTime);
        }

    }
}
