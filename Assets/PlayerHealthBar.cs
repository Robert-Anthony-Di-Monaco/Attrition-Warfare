using UnityEngine;
using System.Collections;

public class PlayerHealthBar : MonoBehaviour {

    float playerHealth;
    float max_playerHealth;
    GameObject gameManager;
    WorldController worldController;
	// Use this for initialization
	void Start () {


        //TODO: make sure this gets the proper health values from the WorldController script once we have it programmed
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        worldController = gameManager.GetComponent<WorldController>();
        max_playerHealth = worldController.max_PlayerHealth;
        playerHealth = worldController.playerHealth;

	}
	
	// Update is called once per frame
	void Update () {
        RenderHealth();
        playerHealth = worldController.playerHealth;
	}

    void ApplyDamage(float damage)
    {
        playerHealth -= damage;
    }
    void RenderHealth()
    {
        float scale = playerHealth / max_playerHealth;
        if(scale < 0){
            scale = 0;
        }
        this.transform.localScale = new Vector3(scale , this.transform.localScale.y, this.transform.localScale.z);
    }
}
