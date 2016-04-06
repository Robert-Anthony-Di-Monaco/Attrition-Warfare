using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Just add a unit to the squad using addUnit(), this script does the rest
//When a unit dies, make sure it is removed from its squad using removeUnit()
public class Squad : MonoBehaviour 
{
	//Example prefabs (squad and unit types) and demonstration 
	//  in the SquadTest scene (need to be scaled to our main map)

	/*----------------------------------------------------------
	 * Using this script: Spawning and Unit behaviour trees
     *
     * -Create squad prefab, make sure unitNavmeshSpeed is set manually (necessary)
	 * -set the Squad's faction (0 is the default)
	 *        -Can use squadPrefab.GetComponent<Squad>().faction = 1 before instantiating, 
	 *        -  or make one squad prefab for each faction
	 * -Instantiate the squad object
	 * 
	 * When instantiating units:
	 * -Make Unit_Melee, Unit_Range, Unit_Siege prefabs, include a NavMeshAgent component
	 * -Set faction (same as above)
	 * -Instantiate a unit prefab, then use yourSquad.AddUnit(newUnit);
	 * 
	 * -Tweak NavMeshAgent's parameters to fit the level (speed, radius, and angular speed -> 360 seems good)
	 * 
	 * -use boolean function Unit_Base.isLeader()
	 * 	  --> Non-leader units: call Unit_Base's MarchInFormation() every FixedUpdate() (out of combat)
	 *    --> Leader: set yourSquad.leader.NavMeshTarget to a destination position, then call leader.NavMeshSeek();
	 * 
	 * -the WorldController object/script has a list allSquads, automatically updated
	 * -See Unit_Base's Kill() function for what to do when a unit dies
	 * 
	 * You don't really need to read anything else, just some of the notes on fields.
	 *
	 *--------------------------------------------------------------------
	 * Using this script: Player
	 * 
	 * -The two player functions in this script are commented out
	 * -Player_AI needs to inherit from Unit_Base, the squad's leader is a pointer to a Unit_Base type
	 *    --> Look over the fields inherited from Unit_Base to see if they conflict with your script
	 *	  --> override the virtual functions Awake() and isPlayer() (example in Unit_Melee)
	 * 
	 * -When this is done, remove comments the player functions below and from removeUnit
	 * -Use selectedSquad.giveControlTo(Player_AI player); to become the leader of a squad
	 * -Use player.squad.stopControlling() to stop leading the squad
	 *       (squad is inherited from Unit_Base)
	 * 
	 * -Everything else is automatic
	 * -the WorldController script has a list of all squads in the game
	 * -May have errors since I couldn't test, but I don't think so, let me know
	 * ------------------------------------------------------------------
	 */ 

	//IMPORTANT: squad movement speed
	//
	//Set this to the same value as the speed of the navmeshagent component for the units you are instantiating
	//Until we decide on a final world scale and navmesh speed, we have to do this
	public float unitNavmeshSpeed;

	//The leader should move slower than the rest of the squad (course slides say about half as fast),
	//  this way the squad can catch up to their slot positions when the leader is moving
	private float leaderSpeedDifference;

	//Every squad has a unique ID, see Awake()
	public int ID;

	//Determines which side the squad belongs to (friendly or enemy), use any int you want
	public int faction;

	//When any unit is triggered to enter combat, the whole squad enters combat
	//Behaviour trees should check thisUnit.squad.isInCombat, not thisUnit.isInCombat
	//Need to figure out a mechanism for exiting combat
	public bool isInCombat = false;

	//May be useful later if we want enemies to prioritize the player or not
	//Currently used to put the old leader into the formation when a squad is taken over by the player
	public bool isPlayerControlled = false;

	public List<Unit_Base> allUnits = new List<Unit_Base>();   // All units in this squad, including leader

	//Does not include the leader, used to generate the formation behind the leader
	public List<Unit_Melee> meleeUnits = new List<Unit_Melee>();
	public List<Unit_Range> rangeUnits = new List<Unit_Range>();
	public List<Unit_Siege> siegeUnits = new List<Unit_Siege>();

	public Unit_Base leader;

	//How far apart units stand when in formation
	public float distanceBetweenUnits = 7.5f;

	void Awake(){

		leaderSpeedDifference = unitNavmeshSpeed / 2;

		WorldController wc = GameObject.Find ("WorldController").GetComponent<WorldController>();

		//Uses the world controller to find a unique ID, and adds this squad to the allSquads list
		ID = wc.totalSquadsInstantiated;
		wc.totalSquadsInstantiated++;
		wc.allSquads.Add (this);
	}

	void FixedUpdate(){
		
		//The squad's anchor position follows the leader, without moving faster than units' navmesh speed
		//When the squad's leader is an AI, it will move at half speed
		//When the squad's leader is the player, the player can move at full speed without screwing up the formation
		if (leader != null)
		{
			if (leader.isPlayer ()) 
			{
				Vector3 Target = leader.transform.position;
				transform.position = (Target - transform.position).normalized * unitNavmeshSpeed * Time.deltaTime;
			} 
			else {
				transform.position = leader.transform.position;
			}

			transform.rotation = leader.transform.rotation;
		}

	}

	/* Player script does not inherit from Unit_Base yet
	 
	//To be called when the player wants to take control of a squad
	public void giveControlTo(Player_AI player){
		if(player.squad != null) {
			Debug.Log("Squad error: Player already controls a squad.");
			return;
		}
		player.squad = this;

		isPlayerControlled = true;
		allUnits.add((Unit_Base)player);

		ChooseLeader();
		CalculateNewAnchorPositions();
		
	}
	//When the player wants to stop controlling this squad
	//call player.squad.stopControlling();
	public void stopControlling(){
		//Make sure the Unit_Base virtual function isPlayer() is overloaded in the player script
		//(see example for isMelee() in Unit_Melee)
		if(!leader.isPlayer()){ 
			Debug.Log("Squad error: the player does not control this squad.");
			return;
		}

		player.squad = null;
		leader = null;
		isPlayerControlled = false;
		
		//Player will always be towards the end of the list, if not always at the end
		for(int i = allUnits.Count - 1; i >= 0; i++){
			if(allUnits[i].isPlayer()){
				allUnits.RemoveAt(i);
			}
		}

		ChooseLeader();
		CalculateNewAnchorPositions();
	}
	*/
	
	public void addUnit(Unit_Base unit){

		if (unit.squad != null) {
			Debug.Log ("Can't add unit to squad: Unit is already in a squad.");
			return;
		}

		allUnits.Add (unit);
		unit.squad = this;

		ChooseLeader ();
		CalculateNewAnchorPositions ();
			
	}
	//These are just for convenience, so you don't have to typecast to use the addUnit function
	//Also saves having to check which type of unit is being added
	public void addUnit(Unit_Melee unit){
		meleeUnits.Add ((Unit_Melee)unit);
		addUnit ((Unit_Base)unit);
	}
	public void addUnit(Unit_Range unit){
		rangeUnits.Add ((Unit_Range)unit);
		addUnit ((Unit_Base)unit);
	}
	public void addUnit(Unit_Siege unit){
		siegeUnits.Add ((Unit_Siege)unit);
		addUnit ((Unit_Base)unit);
	}

	public void removeUnit (Unit_Base unit){
		
		if (!allUnits.Contains (unit)) {
			Debug.Log ("Unit can't be removed from squad: it is not in this squad.");
			return;
		}

		if (unit.isPlayer ()) {
			//remove this comment when Player_AI inherits from Unit_Base
			//stopControlling ();
		}

		else if (unit.isSquadLeader()) 
		{
			leader.squad = null;
			leader = null;
			allUnits.Remove (unit);

			ChooseLeader ();
			CalculateNewAnchorPositions ();
		}
		else { //if it is a unit in the formation
			if (unit.isMelee()) 
			{
				unit.squad = null;
				meleeUnits.Remove ((Unit_Melee)unit);
				allUnits.Remove (unit);
			} 
			else if (unit.isRange()) 
			{
				unit.squad = null;
				rangeUnits.Remove ((Unit_Range)unit);
				allUnits.Remove (unit);
			} 
			else if (unit.isSiege()) 
			{
				unit.squad = null;
				siegeUnits.Remove ((Unit_Siege)unit);
				allUnits.Remove (unit);
			}

			CalculateNewAnchorPositions ();
		}

		//If all units have been removed, remove the squad from the game
		if (allUnits.Count == 0) {
			WorldController wc = GameObject.Find ("WorldController").GetComponent<WorldController>();
			wc.allSquads.Remove (this);
				
			Destroy (gameObject);
		}

	}
	//Removes a unit by its allUnits list index
	public void removeUnitAt(int allUnitsListIndex){
		removeUnit (allUnits [allUnitsListIndex]);
	}


	//Used automatically when adding or removing units to the squad
	void ChooseLeader(){
		
		if (leader == null)
		{
			leader = (Unit_Base)allUnits [0]; //allUnits[0] is never null when this function is called
			leader.agent.speed -= leaderSpeedDifference;
		}

		//Prioritize Player, then melee, then ranged, then siege units as leader
		//Changing the leader involves putting the old leader back into the formation
		//		the new leader is removed from the formation at the very end
		if (isPlayerControlled) {
			if (leader.isPlayer ())
				return;
			
			if (leader.isMelee ()) {
				leader.agent.speed += leaderSpeedDifference;
				meleeUnits.Add ((Unit_Melee)leader);
			} 
			else if (leader.isRange ()) {
				leader.agent.speed += leaderSpeedDifference;
				rangeUnits.Add ((Unit_Range)leader);
			} 
			else {
				leader.agent.speed += leaderSpeedDifference;
				siegeUnits.Add ((Unit_Siege)leader);
			}

			for(int i = allUnits.Count - 1; i >= 0; i++){ //Player will be towards the end of the list
				if(allUnits[i].isPlayer()){
					leader = allUnits[i];
				}
			}
			return;
		}

		if (!leader.isMelee()) {
			if (meleeUnits.Count > 0) {
				if (leader.isRange()) {
					leader.agent.speed += leaderSpeedDifference;
					rangeUnits.Add ((Unit_Range)leader); //put the leader back into ranged unit formation
				} 
				else if (leader.isSiege()){
					leader.agent.speed += leaderSpeedDifference;
					siegeUnits.Add ((Unit_Siege)leader); //or put the leader back into siege unit formation
				}
				leader = (Unit_Base)meleeUnits [0];
				leader.agent.speed -= leaderSpeedDifference;
			} 
			else if (!leader.isRange() && rangeUnits.Count > 0) { //if there are no melee units, look for ranged
				leader.agent.speed += leaderSpeedDifference;
				siegeUnits.Add ((Unit_Siege)leader); //put the leader back into siege unit formation

				leader = (Unit_Base)rangeUnits [0];
				leader.agent.speed -= leaderSpeedDifference;
			}
		} 

		//Remove the leader from its type's list so it is ignored in the formation calculation
		//If the leader is already removed from the formation, Remove returns false and nothing happens
		if (leader.isMelee())
			meleeUnits.Remove ((Unit_Melee)leader);

		else if (leader.isRange())
			rangeUnits.Remove ((Unit_Range)leader);

		else if (leader.isSiege())
			siegeUnits.Remove ((Unit_Siege)leader);

	}

	//Used automatically when adding or removing units to the squad
	void CalculateNewAnchorPositions(){

		leader.offsetFromAnchor = Vector3.zero;

		for (int i = 0; i < meleeUnits.Count; i++) {
			meleeUnits[i].getNewAnchorPosition (i);
		}

		for (int i = 0; i < rangeUnits.Count; i++) {
			rangeUnits[i].getNewAnchorPosition (i);
		}

		for (int i = 0; i < siegeUnits.Count; i++) {
			siegeUnits[i].getNewAnchorPosition (i);
		}

	}




}
