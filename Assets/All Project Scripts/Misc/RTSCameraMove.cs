using UnityEngine;
using System.Collections;

public class RTSCameraMove : MonoBehaviour {
    public float camSpeed = 250;
    public float GUIsize = 100;
	public float cameraDistance = 250;
	public float maxCameraDistance = 400;
	public float minCameraDistance = 100;
	public float cameraZoomFactor = 25;

	public Rect levelBounds = new Rect (200, 135, 1065, 2450);

    Rect bottomRect;
    Rect topRect;
    Rect leftRect;
    Rect rightRect;

	// Use this for initialization
	void Start () {

        bottomRect = new Rect(0, 0, Screen.width, GUIsize);
        topRect = new Rect(0, Screen.height - GUIsize, Screen.width, GUIsize);
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
			float amount = 1 - Input.mousePosition.y / GUIsize; 

			//Using arrow keys isn't dependent on the mouse position
			if(Input.GetKey(KeyCode.DownArrow))
				amount = 1;

			//Translates the vector to world space so the camera can face any direction
			transform.parent.position += transform.parent.TransformVector (new Vector3(0, 0, amount * -camSpeed * Time.deltaTime));

			//transform.parent.Translate(camSpeed, 0, 0, Space.World);
        }
		if (topRect.Contains(Input.mousePosition) || Input.GetKey(KeyCode.UpArrow))
        {
			float amount = (Input.mousePosition.y - (Screen.height - GUIsize)) / GUIsize;
			if(Input.GetKey(KeyCode.UpArrow))
				amount = 1;
			transform.parent.position += transform.parent.TransformVector (new Vector3(0, 0, amount * camSpeed * Time.deltaTime));
            //transform.parent.Translate(-camSpeed, 0, 0, Space.World);
        }
		if (rightRect.Contains(Input.mousePosition) || Input.GetKey(KeyCode.RightArrow))
        {
			float amount = (Input.mousePosition.x - (Screen.width - GUIsize)) / GUIsize;
			if(Input.GetKey(KeyCode.RightArrow))
				amount = 1;
			transform.parent.position += transform.parent.TransformVector (new Vector3(amount * camSpeed * Time.deltaTime, 0, 0));
            //transform.parent.Translate(0, 0, camSpeed, Space.World);
        }
		if (leftRect.Contains(Input.mousePosition) || Input.GetKey(KeyCode.LeftArrow))
        {
			float amount = 1 - Input.mousePosition.x / GUIsize;
			if(Input.GetKey(KeyCode.LeftArrow))
				amount = 1;
			transform.parent.position += transform.parent.TransformVector (new Vector3(amount * -camSpeed * Time.deltaTime, 0, 0));
            //transform.parent.Translate(0, 0, -camSpeed, Space.World);
        }

		//Zoom using the fields at the top of the script
		float mouseWheelInput = Input.GetAxis ("Mouse ScrollWheel");
		if (mouseWheelInput < 0 && cameraDistance < maxCameraDistance)
			cameraDistance += cameraZoomFactor;
		else if(mouseWheelInput > 0 && cameraDistance > minCameraDistance)
			cameraDistance -= cameraZoomFactor;
			
		StayInBounds ();

        //manageHeight();
	}
	void StayInBounds(){
		if (transform.parent.position.x < levelBounds.x)
			transform.parent.position = new Vector3 (levelBounds.x, transform.parent.position.y, transform.parent.position.z);
		else if (transform.parent.position.x > levelBounds.width)
			transform.parent.position = new Vector3 (levelBounds.width, transform.parent.position.y, transform.parent.position.z);
		
		if(transform.parent.position.z < levelBounds.y)
			transform.parent.position = new Vector3 (transform.parent.position.x, transform.parent.position.y, levelBounds.y);
		else if(transform.parent.position.z > levelBounds.height)
			transform.parent.position = new Vector3 (transform.parent.position.x, transform.parent.position.y, levelBounds.height);
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
