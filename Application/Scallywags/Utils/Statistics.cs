using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scallywags
{
    public class Statistics
    {
        int shots;
        int hits;
        int losses;
        int selfDestructs;
        float distanceTravelled;
        int totalGold;
        int totalShips;

        int towerHits;
        int towerLosses;
        int shipHits;
        int shipLosses;

        //set+increment functions
        //Designed to be called in game to increment variables, when the following events happen:
        //When your ship shoots and misses
        /// <summary>
        /// Called when your ship shoots and misses
        /// </summary>
        public void incrementMisses()
        {
            shots++;
        }
        //When your ship shoots and hits an enemy ships
        /// <summary>
        /// Called when your ship shoots hits an enemy ship
        /// </summary>
        public void incrementShipHits()
        {
            shipHits++;
            hits++;
            shots++;
        }
        //When your ship shoots and hits a tower
        /// <summary>
        /// Called when your ship shoots and hits a tower
        /// </summary>
        public void incrementTowerHits()
        {
            towerHits++;
            hits++;
            shots++;
        }
        //When an enemy ship shoots and hits your ship
        /// <summary>
        /// Called when an enemy ship shoots and hits your ship
        /// </summary>
        public void incrementShipLosses()
        {
            shipLosses++;
            losses++;
        }
        //When a tower shoots and hits your ship
        /// <summary>
        /// Called when a tower shoots and hits your ship
        /// </summary>
        public void incrementTowerLosses()
        {
            towerLosses++;
            losses++;
        }
        //When you sink your own ship
        /// <summary>
        /// Called when you sink your own ship
        /// </summary>
        public void incrementSelfDestructs()
        {
            selfDestructs++;
        }
        //When the player moves forward
        /// <summary>
        /// Called when a ship moves forward
        /// </summary>
        public void addDistanceTravelled()
        {
            distanceTravelled++;
        }
        //Adds the distance travelled when the turn ends
        //Designed to be an alternative to the previous function
        /// <summary>
        /// Adds the current travelled distance to the total distance travelled
        /// </summary>
        /// <param name="distance">
        /// The distance travelled
        /// </param>
        public void addDistanceTravelled(int distance)
        {
            distanceTravelled += distance;
        }
        /// <summary>
        /// Passes the player's gold total
        /// </summary>
        /// <param name="gold">The amount of gold the player has</param>
        public void setGoldAmount(int gold)
        {
            totalGold = gold;
        }
        /// <summary>
        /// Passes the player's ship total
        /// </summary>
        /// <param name="ships">The amount of ships the player has</param>
        public void setShipAmount(int ships)
        {
            totalShips = ships;
        }

        //return functions
        //When displaying individual stats at the end of the game, call these functions:
        //Number of shots fired
        /// <summary>
        /// Returns the number of shots fired
        /// </summary>
        /// <returns>
        /// Number of shots fired
        /// </returns>
        public int returnShots()
        {
            return shots;
        }
        //Number of shots hit
        /// <summary>
        /// Returns the number of shots hit
        /// </summary>
        /// <returns>
        /// Number of shots hit
        /// </returns>
        public int returnHits()
        {
            return hits;
        }
        //Number of times sunk
        /// <summary>
        /// Returns the number of times you have been sunk
        /// </summary>
        /// <returns>
        /// Number of ship losses
        /// </returns>
        public int returnLosses()
        {
            return losses;
        }
        //Distance travelled
        /// <summary>
        /// Returns the total distance travelled
        /// </summary>
        /// <returns>
        /// Total distance travelled
        /// </returns>
        public float returnDistance()
        {
            return distanceTravelled;
        }
        //Number of towers you shot
        /// <summary>
        /// Returns the number of towers you shot
        /// </summary>
        /// <returns>
        /// Number of towers shot
        /// </returns>
        public int returnTowerHits()
        {
            return towerHits;
        }
        //Number of times you were hit by tower
        /// <summary>
        /// Returns the number of times you were hit by a tower
        /// </summary>
        /// <returns>
        /// Number of times hit by a tower
        /// </returns>
        public int returnTowerLosses()
        {
            return towerLosses;
        }
        //Number of ships you sunk
        /// <summary>
        /// Returns the number of ships you sunk
        /// </summary>
        /// <returns>
        /// Number of enemy ships hit
        /// </returns>
        public int returnShipHits()
        {
            return shipHits;
        }
        //Number of your ships sunk
        /// <summary>
        /// Returns the number of your ships sunk
        /// </summary>
        /// <returns>
        /// Number of your ships sunk
        /// </returns>
        public int returnShipLosses()
        {
            return shipLosses;
        }
        //Number of times sunk self
        /// <summary>
        /// Number of times you sunk yourself
        /// </summary>
        /// <returns>Number of suicides</returns>
        public int returnSelfInflicted()
        {
            return selfDestructs;
        }
        //Accuracy
        /// <summary>
        /// Returns your hit percentage
        /// </summary>
        /// <returns>
        /// Hit percentage
        /// </returns>
        public float returnHitPercentage()
        {
            if (shots == 0)
            {
                return 0;
            }
            else
            {
                return hits / (float)shots;
            }
        }
        /// <summary>
        /// Returns the total amount of gold the player has
        /// </summary>
        /// <returns>The total amount of gold the player has</returns>
        public int returnTotalGold()
        {
            return totalGold;
        }
        /// <summary>
        /// Returns the total amount of ships the player has
        /// </summary>
        /// <returns>The total amount of ships the player has</returns>
        public int returnTotalShips()
        {
            return totalShips;
        }
    }
}
