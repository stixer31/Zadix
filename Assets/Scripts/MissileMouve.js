#pragma strict

var speed : float = 1; 
var Target : Transform;
var TheDammage = 8;
var destructionBullet = 7;

function Start () {
Invoke("deleteBullet",destructionBullet);

}

function Update () {
	this.transform.Translate(Vector3.forward*Time.deltaTime/speed);
}

function OnCollisionEnter(theCollision : Collision) {
	//impact la vie du joueur
	if (theCollision.gameObject.tag == "Player") {
		Debug.Log("Player has been shot by a bullet");
		var bla : GameObject = GameObject.Find("Vie"); //.ApplyDammage();
		bla.SendMessage("ApplyDammage", TheDammage);
		deleteBullet();
	}
	//impact la vie de l'ennemi
	if (theCollision.gameObject.tag == "Ennemi") {
		Debug.Log("ennemi has been shot by a bulletXXXX");
		//var bla : GameObject = GameObject.Find("Vie"); //.ApplyDammage();
		//bla.SendMessage("ApplyDammage", TheDammage);
		//deleteBullet();
	}
	if (theCollision.gameObject.tag == "Shield") {
		Debug.Log("Shield has been shot by a bullet");
		//var bla : GameObject = GameObject.Find("Vie"); //.ApplyDammage();
		//bla.SendMessage("ApplyDammage", TheDammage);
		deleteBullet();
	}
}


function deleteBullet() {
	Destroy(this.gameObject);
}