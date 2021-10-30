#pragma strict

var leftTrack : MoveTrack;
var rightTrack : MoveTrack;

var acceleration : float = 5;

var currentVelocity : float = 0;
var maxSpeed : float = 25;

var rotationSpeed : float = 30;

var spawnPoint : Transform;
var bulletObject : GameObject;
var fireEffect : GameObject;




function Start() {

	maxSpeed = 10;

	// Get Track Controls
	leftTrack = GameObject.Find(gameObject.name + "/LeftTrack").GetComponent(MoveTrack);
	rightTrack = GameObject.Find(gameObject.name + "/RightTrack").GetComponent(MoveTrack);

}


function Update () {

	// shake the body of tank
	CheckRebound();

	
	if (Input.GetKey (KeyCode.UpArrow)) {
		// plus speed
		if (currentVelocity <= maxSpeed) 
			currentVelocity += acceleration * Time.deltaTime;

	} else if (Input.GetKey (KeyCode.DownArrow)) {
		// minus speed
		if (currentVelocity >= -maxSpeed) 
			currentVelocity -= acceleration * Time.deltaTime;
		
	} else {
		// No key input. 
		if (currentVelocity > 0) 
			currentVelocity -= acceleration * Time.deltaTime;
		else if (currentVelocity < 0) 
			currentVelocity += acceleration * Time.deltaTime;

	}


	// Turn off engine if currentVelocity is too small. 
	if (Mathf.Abs(currentVelocity) <= 0.05)
		currentVelocity = 0;

	// Move Tank by currentVelocity
	transform.Translate(Vector3(0, 0, currentVelocity * Time.deltaTime));

	// Move Tracks by currentVelocity	 
	if (currentVelocity > 0) {
		// Move forward
		leftTrack.speed = currentVelocity;
		leftTrack.GearStatus = 1;
		rightTrack.speed = currentVelocity;
		rightTrack.GearStatus = 1;
	}
	else if (currentVelocity < 0)	{
		// Move Backward
		leftTrack.speed = -currentVelocity;
		leftTrack.GearStatus = 2;
		rightTrack.speed = -currentVelocity;
		rightTrack.GearStatus = 2;
	}
	else {
		// No Move
		leftTrack.GearStatus = 0;	
		rightTrack.GearStatus = 0;		
	}


	// Turn Tank
	if (Input.GetKey (KeyCode.LeftArrow)) {
		if (Input.GetKey(KeyCode.DownArrow)) {
			// Turn right
			transform.Rotate(Vector3(0, rotationSpeed * Time.deltaTime, 0));
			
			leftTrack.speed = rotationSpeed;
			leftTrack.GearStatus = 1;
			rightTrack.speed = rotationSpeed;
			rightTrack.GearStatus = 2;
			
		} else {
			// Turn left
			transform.Rotate(Vector3(0, -rotationSpeed * Time.deltaTime, 0));
			
			leftTrack.speed = rotationSpeed;
			leftTrack.GearStatus = 2;
			rightTrack.speed = rotationSpeed;
			rightTrack.GearStatus = 1;
			
		}
	}

	if (Input.GetKey (KeyCode.RightArrow)) {
		if (Input.GetKey(KeyCode.DownArrow)) {
			// Turn left
			transform.Rotate(Vector3(0, -rotationSpeed * Time.deltaTime, 0));
			leftTrack.speed = rotationSpeed;
			leftTrack.GearStatus = 2;
			rightTrack.speed = rotationSpeed;
			rightTrack.GearStatus = 1;

		} else {
			// Turn right
			transform.Rotate(Vector3(0, rotationSpeed * Time.deltaTime, 0));
			leftTrack.speed = rotationSpeed;
			leftTrack.GearStatus = 1;
			rightTrack.speed = rotationSpeed;
			rightTrack.GearStatus = 2;
			
		}
	}
	
	
	// Fire!
	if (Input.GetKeyDown(KeyCode.Space)) {
		// make fire effect.
		Instantiate(fireEffect, spawnPoint.position, spawnPoint.rotation);
		
		// make ball
		Instantiate(bulletObject, spawnPoint.position, spawnPoint.rotation);
		
		SetRebound();
	}

}


// Tank body shake (bounce)
private var reboundTick : float = 0;
private var reboundPosition : Vector3;
private var reboundPaceIdx : int = 0;

function SetRebound() {

	reboundPaceIdx = 5;

	reboundPosition = transform.position;
}

function CheckRebound() {
	var tmpPos : Vector3;
	var tmpDistance : float = 0;
	
	
	switch (reboundPaceIdx) {
		case 5 :
			tmpDistance = 8;	// back 5
			break;
			
		case 4 : 
			tmpDistance = 8 + 2.4;	// for 5 + 2.5
			break;
			
		case 3 : 
			tmpDistance = 2.4 + 0.72;	// back 2.5, back 1.25
			break;
			
		case 2 : 
			tmpDistance = 0.72 + 0.72 * 0.3;	// 1.25 + 0.6
			break;
			
		case 1 : 
			tmpDistance = 0.72 * 0.3;	// 0.6 + 0.3
			break;
			
		default :
			return;
	}
	
	switch (reboundPaceIdx) {
		case 5 :
		case 4 : 
		case 3 : 
		case 2 : 
		case 1 : 
			if (Time.time - reboundTick > (6 - reboundPaceIdx) * 0.02) {
				reboundTick = Time.time;
				
				tmpPos = reboundPosition + 
					gameObject.transform.TransformDirection(spawnPoint.forward).normalized *
					((reboundPaceIdx % 2 == 1) ? 1 : -1) * tmpDistance * 0.005;

				gameObject.transform.position = tmpPos;
				
				reboundPaceIdx--;
			}
			
			if (reboundPaceIdx == 0) 
				transform.position = reboundPosition;

			break;
	}
}
