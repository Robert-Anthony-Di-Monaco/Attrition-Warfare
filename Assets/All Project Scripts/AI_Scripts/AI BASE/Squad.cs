using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Just add a unit to the squad using addUnit(), this script does the rest
//When a unit dies, make sure it is removed from its squad using removeUnit()
public class Squad : MonoBehaviour 
{
	
	public List<Unit_Base> allUnits = new List<Unit_Base>();   // All units in this squad

	public List<Unit_Melee> meleeUnits = new List<Unit_Melee>();
	public List<Unit_Range> rangeUnits = new List<Unit_Range>();
	public List<Unit_Siege> siegeUnits = new List<Unit_Siege>();

	public Unit_Base leader;

	public float distanceBetweenUnits = 5f;

	private string meleeTag = "Melee";
	private string rangeTag = "Range";
	private string siegeTag = "Siege";

	void Start(){
		
	}

	void FixedUpdate(){
		
		//Keep the squad's anchor position at the leader
		if(leader != null)
			transform.position = leader.transform.position;
	}



	public void addUnit(Unit_Base unit){

		if (unit.squad != null) {
			Debug.Log ("Can't add unit to squad: Unit is already in a squad.");
			return;
		}

		if (unit.gameObject.tag == meleeTag)
			meleeUnits.Add ((Unit_Melee)unit);
		
		else if (unit.gameObject.tag == rangeTag)
			rangeUnits.Add ((Unit_Range)unit);
		
		else if (unit.gameObject.tag == siegeTag)
			siegeUnits.Add ((Unit_Siege)unit);
		
		else {
			Debug.Log ("This unit cannot be added to a squad.");
			return;
		}
		allUnits.Add (unit);
		unit.squad = this;

		ChooseLeader ();
		CalculateNewAnchorPositions ();
			
	}
	//These are so you don't have to typecast to use the addUnit function
	public void addUnit(Unit_Melee unit){
		addUnit ((Unit_Base)unit);
	}
	public void addUnit(Unit_Range unit){
		addUnit ((Unit_Base)unit);
	}
	public void addUnit(Unit_Siege unit){
		addUnit ((Unit_Base)unit);
	}

	public void removeUnit (Unit_Base unit){
		
		if (!allUnits.Contains (unit)) {
			Debug.Log ("Unit can't be removed from squad: it is not in this squad.");
			return;
		}

		if (unit.ID == leader.ID) {
			leader.squad = null;
			leader = null;
			allUnits.Remove (unit);

			ChooseLeader ();
			CalculateNewAnchorPositions ();
		}
		else {
			if (unit.gameObject.tag == meleeTag) {
				unit.squad = null;
				meleeUnits.Remove ((Unit_Melee)unit);
				allUnits.Remove (unit);

				CalculateNewAnchorPositions ();
			} 
			else if (unit.gameObject.tag == rangeTag) {
				unit.squad = null;
				rangeUnits.Remove ((Unit_Range)unit);
				allUnits.Remove (unit);

				CalculateNewAnchorPositions ();
			} 
			else if (unit.gameObject.tag == siegeTag) {
				unit.squad = null;
				rangeUnits.Remove ((Unit_Range)unit);
			}
		}


	}


	//Used automatically when adding or removing units to the squad
	void ChooseLeader(){
		
		if (leader == null)
			leader = (Unit_Base)allUnits [0]; //allUnits[0] is never null when this function is called


		//Prioritize melee, then ranged, then siege units as leader
		//Changing the leader involves removing the chosen leader from the formation so there isn't an empty slot
		if (leader.gameObject.tag != meleeTag) {
			if (meleeUnits.Count > 0) {
				if (leader.gameObject.tag == rangeTag) {
					rangeUnits.Add ((Unit_Range)leader); //put the leader back into ranged unit formation
				} 
				else {
					siegeUnits.Add ((Unit_Siege)leader); //or put the leader back into siege unit formation
				}
				leader = (Unit_Base)meleeUnits [0];
			} 
			else if (leader.gameObject.tag != rangeTag && rangeUnits.Count > 0) { //if there are no melee units, look for ranged
				siegeUnits.Add ((Unit_Siege)leader); //put the leader back into siege unit formation
				leader = (Unit_Base)rangeUnits [0];
			}
		} 

		//Remove the leader from its type's list so it is ignored in the formation calculation
		//If the leader is already removed from the formation, nothing happens
		if (leader.gameObject.tag == meleeTag)
			meleeUnits.Remove ((Unit_Melee)leader);

		else if (leader.gameObject.tag == rangeTag)
			rangeUnits.Remove ((Unit_Range)leader);

		else
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
