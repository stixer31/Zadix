#pragma strict

var TireRange = 1.5;
var TireRepeatTime : float = 3;
private var TireTime : float;
var Missile : GameObject;
var BulletSpawnR : GameObject;
var ObjectShield : GameObject;
private var ObjActif : boolean;

function Start () {
	TireTime = Time.time;
}


function Update () {
	if (Input.GetAxis("openvr-r-trigger-press") > 0.2) {
	TireRight();//tire
	Debug.Log("test de clique sur le trigger Droite");
	}
	if (Input.GetAxis("RightControllerTrackpad") > 0.2) {
	ActiveShield();//desactive bouclier et active
	Debug.Log("gros bouton cliquer");
	}else{DesactiveShield();}
}//Update () FIN

function TireRight()
{
	if (Time.time > TireTime && Time.time > TireRange) 
	{
		

		TireTime = Time.time + TireRepeatTime;
		var go = Instantiate(Missile,BulletSpawnR.transform.position, transform.rotation); 
		go.transform.TransformDirection(BulletSpawnR.transform.position);

	}
}

function ActiveShield()
{
	ObjectShield.SetActive(true);
}
function DesactiveShield()
{
	ObjectShield.SetActive(false);
}


