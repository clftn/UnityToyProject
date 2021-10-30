#pragma strict


var tankList : GameObject[];

var currentTankObj : GameObject;

var currentTankIdx : int;

function Start () {

	currentTankIdx = 0;

	SpawnTank();
}

function Update () {

}

function OnGUI() {
 	// Scene Title
 	if (currentTankObj)
		GUI.Label (Rect(Screen.width / 2 - 120,Screen.height - 140,350,50), currentTankObj.name + "  ("+(currentTankIdx+1) + "/"+tankList.length+")");
	
	
	// Back to menu button
	if (GUI.Button(Rect(Screen.width / 2 - 100, Screen.height - 60,200,50),"Go to Demo1"))
		Application.LoadLevel ("Demo1");
		
	// Select left Tank	
	if (GUI.Button(Rect(100, Screen.height - 160 , 100,50),"<==")) {
		currentTankIdx--;
		if (0 > currentTankIdx)
			currentTankIdx = tankList.length - 1;
			
		SpawnTank();
	}
	
	// Select right Tank
	if (GUI.Button(Rect(Screen.width - 200, Screen.height - 160 , 100,50),"==>")) {
		currentTankIdx++;
		if (currentTankIdx > tankList.length - 1)
			currentTankIdx = 0;
	
		SpawnTank();
	}
	
	GUI.Label (Rect(Screen.width / 2 - 200, Screen.height - 95,420,50), "Arraow : Tank Move, A,D Turret Move, W,S : Gun Move, Space : Fire");		
}



function SpawnTank() {
	if (currentTankObj) {
		DestroyImmediate(currentTankObj);
	}
	
	currentTankObj = Instantiate(tankList[currentTankIdx], Vector3 (153, 0, 141.2), Quaternion.AngleAxis(-175, Vector3.up)) as GameObject;
}
