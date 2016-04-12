using UnityEngine;
using System.Collections;

public class CrystalHover : MonoBehaviour {

	float height;
	float velocity = 0;
	float acceleration = 5;

	float threshold = 7.5f;

	bool up = true;
	float timer = 0;

	public bool dead = false;
	void Start(){
		height = transform.position.y;
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (dead) {
			velocity -= 15 * Time.deltaTime;
			height += velocity * Time.deltaTime;
			transform.position = new Vector3 (transform.position.x, height, transform.position.z);
			if(transform.position.y < 80)
				Destroy(GetComponent<CrystalHover>());
			return;
		}
		if (up) {
			timer += Time.deltaTime;

			if (velocity > threshold)
				up = false;

			velocity += acceleration * Time.deltaTime;
			height += velocity * Time.deltaTime;
			transform.position = new Vector3 (transform.position.x, height, transform.position.z);

		} 
		else {
			timer -= Time.deltaTime;

			if (velocity < -threshold)
				up = true;

			velocity -= acceleration * Time.deltaTime;
			height += velocity * Time.deltaTime;
			transform.position = new Vector3 (transform.position.x, height, transform.position.z);
		}
	}
}
