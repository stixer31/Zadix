#pragma strict
import UnityEngine.UI;

var Health = 100;

var HealthBarColor = Color.green;

function ApplyDammage (TheDammage : int) 
{

	//
	var healthbar : GameObject = GameObject.Find("HealthBar");
	var bla = healthbar.GetComponent.<Text>();
	Health -= TheDammage;
	if (Health <= 0) {
		Health = 0;
		Dead();
	} 
	updateHealthBarColor();
	bla.color = HealthBarColor;	
	bla.text = Health.ToString();
}

function Dead () 
{
	Debug.Log("You Die !");	
}

function OnGui() 
{
	GUI.Box(Rect(60, 50, 90, 20), "Health:" + Health);
}

function updateHealthBarColor() {
	if ( Health >= 70) {
		HealthBarColor = Color.green;
	} else if (Health < 70 && Health > 20) {
		HealthBarColor = Color.yellow;
	} else {
		HealthBarColor = Color.red;
	} 
}