using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Scallywags
{
    class Level
    {
        private ArrayList AllObjects;
        private struct TriggerObject
        {
            double[] objectLoc;
            object referenceObj;
        }
        private static ArrayList TriggerList = new ArrayList();
        public enum RuleList { RULE_PLACEMENT, RULE_TARGET };

        /// <summary>
        /// Default constructor
        /// Not much in here yet, and probably not at all
        /// </summary>
        public Level()
        {

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

            //temp code
            AllObjects = new ArrayList();
            AllObjects.Add(new ArrayList());


            ((ArrayList)AllObjects[0]).Add(new Mine(new int[] { 10, 10 })); TriggerList.Add(AllObjects[0]);
            ((ArrayList)AllObjects[0]).Add(new Mine(new int[] { 15, 15 })); TriggerList.Add(AllObjects[1]);
            ((ArrayList)AllObjects[0]).Add(new Mine(new int[] { 20, 20 })); TriggerList.Add(AllObjects[2]);
            ((ArrayList)AllObjects[0]).Add(new Mine(new int[] { 10, 15 })); TriggerList.Add(AllObjects[3]);
            

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

        public void Update()
        {

        }

        public void Draw()
        {

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



    }
}
