using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

namespace LambMines
{

    struct Edge
    {
        public Vector2 pointA;
        public Vector2 pointB;
    }

    class Level
    {
        //This list must be a list of lists. This way each set of objects is on a different rendering plane so that objects can be rendered ontop of each other.
        private ArrayList AllObjects;
		private ArrayList AllBarriers;
        private ArrayList ObjSpawnList;
        private ArrayList MineSpawnList;
        Texture2D[] textureList;//The grass object
        private SpriteBatch m_sb;               ///< The sprite batch for 2D rendering
                                                ///
        public Offset theOffset;

        int Score;
        SpriteFont scoreFont;

        private struct TriggerObject
        {
			public TriggerObject(float rad, Object objRef) { radius = rad; referenceObj = objRef; }
            public float radius;
            public Object referenceObj;
        }

        private static ArrayList TriggerList = new ArrayList();
        public enum RuleList { RULE_PLACEMENT, RULE_TARGET };
		public enum RenderLevel { RL_TERRAIN, RL_SHADOWS, RL_MINES, RL_OBJECTS };

        private XNAModularApp m_ParentApp;

        /// <summary>
        /// Default constructor
        /// Not much in here yet, and probably not at all
        /// </summary>
        public Level(XNAModularApp ParentApp)
        {
            m_ParentApp = ParentApp;
            m_sb = new SpriteBatch(m_ParentApp.Device);
            ObjSpawnList = new ArrayList();
            MineSpawnList = new ArrayList();
            Score = 0;
        }

        public Texture2D[] TextureList
        {
            get { return textureList; }
        }

        public void Cleanup()
        {
            if (AllObjects != null)
            {
                AllObjects.Clear();
            }
            if (AllBarriers != null)
            {
                AllBarriers.Clear();
            }
            if (TriggerList != null)
            {
                TriggerList.Clear();
            }
            if (ObjSpawnList != null)
            {
                ObjSpawnList.Clear();
            }
            if (MineSpawnList != null)
            {
                MineSpawnList.Clear();
            }
        }

        /// <summary>
        /// Load a level from a a level file.
        /// </summary>
        /// <param name="levelName">The name of the level. ex "Freeforall_Level1.lvl"</param>
        /// <returns>False if the level could not be loaded or if there is already a level loaded.</returns>
        public bool LoadLevel(string levelName)
        {
            Cleanup();
            Score = 500;
            scoreFont = m_ParentApp.Content.Load<SpriteFont>("Content/Font/DebugFont");

            if (AllObjects != null)
                Log.WriteToLog(Log.LogErrorLevel.ERROR_MINOR, "The level has already been loaded.");

            //load the whole level here
            Hashtable fileInfoHash = readLevelFile(levelName);
			if (fileInfoHash.Contains("Textures"))
			{
				textureList = new Texture2D[((ArrayList)fileInfoHash["Textures"]).Count];
				for (int i = 0; i < ((ArrayList)fileInfoHash["Textures"]).Count; i++ )
				{
					try
					{
						textureList[i] = m_ParentApp.Content.Load<Texture2D>("Content/Textures/" + ((ArrayList)fileInfoHash["Textures"])[i].ToString());
					}
					catch (Exception)
					{
						Log.WriteToLog(Log.LogErrorLevel.ERROR_MAJOR,"Could not load resource " + ((ArrayList)fileInfoHash["Textures"])[i].ToString());
					}
				}
			}

            theOffset = new Offset();
			

            //load in all the objects.
            AllObjects = new ArrayList();
            AllObjects.Add(new ArrayList());//Add the first render plane. This plane will be exclusively for the terrain tiles.
			AllObjects.Add(new ArrayList());//Add a second plane for the shadows
			AllObjects.Add(new ArrayList());//Add another place for the mines
			AllObjects.Add(new ArrayList());//Add a third plane to start placing the objects on
			

			AllBarriers = new ArrayList();

			Vector2 tempVec = new Vector2();

            Random rand = new Random();
            for (int x = 8; x <25; x++)
            {
                for (int y = -8; y < 9; y++)
                {
            //for (int x = -8; x < 40; x++)
            //{
            //    for (int y = -24; y < 24; y++)
            //    {
                    Vector2 position = new Vector2(x * Settings.SCREEN_TILE_MULTIPLIER_X, y * Settings.SCREEN_TILE_MULTIPLIER_Y);
                    Texture2D tex = textureList[rand.Next(0,9)];
                    Tile tile = new Tile(position, tex);
                    ((ArrayList)AllObjects[(int)RenderLevel.RL_TERRAIN]).Add(tile);
                    tile.parent = this;
                }
            }

            //for (int x = 8; x < 25; x++)
            //{
            //    for (int y = -8; y < 9; y++)
            //    {
            //        List<Animation> animList = new List<Animation>();
            //        for (int j = 0; j < 8; j++)
            //        {
            //            Animation anim = new Animation(textureList[18], 1, true, new Vector2(35, 35), j);
            //            animList.Add(anim);
            //        }
            //        tempVec = new Vector2(x * Settings.SCREEN_TILE_MULTIPLIER_X, x * Settings.SCREEN_TILE_MULTIPLIER_Y);
            //        Sheep tempSheep = new Sheep(tempVec, animList, textureList[18]);

            //        ((ArrayList)AllObjects[(int)RenderLevel.RL_OBJECTS]).Add(tempSheep);

            //        //Clutter tempShadow = new Clutter(tempSheep.Position, textureList[21]);
            //        //tempShadow.AddShadow(ref tempSheep, textureList[21]);
            //        //((ArrayList)AllObjects[(int)RenderLevel.RL_SHADOWS]).Add(tempShadow);
            //        tempSheep.parent = this;
            //        //tempShadow.parent = this;
            //    }
            //}

            if (fileInfoHash.Contains("Edges"))
            {
				foreach (string table in (ArrayList)fileInfoHash["Edges"])
				{
					//load each of the collision barriers into an array.
					//AllBarriers.Add( table.Trim().Split(',') );
                    float[] edge = new float[4];
                    for (int i = 0; i < 4; i++)
                    {
                        edge[i] = float.Parse(table.Trim().Split(',')[i]) ;
                    }
                    Edge barrier = new Edge();
                    barrier.pointA = new Vector2(edge[0], edge[1]);
                    barrier.pointB = new Vector2(edge[2], edge[3]);
                    AllBarriers.Add(barrier);
				}
            }

            if (fileInfoHash.Contains("Mine"))
            {
                foreach (string value in ((ArrayList)fileInfoHash["Mine"]))
                {
                    int val1 = int.Parse(((string[])value.Split(','))[0]);
                    int val2 = int.Parse(((string[])value.Split(','))[1]);
                    int val3 = int.Parse(((string[])value.Split(','))[2]);
                    List<Animation> anim = new List<Animation>();
                    anim.Add(new Animation(textureList[val3], 0.3f, true, new Vector2(90,45), 0));
                    tempVec = new Vector2(val1 * Settings.SCREEN_TILE_MULTIPLIER_X, val2 * Settings.SCREEN_TILE_MULTIPLIER_Y);
                    Mine tempMine = new Mine(tempVec, anim, textureList[val3]);
                    ((ArrayList)AllObjects[(int)RenderLevel.RL_MINES]).Add(tempMine); TriggerList.Add(new TriggerObject(45.0f, tempMine));
                    tempMine.parent = this;
                }
            }

            if (fileInfoHash.Contains("Rocks") || fileInfoHash.Contains("Trees") || fileInfoHash.Contains("Fences"))
            {
                foreach (string value in ((ArrayList)fileInfoHash["Rocks"]))
                {
                    int val1 = int.Parse(((string[])value.Split(','))[0]);
                    int val2 = int.Parse(((string[])value.Split(','))[1]);
                    int val3 = int.Parse(((string[])value.Split(','))[2]);
                    tempVec = new Vector2(val1 * Settings.SCREEN_TILE_MULTIPLIER_X, val2 * Settings.SCREEN_TILE_MULTIPLIER_Y);
                    Clutter tempClutter = new Clutter(tempVec, textureList[val3]);
                    ((ArrayList)AllObjects[(int)RenderLevel.RL_OBJECTS]).Add(tempClutter);
                    tempClutter.parent = this;
                }
                foreach (string value in ((ArrayList)fileInfoHash["Trees"]))
                {
                    int val1 = int.Parse(((string[])value.Split(','))[0]);
                    int val2 = int.Parse(((string[])value.Split(','))[1]);
                    int val3 = int.Parse(((string[])value.Split(','))[2]);
                    tempVec = new Vector2(val1 * Settings.SCREEN_TILE_MULTIPLIER_X, val2 * Settings.SCREEN_TILE_MULTIPLIER_Y);
                    Clutter tempClutter = new Clutter(tempVec, textureList[val3]);
                    ((ArrayList)AllObjects[(int)RenderLevel.RL_OBJECTS]).Add(tempClutter);
                    tempClutter.parent = this;
                }
                foreach (string value in ((ArrayList)fileInfoHash["Fences"]))
                {
                    int val1 = int.Parse(((string[])value.Split(','))[0]);
                    int val2 = int.Parse(((string[])value.Split(','))[1]);
                    int val3 = int.Parse(((string[])value.Split(','))[2]);
                    tempVec = new Vector2(val1 * Settings.SCREEN_TILE_MULTIPLIER_X, val2 * Settings.SCREEN_TILE_MULTIPLIER_Y);
                    Clutter tempClutter = new Clutter(tempVec, textureList[val3]);
                    ((ArrayList)AllObjects[(int)RenderLevel.RL_OBJECTS]).Add(tempClutter);
                    tempClutter.parent = this;
                }
            }

            if (fileInfoHash.Contains("Sheep"))
            {
                List<Animation> animList = new List<Animation>();
                foreach (string value in ((ArrayList)fileInfoHash["Sheep"]))
                {

                    int val1 = int.Parse(((string[])value.Split(','))[0]);
                    int val2 = int.Parse(((string[])value.Split(','))[1]);
                    int val3 = int.Parse(((string[])value.Split(','))[2]);

                    for (int j = 0; j < 8; j++)
                    {
                        Animation anim = new Animation(textureList[val3], 1, true, new Vector2(35, 35), j);
                        animList.Add(anim);
                    }
                    tempVec = new Vector2(val1 * Settings.SCREEN_TILE_MULTIPLIER_X, val2 * Settings.SCREEN_TILE_MULTIPLIER_Y);
                    Sheep tempSheep = new Sheep(tempVec, animList, textureList[val3]);

                    ((ArrayList)AllObjects[(int)RenderLevel.RL_OBJECTS]).Add(tempSheep);

                    Clutter tempShadow = new Clutter(tempSheep.Position, textureList[21]);
                    tempShadow.AddShadow(ref tempSheep, textureList[21]);
                    ((ArrayList)AllObjects[(int)RenderLevel.RL_SHADOWS]).Add(tempShadow);
                    tempSheep.parent = this;
                    tempShadow.parent = this;
                    //tempSheep.AddShadow(textureList[21]);

                }
            }

			//just create an empty array list for the decals.
			//
			//((ArrayList)AllObjects[(int)RenderLevel.RL_BLOOD_DECALS]) = new ArrayList();
			
            //Player
            List<Animation> anims = new List<Animation>();
            for (int j = 0; j < 8; j++)
            {
                Animation anim = new Animation(textureList[20], 0.1f, true, new Vector2(35, 35), j);
                anims.Add(anim);
            }
            for (int k = 0; k < 8; k++)
            {
                Animation anim = new Animation(textureList[34], 0.1f, true, new Vector2(35, 35), k);
                anims.Add(anim);
            }
            Player tempPlayer = new Player(m_ParentApp.Inputs, new Vector2(400, 0), anims, textureList[20]);
            ((ArrayList)AllObjects[(int)RenderLevel.RL_OBJECTS]).Add(tempPlayer);
            TriggerList.Add(new TriggerObject(200.0f, tempPlayer));
            tempPlayer.parent = this;
			
            return true;
        }

        public bool Spawn(RenderLevel level, Object obj)
        {
            ObjSpawnList.Add(obj);
            return true;
        }

        public bool SpawnMine(RenderLevel level, Object obj)
        {
            if (Score >= 200)
            {
                MineSpawnList.Add(obj);
                Score -= 200;
            }
            return true;
        }

        public bool AddTrigger(float Radius, Object obj)
        {
            TriggerObject trigger = new TriggerObject(Radius, obj);
            ((ArrayList)TriggerList).Add(trigger);
            return true;
        }

        public void Kill(Object theSpriteToKill)
        {
            theSpriteToKill.Kill();
        }

        /// <summary>
        /// Process all the triggers and innitiate any onCollision events if the objects have collided.
        /// </summary>
        public void ProcessTriggers()
        {
            //loop though each trigger in the list.
            for (int i = 0; i < TriggerList.Count; i++ )
            {
                //loop through each tile object. check the collision.
                //if the collision has occured then
				bool flag = true;
				foreach (ArrayList planeList in AllObjects)
				{
					//check the flag so that the first element in the array is skipped. 
					if (flag) { flag = false; continue; }

					for (int c = 0; c < planeList.Count; c++)
					{
						//check the collision and if the two object collide.
						//the call the on collision method.
						Vector2 dist = ((Object)((TriggerObject)TriggerList[i]).referenceObj).Position + new Vector2(16, 16);
						dist = dist - ((Object)planeList[c]).Position;
						float finalDist = dist.Length();
						if (finalDist < ((TriggerObject)TriggerList[i]).radius && finalDist != 0 && ((Object)planeList[c]).WhatAmI() != "Explosion" )
						{
							Object tempObject = ((Object)((TriggerObject)TriggerList[i]).referenceObj).onCollision((Object)planeList[c],textureList);
							if (tempObject != null)
							{
								((ArrayList)AllObjects[(int)RenderLevel.RL_OBJECTS]).Add(tempObject);
							}
						}
					}
				}
            }
        }

        public void Update(float elapsedTime)
        {
			ArrayList triggerToDelete = new ArrayList();
			ArrayList subToDelete = new ArrayList();
			ArrayList shadowsToDelete = new ArrayList();
			ArrayList minesToDelete = new ArrayList();

            theOffset.UpdateVariables();
            theOffset.theExplosion();

            //m_ParentApp.theOffset.incrementVector(m_ParentApp.Inputs.offsetHack());

            theOffset.setMapDisplacement(m_ParentApp.Inputs.offsetHack(
                theOffset.getMapDisplacement(),
                theOffset.getOldMapDisplacement()));

            //loop through each main list
            foreach (ArrayList listMain in AllObjects)
            {
                //loop through each sub list of objects
                foreach (Object listSub in (ArrayList)listMain)
                {
                    //Object thisObject = listSub;
                    if (listSub.WhatAmI() == "Player") {
                        theOffset.followTarget( (-1 *(listSub.Position)) 
                            //+ m_ParentApp.theOffset.varienceDisplacement 
                            + new Vector2(768, 0));//768 is an arbitrary number that allows
                        //m_ParentApp.theOffset.setMapDisplacement(-1*(listSub.Position));
                    }
                    if (!listSub.Update(elapsedTime, AllBarriers))
					{
                        if (String.Compare(listSub.GetType().FullName, "LambMines.Clutter") == 0)
						{
							//the only clutter object that can be destroyed right now is a shadow.
							shadowsToDelete.Add(listSub);
							continue;
						}
						//this object has died.
						//it is a landmine or a sheep.
						//loop through the trigger list to remove the mine from the list
						for (int i = 0; i < TriggerList.Count; i++)
						{
							if (((TriggerObject)TriggerList[i]).referenceObj == listSub)
							{
								//TriggerList.RemoveAt(i);
								triggerToDelete.Add(TriggerList[i]);
                                
								break;
							}
                            
						}
						if (listSub.WhatAmI() == "Mine")
						{
							minesToDelete.Add(listSub);
							continue;
						}
                        if (String.Compare(listSub.GetType().FullName, "LambMines.Mine") == 0)
                        {
                            minesToDelete.Add(listSub);
                            continue;
                        }
                        if (String.Compare(listSub.GetType().FullName, "LambMines.Explosion") == 0)
                            theOffset.setExplosion(false);
						subToDelete.Add(listSub);
					}
                }
            }

            for (int j = 0; j < ObjSpawnList.Count;)
            {
                ((ArrayList)AllObjects[(int)RenderLevel.RL_SHADOWS]).Add(ObjSpawnList[j]);
                ObjSpawnList.Remove(ObjSpawnList[j]);
            }

            for (int k = 0; k < MineSpawnList.Count;)
            {
                ((ArrayList)AllObjects[(int)RenderLevel.RL_MINES]).Add(MineSpawnList[k]);
                MineSpawnList.Remove(MineSpawnList[k]);
            }

			//go from the reverse of the list because we are removing entries.
			for (int i = 0; i < triggerToDelete.Count; i++)
			{
				TriggerList.Remove(triggerToDelete[i]);
			}
			for (int i = 0; i < subToDelete.Count; i++)
			{
				((ArrayList)AllObjects[(int)RenderLevel.RL_OBJECTS]).Remove(subToDelete[i]);
			}
			for (int i = 0; i < shadowsToDelete.Count; i++)
			{
				((ArrayList)AllObjects[(int)RenderLevel.RL_SHADOWS]).Remove(shadowsToDelete[i]);
			}
			for (int i = 0; i < minesToDelete.Count; i++)
			{
				((ArrayList)AllObjects[(int)RenderLevel.RL_MINES]).Remove(minesToDelete[i]);
			}

        }

		public void Draw(GraphicsDevice device, GameTime gameTime)
        {
            //Draw the terrain first.

            device.Clear(Color.Red);
            m_sb.Begin();
            

            Random rand = new Random(gameTime.ElapsedRealTime.Milliseconds);
			/*
            for (int x = -100; x < 100; x++)
            {
                for (int y = -100; y < 100; y++)
                {
                    Vector2 position = new Vector2(x * Settings.SCREEN_TILE_MULTIPLIER_X, y * Settings.SCREEN_TILE_MULTIPLIER_Y );
                    position = GlobalHelpers.GetScreenCoords(position);
					m_sb.Draw(textureList[0], position, Color.White);
                }
            }
            */
            //loop through each main list
            foreach (object listMain in AllObjects)
            {
                //loop through each sub list of objects
                foreach (object listSub in (ArrayList)listMain)
                {
                    Object thisObject = (Object)listSub;
                    thisObject.Draw(m_sb, gameTime, theOffset.getMapDisplacement() + theOffset.varienceDisplacement);
                }
            }

            m_sb.DrawString(scoreFont, String.Concat("Score: $", Score.ToString(), "", ""), new Vector2(5, 5), Color.White);
            
            m_sb.End();

        }

        public void ScoreKill()
        {
            Score += 100;
        }

        /// <summary>
        /// Validates the placement location based on the rule given and the current location of all abjects in the world.
        /// </summary>
        /// <param name="theRule">Which rule to follow</param>
        /// <param name="xVal">The x location</param>
        /// <param name="yVal">The y location</param>
        /// <returns></returns>
        public bool Validate(RuleList theRule, double xVal, double yVal)
        {
            return true;
        }



        private Hashtable readLevelFile(string theFile)
        {
            StreamReader theReader = new StreamReader(theFile);

			Hashtable outHash = new Hashtable();

            string info = "";
			while ((info = theReader.ReadLine()) != null)
			{
				if (info.StartsWith("Dimensions"))
				{
					if (!outHash.Contains("Dimensions"))
						outHash.Add("Dimensions", new ArrayList());
					((ArrayList)outHash["Dimensions"]).Add(info.Trim());
				}
				else if (info.StartsWith("Textures: "))
				{
					if (!outHash.Contains("Textures"))
						outHash.Add("Textures", new ArrayList());
					int count = int.Parse(info.Substring(10).Trim());
					for (int i = 0; i < count; i++)
					{
						info = theReader.ReadLine();
						info = info.Trim();
						info = ((string[])info.Split(' '))[1];
						((ArrayList)outHash["Textures"]).Add(info.Trim());
						
					}
				}
				else if (info.StartsWith("Edges: "))
				{
					if (!outHash.Contains("Edges"))
						outHash.Add("Edges", new ArrayList());
					int count = int.Parse(info.Substring(7).Trim());
					for (int i = 0; i < count; i++)
					{
						info = theReader.ReadLine();
						((ArrayList)outHash["Edges"]).Add(info.Trim());
					}
				}
				else if (info.StartsWith("Mines: "))
				{
					if (!outHash.Contains("Mine"))
						outHash.Add("Mine", new ArrayList());
					int count = int.Parse(info.Substring(6).Trim());
					for (int i = 0; i < count; i++)
					{
						info = theReader.ReadLine();
						((ArrayList)outHash["Mine"]).Add(info.Trim());
					}
				}
				else if (info.StartsWith("Rocks: "))
				{
					if (!outHash.Contains("Rocks"))
						outHash.Add("Rocks", new ArrayList());
					int count = int.Parse(info.Substring(7).Trim());
					for (int i = 0; i < count; i++)
					{
						info = theReader.ReadLine();
						((ArrayList)outHash["Rocks"]).Add(info.Trim());
					}
				}
				else if (info.StartsWith("Trees: "))
				{
					if (!outHash.Contains("Trees"))
						outHash.Add("Trees", new ArrayList());
					int count = int.Parse(info.Substring(7).Trim());
					for (int i = 0; i < count; i++)
					{
						info = theReader.ReadLine();
						((ArrayList)outHash["Trees"]).Add(info.Trim());
					}
				}
				else if (info.StartsWith("Fences: "))
				{
					if (!outHash.Contains("Fences"))
						outHash.Add("Fences", new ArrayList());
					int count = int.Parse(info.Substring(8).Trim());
					for (int i = 0; i < count; i++)
					{
						info = theReader.ReadLine();
						((ArrayList)outHash["Fences"]).Add(info.Trim());
					}
				}
				else if (info.StartsWith("Sheep: "))
				{
					if (!outHash.Contains("Sheep"))
						outHash.Add("Sheep", new ArrayList());
					int count = int.Parse(info.Substring(7).Trim());
					for (int i = 0; i < count; i++)
					{
						info = theReader.ReadLine();
						((ArrayList)outHash["Sheep"]).Add(info.Trim());
					}
				}
			}
			return outHash;
        }
        
    }
}
