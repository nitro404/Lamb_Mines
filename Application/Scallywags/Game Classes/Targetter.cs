using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Scallywags
{

    public class Targetter:Object3D
    {
        List<Vector3> WaveLocations;
        Ship AssignedShip;
        float m_TargetRadius;
        float m_TargetRotation;
        Vector3 m_OldLocation;
        Vector3 m_WobbleTarget;
        bool m_bUseShipRotation;


        Color m_color;

        public float TargetterLocation
        {
            get
            {
                return m_TargetRotation + AssignedShip.shipHeading;
            }
        }

        public bool UseShipRotation
        {
            get
            {
                return m_bUseShipRotation;
            }
            set
            {
                m_bUseShipRotation = value;
            }
        }

        public float Rotation
        {
            set
            {
                m_TargetRotation = value;
            }
        }

        /** @prop  Ship
        *  @brief  The Ship that the targetter is set to;
        */
        public Ship Ship
        {
            get
            {
                return AssignedShip;
            }
            set
            {
                AssignedShip = value;
                m_TargetRadius = 75.0f;
                m_TargetRotation = 0.0f;
            }
        }

        /** @prop  Colour
        *  @brief  The Colour of the Targetter;
        */
        public Color Colour
        {
            set
            {
                m_color = value;
                m_color.A = 100;
            }
        }

        /** @fn     Targetter()
        *   @brief  Set up the Scale, Location, radius, Colour, and Model of hte Targetter
        */
        public Targetter(Model mdl, List<Vector3> Locations, bool UseRotation)
            : base(mdl)
        {
            Colour = Color.Red;
            WaveLocations = Locations;
            Location = new Vector3(0.0f, 1.0f, 0.0f);
            m_TargetRadius = 20.0f;
            m_TargetRotation = 0.0f;
            m_bUseShipRotation = UseRotation;
        }

        /** @fn     OrbitTarget(float)
        *   @brief  Sets the rotation amount relative to the target
        */
        public void OrbitTarget(float Amount)
        {
            m_TargetRotation += Amount;
        }

        /** @fn     ChangeRadius(float)
        *   @brief  Sets the distance to rotate around relative to the target ship
        */
        public void ChangeRadius(float Amount)
        {
            if (m_TargetRadius + Amount > 10 && m_TargetRadius + Amount < 125)
            m_TargetRadius += Amount;
        }

        /** @fn     MoveTarget(Vector2)
        *   @brief  Unassigns a ship to follow and places the Target in a certain location
        */
        public void MoveTarget(Vector2 loc)
        {
            AssignedShip = null;
            X = loc.X;
            Z = loc.Y;
        }

        /** @fn     MoveTarget(Vector2)
        *   @brief  Unassigns a ship to follow and places the Target in a certain location
        */
        public void LockShip(Ship ship)
        {
            AssignedShip = ship;
            //Location = ship.Location;
            m_TargetRadius = 0;
        }
       
        /** @fn     CheckHits(Player[])
        *   @brief  Checks to see if any ships are in the Targetting Circle
        */
        public ArrayList CheckHits(Player[] PlayerList)
        {
            ArrayList ShipsHit = new ArrayList();
            for (int PlayerCount = 0; PlayerCount < 4; PlayerCount++)
            {
                for (int ShipCount = 0; ShipCount < PlayerList[PlayerCount].totalShips; ShipCount++)
                {                    
                    float Distance = (float)Math.Sqrt(Math.Pow(PlayerList[PlayerCount].Ships[ShipCount].X - X, 2) + Math.Pow(PlayerList[PlayerCount].Ships[ShipCount].Z - Z, 2));
                    if (Distance < Settings.TARGET_RADIUS)
                    {
                        ShipsHit.Add(PlayerList[PlayerCount].Ships[ShipCount]);
                    }
                }
            }
            return ShipsHit;
        }

        /** @fn     SavePosition()
        *   @brief  Saves the position of the Targetter
        */
        public void SavePosition()
        {
            m_OldLocation = Location;
            m_WobbleTarget = Location;
        }

        /** @fn     LoadPosition()
        *   @brief  Loads the position of the Targetter
        */
        public void LoadPosition()
        {
            Location = m_OldLocation;
        }

        /** @fn     Wobble()
        *   @brief  Wobbles the Targetter (requires a saved position)
        */
        public void Wobble(InputManager inputs, Camera camera, int NumPlayers, Ship CurShip, Collision collision,int Turn, Player[] playerList,Town theTown)
        {

            Vector3 RangeVec = new Vector3(CurShip.Location.X - m_WobbleTarget.X, CurShip.Location.Y - m_WobbleTarget.Y, CurShip.Location.Z - m_WobbleTarget.Z);
            float Range = RangeVec.Length();

            Random random = new Random();
            m_WobbleTarget.X += (float)(random.NextDouble() - 0.5) * Range / 5;
            m_WobbleTarget.Z += (float)(random.NextDouble() - 0.5) * Range / 5;

            if (X > m_OldLocation.X + 45 || X < m_OldLocation.X - 45)
            {
                m_WobbleTarget.X = m_OldLocation.X;
            }
            if (Z > m_OldLocation.Z + 45 || Z < m_OldLocation.Z - 45)
            {
                m_WobbleTarget.Z = m_OldLocation.Z;
            }

            Vector3 ControlNorth = new Vector3();
            Vector3 ControlWest = new Vector3();

            ControlNorth.X = camera.Radius * (float)Math.Cos(camera.Rotation);
            ControlNorth.Z = camera.Radius * (float)Math.Sin(camera.Rotation);
            ControlNorth.Normalize();

            ControlWest.X = camera.Radius * (float)Math.Cos(camera.Rotation + Math.PI / 2);
            ControlWest.Z = camera.Radius * (float)Math.Sin(camera.Rotation + Math.PI / 2);
            ControlWest.Normalize();

            Vector3 Control = new Vector3(0.0f);
            //Vector2 CameraControl = new Vector2(0.0f);

            if (inputs.IsKeyDown(Keys.L))
            {
                Control -= ControlWest;
                //CameraControl.X += 1;
            }
            if (inputs.IsKeyDown(Keys.J))
            {
                Control += ControlWest;
                //CameraControl.X -= 1;
            }
            if (inputs.IsKeyDown(Keys.I))
            {
                Control -= ControlNorth;
                //CameraControl.Y += 1;
            }
            if (inputs.IsKeyDown(Keys.K))
            {
                Control += ControlNorth;
                //CameraControl.Y -= 1;
            }

            //find the nearest ship - for the AI
            int closestPlayerIndex = -1;
            int closestShipIndex = -1;
            int closestTownIndex = -1;
            float closestDist = 5000;
            Vector2 tempLocation;
            Vector2 targetLocation = new Vector2(X, Z);
            for (int i = 0; i < playerList.Length; i++)
            {
                //check the players
                for (int s = 0; s < playerList[i].Ships.Length; s++) 
                {
                    if( playerList[i].Ships[s] != null )
                    {
                        try {
                            tempLocation = playerList[i].Ships[s].CurrentLocation2D;
                            if (Vector2.Distance(tempLocation, targetLocation) < closestDist) {
                                closestDist = Vector2.Distance(tempLocation, targetLocation);
                                closestPlayerIndex = i;
                                closestShipIndex = s;
                            }
                        }
                        catch (Exception oops) {
                            //if an exception is caught then that means that that ship was not availible...
                            Error.Trace( "Unhandle exception in Targetter.Wobble(): " + oops.Message );
                        }
                    }
                }
            }
            //check the towers too
            tempLocation = new Vector2(0,30);
            if (Vector2.Distance(tempLocation, targetLocation) < closestDist && theTown.Towers[0].IsAlive) {
                closestDist = Vector2.Distance(tempLocation, targetLocation);
                closestPlayerIndex = -1;
                closestShipIndex = -1;
                closestTownIndex = 0;
            }
            tempLocation = new Vector2(-30, 0);
            if (Vector2.Distance(tempLocation, targetLocation) < closestDist && theTown.Towers[1].IsAlive) {
                closestDist = Vector2.Distance(tempLocation, targetLocation);
                closestPlayerIndex = -1;
                closestShipIndex = -1;
                closestTownIndex = 1;
            }
            tempLocation = new Vector2(30, 0);
            if (Vector2.Distance(tempLocation, targetLocation) < closestDist && theTown.Towers[2].IsAlive) {
                closestDist = Vector2.Distance(tempLocation, targetLocation);
                closestPlayerIndex = -1;
                closestShipIndex = -1;
                closestTownIndex = 2;
            }
            tempLocation = new Vector2(0, -30);
            if (Vector2.Distance(tempLocation, targetLocation) < closestDist && theTown.Towers[3].IsAlive) {
                closestDist = Vector2.Distance(tempLocation, targetLocation);
                closestPlayerIndex = -1;
                closestShipIndex = -1;
                closestTownIndex = 3;
            }

            //This is the Input Checker for the Gamepad
            for (int i = 0; i < NumPlayers; i++)
            {
                float PlayerFactor = 2.0f / NumPlayers;

                if (i == Ship.ControllingPlayer)
                    PlayerFactor = 1;
                if (playerList[i].IsAI && !Settings.DISABLE_AI) {
                    Vector2 AITargetAim = Vector2.Zero;
                    if (closestTownIndex == -1) {//if the town is not closest
                        bool shootAt = playerList[i].fireAtTarget(Turn, closestPlayerIndex, closestShipIndex);
                        //if it is this AI's turn then alwase aim towards the target
                        if (i == Ship.ControllingPlayer)
                        {
                            AITargetAim = playerList[i].currentAction.FireAt - targetLocation ;
                            AITargetAim.Normalize();
                        }
                        else if (shootAt) {//if the AI wants to help
                            AITargetAim =playerList[closestPlayerIndex].Ships[closestShipIndex].CurrentLocation2D - targetLocation ;
                            AITargetAim.Normalize();
                        }
                        else {//if the AI wants to mess with the shot.
                            AITargetAim =targetLocation - playerList[closestPlayerIndex].Ships[closestShipIndex].CurrentLocation2D ;
                            AITargetAim.Normalize();

                        }
                    }
                    else {
                        if (closestTownIndex == 0) {
                            //make te aim a little more random
                            if (i % 2 == 0 || i == Turn) {
                                AITargetAim = new Vector2(0, 30) - targetLocation;
                            }
                            else{
                                AITargetAim =targetLocation - new Vector2(0, 30);
                            }
                            if(AITargetAim.Length() > 1)AITargetAim.Normalize();
                        }
                        else if (closestTownIndex == 1) {
                            if (i % 2 == 0 || i == Turn) {
                                AITargetAim = new Vector2(-30, 0) - targetLocation;
                            }
                            else {
                                AITargetAim = targetLocation - new Vector2(-30, 0);
                            }
                            if (AITargetAim.Length() > 1) AITargetAim.Normalize();
                        }
                        else if (closestTownIndex == 2) {
                            if (i % 2 == 0 || i == Turn) {
                                AITargetAim = new Vector2(30, 0) - targetLocation;
                            }
                            else {
                                AITargetAim = targetLocation - new Vector2(30, 0);
                            }
                            if (AITargetAim.Length() > 1) AITargetAim.Normalize();
                        }
                        else {
                            if (i % 2 == 0 || i == Turn) {
                                AITargetAim = new Vector2(0, -30) - targetLocation;
                            }
                            else {
                                AITargetAim = targetLocation - new Vector2(0, -30);
                            }
                            if (AITargetAim.Length() > 1) AITargetAim.Normalize();
                        }
                    }
                    Control.Z += AITargetAim.Y * PlayerFactor;
                    Control.X += AITargetAim.X * PlayerFactor;
                }
                else {
                    if (inputs.CheckRStick(i).Y < -Settings.DEAD_ZONE) {
                        Control -= ControlNorth * inputs.CheckRStick(i).Y * PlayerFactor;
                        //CameraControl.Y += inputs.CheckRStick(i).Y / PlayerFactor;
                    }

                    if (inputs.CheckRStick(i).Y > Settings.DEAD_ZONE) {
                        Control -= ControlNorth * inputs.CheckRStick(i).Y * PlayerFactor;
                        //CameraControl.Y += inputs.CheckRStick(i).Y / PlayerFactor;
                    }

                    if (inputs.CheckRStick(i).X < -Settings.DEAD_ZONE) {
                        Control -= ControlWest * inputs.CheckRStick(i).X * PlayerFactor;
                        // CameraControl.X += inputs.CheckRStick(i).X / PlayerFactor;
                    }

                    if (inputs.CheckRStick(i).X > Settings.DEAD_ZONE) {
                        Control -= ControlWest * inputs.CheckRStick(i).X * PlayerFactor;
                        //CameraControl.X += inputs.CheckRStick(i).X / PlayerFactor;
                    }
                }
            }
            for (int i = 0; i < NumPlayers; i++)
            {

                Vector3 PlayerAmount = new Vector3();
                if (inputs.CheckRStick(i).Y < -Settings.DEAD_ZONE)
                {
                    PlayerAmount -= ControlNorth * inputs.CheckRStick(i).Y;
                }

                if (inputs.CheckRStick(i).Y > Settings.DEAD_ZONE)
                {
                    PlayerAmount -= ControlNorth * inputs.CheckRStick(i).Y;
                }

                if (inputs.CheckRStick(i).X < -Settings.DEAD_ZONE)
                {
                    PlayerAmount -= ControlWest * inputs.CheckRStick(i).X;
                }

                if (inputs.CheckRStick(i).X > Settings.DEAD_ZONE)
                {
                    PlayerAmount -= ControlWest * inputs.CheckRStick(i).X;
                }

                Vector3 NormalControl = new Vector3(Control.X, Control.Y, Control.Z);
                NormalControl.Normalize();
                float Vibration;

                if (PlayerAmount != Vector3.Zero)
                {
                    PlayerAmount.Normalize();

                   Vibration = (1 - (Vector3.Dot(NormalControl, PlayerAmount) + 1) / 2)/2;
                }
                else
                {
                    Vibration = 0.25f;
                }

                if (!playerList[i].IsAI)
                GamePad.SetVibration((PlayerIndex)i, Vibration, Vibration);


            }

            //camera.Wobble(CameraControl);

            Control *= 0.5f;

            Vector3 Distance;
            float WobbleMoveSpeed;

            Distance = new Vector3(m_WobbleTarget.X - X, m_WobbleTarget.Y - Y, m_WobbleTarget.Z - Z);

            WobbleMoveSpeed = Range/600 + 0.2f;

            if (Distance.Length() > WobbleMoveSpeed)
            {
                Distance.Normalize();
            }

            if (Settings.WOBBLE)
            {
                Location += Distance * WobbleMoveSpeed + Control;
            }
            else
            {
                Location += Control;
            }

            collisionInfo Info = collision.findHeight(new Vector2(X, Z));
            Y = Info.heightAtPoint;
            float XYNormalRotation = (float)Math.Atan2(Info.collisionNormal.Y, Info.collisionNormal.X);
            float ZYNormalRotation = (float)Math.Atan2(Info.collisionNormal.Y, Info.collisionNormal.Z);

            //For Later (Need new Height Map)
            if (Y >= 1.0f)
            {
                //Roll = XYNormalRotation;
                //Pitch = ZYNormalRotation;
            }
            else
            {
                Y = 1.0f;
                Pitch = 0.0f;
                Roll = 0.0f;
            }
        }


        /** @fn     Update()
        *   @brief  Updates the rotation and movement of the Targetter
        */
        public void Update(Collision collision)
        {
            float x = 0;
            float z = 0;

            if (AssignedShip != null)
            {
                if (m_bUseShipRotation)
                {
                    x = m_TargetRadius * (float)Math.Cos(m_TargetRotation + AssignedShip.shipHeading);
                    z = m_TargetRadius * (float)Math.Sin(m_TargetRotation + AssignedShip.shipHeading);
                }
                else
                {
                    x = m_TargetRadius * (float)Math.Cos(m_TargetRotation);
                    z = m_TargetRadius * (float)Math.Sin(m_TargetRotation);
                }
            }

            X = AssignedShip.Location.X - x;
            Z = AssignedShip.Location.Z + z;

            if (m_bUseShipRotation)
            {
                Yaw = m_TargetRotation + AssignedShip.shipHeading;
            }
            else
            {
                Yaw = m_TargetRotation;
            }

            collisionInfo Info = collision.findHeight(new Vector2(X, Z));
            Y = Info.heightAtPoint;
            float XYNormalRotation = (float)Math.Atan2(Info.collisionNormal.Y, Info.collisionNormal.X);
            float ZYNormalRotation = (float)Math.Atan2(Info.collisionNormal.Y, Info.collisionNormal.Z);

            //For Later (Need new Height Map)
            if (Y >= 1.0f)
            {
                //Increases the gap betweeen targetter and ground
                Y += 1.0f;
                //Pitch = XYNormalRotation;
                //Roll = ZYNormalRotation;
            }
            else
            {
                Y = 1.0f;
                Pitch = 0.0f;
                Roll = 0.0f;
            }
        }

        /** @fn     void Draw(Matrix matView, Matrix matProjection)
        *  @brief  draw the game object
        *  @param  device [in] the rendering device
        *  @param  matView [in] the active view matrix
        *  @param  matProjection [in] the active projection
        */
        public override void Draw(GraphicsDevice device, Matrix matView, Matrix matProj, Vector3 CameraPosition)
        {
            if (Y > 1.0)
                Shaders[0].CurrentTechnique = Shaders[0].Techniques["OnLandTech"];
            else
                Shaders[0].CurrentTechnique = Shaders[0].Techniques["OnWaterTech"];

            Matrix matWorld = Matrix.Identity *                         //clear
                Matrix.CreateScale( ScaleVec ) *                        //scale
                Matrix.CreateFromYawPitchRoll( Yaw, Pitch, Roll) *      //Rotation
                Matrix.CreateTranslation( Location ); 

            foreach (Effect effect in m_lstEffects)
            {
                //hacks here...
                if (effect.Parameters["g_matTransform"] != null)
                {
                    Matrix matTransform = matWorld * matView * matProj;
                    effect.Parameters["g_matTransform"].SetValue(matTransform);
                }

                if (effect.Parameters["g_vWaveLocation1"] != null)
                    effect.Parameters["g_vWaveLocation1"].SetValue(WaveLocations[0]);
                if (effect.Parameters["g_vWaveLocation2"] != null)
                    effect.Parameters["g_vWaveLocation2"].SetValue(WaveLocations[1]);
                if (effect.Parameters["g_vWaveLocation3"] != null)
                    effect.Parameters["g_vWaveLocation3"].SetValue(WaveLocations[2]);
                if (effect.Parameters["g_vWaveLocation4"] != null)
                    effect.Parameters["g_vWaveLocation4"].SetValue(WaveLocations[3]);

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
                    effect.Parameters["DiffuseColor"].SetValue(m_color.ToVector3());

                if (effect.Parameters["EyePosition"] != null)
                    effect.Parameters["EyePosition"].SetValue(CameraPosition);

                //Active texture
                if (effect.Parameters["BasicTexture"] != null && m_lstTextures.Count > 0)
                    effect.Parameters["BasicTexture"].SetValue(m_lstTextures[0]);

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
        }
    
    }
}
