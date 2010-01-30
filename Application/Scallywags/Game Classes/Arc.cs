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
    public class Arc
    {
        List<Object3D> TickList;
        Model Tick;
        bool m_Enabled;
        Vector3 m_StartPoint;
        Vector3 m_EndPoint;
        World m_WorldRef;
        int NumTicks;
        Vector3 m_Velocity;  ///< To pass the Arc to the Cannonball class

        public Arc(Model mdl, World world)
        {
            TickList = new List<Object3D>();
            Tick = mdl;
            m_WorldRef = world;
            m_Enabled = false;
            m_Velocity = new Vector3();

        }

        public void SetTicks(int numticks)
        {
            TickList.Clear();
            m_WorldRef.ClearArc();
            NumTicks = numticks;
            for (int i = 0; i < NumTicks; i++)
            {
                Object3D obj = new Object3D(Tick);
                obj.Scale = 0.5f;
                TickList.Add(obj);
            }
            m_WorldRef.AddArc(TickList);
        }

        public int Ticks
        {
            get
            {
                return NumTicks;
            }
            set
            {
                NumTicks = value;
                SetTicks(NumTicks);
            }
        }

        public bool Enabled
        {
            get
            {
                return m_Enabled;
            }
            set
            {
                m_Enabled = value;
                if (m_Enabled == true)
                {
                    SetTicks(NumTicks);
                }
                else
                {
                    TickList.Clear();
                    m_WorldRef.ClearArc();
                }
            }
        }

        public Vector3 StartPoint
        {
            set
            {
                m_StartPoint = value;
            }
            get
            {
                return m_StartPoint;
            }
        }

        public Vector3 EndPoint
        {
            set
            {
                m_EndPoint = value;
            }
            get
            {
                return m_EndPoint;
            }
        }

        public Vector3 GetVelocity()
        {
            return m_Velocity;
        }

        public void Update()
        {
            if (m_Enabled == true)
            {
                //The Trajectory of the Ticks
                Vector3 Traj = new Vector3();
                //Horizontal Velocity, Set to a static number to avoid immense complications
                float HVel = Settings.CANNON_VELOCITY;

                //The Distance betweent the Start and the End
                Vector3 Distance = new Vector3(m_EndPoint.X - m_StartPoint.X, m_EndPoint.Y - m_StartPoint.Y, m_EndPoint.Z - m_StartPoint.Z);
                //The Horizontal Distance on the XZ Plane
                Vector2 HDistance = new Vector2(Distance.X, Distance.Z);

                //The Time is the Distance Scalar divided by the Velocity ( S = M/(M/S))
                float TotalTime = HDistance.Length() / HVel;
                //The XZ Velocity Components to be used when building the Arc
                m_Velocity.X = (HDistance.X / TotalTime);
                m_Velocity.Z = (HDistance.Y / TotalTime);

                //A reorganizing of the Trajectory Equation to isolate Vertical Velocity (after making the Y Value 0 as you want it to hit at this point)
                m_Velocity.Y = (Distance.Y + Settings.GRAVITY * (TotalTime * TotalTime) / 2) / TotalTime;

                //For loop to create ticks
                for (int i = 0; i < NumTicks; i++)
                {
                    //Sets up time as a fraction of Time available and number of ticks
                    float Time = ((TotalTime) / NumTicks) * i;

                    //Places each object in it's location by taking the Initial Velocities and Calculating for it's location after Time t
                    Traj.X = m_Velocity.X * Time;
                    Traj.Z = m_Velocity.Z * Time;
                    Traj.Y = m_Velocity.Y * Time - (Settings.GRAVITY * (Time * Time)) / 2;

                    TickList[i].Location = m_StartPoint + Traj;

                }
            }
        }

    }
}
