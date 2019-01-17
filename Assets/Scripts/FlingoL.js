#pragma strict
 import UnityEngine.Networking;
 import UnityEngine.UI;

public class FlingoL extends NetworkBehaviour { 
    
var TireRange = 1.5;
var TireRepeatTime : float = 3;
private var TireTime : float;
var Missile : GameObject;
var BulletSpawnL : GameObject;
var BulletSpawnTransform : Transform;
var ObjectShield1 : GameObject;
//private var ObjActif : boolean;

//var bla : GameObject;



function Start () {
    
	TireTime = Time.time;
    
    //var bla = GameObject.Find("FlingoL");
    //var bli = bla.GetComponent.<FlingoL>();
    
    /*if ( isLocalPlayer) {//permet de controller seulement le joueur local et non les autres script de la scene
            return;//Permet desactive all cameras is not mine et activer only la sienne avec : camera.enabled = false
			//this.GetComponent<Camera>().enabled = true;
		}
        bli.enabled = false;*/

}


function Update () {
    //if ( isLocalPlayer) {//permet de controller seulement le joueur local et non les autres script de la scene
        
	//Tir Controller HTC
	if (Input.GetAxis("openvr-L-trigger-press") > 0.2) {
	TireLeft();//tire
	Debug.Log("test de clique sur le trigger Gauche");
	}
	//Tir souris
	if (Input.GetAxis ("Fire1") > 0.2) {
            CmdTireSouris();//tire
            Debug.Log("Fire 1 cliquez");
	}
	//Shield
	if (Input.GetAxis("LeftControllerTrackpad") > 0.2) {
	ActiveShield();//desactive bouclier et active
	Debug.Log("gros bouton cliquer");
	}else{DesactiveShield();}
//}//if (!isLocalPlayer)
}//Update () FIN



function TireLeft()
{
	if (Time.time > TireTime && Time.time > TireRange) 
	{
		//tir
		TireTime = Time.time + TireRepeatTime;
		var go = Instantiate(Missile,BulletSpawnL.transform.position, transform.rotation); 
		//var go = Instantiate(Missile,BulletSpawnL.transform.position, BulletSpawnL.transform.rotation);
		go.transform.TransformDirection(BulletSpawnL.transform.position);
		//go.transform.forward * 6;
		//Destruction de la balle apres 2 seconde
		//Destroy(go,2.0f);
	}
}

//Lance une commande pour gerer les exeption reseau au cas ou [Command] = C#
//@Command 
function SAVE__TireSouris()
{
		

	if (Time.time > TireTime && Time.time > TireRange) 
	{
		//tir
		TireTime = Time.time + TireRepeatTime;
		//var go1 = Instantiate(Missile,BulletSpawnL.transform.position, transform.rotation); 
		var go1 = Instantiate(Missile,BulletSpawnTransform.transform.position, BulletSpawnTransform.transform.rotation);
		//go1.transform.TransformDirection(BulletSpawnL.transform.position);
		go1.GetComponent(Rigidbody).velocity = go1.transform.forward * 6;

		//faire apparaitre la balle pour les cliens
		//NetworkServer.Spawn(Missile);

		//Destruction de la balle apres 2 seconde
		Destroy(go1,2.0f);
	}
}
@Command 
function CmdTireSouris()
{
		var go1 = Instantiate(Missile,BulletSpawnTransform.position, BulletSpawnTransform.rotation);
		go1.GetComponent(Rigidbody).velocity = go1.transform.forward * 6;
		Destroy(go1,2.0f);
	
}

function ActiveShield()
{
	ObjectShield1.SetActive(true);
}
function DesactiveShield()
{
	ObjectShield1.SetActive(false);
}


//initialisation du joueur local si necessaire
//public override function OnStartLocalPlayer()
//{
//code ici

//}

//Lance une commande pour gerer les exeption reseau au cas ou [Command] = C#
/*@Command 
function CmdjumpAction() 
{ 
 
}*/

}