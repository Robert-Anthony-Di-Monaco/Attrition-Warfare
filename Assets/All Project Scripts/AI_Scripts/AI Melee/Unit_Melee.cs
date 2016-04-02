﻿using UnityEngine;
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
	private BehaviorTree bt;
	
	void Awake () 
	{
		InitBT();
		bt.Start();
	}
	private void InitBT()
	{
		bt = new BehaviorTree(Application.dataPath + "/Melee-AI-Tree.xml", this);
	}


	// LEAFS and CONDITIONS definitions ---> SEE TEMPLATES BELOW FOR HOW TO DO THEM!!!!!!!!
	[BTLeaf("has-target")]
	public bool hasTarget()   // Does this unit have a target
	{
		if(NavMeshTarget != null)
			return true;
		else 
			return false;
	}
	[BTLeaf("seek-target")]
	public BTCoroutine Seek()   // Seek target
	{
		NavMeshSeek ();

		if (Vector3.Distance (NavMeshTarget.position, transform.position) < attackRange)
			yield return BTNodeResult.Success;
		else
			yield return BTNodeResult.NotFinished;
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

