using UnityEngine;
using System.Collections;

/*
	TO USE:	
        GameObject shotInstant = Instantiate(lazerShotPrefab, spawnPoint, player.transform.rotation) as GameObject;
		shotInstant.getComponent<LazerShot>().Fire(target);
 */

public class LazerShot : MonoBehaviour 
{
    public GameObject target;
    public int terrainLayer = 9;
    public int enemyLayer = 8;
	public int damage;
    
	public void Fire(Vector3 target)
	{
		float shotVelocity = 750f;
		this.gameObject.GetComponent<Rigidbody>().AddForce(shotVelocity * transform.forward, ForceMode.VelocityChange);

		// Destroy shot when it hits target
		Vector3 dir = target - transform.position;
		float time2Target = Vector3.Distance(transform.position, target)/ shotVelocity;
		Destroy (this.gameObject, time2Target); 
	}

    // Apply damage to whatever lazer hits
	void OnTriggerEnter(Collider col)   
	{
        if (col.gameObject.Equals(target))
        {
			target.SendMessage("ApplyDamage", damage);
            Destroy(this.gameObject);
        }
	}
}

	

