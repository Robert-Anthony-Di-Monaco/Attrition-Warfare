using UnityEngine;
using System.Collections;

public class ClickToMoveCharacter : MonoBehaviour {


    public NavMeshAgent agent;
	// Use this for initialization
	void Start () {

        agent = this.GetComponent<NavMeshAgent>();

	}
	
	// Update is called once per frame
	void Update () {

        ProcessKeys();
	}

    

    void ProcessKeys()
    {
        //Left Mouse Click
        if (Input.GetMouseButtonDown(0))
        {
            clickToMove();
        }
        //SpaceBar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SnapCameraToPlayer();
        }


    }

    void clickToMove()
    {

            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                agent.SetDestination(hit.point);
                //TODO: check if an enemy is clicked and if in range then attack it
            }
    }
    void SnapCameraToPlayer()
    {

        Camera.main.transform.parent.position = this.transform.position;
    }
}
