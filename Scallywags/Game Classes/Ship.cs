using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Scallywags
{
    /** @class Ship
     *  @brief Stores and handles the information regarding a ship.
     */
    public class Ship : Object3D
    {
        private int m_controllingPlayer;
        private bool m_ToBeEnabled;
        private bool m_hasBooty;
        private bool m_isDisabled;
        private bool m_inPlay;
        private int m_shipIndex;
        private ShipGold m_Gold;
        private bool m_bHasCannon;

        private bool m_bTurning;            ///< Flag to help reduce the roll battle between TurnRight/Left and Update

        private float m_moveSpeed;

        private Vector3 m_vPrevLocation;        ///< The ship's previous location

        private float m_fModelOffset = -MathHelper.PiOver2;       ///< The yaw offset of the model- 

        #region Properties

        /*  @prop hasBooty
         *  @brief whether or not the ship is currently holding precious cargo
         */
        public bool hasBooty
        {
            get
            {
                return m_hasBooty;
            }
            set
            {
                m_hasBooty = value;
            }
        }

        public bool HasCannon
        {
            get
            {
                return m_bHasCannon;
            }
            set
            {
                m_bHasCannon = value;
            }
        }

        /*  @prop Enable
         *  @brief Sets the Ship to be enabled next turn
         */
        public bool PrepEnable
        {
            get
            {
                return m_ToBeEnabled;
            }
            set
            {
                m_ToBeEnabled = value;
            }
        }

        public ShipGold Gold
        {
            get
            {
                return m_Gold;
            }
        }

        /*  @prop isDisabled
         *  @brief wether or not the sihp is currently disabled.
         */
        public bool isDisabled
        {
            get
            {
                return m_isDisabled;
            }
            set
            {
                m_isDisabled = value;
            }
        }

        /*  @prop exists
         *  @brief Whether or not the ship is on the play field
         */
        public bool exists
        {
            get
            {
                return m_inPlay;
            }
        }

        /*  @prop shipHeading
         *  @brief the heading of the ship in degrees. Handled if the value is less than 0 or grater than 360.
         */
        public float shipHeading
        {
            get
            {
                return Yaw;
            }
            set
            {
                //Switched this to radians  <-- Mike
                float temp;
                temp = value;
                while (temp < 0) { temp += MathHelper.TwoPi; }
                while (temp > MathHelper.TwoPi) { temp -= MathHelper.TwoPi; }
                Yaw = temp;
            }
        }

        /** @prop   PrevPosition
         *  @brief  get the previous position of the ship in 2D coordinates
         */
        public Vector2 PreviousLocation2D
        {
            get
            {
                return new Vector2( m_vPrevLocation.X, m_vPrevLocation.Z );
            }
        }

        /** @prop   ControllingPlayer
         *  @brief  gets and sets the player controlling this ship
         */
        public int ControllingPlayer
        {
            get
            {
                return m_controllingPlayer;
            }
            set
            {
                m_controllingPlayer = value;
            }
        }

        /** @prop   CurrentLocation2D
         *  @brief  get the current position of the ship in 2D coordinates
         */
        public Vector2 CurrentLocation2D
        {
            get
            {
                return new Vector2( Location.X, Location.Z );
            }
        }

        public Texture2D Diffuse
        {
            get
            {
                return m_tDiffuse;
            }
        }

        #endregion

        /** @fn     Ship( Model mdl )
         *  @brief  the ship constructor
         *  @param  mdl [in] the model used to display the ship
         */
        public Ship( Model mdl , Model Gold, int shipIndex) : base( mdl )
        {
            //m_vScale = new Vector3(0.0f, 1.0f, 1.0f);
            Yaw = 0f;
            Location = new Vector3(0.0f,0.0f,0.0f);
            m_vPrevLocation = Location;
            m_shipIndex = shipIndex;
            m_moveSpeed = 1.0f;
            m_Gold = new ShipGold(Gold);
            m_bHasCannon = true;

            m_bTurning = false;
        }

        public Ship(Model mdl, Model Gold, int shipIndex, Texture2D DiffuseMap)
            : base(mdl, DiffuseMap)
        {
            //m_vScale = new Vector3(0.0f, 1.0f, 1.0f);
            Yaw = 0f;
            Location = new Vector3(0.0f, 0.0f, 0.0f);
            m_vPrevLocation = Location;
            m_shipIndex = shipIndex;
            m_moveSpeed = 1.0f;
            m_Gold = new ShipGold(Gold);
            m_bHasCannon = true;

            m_bTurning = false;
        }

        #region Functionality

        public void InitGold(Model mdl)
        {
            m_Gold.SwapShader(Shaders[0]);
        }

        /*  @fn moveForward
         *  @brief move the ship forward
         *  @return bool Whether the ship was able to move forward or not.
         */
        public bool moveForward(float Amount)
        {
            m_vPrevLocation = Location;

            m_moveSpeed = Amount;

            float fXDiff = (float)Math.Cos( Yaw ) * Amount;            float fZDiff = (float)Math.Sin( Yaw ) * Amount;
            this.X -= fXDiff;   //Our X Axis is inverted, so subtract in place of add.
            this.Z += fZDiff;

            return true;
        }
        /*  @fn turnLeft
         *  @brief turn the ship to the Left
         */
        public void turnLeft(float Amount)
        {
            m_bTurning = true;

            shipHeading = shipHeading + Amount;

            //Make the ship lean to the side when it is turning
            if (Roll < 3.1415f / 18f)
            {
                Roll += Amount;
            }

        }
        /*  @fn turnRight
         *  @breif turn the ship to the Right.
         */
        public void turnRight(float Amount)
        {
            m_bTurning = true;

            shipHeading = shipHeading - Amount;
            
            if ( Roll > -3.1415f / 18f)
            {
                Roll -= Amount;
            }
        }
        /*  @fn spawnAtHome
         *  @brief Spawns this ship at the home dock
         *  @param xLocation [in] the X location of the spawn position
         *  @param yLocation [in] the Y location of the spawn position
         */
        public void spawnAtHome(float xLocation, float yLocation)
        {
            m_inPlay = true;
            m_isDisabled = false;
            m_hasBooty = false;

            Yaw = 0.0f;
            Location = new Vector3(xLocation, 0.0f, yLocation);
            m_vPrevLocation = Location;
        }

        /*  @fn moveToLocation
         *  @brief Moves the ship to the specified location
         *  @param Vector2 newLocation The location in which to move the ship
         */
        public float moveToLocation(Vector2 newLocation) {

            float distance = (float)Math.Sqrt(Math.Pow(newLocation.X - m_vPrevLocation.X, 2) + Math.Pow(newLocation.Y - m_vPrevLocation.Z, 2));

            X = newLocation.X;
            Z = newLocation.Y;

            //Prevlocation becomes the same as new location because the ship just warped.
            m_vPrevLocation = Location;

            return distance / m_moveSpeed;
        }

        public void moveTo(float xLoc, float yLoc)
        {
            X = xLoc;
            Z = yLoc;

            //Prevlocation becomes the same as new location because the ship just warped.
            m_vPrevLocation = Location;
        }

        /*  @fn moveToAISafeRegion
         *  @brief moves the ship to a location that is outside the AI's collision radius.
         */
        public void moveToAISafeRegion(float speed) {

            Vector2 currShipLoc = CurrentLocation2D;
            Vector2 newLocation = new Vector2(0.0f,0.0f);
            
            if (CurrentLocation2D.Length() > 270) {//if the ship is outside the collision sphere
                //check to se if it is outside the collision box
                if (CurrentLocation2D.X > 230 || CurrentLocation2D.X < -230 || CurrentLocation2D.Y > 230 || CurrentLocation2D.Y < -230) {
                    //ship is outside the playable world area

                    //shipHeading += MathHelper.Pi;
                    newLocation = currShipLoc;
                    //if (newLocation.X < 230 && newLocation.X > -230 && newLocation.Y < 230 && newLocation.Y > -230) {
                        //newLocation = currShipLoc;
                        //if the ship is within the square collision box
                        if (newLocation.X > 230){
                            //move it back to within the square collision area.
                            newLocation.X = 230;
                        }
                        else if (newLocation.X < -230) {
                            //move it back to within the square collision area.
                            newLocation.X = -230;
                        }
                        if (newLocation.Y > 230) {
                            //move it back to within the square collision area.
                            newLocation.Y = 230;
                        }
                        else if (newLocation.Y < -230) {
                            //move it back to within the square collision area.
                            newLocation.Y = -230;
                            //newLocation *= 230f / (float)Math.Sin(shipHeading);
                        }
                    //}

                        if (newLocation.Length() < 270) {
                            //move the ship to within the outer collision circle
                            newLocation = currShipLoc;
                            newLocation.Normalize();
                            newLocation *= 270f;
                        }

                    moveToLocation(newLocation);
                    return;
                }
            }

            //check if the ship is within the inner collision radius.
            if (CurrentLocation2D.Length() < 75) {

                Vector2 newPortVec = new Vector2(0, 75);//one port outer circle area.
                newPortVec -= CurrentLocation2D;
                bool letItThrough = false;
                if (newPortVec.Length() >= 30 && newPortVec.Length() < 30 + speed*1.1) {//the player is in the safe area of this port.
                    newPortVec.Normalize();
                    newPortVec *= 30;
                    newLocation = new Vector2(0, 75) - newPortVec;
                    moveToLocation(newLocation);
                    return;
                }
                else if (newPortVec.Length() < 30) {
                    letItThrough = true;
                }
                newPortVec = new Vector2(0, -75);//one port outer circle area.
                newPortVec -= CurrentLocation2D;
                if (newPortVec.Length() >= 30 && newPortVec.Length() < 30 + speed*1.1) {//the player is in the safe area of this port.
                    newPortVec.Normalize();
                    newPortVec *= 30;
                    newLocation = new Vector2(0, -75) - newPortVec;
                    moveToLocation(newLocation);
                    return;
                }
                else if (newPortVec.Length() < 30) {
                    letItThrough = true;
                }
                newPortVec = new Vector2(75, 0);//one port outer circle area.
                newPortVec -= CurrentLocation2D;
                if (newPortVec.Length() >= 30 && newPortVec.Length() < 30 + speed * 1.1) {//the player is in the safe area of this port.
                    newPortVec.Normalize();
                    newPortVec *= 30;
                    newLocation = new Vector2(75, 0) - newPortVec;
                    moveToLocation(newLocation);
                    return;
                }
                else if (newPortVec.Length() < 30) {
                    letItThrough = true;
                }
                newPortVec = new Vector2(-75, 0);//one port outer circle area.
                newPortVec -= CurrentLocation2D;
                if (newPortVec.Length() >= 30 && newPortVec.Length() < 30 + speed * 1.1) {//the player is in the safe area of this port.
                    newPortVec.Normalize();
                    newPortVec *= 30;
                    newLocation = new Vector2(-75, 0) - newPortVec;
                    moveToLocation(newLocation);
                    return;
                }
                else if (newPortVec.Length() < 30) {
                    letItThrough = true;
                }

                if (!letItThrough) {
                    newLocation = currShipLoc;
                    newLocation.Normalize();
                    newLocation *= 75.1f;
                    moveToLocation(newLocation);
                }

            }


        }


        /*  @fn disableShip
         *  @brief Disables this ship for a turn.
         */
        public void disableShip()
        {
            m_isDisabled = true;
            //m_hasBooty = false;
            Roll = MathHelper.Pi;
        }

        /*  @fn enableShip
         *  @brief Disables this ship for a turn.
         */
        public void EnableShip()
        {
            m_isDisabled = false;
            Roll = 0f;
        }
        
        /*  @fn Update
         *  @brief Virtual Override for the Update function.
         *  @param fElapsedTime [in] The elapsed time for the last frame
         */
        public override void Update(float fElapsedTime)
        {

            if (!m_isDisabled && !m_bTurning)
            {
                if ( Roll < 0)
                {
                    Roll += 0.005f;
                }
                if (Roll > 0)
                {
                    Roll -= 0.005f;
                }
                if (Roll < 0.005f && Roll > -0.005f)
                {
                    Roll = 0;
                }
            }

            m_Gold.Enabled = hasBooty;

            m_bTurning = false;
        }

        public override void Draw(GraphicsDevice device, Matrix matView, Matrix matProjection, Vector3 CameraPosition) 
        {
                Yaw += m_fModelOffset;
             
                //To prevent skipping, can't find another way :S -Tom
                m_Gold.Location = Location;
                m_Gold.Yaw = Yaw + (float)Math.PI;
                m_Gold.Roll = -Roll; 

                //create the model's world transform matrix
                Matrix matWorld = Matrix.Identity *                 //clear
                    Matrix.CreateScale(Scale) *                  //scale
                    Matrix.CreateFromYawPitchRoll(Yaw, Pitch, Roll) *   //Rotation
                    Matrix.CreateTranslation(Location);          //translate

                if (Settings.DETECT_EDGES == true)
                    Shaders[0].CurrentTechnique = m_lstEffects[0].Techniques["NormalDepthWobble"];
                else
                {
                    if (m_tDiffuse != null)
                    {
                        m_lstEffects[0].CurrentTechnique = m_lstEffects[0].Techniques["DiffuseWobbleTech"];
                    }
                    else
                    {
                        Shaders[0].CurrentTechnique = Shaders[0].Techniques["WobbleTech"];
                    }
                }

                    foreach (Effect effect in m_lstEffects)
                    {
                        if (effect.Parameters["g_vBoatPos"] != null)
                            effect.Parameters["g_vBoatPos"].SetValue(Location);

                        if (effect.Parameters["g_fBoatRotation"] != null)
                            effect.Parameters["g_fBoatRotation"].SetValue(Yaw);
                    }


                    foreach (Effect effect in m_lstEffects)
                    {
                        ////////////////////////////
                        //Basic Effect parameters
                        if (effect.Parameters["World"] != null)
                            effect.Parameters["World"].SetValue(matWorld);

                        if (effect.Parameters["View"] != null)
                            effect.Parameters["View"].SetValue(matView);

                        if (effect.Parameters["Projection"] != null)
                            effect.Parameters["Projection"].SetValue(matProjection);

                        if (effect.Parameters["g_Alpha"] != null)
                            effect.Parameters["g_Alpha"].SetValue(m_Alpha);

                        //Material colour
                        if (effect.Parameters["DiffuseColor"] != null && m_Color != Color.TransparentWhite)
                            effect.Parameters["DiffuseColor"].SetValue(m_Color.ToVector3());
                        else if (effect.Parameters["DiffuseColor"] != null && m_lstMaterials.Count > 0)
                            effect.Parameters["DiffuseColor"].SetValue(m_lstMaterials[0]);

                        //Active texture
                        if (effect.Parameters["BasicTexture"] != null && m_lstTextures.Count > 0)
                            effect.Parameters["BasicTexture"].SetValue(m_lstTextures[0]);

                        if (effect.Parameters["DiffuseMap"] != null && m_tDiffuse != null)
                            effect.Parameters["DiffuseMap"].SetValue(m_tDiffuse);

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

                    }

                    Shaders[0].CurrentTechnique = Shaders[0].Techniques["BasicTech"];

                Yaw -= m_fModelOffset;

                

        }

        #endregion
    }

}
