using UnityEngine;
using System.Collections;

public class Crystal : Unit_Base {

	public Material deadMat;
	public const int maxHP = 1000;
	float timer = 0;

	// Use this for initialization
	public override void Awake () {
		health = maxHP;
	}
	void Start(){
		layerSetUp ();
	}
	void FixedUpdate(){
		timer += Time.deltaTime;
		if (timer > 2) {
			ApplyDamage(100);
			timer = 0;
		}
	}

	public override void ApplyDamage (int amount)
	{
		health -= amount;

		if (health <= 0) {
			CrystalKill ();
		}

	}
	public override bool isCrystal(){
		return true;
	}
	public void CrystalKill(){
		GetComponentInChildren<Renderer> ().material = deadMat;

		Destroy (GetComponent<CrystalHP> ());
		GetComponentInChildren<CrystalHover> ().dead = true;
		gameObject.layer = 1 << 0;
		Destroy (GetComponent<Rigidbody> ());
		Destroy(GetComponent<Unit_Base> ());

	}
}
