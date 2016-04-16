using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

using Random = UnityEngine.Random;
using Coroutine = System.Collections.IEnumerator;
using BTCoroutine = System.Collections.Generic.IEnumerator<BTNodeResult>;

public class Unit_Range : Unit_Base
{

    //public BehaviorTree bt;
    public override void Awake()
    {
        attackRange = 125;
        attackCooldown = 0.4f;
        damageOutput = 24;
        visionRange = 200f;
        isInCombat = false;
        base.Awake();
        
    }
    
    public override void ApplyDamage(int amount)
    {
        health -= amount / 3;
        if (health <= 0)
        {
            StartCoroutine("Kill");
        }
    }

    public void getNewAnchorPosition(int unitIndex, int numMeleeLines)
    {
		int numRange = squad.rangeUnits.Count;
		int perLine = squad.numUnitsPerLine;
		float unitDistance = squad.distanceBetweenUnits;

		int inFirstLine = numRange % perLine;
		if (inFirstLine == 0)
			inFirstLine = perLine;

		int line = unitIndex / inFirstLine;
		if(line != 0)
			line = ( (unitIndex - inFirstLine) / perLine) + 1;

		//Ranged units stand in a line behind the melee units by a distance of (1.5 * unitDistance)
		float zOffset = 0;
		zOffset += ((-1.5f * unitDistance) - ((numMeleeLines - 1) * unitDistance)); //Get the offset of the last melee unit line
		zOffset += ((-1.5f * unitDistance) - (line * unitDistance)); //Add the offset for this range unit's line

		//Lines up the units side by side (comments in Unit_Melee)
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
	public override bool isRange(){
		return true;
	}	
}
	
	


