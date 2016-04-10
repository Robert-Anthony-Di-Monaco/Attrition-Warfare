using UnityEngine;
using System.Collections;

public class RTSCameraMove : MonoBehaviour {
    public float camSpeed = 250;
    public float GUIsize = 100;
	public float cameraDistance = 250;
	public float maxCameraDistance = 400;
	public float minCameraDistance = 100;
	public float cameraZoomFactor = 25;

    Rect bottomRect;
    Rect topRect;
    Rect leftRect;
    Rect rightRect;

	// Use this for initialization
	void Start () {

		//point (0, 0) on the screen is the top left corner
        topRect = new Rect(0, 0, Screen.width, GUIsize);
        bottomRect = new Rect(0, Screen.height - GUIsize, Screen.width, GUIsize);
        leftRect = new Rect(0,0, GUIsize, Screen.height);
        rightRect = new Rect(Screen.width - GUIsize, 0, GUIsize, Screen.height);

		//Only have to set the camera's starting position in the inspector, not the rotation
		transform.forward = (transform.parent.position - transform.position).normalized;
	}
	
	// Update is called once per frame
	void FixedUpdate () {


		transform.position = transform.parent.position - transform.forward * cameraDistance;

		if (bottomRect.Contains(Input.mousePosition) || Input.GetKey(KeyCode.DownArrow))
        {
			//Moves faster when closer to the edge of the screen
			float amount = (Input.mousePosition.y - (Screen.height - GUIsize)) / GUIsize; 

			//Translates the vector to world space so the camera can face any direction
			transform.parent.position += transform.parent.TransformVector (new Vector3(0, 0, amount * camSpeed * Time.deltaTime));

			//transform.parent.Translate(camSpeed, 0, 0, Space.World);
        }
		if (topRect.Contains(Input.mousePosition) || Input.GetKey(KeyCode.UpArrow))
        {
			float amount = 1 - Input.mousePosition.y / GUIsize;
			transform.parent.position += transform.parent.TransformVector (new Vector3(0, 0, amount * -camSpeed * Time.deltaTime));
            //transform.parent.Translate(-camSpeed, 0, 0, Space.World);
        }
		if (rightRect.Contains(Input.mousePosition) || Input.GetKey(KeyCode.RightArrow))
        {
			float amount = (Input.mousePosition.x - (Screen.width - GUIsize)) / GUIsize;
			transform.parent.position += transform.parent.TransformVector (new Vector3(amount * camSpeed * Time.deltaTime, 0, 0));
            //transform.parent.Translate(0, 0, camSpeed, Space.World);
        }
		if (leftRect.Contains(Input.mousePosition) || Input.GetKey(KeyCode.LeftArrow))
        {
			float amount = 1 - Input.mousePosition.x / GUIsize;
			transform.parent.position += transform.parent.TransformVector (new Vector3(amount * -camSpeed * Time.deltaTime, 0, 0));
            //transform.parent.Translate(0, 0, -camSpeed, Space.World);
        }

		//Zoom using the fields at the top of the script
		float mouseWheelInput = Mathf.Clamp(Input.GetAxis ("Mouse ScrollWheel"), -1, 1);
		if (mouseWheelInput < 0 && cameraDistance < maxCameraDistance)
			cameraDistance += cameraZoomFactor;
		else if(mouseWheelInput > 0 && cameraDistance > minCameraDistance)
			cameraDistance -= cameraZoomFactor;
			
			

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
