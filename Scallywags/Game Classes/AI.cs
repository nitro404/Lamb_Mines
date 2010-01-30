using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Scallywags{
    public class AI:BasePlayer{

        Queue<ActionStates> m_ToDO;//all the decisions that the AI makes are stored here to be retrieved at the correct time
        public float currentDelay;//the delay before the AI does each of the actions
        public int actionStatus;//the current state of the action
        HeatMeter heatStats = new HeatMeter();//the accumulated heat of all the objects in the game
        Player[] m_playerList;
        ActionStates OverideAction;
        Town m_town;
        public Random joe;// = new Random((int)(fTime * 1000));
        //Point[] wayPoints;
        //Vector2[] wayPoints;

        //gets the current action that the AI is performing
        public ActionStates currentAction {
            get {
                if (m_ToDO.Count == 0) {
                    return ActionStates.None;
                }
                else{
                    if (OverideAction.Action == ActionStates.None.Action) {
                        return m_ToDO.Peek();
                    }
                    else {
                        return OverideAction;
                    }
                }
            }
        }

        /*  @fn setGeneralHeatLevel
         *  @brief This will set the overall Agressivnes of the AI player.
         */
        public void setGeneralHeatLevel(float heatLevel) {

            for (int i = 0; i < 4; i++) {
                //if (i != playerNumber) {
                    heatStats.townHeat[i] = heatLevel;
                //}
                //else {
                //    heatStats.coveHeat[i] = -1;//-1 means that the AI will never attack this port
                //}
            }
        }
        /*  @fn setGeneralHeatLevel
         *  @brief This will set the overall Agressivnes of the AI player. This function is defaulted to 20
         */
        public void setGeneralHeatLevel(){
            float heatLevel = 20.0f;
            for (int i = 0; i < 4; i++)
            {
                //if (i != playerNumber)
                //{
                    heatStats.townHeat[i] = heatLevel;
                //}
                //else
                //{
                    //heatStats.coveHeat[i] = -1;//-1 means that the AI will never attack this port
                //}
            }
        }

        /*  @fn AI
         *  @brief the constructor
         */
        public AI(World theWorld, Player[] playerList,Town town) {
            m_ToDO = new Queue<ActionStates>();
            currentDelay = 0.0f;
            actionStatus = 0;
            heatStats = new HeatMeter();
            m_playerList = playerList;
            m_town = town;
        }

        public void affectCoveHeat(int coveIndex, int newCoinCount){
            if (coveIndex != m_playerNum) {
                heatStats.coveHeat[coveIndex] = (float)newCoinCount;
            }
        }
        public void affectShipHeat(int player, int shipIndex, int variance) {
            if (player != m_playerNum) {
                heatStats.playerHeat[player, shipIndex] = Math.Min(Math.Max(0, variance),1);//never less than 0 or greater than 1
            }
        }
        public void affectTownHeat(int townIndex, int coins) {
            heatStats.townHeat[townIndex] = (float)coins;
        }

        /*  @fn AIBeginTurn
         *  @brief This is the general initialization that the AI needs to do at the beginning of it's turn.
         */

        public void AIBeginTurn(float fTime) {

            currentDelay = 0.0f;
            decideAI(fTime,true);
        }

        /*  @fn checkIA
         *  @brief rechecks the AI actions to make sure nothing has changed.
         */
        public void checkAI() {
            if (!m_isAI) {
                return;
            }
            if (OverideAction.Action != ActionStates.None.Action) {
                if (CurrentShip.CurrentLocation2D.Length() > 75) {
                    OverideAction = ActionStates.None;
                }
            }

            if (currentAction.Action == ActionStates.Events.MoveTo) {//the ship is currently moving to a new location
                ActionStates newAction = new ActionStates(true);

                //check to see if the ship is inside one of the ports
                if ( CurrentShip.CurrentLocation2D.Length() < 75 && (Vector2.Distance( currentAction.MoveTo, CurrentShip.CurrentLocation2D ) > 50 || currentAction.MoveTo.Length() > 75 )) {
                    
                    if ((Math.Abs(currentAction.MoveTo.X) == 105 && currentAction.MoveTo.Y == 0) || (currentAction.MoveTo.X == 0 && Math.Abs(currentAction.MoveTo.Y) == 105)){

                    }
                    else{
                        if (currentAction.MoveTo != m_homeLoc) {//ship is not going home
                            if (CurrentShip.hasBooty) {//if it has picked up a coin recently the go home instead.
                                m_ToDO.Clear();
                                newAction = new ActionStates(true);
                                newAction.setMoveTo(m_homeLoc);
                                m_ToDO.Enqueue(newAction);
                                newAction = ActionStates.ReEvaluate;
                                m_ToDO.Enqueue(newAction);
                                OverideAction = ActionStates.None;
                            }
                        }

                        //Stack<ActionStates> tempStack = new Stack<ActionStates>(m_ToDO.ToArray());
                        //m_ToDO.Reverse();
                        //tempStack.Push(newAction);
                        //m_ToDO = new Queue<ActionStates>(tempStack.ToArray());
                        //m_ToDO.Enqueue(newAction);
                        //m_ToDO.Reverse();
                        //OverideAction = newAction;
                    }
                    

                    Vector2 newLocation = currentAction.MoveTo;

                    Vector2 newPortVec = new Vector2(0, 75);//one port outer circle area.
                    newPortVec -= CurrentShip.CurrentLocation2D;
                    if (newPortVec.Length() < 30) {//the player is in the safe area of this port.
                        newLocation = new Vector2(0, 105);
                        newAction.setMoveTo(newLocation);
                        OverideAction = newAction;
                    }
                    newPortVec = new Vector2(0, -75);//one port outer circle area.
                    newPortVec -= CurrentShip.CurrentLocation2D;
                    if (newPortVec.Length() < 30) {//the player is in the safe area of this port.
                        newLocation = new Vector2(0, -105);
                        newAction.setMoveTo(newLocation);
                        OverideAction = newAction;
                    }
                    newPortVec = new Vector2(75, 0);//one port outer circle area.
                    newPortVec -= CurrentShip.CurrentLocation2D;
                    if (newPortVec.Length() < 30) {//the player is in the safe area of this port.
                        newLocation = new Vector2(105, 0);
                        newAction.setMoveTo(newLocation);
                        OverideAction = newAction;
                    }
                    newPortVec = new Vector2(-75, 0);//one port outer circle area.
                    newPortVec -= CurrentShip.CurrentLocation2D;
                    if (newPortVec.Length() < 30) {//the player is in the safe area of this port.
                        newLocation = new Vector2(-105, 0);
                        newAction.setMoveTo(newLocation);
                        OverideAction = newAction;
                    }

                }
                

                //make sure that the AI is not currently trying to avoid a collision issue.
                if (OverideAction.Action != ActionStates.None.Action){ //(Math.Abs(currentAction.MoveTo.X) == 105 && currentAction.MoveTo.Y == 0) || (currentAction.MoveTo.X == 0 && Math.Abs(currentAction.MoveTo.Y) == 105)) {

                }
                else {
                    if (currentAction.MoveTo != m_homeLoc) {//ship is not going home
                        if (CurrentShip.hasBooty) {//if it has picked up a coin recently the go home instead.
                            m_ToDO.Clear();
                            newAction = new ActionStates(true);
                            newAction.setMoveTo(m_homeLoc);
                            m_ToDO.Enqueue(newAction);
                            newAction = ActionStates.ReEvaluate;
                            m_ToDO.Enqueue(newAction);
                            OverideAction = ActionStates.None;
                        }
                    }
                }
            }

        }

        /*  @fn fireAtTarget
         *  @brief Returns true if the AI wants to aim at the targetted ship
         *  @return true to aim towards false to aim away.
         */
        public bool fireAtTarget(int attacker, int target, int shipIndex) {

            int Number = joe.Next(100);
            if (Number < 50)
            {
                return true;
            }
            return false;
            /*
            //if this player is the attacker then attack
            if (attacker == m_playerNum) {
                return true;
            }
            else {
                //if teh player being targeted is you then aim away.
                if (target == m_playerNum) {
                    return false;
                }

                //if the target has more gold then you.
                if (m_totalGold < heatStats.coveHeat[target]) {
                    //if the attacker is on the other side from you
                    if (attacker % 2 == m_playerNum % 2) {
                        return false;
                    }
                    return true;
                }
                else {
                    //attack him unless that attacker has more gold then the target.
                    if (heatStats.coveHeat[attacker] < heatStats.coveHeat[target]) {
                        return true;
                    }
                    return false;
                }
            }
            */
        }

        public void decideAI(float fTime)
        {
            decideAI(fTime, false);
        }
        public void decideAI(float fTime, bool first) {

            m_ToDO.Clear();
            ActionStates tempState = new ActionStates(true);
            if (CurrentShip.CurrentLocation2D.Length() > 75) {//ship is outside the collision
                OverideAction = ActionStates.None;
            }

            /*
            tempState.setMoveTo(new Vector2(100,0));
            m_ToDO.Enqueue(tempState);
            tempState.setFireAt(new Vector2(100, 20));
            m_ToDO.Enqueue(tempState);*/

            int state = 0;
            int index1 = 0, index2= 0;
            float maxVal = 0;

            if(m_fMovePoints >= 75 && first){

                int nNext = joe.Next(3) % totalShips;

                while( m_playerShips[ nNext ].isDisabled )
                    nNext = joe.Next(3) % totalShips;

                tempState.setSwitch( nNext );
                //m_currentShip = joe.Next(3)%totalShips;
                m_ToDO.Enqueue(tempState);
                m_ToDO.Enqueue(ActionStates.ReEvaluate);
                return;
            }

            //calculate the heat of anything in the world
            HeatMeter tempHeatStats = new HeatMeter(heatStats);
            for (int i = 0; i < m_playerList.Length; i++) {
                //players
                Vector2 tempVec = new Vector2(m_playerList[i].Ships[0].Location.X,m_playerList[i].Ships[0].Location.Z);
                Vector2 tempVec2 = new Vector2(CurrentShip.Location.X,CurrentShip.Location.Z);
                //tempHeatStats.playerHeat[i, 0] *= 1 - Math.Abs((tempVec2 - tempVec).Length()) / (500.0f-100*m_totalAvailableShips);//-1*distace/500;
                //tempHeatStats.playerHeat[i, 0] *= Math.Abs(((424 - (tempVec2 - tempVec).Length()) / 848));
                tempHeatStats.playerHeat[i, 0] = (848 - Vector2.Distance(tempVec,tempVec2))*Math.Min(1,tempHeatStats.playerHeat[i, 0]);
                if ((tempVec2 - tempVec).Length() > m_fMovePoints*2.14) {
                    tempHeatStats.playerHeat[i, 0] *= 0.75f;
                }

                if (m_playerList[i].Ships[1] != null) {
                    tempVec = new Vector2(m_playerList[i].Ships[1].Location.X, m_playerList[i].Ships[1].Location.Z);
                    tempVec2 = new Vector2(CurrentShip.Location.X, CurrentShip.Location.Z);
                    //tempHeatStats.playerHeat[i, 1] *= 1 - Math.Abs((tempVec2 - tempVec).Length()) / (500.0f - 100 * m_totalAvailableShips);//-1*distace/500;
                    //tempHeatStats.playerHeat[i, 1] *= Math.Abs(((424 - (tempVec2 - tempVec).Length()) / 848));
                    tempHeatStats.playerHeat[i, 1] = (848 - Vector2.Distance(tempVec, tempVec2)) * Math.Min(1,tempHeatStats.playerHeat[i, 1]);
                    if ((tempVec2 - tempVec).Length() > m_fMovePoints * 2.14) {
                        tempHeatStats.playerHeat[i, 1] *= 0.75f;
                    }
                }
                if (m_playerList[i].Ships[2] != null) {
                    tempVec = new Vector2(m_playerList[i].Ships[2].Location.X, m_playerList[i].Ships[2].Location.Z);
                    tempVec2 = new Vector2(CurrentShip.Location.X, CurrentShip.Location.Z);
                    //tempHeatStats.playerHeat[i, 2] *= 1 - Math.Abs((tempVec2 - tempVec).Length()) / (500.0f - 100 * m_totalAvailableShips);//-1*distace/500;
                    //tempHeatStats.playerHeat[i, 2] *= Math.Abs(((424 - (tempVec2 - tempVec).Length()) / 848));
                    tempHeatStats.playerHeat[i, 2] = (848 - Vector2.Distance(tempVec, tempVec2)) * Math.Min(1,tempHeatStats.playerHeat[i, 2]);
                    if ((tempVec2 - tempVec).Length() > m_fMovePoints *2.14) {
                        tempHeatStats.playerHeat[i, 2] *= 0.75f;
                    }
                }
                //town
                tempVec = new Vector2(m_town.Ports[i].Location.X, m_town.Ports[i].Location.Z);
                tempVec2 = new Vector2(CurrentShip.Location.X, CurrentShip.Location.Z);
                //tempHeatStats.townHeat[i] *= 1 - Math.Abs((tempVec2 - tempVec).Length()) / (500.0f - 100 * m_totalAvailableShips);//-1*distace/500;
                //tempHeatStats.townHeat[i] *= Math.Abs(((424 - (tempVec2 - tempVec).Length()) / 848));
                tempHeatStats.townHeat[i] = (848 - Vector2.Distance(tempVec, tempVec2)) * Math.Min(tempHeatStats.townHeat[i],1);//the town heat is always one unless there are no coins left. This will promote more assault on other players before the towns run dry.
                if ((tempVec2 - tempVec).Length() > m_fMovePoints * 2.14) {
                    tempHeatStats.playerHeat[i, 0] *= 0.75f;
                }
                //pirate coves
                tempVec = new Vector2(m_playerList[i].Port.Location.X, m_playerList[i].Port.Location.Z);
                tempVec2 = new Vector2(CurrentShip.Location.X, CurrentShip.Location.Z);
                //tempHeatStats.townHeat[i] *= 1 - Math.Abs((tempVec2 - tempVec).Length()) / (500.0f - 100 * m_totalAvailableShips);//-1*distace/500;
                //tempHeatStats.coveHeat[i] *= Math.Abs(( (424-(tempVec2 - tempVec).Length()) / 848));
                tempHeatStats.coveHeat[i] = (848 - Vector2.Distance(tempVec, tempVec2)) * Math.Min(1,tempHeatStats.coveHeat[i]);
                if ((tempVec2 - tempVec).Length() > m_fMovePoints * 2.14) {
                    tempHeatStats.playerHeat[i, 0] *= 0.75f;
                }
            }


            //decide what to do
            for (int i = 0; i < 4; i++) {
                //player heat
                if ((maxVal = Math.Max(maxVal, tempHeatStats.playerHeat[i, 0])) == tempHeatStats.playerHeat[i, 0]) {
                    state = 0; index1 = i; index2 = 0;
                }
                if ((maxVal = Math.Max(maxVal, tempHeatStats.playerHeat[i, 1])) == tempHeatStats.playerHeat[i, 1]) {
                    state = 0; index1 = i; index2 = 1;
                }
                if ((maxVal = Math.Max(maxVal, tempHeatStats.playerHeat[i, 2])) == tempHeatStats.playerHeat[i, 2]) {
                    state = 0; index1 = i; index2 = 2;
                }
                //town
                if ((maxVal = Math.Max(maxVal, tempHeatStats.townHeat[i])) == tempHeatStats.townHeat[i]) {
                    state = 1; index1 = i; index2 = -1;
                }
                //cove
                if ((maxVal = Math.Max(maxVal, tempHeatStats.coveHeat[i])) == tempHeatStats.coveHeat[i]) {
                    state = 2; index1 = i; index2 = -1;
                }
            }

            Vector2 targetLoc;
            Vector2 yourLoc;

            //check for special events
            if (m_playerShips[0].hasBooty) {
                m_ToDO.Clear();
                tempState.setSwitch(0);
                m_ToDO.Enqueue(tempState);
                tempState = new ActionStates();
                tempState.setMoveTo(m_homeLoc);
                m_ToDO.Enqueue(tempState);
                m_ToDO.Enqueue(ActionStates.ReEvaluate);
                return;
            }
            else if(m_playerShips[1] != null){
                if (m_playerShips[1].hasBooty) {
                    m_ToDO.Clear();
                    tempState.setSwitch(1);
                    m_ToDO.Enqueue(tempState);
                    tempState = new ActionStates();
                    tempState.setMoveTo(m_homeLoc);
                    m_ToDO.Enqueue(tempState);
                    m_ToDO.Enqueue(ActionStates.ReEvaluate);
                    return;
                }
            }
            else if (m_playerShips[2] != null){
                if(m_playerShips[2].hasBooty) {
                    m_ToDO.Clear();
                    tempState.setSwitch(2);
                    m_ToDO.Enqueue(tempState);
                    tempState = new ActionStates();
                    tempState.setMoveTo(m_homeLoc);
                    m_ToDO.Enqueue(tempState);
                    m_ToDO.Enqueue(ActionStates.ReEvaluate);
                    return;
                }
            }

            if (m_playerList[m_playerNum].Port.Coins > 0
                && m_playerList[m_playerNum].m_totalAvailableShips < 3)
            {
                tempState.setPurchase();
                m_ToDO.Enqueue(tempState);
                //tempState = new ActionStates();
                //tempState.setSwitch(1);
                //m_ToDO.Enqueue(tempState);
                m_ToDO.Enqueue(ActionStates.ReEvaluate);
                return;
            }

            for (int i = 0; i < m_world.Coins.Count; i++) {
                if (Vector2.Distance(CurrentShip.CurrentLocation2D, new Vector2(m_world.Coins[i].Location.X, m_world.Coins[i].Location.Z)) < 150) {
                    m_ToDO.Clear();
                    tempState = new ActionStates(true);
                    tempState.setMoveTo(new Vector2(m_world.Coins[i].Location.X , m_world.Coins[i].Location.Z));
                    m_ToDO.Enqueue(tempState);
                    tempState = new ActionStates(true);
                    tempState.setMoveTo(m_homeLoc);
                    m_ToDO.Enqueue(tempState);
                    m_ToDO.Enqueue(ActionStates.ReEvaluate);
                    return;
                }
            }

            //queue up the events required to perform the actions.
            switch(state){
                case 0://other player has highest heat level
                    //Move the ship nearby
                    targetLoc = new Vector2(m_playerList[index1].Ships[index2].Location.X,m_playerList[index1].Ships[index2].Location.Z);
                    yourLoc = new Vector2(CurrentShip.Location.X, CurrentShip.Location.Z);
                    float distance = Vector2.Distance(targetLoc,yourLoc);   //checked with Tom: 75.0f is the default starting range.
                    if (Math.Abs(distance) > 75) {//if the AI if farther than 75 units then
                        if (distance >= 75.0f) {
                            distance -= 75.0f;
                        }
                        if (distance < -75.0f) {
                            distance += 75.0f;
                        }
                        Vector2 oldLoc = targetLoc;
                        targetLoc = (targetLoc - yourLoc); targetLoc.Normalize();
                        targetLoc = yourLoc + (targetLoc * distance);
                        
                        if (targetLoc.Length() < 75)//The Target is inside the Circle
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                targetLoc.Normalize();
                                targetLoc *= 75;
                                targetLoc = targetLoc - oldLoc; targetLoc.Normalize();
                                targetLoc *= 75;
                                targetLoc += oldLoc;
                            }
                            targetLoc.Normalize();
                            targetLoc *= 80;
                        }


                        tempState.setMoveTo(targetLoc);
                        m_ToDO.Enqueue(tempState);
                    }

                    if (CurrentShip.HasCannon)
                    {
                        //fire at him
                        tempState = new ActionStates(true);
                        tempState.setFireAt(new Vector2(m_playerList[index1].Ships[index2].Location.X, m_playerList[index1].Ships[index2].Location.Z));
                        m_ToDO.Enqueue(tempState);
                    }
                    else
                    {
                        if(m_playerShips[0].HasCannon && !m_playerShips[0].isDisabled){
                            m_ToDO.Clear();
                            tempState = new ActionStates(true);
                            tempState.setSwitch(0);
                            m_ToDO.Enqueue(tempState);
                            tempState = new ActionStates(true);
                            tempState.setFireAt(new Vector2(m_playerList[index1].Ships[index2].Location.X, m_playerList[index1].Ships[index2].Location.Z));
                            m_ToDO.Enqueue(tempState);
                            m_ToDO.Enqueue(ActionStates.ReEvaluate);
                            return;
                        }
                        if(m_playerShips[1] != null){
                            if (m_playerShips[1].HasCannon && !m_playerShips[1].isDisabled)
                            {
                                m_ToDO.Clear();
                                tempState = new ActionStates(true);
                                tempState.setSwitch(1);
                                m_ToDO.Enqueue(tempState);
                                tempState = new ActionStates(true);
                                tempState.setFireAt(new Vector2(m_playerList[index1].Ships[index2].Location.X, m_playerList[index1].Ships[index2].Location.Z));
                                m_ToDO.Enqueue(tempState);
                                m_ToDO.Enqueue(ActionStates.ReEvaluate);
                                return;
                            }
                        }
                        else{
                            m_ToDO.Clear();
                            tempState = new ActionStates(true);
                            tempState.setMoveTo(m_homeLoc);
                            m_ToDO.Enqueue(tempState);
                            m_ToDO.Enqueue(ActionStates.ReEvaluate);
                        }
                        if(m_playerShips[2] != null){
                            if (m_playerShips[2].HasCannon && !m_playerShips[2].isDisabled)
                            {
                                m_ToDO.Clear();
                                tempState = new ActionStates(true);
                                tempState.setSwitch(2);
                                m_ToDO.Enqueue(tempState);
                                tempState = new ActionStates(true);
                                tempState.setFireAt(new Vector2(m_playerList[index1].Ships[index2].Location.X, m_playerList[index1].Ships[index2].Location.Z));
                                m_ToDO.Enqueue(tempState);
                                m_ToDO.Enqueue(ActionStates.ReEvaluate);
                                return;
                            }
                            else{
                                m_ToDO.Clear();
                                tempState = new ActionStates(true);
                                tempState.setMoveTo(m_homeLoc);
                                m_ToDO.Enqueue(tempState);
                                m_ToDO.Enqueue(ActionStates.ReEvaluate);
                            }
                        }
                        else
                        {
                            m_ToDO.Clear();
                            tempState = new ActionStates(true);
                            tempState.setMoveTo(m_homeLoc);
                            m_ToDO.Enqueue(tempState);
                            m_ToDO.Enqueue(ActionStates.ReEvaluate);
                        }

                    }
                    //Re Evaluate the situation
                    m_ToDO.Enqueue(ActionStates.ReEvaluate);
                    break;
                case 1://Town has highest heat level
                    //REMEMBER THE TOWERS!!!
                    //remembering the towers...
                    if (m_town.Towers[index1].IsAlive && CurrentShip.HasCannon)//if the town's tower is alive, the AI needs to shoot it
                    {
                        //Vector2 prevTargetLoc = targetLoc;//stores the previous location in a temporary variable
                        targetLoc = new Vector2(m_town.Towers[index1].Location.X, m_town.Towers[index1].Location.Z);//targets the same tower the port is assigned to
                        yourLoc = new Vector2(CurrentShip.Location.X, CurrentShip.Location.Z);
                        float theDistance = Vector2.Distance(targetLoc, yourLoc);//same as code thrown in above
                        if (Math.Abs(theDistance) > 75) {//if the AI is farther than 75 then move to within 75 units.
                            if (theDistance >= 75.0f) {
                                theDistance -= 75.0f;
                            }
                            if (theDistance < -75.0f) {
                                theDistance += 75.0f;
                            }
                            Vector2 oldLoc = targetLoc;
                            targetLoc = (targetLoc-yourLoc); targetLoc.Normalize();
                            targetLoc = yourLoc + (targetLoc * theDistance);
                            if (targetLoc.Length() < 75)//The Target is inside the Circle
                            {
                                for (int i = 0; i < 10; i++)
                                {
                                    targetLoc.Normalize();
                                    targetLoc *= 75;
                                    targetLoc = targetLoc - oldLoc; targetLoc.Normalize();
                                    targetLoc *= 75;
                                    targetLoc += oldLoc;
                                }
                                targetLoc.Normalize();
                                targetLoc *= 80;
                            }

                            tempState.setMoveTo(targetLoc);
                            m_ToDO.Enqueue(tempState);

                        }
                        

                        if (CurrentShip.HasCannon)
                        {
                            //fire at him
                            tempState = new ActionStates(true);
                            tempState.setFireAt(new Vector2(m_town.Towers[index1].Location.X, m_town.Towers[index1].Location.Z));
                            m_ToDO.Enqueue(tempState);
                        }
                        else
                        {
                            m_ToDO.Enqueue(ActionStates.ReEvaluate);
                        }

                        //Re Evaluate the situation
                        m_ToDO.Enqueue(ActionStates.ReEvaluate);
                        return;
                    }
                    //move to the town port
                    targetLoc = new Vector2(m_town.Ports[index1].Location.X, m_town.Ports[index1].Location.Z);
                    tempState = new ActionStates(true);
                    tempState.setMoveTo(targetLoc);
                    m_ToDO.Enqueue(tempState);

                    //targetLoc = prevTargetLoc;//restores the previous location

                    //pick up the coin
                    //m_ToDO.Enqueue(ActionStates.PickUp); --not used anymore. Automatic Pickup.

                    //Return home
                    tempState = new ActionStates(true);
                    tempState.setMoveTo(new Vector2(this.Port.Location.X, this.Port.Location.Z));
                    m_ToDO.Enqueue(tempState);

                    break;
                case 2://player cove has highest heat level
                    //move to the players port
                    targetLoc = new Vector2(m_playerList[index1].Port.Location.X, m_playerList[index1].Port.Location.Z);
                    tempState.setMoveTo(targetLoc);
                    m_ToDO.Enqueue(tempState);

                    //take a coin
                    //--m_ToDO.Enqueue(ActionStates.PickUp); --not used any more. Automatic pickup

                    //Head back home
                    tempState = new ActionStates(true);
                    tempState.setMoveTo(new Vector2(this.Port.Location.X, this.Port.Location.Z));
                    m_ToDO.Enqueue(tempState);

                    break;
            }
            
            m_ToDO.Enqueue(ActionStates.EndOfTurn);
        }
        public bool doneAction() {
            currentDelay = 0.0f;
            actionStatus = 0;
            if (OverideAction.Action == ActionStates.None.Action) {
                if( m_ToDO.Count > 0 )
                {
                    m_ToDO.Dequeue();
                    if (currentAction.Action == ActionStates.Events.EndTurn) {
                        return true;
                    }
                }
                return false;
            }
            else {
                OverideAction = ActionStates.None;
                return false;
            }
        }


    }//end class





#region ActionStates
    public struct ActionStates {
        Events actionEvent;
        Vector2 moveToPoint; public Vector2 MoveTo { get { return moveToPoint; } }
        Vector2 fireAtPoint; public Vector2 FireAt { get { return fireAtPoint; } }

        int switchToShip; public int SwitchShip { get { return switchToShip; } }

        public static ActionStates EndOfTurn {
            get {
                ActionStates temp = new ActionStates(true);
                temp.actionEvent = Events.EndTurn;
                return temp;
            }
        }
        public static ActionStates ReEvaluate {
            get{
                ActionStates temp = new ActionStates(true);
                temp.actionEvent = Events.ReEvaluate;
                return temp;
            }
        }
        public static ActionStates PickUp {
            get{
                ActionStates temp = new ActionStates(true);
                temp.actionEvent = Events.PickUp;
                return temp;
            }
        }
        public static ActionStates PurchaseShip {
            get {
                ActionStates temp = new ActionStates(true);
                temp.actionEvent = Events.Purchase;
                return temp;
            }
        }
        public static ActionStates None {
            get{
                ActionStates temp = new ActionStates(true);
                temp.actionEvent = Events.None;
                return temp;
            }
        }


        public Events Action {
            get {
                return actionEvent;
            }
        }

        public ActionStates(bool cSharpIsUseless) {
            actionEvent = Events.EndTurn;

            moveToPoint = Vector2.Zero;
            fireAtPoint = Vector2.Zero;
            switchToShip = -1;
        }

        public void setFireAt(Vector2 firePos) {
            actionEvent = Events.FireAt;
            fireAtPoint = firePos;
        }
        public void setMoveTo(Vector2 movePos) {
            actionEvent = Events.MoveTo;
            moveToPoint = movePos;
        }
        public void setSwitch(int shipNum) {
            actionEvent = Events.Switch;
            switchToShip = shipNum;
        }
        public void setPurchase() {
            actionEvent = Events.Purchase;
        }
        
        public enum Events {
            None = 0,
            MoveTo,
            FireAt,
            PickUp,
            Purchase,
            Switch,
            ReEvaluate,
            EndTurn
        }
    
    }
#endregion

#region HeatMechanics
    public class HeatMeter {
        public float[] coveHeat = new float[4];
        public float[] townHeat = new float[4];
        public float[/*player*/,/*Boat*/] playerHeat = new float [4,3];

        //Rage is a temporary stat that is not affected by distance,
        //but diminishes over time.
        //For example when a player shoots at this player their,
        //temporary rage goes up eventualy the AI will go after
        //this player regardless of the distance because of the rage.
        //public float[] coveRage = new float[4];
        //public float[] townRage = new float[4];
        //public float[,] playerRage = new float[4, 3];
        public HeatMeter() {
        }
        public HeatMeter(HeatMeter toCopyHeat) {
            toCopyHeat.coveHeat.CopyTo(coveHeat,0);
            
            //toCopyHeat.playerHeat.CopyTo(playerHeat, 0);

            for (int i = 0; i < 3; i++) {
                playerHeat[0, i] = toCopyHeat.playerHeat[0, i];
                playerHeat[1, i] = toCopyHeat.playerHeat[1, i];
                playerHeat[2, i] = toCopyHeat.playerHeat[2, i];
                playerHeat[3, i] = toCopyHeat.playerHeat[3, i];
            }

            toCopyHeat.townHeat.CopyTo(townHeat, 0);
        }

    }
#endregion

}
