using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestSquad : MonoBehaviour {

	public GameObject squadPrefab;
	public GameObject meleePrefab;
	public GameObject rangePrefab;
	public GameObject siegePrefab;

	private List<Squad> squad = new List<Squad>();

	private Transform target;

	//These can be tweaked in the inspector
	//See "DropThisIn...." object in the SquadTest scene
	public float numMelee = 5;
	public float numRange = 5;
	public float numSiege = 5;

	public List<Vector3> unitSpawnPoint = new List<Vector3>();

	private float timer = 10;


	// Use this for initialization
	void Start () {

		unitSpawnPoint.Add(new Vector3 (350, 0, 400));
		unitSpawnPoint.Add(new Vector3 (150, 0, 350));
		unitSpawnPoint.Add(new Vector3 (150, 0, 450));

		target = GameObject.Find ("NavMesh Target").transform;

		//Instantiate 3 squads, with varying unit types
		//Lazy programming, sorry for readability...
		for (int squadIndex = 0; squadIndex < 3; squadIndex++) {
			
			GameObject squadGO = Instantiate (squadPrefab, unitSpawnPoint[squadIndex], Quaternion.identity) as GameObject;
			squad.Add(squadGO.GetComponent<Squad> ());

			//Insert siege units, then range, then melee to check that leader selection is working
			for (int i = 0; i < numSiege; i++) { 
				GameObject siegeUnit = Instantiate (siegePrefab, unitSpawnPoint[squadIndex], Quaternion.identity) as GameObject;
				squad[squadIndex].addUnit (siegeUnit.GetComponent<Unit_Siege> ());
			}
			if (squadIndex != 1) { //No range units in the second squad
				for (int i = 0; i < numRange; i++) {
					GameObject rangeUnit = Instantiate (rangePrefab, unitSpawnPoint [squadIndex], Quaternion.identity) as GameObject;
					squad [squadIndex].addUnit (rangeUnit.GetComponent<Unit_Range> ());
				}
			}
			if (squadIndex != 2) { //No melee units in the third squad
				for (int i = 0; i < numMelee; i++) {
					GameObject meleeUnit = Instantiate (meleePrefab, unitSpawnPoint [squadIndex], Quaternion.identity) as GameObject;
					squad [squadIndex].addUnit (meleeUnit.GetComponent<Unit_Melee> ());
				}
			}

			Debug.Log ("SquadTest - Leader: " + squad[squadIndex].leader.gameObject.tag);
			Debug.Log ("SquadTest - Units - all: " + squad[squadIndex].allUnits.Count + ", melee: " + squad[squadIndex].meleeUnits.Count +
				", range: " + squad[squadIndex].rangeUnits.Count + ", siege: " + squad[squadIndex].siegeUnits.Count);

			//After the units have all been spawned (simultaneously on top of each other), make them seek to their slot positions
			foreach (Unit_Base u in squad[squadIndex].allUnits) {
				if (squad[squadIndex].leader.ID != u.ID) { //Leader doesn't move
					u.MarchInFormation ();
				}
			}
			Debug.Log("Units spawned, non-leader units will constantly seek to their slot positions relative to the leader position (every frame).");
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (timer <= 10)
            timer -= Time.deltaTime;

        if (timer < 0)
        {
            for (int squadIndex = 1; squadIndex < 3; squadIndex++)
            {
                squad[squadIndex].leader.NavMeshTarget = (target.position);
                squad[squadIndex].leader.NavMeshSeek();
            }
            Debug.Log("Leaders are now seeking to the target.");
            timer = 12;
        }

        if (timer > 11)
        {
            timer += Time.deltaTime;
            for (int squadIndex = 0; squadIndex < 3; squadIndex++)
            {
                foreach (Unit_Base unit in squad[squadIndex].allUnits)
                {
                    //Debug.Log ("anchor offset: " + unit.offsetFromAnchor);

                    if (squad[squadIndex].leader.ID != unit.ID)
                    {

                        Vector3 targetPosition = (squad[squadIndex].transform.position + squad[squadIndex].transform.TransformVector(unit.offsetFromAnchor));
                        targetPosition += squad[squadIndex].transform.forward;
                        unit.NavMeshTarget = targetPosition;
                        unit.NavMeshSeek();
                    }

                }
            }

        }
        if (timer > 20 && timer < 21)
        {
            squad[0].leader.NavMeshStop();
            squad[0].leader.Kill();
            squad[0].leader.NavMeshTarget = new Vector3(400, 0, 100);
            squad[0].leader.NavMeshSeek();

            Debug.Log("Bottom squad's leader killed. A new leader is chosen automatically and the new leader is set to go to the bottom right.");
            timer = 22;
        }
        if (timer > 35 && timer < 36)
        {
            squad[1].leader.NavMeshTarget = new Vector3(250, 0, 100);
            squad[1].leader.NavMeshSeek();

            Debug.Log("Second squad's leader has a new destination. Note: NavMeshAgents turn strangely after reaching their destination.");
            timer = 37;
        }
	}
}
