using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veal
{
    class Level
    {
        /// <summary>
        /// Default constructor
        /// Not much in here yet, and probably not at all
        /// </summary>
        Level()
        {

        }

        /// <summary>
        /// Load a level from a a level file.
        /// </summary>
        /// <param name="levelName">The name of the level. ex "Freeforall_Level1.lvl"</param>
        /// <returns>False if the level could not be loaded or if there is already a level loaded.</returns>
        bool LoadLevel(string levelName)
        {


            return true;
        }


        bool Spawn()
        {


            return true;
        }

        void Kill()
        {

        }

        void ProcessTriggers()
        {
        
        }

        void Update()
        {

        }

        void Draw()
        {

        }

        /// <summary>
        /// Validates the placement location based on the rule given and the current location of all abjects in the world.
        /// </summary>
        /// <param name="theRule">Which rule to follow</param>
        /// <param name="xVal">The x location</param>
        /// <param name="yVal">The y location</param>
        /// <returns></returns>
        bool Validate(global.RuleList theRule, double xVal, double yVal)
        {
            return true;
        }



    }
}
