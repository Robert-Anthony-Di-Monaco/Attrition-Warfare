using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

using Random = UnityEngine.Random;
using Coroutine = System.Collections.IEnumerator;
using BTCoroutine = System.Collections.Generic.IEnumerator<BTNodeResult>;



/* 
	  Inherited Variables : See Unit_Base class

	  Inherited Functions:     NavMeshSeek()             
							   NavMeshStop()
							   ApplyDamage(amount)
							   Heal(int amount) 
*/ 


public class Unit_Melee : Unit_Base
{
	
	
	public override void Awake () 
	{
        attackRange = 15;
        attackCooldown = 2f;
        damageOutput = 8;
        visionRange = 100f;
        isInCombat = false;
		base.Awake ();

		
	}




	public void getNewAnchorPosition(int unitIndex, int numMeleeLines){
		
		int numMelee = squad.meleeUnits.Count;
		int perLine = squad.numUnitsPerLine;
		float unitDistance = squad.distanceBetweenUnits;

		int inFirstLine = numMelee % perLine;
		if (inFirstLine == 0)
			inFirstLine = perLine;
		
		int remaining = numMelee - inFirstLine; //number of units not in the first line

		int line = unitIndex / inFirstLine;
		if(line != 0)
			line = ( (unitIndex - inFirstLine) / perLine) + 1;
	
		//Melees stand in a line behind the leader by a distance of (1.5 * unitDistance) 
		float zOffset = (-1.5f * unitDistance) - (line * unitDistance);

		//Lines up the units side by side
		float xOffset;
		if (line == 0) { //the first line may have fewer units (it contains "inFirstLine" units)
			xOffset = ((float)unitIndex / inFirstLine) * (unitDistance * inFirstLine); //line up the units starting from the center and to the right
			xOffset -= (unitDistance / 2) * (inFirstLine - 1); //shift the units to the left so they are centered
		} else { //subsequent lines always have "perLine" units, same as above except for the number of units in this line
			xOffset = ((float)(unitIndex % perLine) / perLine) * (unitDistance * perLine);
			xOffset -= (unitDistance / 2) * (perLine - 1);
		}

		offsetFromAnchor = new Vector3 (xOffset, 0, zOffset);

	}

	//Overloads the function in Unit_Base
	public override bool isMelee(){
		return true;
	}


    


    
	
	
   
}





/*		TEMPLATES FOR	 MAKING THE AI ACTIONS 


	// CONDITION TEMPLATE  --> how to make a condition
	[BTLeaf(" this is the name the BehaviourTree sees, it is an Alias for this function ")]
	public bool exampleFunction()
	{
		return true;
	}


	// LEAF TEMPLATE  --> how to make an action
	[BTLeaf( this is the name the BehaviourTree sees, it is an Alias for this Coroutine ")]
	public BTCoroutine exampleCoroutine()
	{
		IF YOUVE NEVER USED COROUTINES --> dont worry, theyre awesome
		--> They are like functions except they are capable of pausing their execution between frames  
		--> THIS PAUSING IS DONE LIKE THIS: yield return new WaitForSeconds(3);  // Coroutine waits for 3 seconds before continuing to execute  

		// THE FOLLOWING ARE FOR THE BEHAVIOUR TREE ---> ASK ROBERT IF YOU DONT UNDERSTAND ---> dont feel stupid for asking, this wastes time, just ask me, we are all students here, its fine
		yield return BTNodeResult.Success;		   // Return this when Action is successful 
		yield return BTNodeResult.NotFinished;     // Return this when Action is still in progress
		yield return BTNodeResult.Failure;		   // Return this when Action failed
	}
 */

