using UnityEngine;
using System.Collections;

/*
	Derek put your camera code here. I have a basic follow the player camera implemented. 
		-Rob
*/

public class CameraController : MonoBehaviour
{
	public Transform player; 

	void Start () 
	{
		
	}

	void Update () 
	{
		transform.position = new Vector3 (player.position.x, player.position.y + 2f, player.position.z - 20f);
	}
}
