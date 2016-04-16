using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

using Random = UnityEngine.Random;
using Coroutine = System.Collections.IEnumerator;
using BTCoroutine = System.Collections.Generic.IEnumerator<BTNodeResult>;


public class Unit_Siege : Unit_Base
{
   // public BehaviorTree bt;
    public override void Awake()
    {
        attackRange = 400;
        attackCooldown = 3;
        damageOutput = 104;
        visionRange = 250f;
        isInCombat = false;
        base.Awake();
        
    }

	public override void ApplyDamage(int amount)
	{
		health -= amount/4;
		if (health <= 0)
		{
			StartCoroutine("Kill");		
		}
	}

	public void getNewAnchorPosition(int unitIndex, int numMeleeLines, int numRangeLines){

		int numSiege = squad.siegeUnits.Count;
		int perLine = squad.numUnitsPerLine;
		float unitDistance = squad.distanceBetweenUnits;

		int inFirstLine = numSiege % perLine;
		if (inFirstLine == 0)
			inFirstLine = perLine;

		int line = unitIndex / inFirstLine;
		if(line != 0)
			line = ( (unitIndex - inFirstLine) / perLine) + 1;

		//Siege units stand in a line behind the range units by a distance of (1.5 * unitDistance)
		float zOffset = 0;
		zOffset += ((-1.5f * unitDistance) - ((numMeleeLines - 1) * unitDistance)); //Get the offset of the last melee unit line
		zOffset += ((-1.5f * unitDistance) - ((numRangeLines - 1) * unitDistance)); //Add the offset of the last range unit line
		zOffset += ((-1.5f * unitDistance) - (line * unitDistance)); //Add the offset for this range unit's line

		//Lines up the unit side by side (comments in Unit_Melee)
		float xOffset;
		if (line == 0) {
			xOffset = ((float)unitIndex / inFirstLine) * (unitDistance * inFirstLine);
			xOffset -= (unitDistance / 2) * (inFirstLine - 1);
		} else {
			xOffset = ((float)(unitIndex % perLine) / perLine) * (unitDistance * perLine);
			xOffset -= (unitDistance / 2) * (perLine - 1);
		}

		offsetFromAnchor = new Vector3 (xOffset, 0, zOffset);

	}
	//Overloads the function in Unit_Base
	public override bool isSiege(){
		return true;
	} 
}
