using UnityEngine;
using System.Collections;

/*
	For now this script just creates then destroys the shot

	Derek: add this when you want to use the shot  
		GameObject shotInstant = Instantiate(lazerShotPrefab, spawnPoint, player.transform.rotation) as GameObject;
		shotInstant.getComponent<LazerShot>().Fire(target);
 */

public class LazerShot : MonoBehaviour 
{
	public void Fire(Vector3 target)
	{
		float shotVelocity = 250f;
		this.gameObject.GetComponent<Rigidbody>().AddForce(shotVelocity * transform.forward, ForceMode.VelocityChange);

		// Destroy shot when it hits target
		Vector3 dir = target - transform.position;
		float time2Target = dir.magnitude / shotVelocity;
		Destroy (this.gameObject, time2Target); 
	}

	void OnTriggerEnter(Collider col)   // Currently the prefab has a trigger collider
	{
		// ....
	}
	void OnCollisionEnter(Collision col)
	{
		// ....
	}
}

	

