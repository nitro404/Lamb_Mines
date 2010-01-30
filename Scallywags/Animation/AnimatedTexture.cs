using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags
{
    /** @class
     *  @brief  a texture that contains multiple frames of an animation
     */
    public class AnimatedTexture
    {
        #region DATA_MEMBERS

        private Texture2D m_tex;        ///< The texture containing the animation
        private int m_nCellWidth;       ///< WIdth of a cell in the texture
        private int m_nCellHeight;      ///< Height of a cell in the texture
        private Color m_tint;           ///< Tint of the texture, default White
        private Vector2 m_vScale;       ///< Scale of the cells default (1,1)                         

        #endregion

        #region PROPERTIES

        public int CellWidth
        {
            get
            {
                return m_nCellWidth;
            }
        }

        public int CellHeight
        {
            get
            {
                return m_nCellHeight;
            }
        }

        public float XScale
        {
            set
            {
                m_vScale.X = value;
            }
        }

        public float YScale
        {
            set
            {
                m_vScale.Y = value;
            }
        }

        public int Width
        {
            get
            {
                return m_tex.Width;
            }
        }

        public int Height
        {
            get
            {
                return m_tex.Height;
            }
        }

        public int CellsPerRow
        {
            get
            {
                return Width / m_nCellWidth;
            }
        }

        public Color Tint
        {
            get
            {
                return m_tint;
            }
            set
            {
                m_tint = value;
            }
        }

        public int NumCells
        {
            get
            {
                int nCellsPerColumn = m_tex.Height / m_nCellHeight;
                return nCellsPerColumn * CellsPerRow;
            }
        }

        #endregion

        #region CONSTRUCTION

        /** @fn
         *  @brief  contructor
         */
        public AnimatedTexture(Texture2D tex, int nCellWidth, int nCellHeight)
        {
            m_tex           = tex;
            m_nCellWidth    = nCellWidth;
            m_nCellHeight   = nCellHeight;
            m_tint          = Color.White;
            m_vScale        = new Vector2( 1.0f, 1.0f );
        }

        #endregion

        #region INTERFACE

        /** @fn
         *  @brief  draw a cell within the texture at a specific location
         */
        public void Draw(int nCell, Vector2 vLoc, float fOrientation, SpriteBatch sb)
        {
            //get the cell coordinates
            int nCellX = nCell % CellsPerRow;
            int nCellY = nCell / CellsPerRow;

            //get the pixel coordinates
            int nPixelX = nCellX * m_nCellWidth;
            int nPixelY = nCellY * m_nCellHeight;

            //determine the center of that cell
            int nCenterX = m_nCellWidth / 2; //nPixelX + m_nCellWidth / 2;
            int nCenterY = m_nCellHeight / 2; //nPixelY + m_nCellHeight / 2;

            Vector2 vOrigin = new Vector2( nCenterX, nCenterY );

            //Get the drawing rect
            Rectangle rectDraw = new Rectangle(
                nPixelX, nPixelY,
                m_nCellWidth, m_nCellHeight );

            sb.Draw( m_tex, 
                vLoc,
                rectDraw,
                m_tint,
                fOrientation, 
                vOrigin, 
                m_vScale, 
                SpriteEffects.None, 
                0.5f ); 
        }

        #endregion
    }
}
