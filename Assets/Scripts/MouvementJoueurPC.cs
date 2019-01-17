using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MouvementJoueurPC : NetworkBehaviour {

	public Camera cameraJoueur;
	// Use this for initialization
    [SerializeField]
    Behaviour[] componentsToDisable;
	[SerializeField]
	private float MouseSensibility;
	public float SpeedWalk;
	public float SpeedRun;
	private Animator animation;
	public GameObject PersonnageAnimation;


	void Start () {
        if ( ! isLocalPlayer) {//permet de controller seulement le joueur local et non les autres script de la scene
			//GameObject.Find("Camera").GetComponent<Camera>().enabled = false;
		}
        if ( isLocalPlayer) {//permet de controller seulement le joueur local et non les autres script de la scene
            return;//Permet desactive all cameras is not mine et activer only la sienne avec : camera.enabled = false
			//this.GetComponent<Camera>().enabled = true;
		}
        //boucle pour annuler tous ce qui n'est pas le joueur voir dans les propriétés du joueur pour ajouter desx elements
        for(int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }

		 //animation = GetComponent<Animator>();
	}
    
    
    



	// Update is called once per frame
	void Update () {

		if (isLocalPlayer) {//permet de controller seulement le joueur local et non les autres script de la scene
			
			//*********************
			/// mouvement courrir et marcher
			///*******************
			/// 
			if (Input.GetAxis ("RunButton") > 0 & Input.GetAxis ("Vertical") > 0 & Input.GetAxis ("CrouchButton") <= 0) {
				//mouvement Courrir joueur
				var x = Input.GetAxis ("Horizontal") * Time.deltaTime * SpeedWalk * SpeedRun;
				var z = Input.GetAxis ("Vertical") * Time.deltaTime * SpeedWalk * SpeedRun;
				//Avance le joueur
				transform.Rotate (0, x, 0);
				//Deplacement latterale joueur
				transform.Translate (x, 0, 0);
				//legere rotation du joueur quand il passe en laterale
				transform.Translate (0, 0, z);

			} else if (Input.GetAxis ("Vertical") > 0 & Input.GetAxis ("CrouchButton") > 0) {
				//mouvement Crouch Avance joueur
				var x = Input.GetAxis ("Horizontal") * Time.deltaTime / 3;
				var z = Input.GetAxis ("Vertical") * Time.deltaTime  / 3;
				//Avance le joueur
				transform.Rotate (0, x, 0);
				//Deplacement latterale joueur
				transform.Translate (x, 0, 0);
				//legere rotation du joueur quand il passe en laterale
				transform.Translate (0, 0, z);

			}else if (Input.GetAxis ("Vertical") > 0){
				//mouvement Marcher joueur
				var x = Input.GetAxis ("Horizontal") * Time.deltaTime * SpeedWalk;
				var z = Input.GetAxis ("Vertical") * Time.deltaTime * SpeedWalk;
				//Avance le joueur
				transform.Rotate (0, x, 0);
				//Deplacement latterale joueur
				transform.Translate (x, 0, 0);
				//legere rotation du joueur quand il passe en laterale
				transform.Translate (0, 0, z);
			} else if (Input.GetAxis ("CrouchButton") > 0){
				//mouvement crouch idle
			}


		
			//*************
			//vue souris du joueur
			//**************

			if (Input.GetAxis("Mouse X") > 0)//droite
			{
				float Ymouse = Input.GetAxisRaw("Mouse X") * Time.deltaTime * MouseSensibility;
				float Xmouse = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * MouseSensibility;
				//rotation du personnage en fonction de la souris faisant suivre la camera
				transform.eulerAngles +=  new Vector3(0, Ymouse, 0);
			}
			if (Input.GetAxis("Mouse X") < 0)//gauche
			{
				float Ymouse = Input.GetAxisRaw("Mouse X") * Time.deltaTime * MouseSensibility;
				float Xmouse = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * MouseSensibility;
				//rotation du personnage en fonction de la souris faisant suivre la camera
				transform.eulerAngles +=  new Vector3(0, Ymouse, 0);
			}
			if (Input.GetAxis("Mouse Y") > 0)//haut
			{
				float Ymouse = Input.GetAxisRaw("Mouse X") * Time.deltaTime * MouseSensibility;
				float Xmouse = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * MouseSensibility;
				//deplace only la camera
				cameraJoueur.transform.eulerAngles -=  new Vector3 (Xmouse, 0, 0);
			}

			if (Input.GetAxis("Mouse Y") < 0)//bas
			{
				float Ymouse = Input.GetAxisRaw("Mouse X") * Time.deltaTime * MouseSensibility;
				float Xmouse = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * MouseSensibility;
				//deplace only la camera
				cameraJoueur.transform.eulerAngles -=  new Vector3 (Xmouse, 0, 0);
			}


			//*********************
			//Animation du personnage du joueur
			//*********************

			//Avance animation
			if (Input.GetAxis ("Vertical") > 0 & !Input.GetButtonDown ("CrouchButton")) {
				animation = PersonnageAnimation.GetComponent<Animator> ();
				animation.SetBool ("ConditionMarcher", true);
			} else {
				animation = PersonnageAnimation.GetComponent<Animator> ();
				animation.SetBool ("ConditionMarcher", false);
			}
			//Animation courrir
			if (Input.GetAxis ("RunButton") > 0) {
				animation = PersonnageAnimation.GetComponent<Animator> ();
				animation.SetBool ("ConditionRun", true);
			} else {
				animation = PersonnageAnimation.GetComponent<Animator> ();
				animation.SetBool ("ConditionRun", false);
			}


			//Animation Crouch idle
			if (Input.GetAxis ("CrouchButton") > 0) {
				animation = PersonnageAnimation.GetComponent<Animator> ();
				animation.SetBool ("ConditionCrouchIdle", true);
			} else {
				animation = PersonnageAnimation.GetComponent<Animator> ();
				animation.SetBool ("ConditionCrouchIdle", false);
			}
			//Animation Crouch avancé
			if (Input.GetAxis ("CrouchButton") > 0 & Input.GetAxis ("Vertical") > 0) {
				animation = PersonnageAnimation.GetComponent<Animator> ();
				animation.SetBool ("ConditionCrouchWalk", true);
			} else {
				animation = PersonnageAnimation.GetComponent<Animator> ();
				animation.SetBool ("ConditionCrouchWalk", false);
			}

			//float Xmouse = Input.GetAxisRaw("Mouse Y");
			//Vector3 rotation2 = new Vector3 (0, Xmouse, 0);
		}//if (isLocalPlayer)

	}//updat


}
