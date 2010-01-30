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
        Texture2D[] textureList;//The grass object
        private SpriteBatch m_sb;               ///< The sprite batch for 2D rendering

        private struct TriggerObject
        {
            double[] objectLoc;
            object referenceObj;
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
					catch (Exception error)
					{
						Log.WriteToLog(Log.LogErrorLevel.ERROR_MAJOR,"Could not load resource " + ((ArrayList)fileInfoHash["Textures"])[i].ToString());
					}
				}
			}

            //load the texture resources
            //grass = m_ParentApp.Content.Load<Texture2D>("Content/Textures/grass_base01");
            //tempTex = m_ParentApp.Content.Load<Texture2D>("Content/Textures/grass_base555-01");

            //load in all the objects.
            //temp code
            AllObjects = new ArrayList();
            AllObjects.Add(new ArrayList());

			if (fileInfoHash.Contains("Mine"))
			{
				foreach (string value in ((ArrayList)fileInfoHash["Mine"]))
				{
					int val1 = int.Parse(((string[])value.Split(','))[0]);
					int val2 = int.Parse(((string[])value.Split(','))[1]);
					int val3 = int.Parse(((string[])value.Split(','))[2]);

					Mine tempMine = new Mine(new Vector2(val1,val2), textureList[val3]);
					((ArrayList)AllObjects[0]).Add(tempMine); TriggerList.Add(tempMine);
				}
			}

            for (int i = 0; i < 10; i++)
            {
                Animation anim = new Animation(textureList[1], 1, true, new Vector2(textureList[1].Width, textureList[1].Height), 0);
                List<Animation> animList = new List<Animation>();
                animList.Add(anim);
                Sheep tempSheep = new Sheep(new Vector2(i * 48, i ), animList, textureList[1]);
                ((ArrayList)AllObjects[0]).Add(tempSheep);
            }

			/*
			Mine tempMine = new Mine(new int[] { 1, 1 }, ref textureList[1]);
			((ArrayList)AllObjects[0]).Add(tempMine); TriggerList.Add(tempMine);
			tempMine.Position = new double[]{ 5, 5};
			((ArrayList)AllObjects[0]).Add(tempMine); TriggerList.Add(tempMine);
			tempMine.Position = new double[]{ 2, 2 };
			((ArrayList)AllObjects[0]).Add(tempMine); TriggerList.Add(tempMine);
			tempMine.Position = new double[]{ 0, 5};
			((ArrayList)AllObjects[0]).Add(tempMine); TriggerList.Add(tempMine);
            */

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
            }
        }

        public void Update(float elapsedTime)
        {
            //loop through each main list
            foreach (object listMain in AllObjects)
            {
                //loop through each sub list of objects
                foreach (object listSub in (ArrayList)listMain)
                {
                    Object thisObject = (Object)listSub;
                    thisObject.Update(elapsedTime);
                }
            }
        }

        public void Draw(GraphicsDevice device, GameTime gameTime)
        {
            //Draw the terrain first.

            device.Clear(Color.Red);
            m_sb.Begin();

            for (int x = -100; x < 100; x++)
            {
                for (int y = -100; y < 100; y++)
                {
                    Vector2 position = new Vector2(x * Settings.SCREEN_TILE_MULTIPLIER_X, y * Settings.SCREEN_TILE_MULTIPLIER_Y);
                    position = GlobalHelpers.GetScreenCoords(position);
					m_sb.Draw(textureList[0], position, Color.White);
                }
            }
            
            //loop through each main list
            foreach (object listMain in AllObjects)
            {
                //loop through each sub list of objects
                foreach (object listSub in (ArrayList)listMain)
                {

                    Object thisObject = (Object)listSub;
                    thisObject.Draw(m_sb, gameTime);
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
            int readIndex = 0;
			Hashtable outHash = new Hashtable();

            string info = "";
            while ((info = theReader.ReadLine()) != null)
            {
                if (readIndex == 0)
                {
                    if (info.StartsWith("Textures: "))
                    {
                        readIndex = 1;
                    }
					else if (info.StartsWith("Mine: "))
					{
						readIndex = 2;
					}
                }
                else if (readIndex == 1)
                {
					if (info.Trim().ToLower() == "end")
					{
						readIndex = 0;
						continue;
					}
					if (!outHash.Contains("Textures"))
						outHash.Add("Textures", new ArrayList());
					((ArrayList)outHash["Textures"]).Add(info.Trim());
                }
				else if (readIndex == 2)
				{
					if (info.Trim().ToLower() == "end")
					{
						readIndex = 0;
						continue;
					}
					if (!outHash.Contains("Mine"))
						outHash.Add("Mine",new ArrayList());
					((ArrayList)outHash["Mine"]).Add(info.Trim());
				}


            }
			return outHash;
        }
        
    }
}
