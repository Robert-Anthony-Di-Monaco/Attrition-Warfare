using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

using Random = UnityEngine.Random;
using Coroutine = System.Collections.IEnumerator;
using BTCoroutine = System.Collections.Generic.IEnumerator<BTNodeResult>;

public class Unit_Melee : Unit_Base
{
	public override void Awake () 
	{
        attackRange = 30f;
        attackCooldown = 0.75f;
        damageOutput = 32;
        visionRange = 150f;
        isInCombat = false;
		base.Awake ();
	}


	public override void ApplyDamage(int amount)
	{
		health -= amount/2;
		if (health <= 0)
		{
            StartCoroutine("Kill");
        }
	}

	public void getNewAnchorPosition(int unitIndex){
		
		int numMelee = squad.meleeUnits.Count;
		int perLine = squad.numUnitsPerLine;
		float unitDistance = squad.distanceBetweenUnits;

		int inFirstLine = numMelee % perLine;
		if (inFirstLine == 0)
			inFirstLine = perLine;

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