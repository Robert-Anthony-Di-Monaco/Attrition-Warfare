using UnityEngine;
using System.Collections;

public class PlayerHealthBar : MonoBehaviour {


    public GameObject worldController;
    public GameObject player;
    public int health;
	// Use this for initialization
	void Start () {

        player = worldController.GetComponent<WorldController>().thePlayer.gameObject;

	}
	
	// Update is called once per frame
	void Update () {
        updateHealthBar();
	}

    public void updateHealthBar()
    {
        health = player.GetComponent<Player_AI>().health;
        this.transform.localScale = new Vector3(health/100, 1, 1);
    }
}
