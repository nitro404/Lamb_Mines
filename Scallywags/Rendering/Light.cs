using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags
{
    /** @class  Light
     *  @brief  class to describe a light in the game world
     */
    public class Light
    {
        //Going to try and make it so a colour value of black will produce no effect in the combined pixel colour calculation
        //private Vector3 m_vPosition;
        private Vector3 m_vDirection;   ///< The direction of the light
        private Vector3 m_vDiffuse;     ///< the diffuse colour value of the light
        private Vector3 m_vSpecular;    ///< The specular colour value of the light
              
        //private bool    m_bActive;

        #region PROPERTIES

        /** @prop   Diffuse
         *  @brief  the diffuse value of the light
         */
        public Color Diffuse
        {
            get
            {
                return new Color(m_vDiffuse.X, m_vDiffuse.Y, m_vDiffuse.Z, 1.0f);
            }
            set
            {
                m_vDiffuse = value.ToVector3();
            }
        }

        /** @prop   Specular
         *  @brief  the specular value of the light
         */
        public Color Specular
        {
            get
            {
                return new Color(m_vSpecular.X, m_vSpecular.Y, m_vSpecular.Z, 1.0f);
            }
            set
            {
                m_vSpecular = value.ToVector3();
            }
        }

        /** @prop   Direction
         *  @brief  the direction of the light
         */
        public Vector3 Direction
        {
            get
            {
                return m_vDirection;
            }
            set
            {
                m_vDirection = value;
            }
        }
        
        #endregion

        /** @fn     Light()
         *  @brief  contructor
         */
        public Light()
        {
            m_vDiffuse      = Vector3.Zero;
            m_vSpecular     = Vector3.Zero;
            m_vDirection    = Vector3.Right;
        }


    }
}
