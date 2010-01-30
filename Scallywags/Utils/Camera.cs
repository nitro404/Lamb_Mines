using System;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags
{
    /** @class  Camera
     *  @brief  maintains the view matrix...
     */
    public class Camera
    {
        #region DATA_MEMBERS

        const float WAYPOINTDISTANCE = 45.0f;

        private Vector3 m_vView;                ///< x,y,z location the camera is looking at
        private Vector3 m_vTarget;              ///< The LookAt Target it is attempting to reach - Tom
        private Vector3 m_vPosition;            ///< the position of the camera in the world
        private Vector3 m_vGoto;                ///< The Goto target for the camera to pan to - Tom
        private Vector3 m_vUp;                  ///< what the camera considers as up
        private Vector3 m_vOffset;              ///< how far back do we want the camera to sit
        private float m_fOldRotation;             ///< The Default Height of the Camera
        private float m_fOldHeight;                                          ///
                                    
        private Matrix m_matView;               ///< camera view matrix
        private Matrix m_matProj;               ///< camera projection matrix

        private float m_CameraMoveSpeed;        ///< The current camera move speed
        private float m_CameraRotSpeed;         ///< The current camera rotation speed
        private float m_CameraRotation;         ///< The current camera rotation
        private float m_CameraRadius;           ///< The camera orbit radius, distance to target
        private bool m_bIsBusy;                 ///< Is it in a Scripted Sequence?
        private Queue<CameraWaypoint> m_lstScriptedWaypoints; ///< A Series of locations to script through
        private CameraWaypoint m_wOriginalLocations;        ///< The Default Camera Location before Waypoints
        private float m_fScriptSpeed;
        private Vector3 m_CameraVector;
        private Vector3 m_TargetVector;
                        
        //private float m_fTargetYaw;             ///< The desired yaw of the camera
                                                
        #endregion

        #region CLASS_CONSTRUCTORS

        /** @fn     Camera()
         *  @brief  Default constructor
         */
        public Camera()
        {
            m_vView     = new Vector3(  0.0f,  0.0f,  0.0f );
            m_vPosition = new Vector3(  0.0f,  5.0f, -8.0f );
            m_vUp       = new Vector3(  0.0f,  1.0f,  0.0f );
            m_vOffset   = new Vector3( 10.0f, 20.0f, 20.0f );

            m_vTarget   = new Vector3(  0.0f,  0.0f,  0.0f );
            m_vGoto     = new Vector3(  0.0f, 50.0f, -8.0f );

            m_CameraMoveSpeed   = 0.0f;
            m_CameraRotSpeed    = 0.0f;
            m_CameraRadius = Settings.CAMERA_DEFAULT_RADIUS;
            m_CameraRotation    = -1.0f;
            m_bIsBusy = false;
            m_lstScriptedWaypoints = new Queue<CameraWaypoint>();
            m_fScriptSpeed = 0;
            m_CameraVector = new Vector3(0.0f,0.0f,0.0f);
            m_TargetVector = new Vector3(0.0f, 0.0f, 0.0f);
        }

        #endregion

        #region PROPERTIES

        /** @prop   Vector3 View
         *  @brief  Gets and Sets the m_vView vector
         */
        public Vector3 View
        {
            get
            {
                return m_vView;
            }
            set
            {
                m_vView = value;
            }
        }

        /** @prop   Vector3 Position
         *  @brief  Gets and Sets the m_vPosition vector
         */
        public Vector3 Position
        {
            get
            {
                return m_vPosition;
            }
            set 
            {
                m_vPosition = value;
            }
        }

        /** @prop   Vector3 Up
         *  @brief  Gets and Sets the m_vUp vector
         */
        public Vector3 Up
        {
            get
            {
                return m_vUp;
            }
            set
            {
                m_vUp = value;
            }
        }

        public bool IsBusy
        {
            get
            {
                return m_bIsBusy;
            }
        }

        /** @prop   Vector3 Offset
        *   @brief  Gets and Sets the m_vOffset vector
        */
        public Vector3 Offset
        {
            get
            {
                return m_vOffset;
            }
            set
            {
                m_vOffset = value;
            }
        }


        public float Height
        {
            get
            {
                return m_vGoto.Y;
            }
            set
            {
                if (!m_bIsBusy)
                    m_vGoto.Y = value;
            }
        }

        /** @prop   float Rotation
        *   @brief  Gets and Sets the camera rotation
        */
        public float Rotation
        {
            set
            {
                m_CameraRotation = value;
            }
            get
            {
                return m_CameraRotation;
            }
        }

        /** @prop   float Radius
        *   @brief  Gets and Sets the camera rotation
        */
        public float Radius
        {
            get
            {
                return m_CameraRadius;
            }
            set
            {
                m_CameraRadius = value;
            }
        }

        /** @prop   Matrix ViewMatrix
        *   @brief  Gets m_matView matrix
        */
        public Matrix ViewMatrix
        {
            get
            {
                return m_matView;
            }
        }

     

        /** @prop   Matrix ProjectionMatrix
        *   @brief  Gets m_matProj matrix
        */
        public Matrix ProjectionMatrix
        {
            get
            {
                return m_matProj;
            }
        }

        /** @prop   InPosition
         *  @brief  Is the camera in position?  Or is it moving somewhere?
         *
         */
        public bool InPosition
        {
            get
            {
                Vector2 ThePosition = new Vector2(m_vPosition.X, m_vPosition.Z);
                Vector2 TheGoto = new Vector2(m_vGoto.X, m_vGoto.Z);
                float fDist = (ThePosition - TheGoto).LengthSquared();
                float fComp = Settings.CAMERA_READY_DISTANCE * Settings.CAMERA_READY_DISTANCE;
 
                if( fDist <= fComp )
                    return true;
                else
                    return false;
            }
        }

        #endregion

        #region MODIFIERS

        /** @fn     void CameraInit( int nScreenWidth, int nScreenHeight )
         *  @brief  Initializes the camera
         *  @param  nScreenWidth [in] the width of the screen
         *  @param  nScreenHeight [in] the height of the screen
         */
        public void CameraInit( int nScreenWidth, int nScreenHeight )
        {
            float fAspectRatio = nScreenWidth / (float)nScreenHeight;

            //Create the projection matrix
            m_matProj = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4.0f, fAspectRatio, 1.0f, 10000.0f);
            m_matView = Matrix.CreateLookAt( m_vPosition, m_vView, m_vUp );
            m_vGoto   = m_vPosition;
            m_CameraVector = new Vector3(0.0f, 0.0f, 0.0f);
            m_TargetVector = new Vector3(0.0f, 0.0f, 0.0f);

            ClearScript();
        }

        public void SetWobble()
        {
            m_fOldRotation = m_CameraRotation;
            m_fOldHeight = m_vPosition.Y;
        }

        public void Wobble(Vector2 Control)
        {          
            m_CameraRotation = m_fOldRotation;
            m_CameraRotation += Control.X / 5;
            m_vPosition.Y = m_fOldHeight;
            m_vPosition.Y += Control.Y /5;
        }

        public void ResetWobble()
        {
            m_CameraRotation = m_fOldRotation;
            m_vPosition.Y = m_fOldHeight;
        }

        /** @fn     void Reset( )
         *  @brief  Resets the Camera
         */
        public void Reset()
        {
            m_CameraRotation = 0.0f;
            m_CameraRadius = Settings.CAMERA_DEFAULT_RADIUS;
        }

        /** @fn     void Rotate( float Amount )
         *  @brief  Rotates the camera around the Object
         */
        public void Rotate(float Amount)
        {
            m_CameraRotation += Amount;

            if (m_CameraRotation < 0)
                m_CameraRotation += MathHelper.TwoPi;
            if (m_CameraRotation > MathHelper.TwoPi)
                m_CameraRotation -= MathHelper.TwoPi;
        }

        /** @fn     void Zoom( float Amount )
        *  @brief   Zooms the camera
           */
        public void Zoom(float Amount)
        {
            if (m_CameraRadius + Amount > Settings.CAMERA_DEFAULT_RADIUS && m_CameraRadius + Amount < 400)
            {
                m_CameraRadius += Amount;
                m_vGoto.Y += Amount * 1.5f;
            }
        }

        /** @fn     void CameraUpdate()
         *  @brief  Updates our camera
         */
        public void FollowTarget( Vector3 target )
        {
            if(!m_bIsBusy)
            m_vTarget = new Vector3( target.X, target.Y + Settings.CAMERA_LOOKAT_HEIGHT, target.Z );
        }

        public void OrientCamera( float fAngle )
        {
            m_CameraRotation = fAngle ;//- target.Yaw; //Align to the target's view
        }

        /** @fn     void OrbitPlayer( Player player )
         *  @brief  Orbits the camera behind the player target
         */
        public void OrbitPlayer( Player player )
        {
            m_CameraRotation = -player.CurrentShip.Yaw + (float)Math.PI * 3 / 2;
        }

        //Adds a waypoint to the list
        public void AddCameraScript(CameraWaypoint Waypoint)
        {
            m_lstScriptedWaypoints.Enqueue(Waypoint);
        }

        //Adds a Series of Waypoints to the List
        public void AddCameraScript(Queue<CameraWaypoint> WaypointList)
        {
            m_lstScriptedWaypoints = WaypointList;
        }

        //Clears the Waypoints
        public void ClearScript()
        {
            m_lstScriptedWaypoints.Clear();
        }

        //Checks to see if Waypoints are Hit or if the List is completed and Goes to the Next Waypoint
        public void CheckWaypoints()
        {
            if (m_bIsBusy)
            {
                if (m_lstScriptedWaypoints.Count != 0)
                {
                    CameraWaypoint Waypoint = m_lstScriptedWaypoints.Peek();

                    if (new Vector2(Waypoint.m_Position.X - m_vPosition.X, Waypoint.m_Position.Z - m_vPosition.Z).Length() < WAYPOINTDISTANCE)
                    {
                        if (new Vector2(Waypoint.m_Target.X - m_vView.X, Waypoint.m_Target.Z - m_vView.Z).Length() < WAYPOINTDISTANCE)
                        {
                            m_lstScriptedWaypoints.Dequeue();
                            if (m_lstScriptedWaypoints.Count != 0)
                            {
                                CameraWaypoint newWaypoint = m_lstScriptedWaypoints.Peek();
                                m_vGoto = newWaypoint.m_Position;
                                //m_vTarget = newWaypoint.m_Target;
                            }
                            else
                            {
                                m_vGoto = m_wOriginalLocations.m_Position;
                                m_vTarget = m_wOriginalLocations.m_Target;
                                m_fScriptSpeed = 0.0f;
                                m_bIsBusy = false;
                            }
                        }
                    }
                }
                else
                {
                    m_vGoto = m_wOriginalLocations.m_Position;
                    m_vTarget = m_wOriginalLocations.m_Target;
                    m_fScriptSpeed = 0.0f;
                    m_bIsBusy = false;
                }
            }
        }

        //Begins running the Waypoints
        public void RunScript(float DurationInSeconds)
        {
            if (!m_bIsBusy)
            {
                if (m_lstScriptedWaypoints.Count != 0)
                {
                    m_bIsBusy = true;
                    m_wOriginalLocations.m_Position = m_vPosition;
                    m_wOriginalLocations.m_Target = m_vView;
                    CameraWaypoint Waypoint = m_lstScriptedWaypoints.Peek();
                    m_vGoto = Waypoint.m_Position;
                    m_vTarget = Waypoint.m_Target;
                    float Distance = 0;
                    float TargetDistance = 0;
                    
                    
                    for (int i = 1; i < m_lstScriptedWaypoints.Count; i++)
                    {
                        Distance += (m_lstScriptedWaypoints.ToArray()[i].m_Position - m_lstScriptedWaypoints.ToArray()[i-1].m_Position).Length();
                        TargetDistance += (m_lstScriptedWaypoints.ToArray()[i].m_Target - m_lstScriptedWaypoints.ToArray()[i-1].m_Target).Length();
                    }
                    float Speed;
                    if(Distance > TargetDistance)
                    {
                        Speed = Distance / DurationInSeconds;
                    }
                    else
                    {
                        Speed = TargetDistance / DurationInSeconds;
                    }
                    m_fScriptSpeed = Speed;
                    
                    //m_fScriptSpeed = 475.0f;
                }
            }
        }

        /** @fn     void Update( float fElapsedTime )
        *  @brief  Updates the camera to Interpolate between locations
        *  @param  fElapsedTime [in] information about the time between frames
        */
        public void Update(float fElapsedTime)
        {
            Vector3 Distance;

            //CAMERA MANAGEMENT
            if (!m_bIsBusy)
            {
                float x = m_CameraRadius * (float)Math.Cos(m_CameraRotation);
                float z = m_CameraRadius * (float)Math.Sin(m_CameraRotation);
                m_vGoto = new Vector3(m_vTarget.X + x, m_vGoto.Y, m_vTarget.Z + z);
            }
            //The Camera
            Distance = new Vector3(m_vGoto.X - m_vPosition.X, m_vGoto.Y - m_vPosition.Y, m_vGoto.Z - m_vPosition.Z);

            float fDistance = Distance.Length();

            if (!m_bIsBusy)
            {
                //Move the camera
                m_CameraMoveSpeed = Distance.Length() * 2;
            }
            else
            {
                //m_CameraMoveSpeed = 25.0f;//!TODO: Create control for this
                m_CameraMoveSpeed = m_fScriptSpeed;
            }

            if (fDistance > m_CameraMoveSpeed * fElapsedTime)
            {
                Distance.Normalize();
                Distance *= m_CameraMoveSpeed * fElapsedTime;
            }

            float CameraOffset = 5.0f * fElapsedTime;

            if (m_CameraVector.X < Distance.X - CameraOffset)
            {
                m_CameraVector.X += CameraOffset;
            }
            else if (m_CameraVector.X > Distance.X + CameraOffset)
            {
                m_CameraVector.X -= CameraOffset;
            }
            else
            {
                m_CameraVector.X = Distance.X;
            }

            if (m_CameraVector.Y < Distance.Y - CameraOffset)
            {
                m_CameraVector.Y += CameraOffset;
            }
            else if (m_CameraVector.Y > Distance.Y + CameraOffset)
            {
                m_CameraVector.Y -= CameraOffset;
            }
            else
            {
                m_CameraVector.Y = Distance.Y;
            }

            if (m_CameraVector.Z < Distance.Z - CameraOffset)
            {
                m_CameraVector.Z += CameraOffset;
            }
            else if (m_CameraVector.Z > Distance.Z + CameraOffset)
            {
                m_CameraVector.Z -= CameraOffset;
            }
            else
            {
                m_CameraVector.Z = Distance.Z;
            }
            if (!m_bIsBusy)
            {
                m_CameraVector = Distance;
            }
            //if ((m_vPosition - m_vGoto).LengthSquared() < (Distance * m_CameraRotSpeed * fElapsedTime).LengthSquared())
            //{
            //    m_vPosition = m_vGoto;
            //}
            //else
            //{
            m_vPosition += m_CameraVector;
            //m_vPosition += Distance * m_CameraMoveSpeed * fElapsedTime;
            //}


            //TARGET MANAGEMENT
            //Move the look at Target
            Distance = new Vector3(m_vTarget.X - m_vView.X, m_vTarget.Y - m_vView.Y, m_vTarget.Z - m_vView.Z);

            if (!m_bIsBusy)
            {
                //Move the camera
                m_CameraRotSpeed = Distance.Length() * 2;
            }
            else
            {
                //m_CameraRotSpeed = 25.0f;//!TODO: Create control for this
                m_CameraRotSpeed = m_fScriptSpeed;
                //m_vTarget = m_lstScriptedWaypoints.Peek().m_Target;
            }

            if (Distance.Length() > m_CameraRotSpeed * fElapsedTime)
            {
                Distance.Normalize();
                Distance *= m_CameraRotSpeed * fElapsedTime;
            }

            float TargetOffset = 50.0f * fElapsedTime;
            //////////
            if (m_TargetVector.X < Distance.X - TargetOffset)
            {
                m_TargetVector.X += TargetOffset;
            }
            else if (m_TargetVector.X > Distance.X + TargetOffset)
            {
                m_TargetVector.X -= TargetOffset;
            }
            else
            {
                m_TargetVector.X = Distance.X;
            }
            if (m_TargetVector.Y < Distance.Y - TargetOffset)
            {
                m_TargetVector.Y += TargetOffset;
            }
            else if (m_TargetVector.Y > Distance.Y + TargetOffset)
            {
                m_TargetVector.Y -= TargetOffset;
            }
            else
            {
                m_TargetVector.Y = Distance.Y;
            }
            if (m_TargetVector.Z < Distance.Z - TargetOffset)
            {
                m_TargetVector.Z += TargetOffset;
            }
            else if (m_TargetVector.Z > Distance.Z + TargetOffset)
            {
                m_TargetVector.Z -= TargetOffset;
            }
            else
            {
                m_TargetVector.Z = Distance.Z;
            }
            ///////////
            if (!m_bIsBusy)
            {
                m_TargetVector = Distance;
            }
            //if ((m_vView - m_vTarget).LengthSquared() < (Distance * m_CameraRotSpeed * fElapsedTime).LengthSquared()) {
            //    m_vView = m_vTarget;
            //}
            //else {
            //m_vView += Distance * m_CameraRotSpeed * fElapsedTime;
            m_vView += m_TargetVector;
            //}

            m_matView = Matrix.CreateLookAt(m_vPosition, m_vView, m_vUp);

            CheckWaypoints();
        }

        ///////////////////////////////
        //Debug camera
        public Matrix DebugView
        {
            get
            {
                Vector3 vPos        = new Vector3( 0.0f, 1000.0f, 0.0f );
                Vector3 vUp         = new Vector3( 0.0f, 0.0f, 1.0f );
                Vector3 vTarget     = Vector3.Zero;

                Matrix matView = Matrix.CreateLookAt(
                    vPos, 
                    vTarget,
                    vUp );
                
                return matView;
            }
        }

        #endregion
    }

#region Camera Waypoint
    public struct CameraWaypoint
    {
        public Vector3 m_Position;
        public Vector3 m_Target;
    }

#endregion

}