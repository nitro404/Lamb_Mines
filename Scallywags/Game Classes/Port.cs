using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags
{
    /** @class  Port
     *  @brief  
     */
    public class Port : Object3D
    {
        int portCoins;// = 10;
        int m_nOwnerID;
        float m_fPickupRadius;

        /** @prop  Coins
        *  @brief  The amount of coins in the Port
        */
        public int Coins
        {
            get
            {
                return portCoins;
            }
        }

        public float PickupRadius
        {
            get
            {
                return m_fPickupRadius;
            }
            set
            {
                m_fPickupRadius = value;
            }
        }

        /** @prop   Owner
         *  @brief  the ID of the player that owns the port, or -1 if the port doesn't belong to anyone
         */
        public int Owner
        {
            get
            {
                return m_nOwnerID;
            }
            set
            {
                m_nOwnerID = value;
            }
        }

        /** @fn     Port( Model mdl )
         *  @brief  Constructor
         *  @param  mdl [in] the model to use to display the port
         */
        public Port( Model mdl ) : base( mdl )
        {
            portCoins   = 0;
            Location    = new Vector3(0, 0, 0);
            m_nOwnerID  = -1;
            m_fPickupRadius = 5.0f;
        }

        /** @fn     Port()
         *  @brief  Constructor
         */
        public Port()
            : base()
        {
            portCoins   = 0;
            Location    = new Vector3(0, 0, 0);
            m_nOwnerID  = -1;
            m_fPickupRadius = 5.0f;
        }

        /** @fn     incrementCoins()
         *  @brief  Increments the number of coins that the town has by 1
         */
        public void incrementCoins()
        {
            portCoins++;
        }

        /** @fn     incrementCoins()
         *  @brief  Increments the number of coins that the town has by a set amount
         */
        public void incrementCoins(int coins)
        {
            portCoins = portCoins + coins;
        }

        /** @fn     decrementCoins()
         *  @brief  Decrements the number of coins that the town has by 1
         */
        public void decrementCoins()
        {
            if (portCoins > 0)
            {
                portCoins--;
            }
        }
        /** @fn     decrementCoins()
         *  @brief  Decrements the number of coins that the town has by a set amount
         */
        public void decrementCoins(int coins)
        {
            if (portCoins - coins > -1)
            {
                portCoins = portCoins - coins;
            }
        }

        /** @fn     initCoins()
         *  @brief  Sets the starting amount of coins in the environment to 10
         */
        public void initCoins()
        {
            portCoins = Settings.MAX_PORT_COINS;
        }

        /** @fn     initCoins()
         *  @brief  Sets the starting amount of coins in the environment to a coder-specified amount
         */
        public void initCoins(int coins)
        {
            portCoins = coins;
        }
        /** @fn     hasCoins()
        *  @brief  Checks to see if the Port has coins
        */
        public bool hasCoins()
        {
            if (portCoins < 1)
            {
                return false;
            }
            return true;
        }

        /** @fn     checkPort(Ship)
        *  @brief  Checks to see if there is a ship in the port
        */
        public bool checkPort(Ship ship)
        {
            float Distance = (float)Math.Sqrt(Math.Pow(ship.Location.X - Location.X, 2) + Math.Pow(ship.Location.Z - Location.Z, 2));
            
            if (Distance < m_fPickupRadius)
                return true;

            return false;
        }


                /** @fn     void Draw(Matrix matView, Matrix matProjection)
        *  @brief  draw the game object
        *  @param  device [in] the rendering device
        *  @param  matView [in] the active view matrix
        *  @param  matProjection [in] the active projection
        */
        public override void Draw(GraphicsDevice device, Matrix matView, Matrix matProj, Vector3 CameraPosition)
        {

            if (Settings.DETECT_EDGES == true)
                Shaders[0].CurrentTechnique = m_lstEffects[0].Techniques["NormalDepth"];
            else
                Shaders[0].CurrentTechnique = Shaders[0].Techniques["BasicTechNoTex"];

            Matrix matWorld = Matrix.Identity *                         //clear
        Matrix.CreateScale(ScaleVec) *                        //scale
        Matrix.CreateFromYawPitchRoll(Yaw, Pitch, Roll) *      //Rotation
        Matrix.CreateTranslation(Location);

            foreach (Effect effect in m_lstEffects)
            {
                //hacks here...
                if (effect.Parameters["g_matTransform"] != null)
                {
                    Matrix matTransform = matWorld * matView * matProj;
                    effect.Parameters["g_matTransform"].SetValue(matTransform);
                }

                ////////////////////////////
                //Basic Effect parameters
                if (effect.Parameters["World"] != null)
                    effect.Parameters["World"].SetValue(matWorld);

                if (effect.Parameters["View"] != null)
                    effect.Parameters["View"].SetValue(matView);

                if (effect.Parameters["Projection"] != null)
                    effect.Parameters["Projection"].SetValue(matProj);

                if (effect.Parameters["g_Alpha"] != null)
                    effect.Parameters["g_Alpha"].SetValue(m_Alpha);

                //Material colour
                if (effect.Parameters["DiffuseColor"] != null && m_lstMaterials.Count > 0)
                    effect.Parameters["DiffuseColor"].SetValue(m_Color.ToVector3());

                if (effect.Parameters["EyePosition"] != null)
                    effect.Parameters["EyePosition"].SetValue(CameraPosition);


                ///////////////////////////////////////////
                //Render the mesh with the current effect

                effect.Begin();

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Begin();

                    //mesh.Draw();
                    DrawMesh(device);

                    pass.End();
                }

                effect.End();

                Shaders[0].CurrentTechnique = Shaders[0].Techniques["BasicTech"];
            }
        }
    }
}
