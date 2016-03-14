using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

using Random = UnityEngine.Random;
using Coroutine = System.Collections.IEnumerator;
using BTCoroutine = System.Collections.Generic.IEnumerator<BTNodeResult>;

public class AI_Mutant : MonoBehaviour 
{
	private BehaviorTree bt;
	
	void Awake () 
	{
		InitBT();
		bt.Start();
	}
	
	void Update () 
	{
		
	}
	
	private void InitBT()
	{
		bt = new BehaviorTree(Application.dataPath + "/Behaviour_Mutant.xml", this);
	}
}



/*
	// CONDITION TEMPLATE
	[BTLeaf(" ")]
	public bool exampleFunction()
	{
		return true;
	}

	
	// LEAF TEMPLATE
	[BTLeaf(" ")]
	public BTCoroutine exampleFunction()
	{
		yield return BTNodeResult.Success;			
		yield return BTNodeResult.NotFinished;
		yield return BTNodeResult.Failure;
	}
 */

