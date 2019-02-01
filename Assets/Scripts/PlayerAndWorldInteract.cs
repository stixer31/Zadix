using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAndWorldInteract : MonoBehaviour {

	//Debug text value...
	string debugClick = "Click on : ";

    //POUR UTILISER LE NEVMESH AgENT, IL FAUT OBLIGATOIREMENT UN EVENT MANAGER...
	NavMeshAgent playerAgent;

	string mouseMove = "MouseMove";
	float mouseRightClick;

	// Use this for initialization
	void Start () {

        playerAgent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {

		mouseRightClick = Input.GetAxisRaw (mouseMove);

        //Si le joueur clique sur un objet avec le boutton gauche de la souris et non sur l'UI, alors il recupère les infos de l'objet...
		if (mouseRightClick == 1 && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            GetInteraction();
        }
	}

    void FixedUpdate()
    {
        //Affiche un ligne rouge avec pour point de départ : transform du joueur, et destination le point : interactionInfo, de couleur rouge,
        Debug.DrawLine(transform.position, playerAgent.destination, Color.red);
    }

    //FONCTION = crée pour gérer les intreractions entre le joueur et l'environnement... (quetes, monstres, objets ??)
    void GetInteraction()
    {
        //Le RAY récupère la valeur de la position de la souris sur les objets avec comme paramètre la position de la souris dans le monde 3D,
        Ray interactRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit interactInfo;

        if (Physics.Raycast(interactRay, out interactInfo, Mathf.Infinity))
        {
            //Crée un objet de type GameObject contenant la valeur de collision entre le ray et les sol.
            GameObject interactObject = interactInfo.collider.gameObject;

            //... Si l'info de l'objet est égal au tag interactable, alors il effectue une interaction avec...
            if (interactObject.tag == "Interactable Object")
            {
                //... L'interaction test entre le joueur et l'objet portant ce tag (ex on : Interactable Object).
                Debug.Log("Interacted with...");
                interactObject.GetComponent<Interactable>().MoveToInteraction(playerAgent);
            }
            //... Sinon déplace le joueur à la position définie dans interactInfo sur l'endroit cliqué,
            else
            {
                playerAgent.stoppingDistance = 0f;
                playerAgent.destination = interactInfo.point;
            }
        }

		Debug.Log (debugClick + interactInfo.collider.gameObject);
    }
}