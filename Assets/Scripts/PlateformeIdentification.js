#pragma strict
var PortailDesable : GameObject; 
function Start () {
	
}

function Update () {
	
}

/*function OnCollisionEnter(theCollision : Collision) {
	if (theCollision.gameObject.tag == "Player") {
		Debug.Log("Player is enter in zone");
		//var bla : GameObject = GameObject.Find("Vie"); //.ApplyDammage();
		//bla.SendMessage("ApplyDammage", TheDammage);
		//deleteBullet();
	}
}*/

function OnTriggerEnter(otherObj: Collider){
 
         if (otherObj.gameObject.tag == "Player"){ 
             Debug.Log("Player is enter in zone");
             PortailDesable.SetActive(false);
         }else{PortailDesable.SetActive(true);}
 }