using UnityEngine;
using System.Collections;

/*
	DO NOT PLACE ANY AI CODE IN THIS SCRIPT.
		-ROB
*/

public class PlayerController : MonoBehaviour 
{
	public float speed = 5f; 

	void Start () 
	{
		//transform.position = new Vector3 (500f, 10f, 500f);
	}

	void Update () 
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");
		Vector3 movement = new Vector3 (moveHorizontal, 0, moveVertical) * speed * Time.deltaTime;
		transform.Translate(movement.x, 0, movement.z);
	}
}
