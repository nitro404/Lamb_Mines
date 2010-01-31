using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LambMines{
    public class Offset{

        private Vector2 m_MapDisplacement;
        private Vector2 m_OldMapDisplacement;
        private bool m_isThereAnExplosion;
        private bool m_isScreenPanning;

        public Offset() {
            m_MapDisplacement = Vector2.Zero;
            m_OldMapDisplacement = Vector2.Zero;
        }

        public void UpdateVariables() {
            m_OldMapDisplacement = m_MapDisplacement;
            //m_MapDisplacement = new Vector2();
        }

        public void incrementVector(Vector2 theVector) {
            m_MapDisplacement = m_OldMapDisplacement + theVector;
        }
        public void incrementVector(int x, int y) {
            m_MapDisplacement.X = m_OldMapDisplacement.X + x;
            m_MapDisplacement.Y = m_OldMapDisplacement.Y + y;
        }
        public void incrementVector(float x, float y) {
            m_MapDisplacement.X = m_OldMapDisplacement.X + x;
            m_MapDisplacement.Y = m_OldMapDisplacement.Y + y;
        }
        public void decrementVector(Vector2 theVector) {
            m_MapDisplacement = m_OldMapDisplacement - theVector;
        }
        public void decrementVector(int x, int y) {
            m_MapDisplacement.X = m_OldMapDisplacement.X - x;
            m_MapDisplacement.Y = m_OldMapDisplacement.Y - y;
        }
        public void decrementVector(float x, float y) {
            m_MapDisplacement.X = m_OldMapDisplacement.X - x;
            m_MapDisplacement.Y = m_OldMapDisplacement.Y - y;
        }

        public void theExplosion() {
            if (m_isThereAnExplosion) {
                if (m_isScreenPanning) {
                    m_MapDisplacement.X = m_OldMapDisplacement.X + 1;
                    m_MapDisplacement.Y = m_OldMapDisplacement.Y - 1;
                    m_isScreenPanning = !m_isScreenPanning;
                }
                else {
                    m_MapDisplacement.X = m_OldMapDisplacement.X - 1;
                    m_MapDisplacement.Y = m_OldMapDisplacement.Y + 1;
                    m_isScreenPanning = !m_isScreenPanning;
                }
            }
            //return m_MapDisplacement;
        }
        public void followTarget(Vector2 targetPos) {
            m_MapDisplacement.X = targetPos.X;
            m_MapDisplacement.Y = targetPos.Y;
            //return m_MapDisplacement;
        }
        public Vector2 getMapDisplacement() {
            return m_MapDisplacement;
        }
        public void setMapDisplacement(Vector2 NewDisplacement) {
            m_MapDisplacement = NewDisplacement;
        }
        public Vector2 getOldMapDisplacement() {
            return m_OldMapDisplacement;
        }
        public void setOldMapDisplacement(Vector2 NewDisplacement) {
            m_OldMapDisplacement = NewDisplacement;
        }
        public void setExplosion(bool doIExplode) {
            m_isThereAnExplosion = doIExplode;
        }
        public bool getExplosion() {
            return m_isThereAnExplosion;
        }
    }
}