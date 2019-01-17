using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Shoot : NetworkBehaviour {

	public float TireRange = 1;
	public float TireRepeatTime = 3;
	private float TireTime;
	public GameObject Missile;
	public Transform BulletSpawnTransform;
	public GameObject ObjectShield1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if ( !isLocalPlayer) {//permet de controller seulement le joueur local et non les autres script de la scene
			return;
		}
		//Tir Controller HTC
		if (Input.GetAxis("openvr-L-trigger-press") > 0.2) {
			//TireLeft();//tire
			Debug.Log("test de clique sur le trigger Gauche");
		}
		//Tir souris
		if (Input.GetButtonDown ("Fire1")) {
			CmdShootTire();//tire
			Debug.Log("Fire 1 cliquez");
		}
		//Shield
		if (Input.GetAxis("LeftControllerTrackpad") > 0.2) {
			//ActiveShield();//desactive bouclier et active
			Debug.Log("gros bouton cliquer");
		}else{DesactiveShield();}
		//}//if (!isLocalPlayer)
	}

	//l'anotation command permet au serveur de gerer les nouvelle instance pour tous les joueur
	[Command]
	void CmdShootTire(){
		//Debug.Log("tirerrrrrrrrrrrrrrr");
		//creation de la balle a partir du prefab dans les propriétés
		var go1 = (GameObject)Instantiate(Missile,BulletSpawnTransform.position, BulletSpawnTransform.rotation);
		//ajout velocity
		go1.GetComponent<Rigidbody>().velocity = go1.transform.forward * 25;
		NetworkServer.Spawn (go1);
		Destroy(go1,2.0f);
	}
	void TireLeft(){
		
	}
	void ActiveShield(){
		
	}
	void DesactiveShield(){
		
	}
}
