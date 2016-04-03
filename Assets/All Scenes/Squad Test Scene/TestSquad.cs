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

	public float numMelee = 5;
	public float numRange = 5;
	public float numSiege = 5;

	public List<Vector3> unitSpawnPoint = new List<Vector3>();

	private float timer = 10;


	// Use this for initialization
	void Start () {

		unitSpawnPoint.Add(new Vector3 (150, 0, 100));
		unitSpawnPoint.Add(new Vector3 (250, 0, 350));
		unitSpawnPoint.Add(new Vector3 (150, 0, 450));

		target = GameObject.Find ("NavMesh Target").transform;
		for (int squadIndex = 0; squadIndex < 3; squadIndex++) {
			
			GameObject squadGO = Instantiate (squadPrefab, unitSpawnPoint[squadIndex], Quaternion.identity) as GameObject;
			squad.Add(squadGO.GetComponent<Squad> ());

			for (int i = 0; i < numSiege; i++) {
				GameObject siegeUnit = Instantiate (siegePrefab, unitSpawnPoint[squadIndex], Quaternion.identity) as GameObject;
				squad[squadIndex].addUnit (siegeUnit.GetComponent<Unit_Siege> ());
			}
			if (squadIndex != 1) {
				for (int i = 0; i < numRange; i++) {
					GameObject rangeUnit = Instantiate (rangePrefab, unitSpawnPoint [squadIndex], Quaternion.identity) as GameObject;
					squad [squadIndex].addUnit (rangeUnit.GetComponent<Unit_Range> ());
				}
			}
			if (squadIndex != 2) {
				for (int i = 0; i < numMelee; i++) {
					GameObject meleeUnit = Instantiate (meleePrefab, unitSpawnPoint [squadIndex], Quaternion.identity) as GameObject;
					squad [squadIndex].addUnit (meleeUnit.GetComponent<Unit_Melee> ());
				}
			}

			Debug.Log ("SquadTest - Leader: " + squad[squadIndex].leader.gameObject.tag);
			Debug.Log ("SquadTest - Units - all: " + squad[squadIndex].allUnits.Count + ", melee: " + squad[squadIndex].meleeUnits.Count +
				", range: " + squad[squadIndex].rangeUnits.Count + ", siege: " + squad[squadIndex].siegeUnits.Count);


			foreach (Unit_Base u in squad[squadIndex].allUnits) {
				if (squad[squadIndex].leader.ID != u.ID) {
					u.agent.speed += 10;

					Vector3 targetPosition = (squad[squadIndex].transform.position + squad[squadIndex].transform.TransformVector (u.offsetFromAnchor));
					targetPosition += squad[squadIndex].transform.forward;
					u.NavMeshTarget = targetPosition;
					u.NavMeshSeek ();
				}
			}
			Debug.Log("Units spawned, non-leader units will constantly seek to their slot positions relative to the leader position (every frame).");
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if(timer <= 10)
			timer -= Time.deltaTime;

		if (timer < 0) {
			for (int squadIndex = 0; squadIndex < 3; squadIndex++) {
				squad[squadIndex].leader.NavMeshTarget = (target.position);
				squad[squadIndex].leader.NavMeshSeek ();
			}
			Debug.Log ("Leaders are now seeking to the target.");
			timer = 12;
		}

		if (timer > 11) {
			timer += Time.deltaTime;
			for (int squadIndex = 0; squadIndex < 3; squadIndex++) {
				foreach (Unit_Base unit in squad[squadIndex].allUnits) {
					//Debug.Log ("anchor offset: " + unit.offsetFromAnchor);

					if (squad[squadIndex].leader.ID != unit.ID) {
					
						Vector3 targetPosition = (squad[squadIndex].transform.position + squad[squadIndex].transform.TransformVector (unit.offsetFromAnchor));
						targetPosition += squad[squadIndex].transform.forward;
						unit.NavMeshTarget = targetPosition;
						unit.NavMeshSeek ();
					}

				}
			}

		}
		if (timer > 20 && timer < 21) {
			squad [0].leader.NavMeshStop ();
			squad [0].removeUnit (squad [0].leader);
			squad [0].leader.NavMeshTarget = new Vector3 (400, 0, 100);
			squad [0].leader.NavMeshSeek ();

			Debug.Log ("Bottom squad's leader killed. A new leader is chosen automatically and the new leader is set to go to the bottom right.");
			timer = 22;
		}
		if (timer > 35 && timer < 36) {
			squad[1].leader.NavMeshTarget = new Vector3 (250, 0, 100);
			squad [1].leader.NavMeshSeek ();

			Debug.Log ("Second squad's leader has a new destination.");
			timer = 37;
		}
	}
}
