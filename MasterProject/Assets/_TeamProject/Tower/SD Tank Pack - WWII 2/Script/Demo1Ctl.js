#pragma strict

function Start () {

}

function Update () {

}


function OnGUI() {
 	// Scene Title
	// Back to menu button
	if (GUI.Button(Rect(Screen.width / 2 - 200, Screen.height - 60,200,50),"Go to Demo2"))
		Application.LoadLevel ("Demo2");
		
		
	if (GUI.Button(Rect(Screen.width / 2 + 0, Screen.height - 60,200,50),"Reload This Demo"))
		Application.LoadLevel ("Demo1");
	
		
	GUI.Label (Rect(Screen.width / 2 - 200,Screen.height - 95,420,50), "Arraow : Tank Move, A,D Turret Move, W,S : Gun Move, Space : Fire");		
}
