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

namespace Scallywags
{
    class Level
    {
        //This list must be a list of lists. This way each set of objects is on a different rendering plane so that objects can be rendered ontop of each other.
        private ArrayList AllObjects;
		private ArrayList AllBarriers;
        Texture2D[] textureList;//The grass object
        private SpriteBatch m_sb;               ///< The sprite batch for 2D rendering

        private struct TriggerObject
        {
			public TriggerObject(float rad, Object objRef) { radius = rad; referenceObj = objRef; }
            public float radius;
            public Object referenceObj;
        }
        private static ArrayList TriggerList = new ArrayList();
        public enum RuleList { RULE_PLACEMENT, RULE_TARGET };

        private XNAModularApp m_ParentApp;

        /// <summary>
        /// Default constructor
        /// Not much in here yet, and probably not at all
        /// </summary>
        public Level(XNAModularApp ParentApp)
        {
            m_ParentApp = ParentApp;
            m_sb = new SpriteBatch(m_ParentApp.Device);
        }

        /// <summary>
        /// Load a level from a a level file.
        /// </summary>
        /// <param name="levelName">The name of the level. ex "Freeforall_Level1.lvl"</param>
        /// <returns>False if the level could not be loaded or if there is already a level loaded.</returns>
        public bool LoadLevel(string levelName)
        {
            if (AllObjects != null)
                Log.WriteToLog(Log.LogErrorLevel.ERROR_MINOR, "The level has already been loaded.");

            //load the whole level here
            Hashtable fileInfoHash = readLevelFile("Content/Levels/level1.2d");
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
			

            //load in all the objects.
            AllObjects = new ArrayList();
            AllObjects.Add(new ArrayList());//Add th efirst render plane. This plane will be exclusively for the terrain tiles.

			AllBarriers = new ArrayList();

			Vector2 tempVec = new Vector2();

            Random rand = new Random();
            for (int x = -1; x < 35; x++)
            {
                for (int y = -20; y < 30; y++)
                {
                    Vector2 position = new Vector2(x * Settings.SCREEN_TILE_MULTIPLIER_X, y * Settings.SCREEN_TILE_MULTIPLIER_Y);
                    Tile tile = new Tile(position, textureList[rand.Next(0,9)]);
                    ((ArrayList)AllObjects[0]).Add(tile);
                }
            }

			if (fileInfoHash.Contains("Barrier"))
			{
				foreach (Hashtable table in (Hashtable)fileInfoHash["Barrier"])
				{
					AllBarriers.Add(new ArrayList());
					foreach (ArrayList secondTable in table)
					{
						for (int i = 0; i < secondTable.Count; i++)
						{
							//load each of the collision barriers into an array.
							((ArrayList)AllBarriers[i]).Add(secondTable[i]);
						}
					}
				}
			}
			AllObjects.Add(new ArrayList());//Add a second plane to start placing the objects on
			if (fileInfoHash.Contains("Mine"))
			{
				foreach (string value in ((ArrayList)fileInfoHash["Mine"]))
				{
					int val1 = int.Parse(((string[])value.Split(','))[0]);
					int val2 = int.Parse(((string[])value.Split(','))[1]);
					int val3 = int.Parse(((string[])value.Split(','))[2]);

					tempVec = new Vector2(val1 * Settings.SCREEN_TILE_MULTIPLIER_X, val2 * Settings.SCREEN_TILE_MULTIPLIER_Y);
					Mine tempMine = new Mine(tempVec, textureList[val3]);
					((ArrayList)AllObjects[1]).Add(tempMine); TriggerList.Add(new TriggerObject(45.0f,tempMine));
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
					((ArrayList)AllObjects[1]).Add(tempClutter);
				}
				foreach (string value in ((ArrayList)fileInfoHash["Trees"]))
				{
					int val1 = int.Parse(((string[])value.Split(','))[0]);
					int val2 = int.Parse(((string[])value.Split(','))[1]);
					int val3 = int.Parse(((string[])value.Split(','))[2]);
					tempVec = new Vector2(val1 * Settings.SCREEN_TILE_MULTIPLIER_X, val2 * Settings.SCREEN_TILE_MULTIPLIER_Y);
					Clutter tempClutter = new Clutter(tempVec, textureList[val3]);
					((ArrayList)AllObjects[1]).Add(tempClutter);
				}
				foreach (string value in ((ArrayList)fileInfoHash["Fences"]))
				{
					int val1 = int.Parse(((string[])value.Split(','))[0]);
					int val2 = int.Parse(((string[])value.Split(','))[1]);
					int val3 = int.Parse(((string[])value.Split(','))[2]);
					tempVec = new Vector2(val1 * Settings.SCREEN_TILE_MULTIPLIER_X, val2 * Settings.SCREEN_TILE_MULTIPLIER_Y);
					Clutter tempClutter = new Clutter(tempVec, textureList[val3]);
					((ArrayList)AllObjects[1]).Add(tempClutter);
				}
			}

			if (fileInfoHash.Contains("Sheep"))
			{
				List<Animation> animList = new List<Animation>();
				foreach(string value in ((ArrayList)fileInfoHash["Sheep"])){

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
					tempSheep.AddShadow(textureList[21]);
					((ArrayList)AllObjects[1]).Add(tempSheep);
				}
			}
			
            //for (int i = 0; i < 20; i++)
            //{
            //    List<Animation> animList = new List<Animation>();
            //    for (int j = 0; j < 8; j++)
            //    {
            //        Animation anim = new Animation(textureList[18], 1, true, new Vector2(35,35), j);
            //        animList.Add(anim);
            //    }
            //    Sheep tempSheep = new Sheep(new Vector2((i * 64) + 10, i * -24 ), animList, textureList[18]);
            //    ((ArrayList)AllObjects[1]).Add(tempSheep);
            //}

            //Player
            List<Animation> anims = new List<Animation>();
            for (int j = 0; j < 8; j++)
            {
                Animation anim = new Animation(textureList[20], 0.1f, true, new Vector2(35, 35), j);
                anims.Add(anim);
            }
            Player tempPlayer = new Player(m_ParentApp.Inputs , new Vector2(512, 256), anims, textureList[20]);
            ((ArrayList)AllObjects[1]).Add(tempPlayer);
            TriggerList.Add(new TriggerObject(100.0f, tempPlayer));
			
            return true;
        }


        public bool Spawn()
        {
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
						Vector2 dist = ((Object)((TriggerObject)TriggerList[i]).referenceObj).Position;
						dist = dist - ((Object)planeList[c]).Position;
						float finalDist = dist.Length();
						if (finalDist < ((TriggerObject)TriggerList[i]).radius && finalDist != 0 && ((Object)planeList[c]).WhatAmI() != "Explosion" )
						{
							Object tempObject = ((Object)((TriggerObject)TriggerList[i]).referenceObj).onCollision((Object)planeList[c],textureList);
							if (tempObject != null)
							{
								((ArrayList)AllObjects[1]).Add(tempObject);
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
            //loop through each main list
            foreach (ArrayList listMain in AllObjects)
            {
                //loop through each sub list of objects
                foreach (Object listSub in (ArrayList)listMain)
                {
                    //Object thisObject = listSub;
					if (!listSub.Update(elapsedTime))
					{
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
						subToDelete.Add(listSub);
					}
                }
            }
			//go from the reverse of the list because we are removing entries.
			for (int i = 0; i < triggerToDelete.Count; i++)
			{
				TriggerList.Remove(triggerToDelete[i]);
			}
			for (int i = 0; i < subToDelete.Count; i++)
			{
				((ArrayList)AllObjects[1]).Remove(subToDelete[i]);
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
                    thisObject.Draw(m_sb, gameTime, m_ParentApp.Inputs.offsetHack());
                }
            }
            
            m_sb.End();

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
				else if (info.StartsWith("Barriers: "))
				{
					if (!outHash.Contains("Barriers"))
						outHash.Add("Barriers", new Hashtable());
					int count = int.Parse(info.Substring(10).Trim());
					for (int i = 0; i < count; i++)
					{
						info = theReader.ReadLine();
						((Hashtable)outHash["Barriers"]).Add(i, new ArrayList());
						int count2 = int.Parse(info.Substring(7));//"Edges: "
						for (int c = 0; c < count2; c++)
						{
							info = theReader.ReadLine();
							((ArrayList)((Hashtable)outHash["Barriers"])[i]).Add(info.Trim());
						}
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
