using UnityEngine;
using System.Collections;

public class PlayerHealthBar : MonoBehaviour
{
    public GameObject worldController;
    public GameObject player;
    int health;
	
	void Start ()
    {
        player = worldController.GetComponent<WorldController>().thePlayer.gameObject;
	}
	
	void Update ()
    {
        updateHealthBar();
	}

    public void updateHealthBar()
    {
        health = player.GetComponent<Player_AI>().health;
        float healthRatio = (float)health/100;
        if (healthRatio > 0.0){
            this.transform.localScale = new Vector3(healthRatio, 1f, 1f);
        }
    }
}
