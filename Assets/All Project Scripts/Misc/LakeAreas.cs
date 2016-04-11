using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//For units to check if their slot position is inside a lake
//Views the lake as an ellipse, uses geometry formulas to detect if a point is inside the lake

public class LakeAreas {

	public static Vector3[] lakeCenters = new Vector3[3];
	public static float[] lakeHalfWidths = new float[3];
	public static float[] lakeHalfHeights = new float[3];
	public static float[] lakeSqrHalfWidths = new float[3];
	public static float[] lakeSqrHalfHeights = new float[3];

	// Use this for initialization
	public static void Initialize(){
		lakeCenters [0] = new Vector3 (620, 75, 669);
		lakeHalfWidths [0] = 140;
		lakeHalfHeights [0] = 160;
		lakeSqrHalfWidths [0] = lakeHalfWidths [0] * lakeHalfWidths [0];
		lakeSqrHalfHeights [0] = lakeHalfHeights [0] * lakeHalfHeights [0];


		lakeCenters [1] = new Vector3 (609, 75, 1294);
		lakeHalfWidths [1] = 140;
		lakeHalfHeights [1] = 162;
		lakeSqrHalfWidths [1] = lakeHalfWidths [1] * lakeHalfWidths [1];
		lakeSqrHalfHeights [1] = lakeHalfHeights [1] * lakeHalfHeights [1];

		lakeCenters [2] = new Vector3 (626, 75, 1894);
		lakeHalfWidths [2] = 135;
		lakeHalfHeights [2] = 162;
		lakeSqrHalfWidths [2] = lakeHalfWidths [2] * lakeHalfWidths [2];
		lakeSqrHalfHeights [2] = lakeHalfHeights [2] * lakeHalfHeights [2];
	}
	//Geometry formula to check if a point is inside an ellipse
	public static bool isInsideLake(Vector3 position){

		for (int i = 0; i < lakeCenters.Length; i++) {
			
			float xDist = position.x - lakeCenters[i].x;
			float yDist = position.z - lakeCenters[i].z;

			if ( ((xDist * xDist) / lakeSqrHalfHeights[i]) + ((yDist * yDist) / lakeSqrHalfHeights [i]) < 1)
				return true;

		}
		return false;
	}


	public static Vector3 GetNewNavmeshTarget(int lake, Vector3 currentTarget){
		
		//Implement this if the current behavior isn't good
		return Vector3.zero;

	}

	// Update is called once per frame
	void Update () {
	
	}
}
