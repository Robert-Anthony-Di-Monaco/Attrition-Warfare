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




	public void getNewAnchorPosition(int unitIndex){

		if (squad.leader.ID == this.ID)
			offsetFromAnchor = Vector3.zero;

		int numMelee = squad.meleeUnits.Count;
		
		//Melees stand in a line behind the leader 
		float zOffset = -1.5f * squad.distanceBetweenUnits;

		//Lines up the units side by side
		float xOffset = ((float)unitIndex / numMelee) * (squad.distanceBetweenUnits * numMelee);
		xOffset -= (squad.distanceBetweenUnits / 2) * (numMelee - 1);

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

