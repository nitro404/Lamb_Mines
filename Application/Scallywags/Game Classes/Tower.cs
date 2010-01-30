using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Scallywags
{
    /** @class  Tower
     *  @brief  Class to describe one of the town's defensive structures
     */
    public class Tower : Object3D
    {
        const float     COLLISION_DIST      = 7.5f;

        private float   m_fRange;           ///< The tower's range
        private float   m_fHealth;          ///< The health of the tower                  
        private bool    m_bCanFire;         ///< Can the tower fire?

        Model[] m_vModelStates;             ///< The possible states of the model ( from repaired.. damaged.. ruined... that sort of thing )
        int     m_nCurrentModelIndex;       ///< The index of the current model
        int     m_nID;                      ///< The ID of the tower.

        Object3D    m_cannon;               ///< The tower's cannon

        private List< Texture2D >   m_lstTowerTextures;
        private List< Vector3 >     m_lstTowerMaterials;

        public string Name
        {
            get
            {
                switch( m_nID )
                {
                    case 0:
                        return "East Tower";
                    case 1:
                        return "North Tower";
                    case 2:
                        return "West Tower";
                    case 3:
                        return "South Tower";
                    default:
                        return "East Tower";
                }            
            }
        }

        /** @prop   Range
         *  @brief  the range of the tower in World Units
         */
        public float Range
        {
            get
            {
                return m_fRange;
            }
        }

        /** @prop   IsAlive
         *  @brief  is the tower alive?
         */
        public bool IsAlive
        {
            get
            {
                return m_fHealth > 0;
            }
        }

        /** @prop   Direction
         *  @brief  the direction the tower is protecting
         */
        public float Direction
        {
            get
            {
                return Yaw;
            }
            set
            {
                Yaw = value;
            }
        }

        /** @prop   CanFire
         *  @brief  is the tower allowed to fire?
         */
        public bool CanFire
        {
            get
            {
                return m_bCanFire;
            }
            set
            {
                m_bCanFire = value;
            }
        }

        /** @fn     Tower( Model[] mdls )
         *  @brief  constructor
         *  @param  mdls [in] the tower models
         */
        public Tower()
            : base()
        {
            m_fRange                = Settings.TOWER_RANGE;
            m_vModelStates          = null;
            m_nCurrentModelIndex    = 0;
            m_fHealth               = Settings.TOWER_HEALTH;
            m_bCanFire              = false;

        }

        /** @fn     void Init( Model[] vModels )
         *  @brief  initialize the tower
         *  @param  vModels [in] the models that represent the states of the tower
         */
        public void Init( Model[] vModels, Model mdlCannon, int nID, Vector3 vLocation, float fDirection  )
        {
            Location    = vLocation;
            Direction   = fDirection;

            m_vModelStates          = vModels;
            m_nCurrentModelIndex    = 0;
            m_nID                   = nID;
            

            Vector3 vDir            = new Vector3( -(float)Math.Cos( Direction ), 0, (float)Math.Sin( Direction ) );

            m_cannon                = new Object3D( mdlCannon );
            m_cannon.Location       = vLocation + ( Vector3.Up * 8.0f ) + ( vDir * 1.0f );
            m_cannon.Yaw            = Direction;

            Reset();

            ExtractTowerTextures();
        }

        private void ExtractTowerTextures()
        {
            m_lstTowerTextures = new List< Texture2D >();
            m_lstTowerMaterials = new List< Vector3 >();

            foreach( Model model in m_vModelStates )
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach( ModelMeshPart part in mesh.MeshParts )
                    {
                        Vector3 vDiffuse = part.Effect.Parameters["DiffuseColor"].GetValueVector3();
                        m_lstTowerMaterials.Add(vDiffuse);

                        Texture2D tex = part.Effect.Parameters["BasicTexture"].GetValueTexture2D();

                        tex = part.Effect.Parameters["BasicTexture"].GetValueTexture2D();
                        m_lstTowerTextures.Add(tex);
                    }
                }
            }

            m_lstTextures = new List<Texture2D>();
            m_lstMaterials = new List<Vector3>();

            m_lstTextures.Add( m_lstTowerTextures[ 0 ] );
            m_lstMaterials.Add( m_lstTowerMaterials[ 0 ] );
        }

        /** @fn     void Reset()
         *  @brief  set the tower to it's default state
         */
        public void Reset()
        {
            m_nCurrentModelIndex    = 0;
            m_fHealth               = Settings.TOWER_HEALTH;
        }

        /** @fn     void Draw(GraphicsDevice device, Matrix matView, Matrix matProjection)
         *  @brief  draw the tower
         *  @param  device [in] the graphics device
         *  @param  matView [in] the view matrix
         *  @param  matProjection [in] the projection matrix
         */
        public override void Draw(GraphicsDevice device, Matrix matView, Matrix matProjection, Vector3 CameraPosition)
        {
            this.SetModel(m_vModelStates[m_nCurrentModelIndex]);

            //Set the active material/texture
            m_lstTextures[ 0 ]  = m_lstTowerTextures[ m_nCurrentModelIndex ];
            m_lstMaterials[ 0 ] = m_lstTowerMaterials[ m_nCurrentModelIndex ];

            base.Draw(device, matView, matProjection, CameraPosition);

            if( m_fHealth > 0 )
            {
                //Draw the cannon
                m_cannon.Draw( device, matView, matProjection, CameraPosition );
            }
        }

        /** @fn     void TakeDamage( float fAmount )
         *  @brief  have the tower take damage
         *  @param  fAmount [in] the amount of damage to take
         */
        public void TakeDamage( float fAmount )
        {
            m_fHealth -= fAmount;

            //Go to the last model if dead
            if( m_fHealth <= 0 )
                m_nCurrentModelIndex = m_vModelStates.Length - 1;
            else
            {
                //Pick an index that best represents the state of the tower
                int     nNumberOfModels = m_vModelStates.Length;
                float   fPercent        = m_fHealth / Settings.TOWER_HEALTH;

                m_nCurrentModelIndex = 0 + (int)Math.Round( nNumberOfModels * ( 1.0f - fPercent ) );
            }
        }

        /** @fn     bool IsInRange( Ship ship )
         *  @brief  check if a ship is in the tower's range
         *  @return true if the ship is in range, false otherwise
         *  @param  ship [in] the ship to check
         */
        public bool IsInRange( Ship ship )
        {
            bool bInRange = false;

            //Checking squared distances to avoid sqrt function calls
            float fRangeSquared     = m_fRange * m_fRange;
            Vector2 vDisplacement   = new Vector2( ship.X - X, ship.Z - Z );
            float fDistSquared      = vDisplacement.LengthSquared();

            if( fRangeSquared > fDistSquared )
            {
                //The ship is in range, check if the tower is facing it
               
                Vector2 vDirection = new Vector2( -(float)Math.Cos( Yaw ), (float)Math.Sin( Yaw ) );
                vDisplacement.Normalize();

                float fDot = Vector2.Dot( vDisplacement, vDirection );

                //Check that the ship is in front
                if( fDot > 0 )
                {
                    //check if the ship is in the attack angle range
                    float fAngleDifference = (float)Math.Acos( fDot );
                    if ( fAngleDifference < Settings.TOWER_ATTACK_ANGLE * 0.5f )
                    {
                        bInRange = true;
                    }
                }
            }

            return bInRange;
        }

        /** @fn     bool CheckCollision( CannonBall cb )
         *  @brief  check if a cannonball has collided with the tower
         *  @return true if they collide, false otherwise
         *  @param  cb [in] the cannonball to check
         */
        public bool CheckCollision( CannonBall cb )
        {
            bool bCollide = false;

            Vector3 vDisp = cb.Location - Location;
            
            float fDistSquared          = vDisp.LengthSquared();
            float fCollisionDistSquared = COLLISION_DIST * COLLISION_DIST;
            
            if( fDistSquared < fCollisionDistSquared )
                bCollide = true;

            return bCollide;
        }
    }
}
