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
    /** @class  CannonBall
     *  @brief  the weapon ships and towers hurl at each other
     */
    public class CannonBall : Object3D
    {
        string  m_strOwner;     ///< The player/tower that launched the cannonball
        Vector3 m_Velocity;
        Vector3 m_StartPoint;
        Vector3 m_EndPoint;
        bool    m_Firing;
        bool    m_bComplete;    ///< Has the cannonball completed its path
        float   m_Timer;
        bool    m_DownSwing;

        public bool Firing
        {
            get
            {
                return m_Firing;
            }
        }

        public bool Complete
        {
            get
            {
                return m_bComplete;
            }
            set
            {
                m_bComplete = value;
            }
        }

        public string Owner
        {
            get
            {
                return m_strOwner;
            }
            set
            {
                m_strOwner = value;
            }
        }

        /** @fn     CannonBall( Model mdl )
         *  @brief  Constructor
         *  @param  mdl [in] the model to use to display the CannonBall
         */
        public CannonBall(Model mdl)
            : base(mdl)
        {
            m_Firing    = false;
            m_bComplete = false;
            m_EndPoint = new Vector3();
        }

        public void FireCannon( string strOwner, Vector3 Velocity, Vector3 StartPoint, Vector3 EndPoint)
        {
             m_strOwner = strOwner;

             m_Velocity     = Velocity;
             m_StartPoint   = StartPoint;
             m_StartPoint.Y = 2.0f;
             m_EndPoint     = EndPoint;
             m_Firing       = true;
             m_bComplete    = false;
             m_Timer        = 0.0f;
             Y              = 0.0f;
             m_DownSwing = false;
        }

        public void FireCannonAtTarget( string strOwner, Vector3 StartPoint, Vector3 EndPoint)
        {
            m_strOwner = strOwner;

            //Horizontal Velocity, Set to a static number to avoid immense complications
            float HVel = Settings.CANNON_VELOCITY;

            //The Distance betweent the Start and the End
            Vector3 Distance = new Vector3(EndPoint.X - StartPoint.X, EndPoint.Y - StartPoint.Y, EndPoint.Z - StartPoint.Z);
            //The Horizontal Distance on the XZ Plane
            Vector2 HDistance = new Vector2(Distance.X, Distance.Z);

            //The Time is the Distance Scalar divided by the Velocity ( S = M/(M/S))
            float TotalTime = HDistance.Length() / HVel;
            //The XZ Velocity Components to be used when building the Arc
            m_Velocity.X = (HDistance.X / TotalTime);
            m_Velocity.Z = (HDistance.Y / TotalTime);

            //A reorganizing of the Trajectory Equation to isolate Vertical Velocity (after making the Y Value 0 as you want it to hit at this point)
            m_Velocity.Y = (Distance.Y + Settings.GRAVITY * (TotalTime * TotalTime) / 2) / TotalTime;

            m_StartPoint    = StartPoint;
            m_EndPoint      = EndPoint;
            m_Firing        = true;
            m_bComplete     = false;
            m_Timer         = 0.0f;
            Y               = 0.0f;
            m_DownSwing     = false;
        }

        /** @fn     void Draw()
         *  @brief  draw the cannon ball
         */
        public override void Draw(GraphicsDevice device, Matrix matView, Matrix matProjection, Vector3 CameraPosition)
        {
            if( m_Firing )
                base.Draw(device, matView, matProjection, CameraPosition);
        }

        /** @fn     Update( float fElapsedTime )
         *  @brief  update the cannon ball
         */
        public override void Update(float fElapsedTime)
        {
            if (m_Firing == true)
            {
                //if (Location.Y >= m_EndPoint.Y)
                if ((m_DownSwing == false || (Location - m_EndPoint).Length() >= 3.0f) && Location.Y > -1.0f)
                {
                    Vector3 Trajectory = new Vector3();
                    Trajectory.X = m_Velocity.X * m_Timer;
                    Trajectory.Z = m_Velocity.Z * m_Timer;
                    Trajectory.Y = m_Velocity.Y * m_Timer - (Settings.GRAVITY * (m_Timer * m_Timer)) / 2;

                    Location = m_StartPoint + Trajectory;
                    m_Timer += fElapsedTime;
                    Yaw += fElapsedTime * 5;
                    Roll += fElapsedTime * 5;

                    if (Location.Y >= m_EndPoint.Y)
                        m_DownSwing = true;
                }
                else
                {
                    m_Firing    = false;
                    m_bComplete = true;
                }  
            }
        }

        /** @fn     bool CheckColllision( Ship ship )
         *  @brief  check if the cannonball collided with a ship
         *  @return true if collided, false otherwise
         */
        public bool CheckCollision( Ship ship )
        {
            Vector2 vShipPos = new Vector2( ship.X, ship.Z );
            Vector2 vThisPos = new Vector2( X, Z );

            float fDistanceSquared  = ( vShipPos - vThisPos ).LengthSquared();
            float fRangeSquared     = Settings.CANNON_COLLIDE_RADIUS * Settings.CANNON_COLLIDE_RADIUS;

            if( fDistanceSquared < fRangeSquared )
                return true;

            return false;
        }
    }
}