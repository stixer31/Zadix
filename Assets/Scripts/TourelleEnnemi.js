#pragma strict
//var BaseTourelleRotation : GameObject;
var TargetVieJoueur : Transform;
var BulletsEnnemi : GameObject;
var BaseTournante : Transform;
var faireFeu;
var SpawnBullet : GameObject;

function Start () {
	
}

function Update () {
var rotation = Quaternion.LookRotation(TargetVieJoueur.position - transform.position);
BaseTournante.transform.rotation = Quaternion.Slerp(BaseTournante.transform.rotation, rotation, Time.deltaTime*1);

var temps : int = Time.time; var cadence = (temps % 4);
if(cadence){
tir(temps);
}

//BaseTourelleRotation.transform.Rotate(Vector3.y * Time.deltaTime * VitesseBaseTourelleRotation);
	
}//update

function tir(temps){
if(temps!=faireFeu){
//var tirer = Instantiate(BulletsEnnemi, SpawnBullet.transform.position, transform.rotation);
var tirer = Instantiate(BulletsEnnemi, SpawnBullet.transform.position, transform.rotation); 
	tirer.transform.TransformDirection(BulletsEnnemi.transform.position);

//tirer.transform.Translate(Vector3.forward*Time.deltaTime);
//tirer.transform.TransformDirection(TargetVieJoueur.transform.position);
//BulletsEnnemi.transform.Translate(Vector3.forward*Time.deltaTime);
//var go = Instantiate(Missile,spawnBall.transform.position, transform.rotation); 
		//go.transform.TransformDirection(spawnBall.transform.position);

//var Bullet = tirer.GetComponent.<Rigidbody>();
//Bullet.transform.Translate(Vector3.forward*Time.deltaTime);
//Bullet.transform.TransformDirection(SpawnBullet.transform.position);
//Bullet.AddForce(Vector3.forward*Time.deltaTime/5 );
faireFeu = temps;
}
}
