using UnityEngine;
using System.Collections;

public class RTSCameraMove : MonoBehaviour {
    public float camSpeed;
    public float GUIsize;
    Rect bottomRect;
    Rect topRect;
    Rect leftRect;
    Rect rightRect;

	// Use this for initialization
	void Start () {


        GUIsize = 20f;
        camSpeed = 0.75f;
        bottomRect = new Rect(0, 0, Screen.width, GUIsize);
        topRect = new Rect(0, Screen.height - GUIsize, Screen.width, GUIsize);
        leftRect = new Rect(0,0, GUIsize, Screen.height);
        rightRect = new Rect(Screen.width - GUIsize, 0, GUIsize, Screen.height);
	}
	
	// Update is called once per frame
	void Update () {


        if (bottomRect.Contains(Input.mousePosition))
        {
            transform.parent.Translate(camSpeed, 0, 0, Space.World);
        }
        if (topRect.Contains(Input.mousePosition))
        {
            this.transform.parent.Translate(-camSpeed, 0, 0, Space.World);
        }
        if (rightRect.Contains(Input.mousePosition))
        {
            transform.parent.Translate(0, 0, camSpeed, Space.World);
        }
        if (leftRect.Contains(Input.mousePosition))
        {
            transform.parent.Translate(0, 0, -camSpeed, Space.World);
        }
        //manageHeight();
	}


    //this method is called to raycast to the ground and hold the camera at a constant distance from the ground 
    //in the center of the camera
    //void manageHeight()
    //{
    //     RaycastHit hit;
    //     Vector3  eyeInTheSkye = transform.parent.position;
    //     eyeInTheSkye.y += 1000f;
      
    //     if(Physics.Raycast(eyeInTheSkye, Vector3.down, out hit))
    //     {
    //         transform.parent.position = Vector3.Lerp(transform.parent.position, new Vector3(transform.parent.position.x, hit.transform.position.y, transform.parent.position.z), 1f);
    //     }
    //}
}
