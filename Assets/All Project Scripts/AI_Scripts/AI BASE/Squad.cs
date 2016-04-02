using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/*
 		SAVE THIS FOR LAST, LETS FIRST GET OUR INDIVIDUAL UNITS WORKING
		This Script will function as a 2nd layer of AI ---> first layer is the units individual AI, 
		The Squad a unit belongs to will provide a 2nd layer, so lets first get each individual unit AI working first

		Ideally though and to keep it simple this class should just do 2 things:
				i) Places all units in a certain formation, by using anchor positions.
				ii) Moves the squad:  the leader's NavMeshTarget [in its script] is set to the squads target.
                                      the rest have their NavMeshTarget set to their anchor position within the formation
				---> When the squad gets to its target, each individual Unit AI should take over, we can still impose the formation restrictions but again this will be done at the end
 */


public class Squad : MonoBehaviour 
{
	public List<Unit_Base> units;   // All units in this squad
	public int numOfMelee = 0;
	public int numOfRange = 0;
	public int numOfSiege = 0;
}
