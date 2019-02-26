using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/*tuto : //https://www.youtube.com/watch?v=IyrwmNOy77I&index=2&list=PLDHwkMpIGgVypDQQEQYZkfz8vnHrsyQhO */
public class MouvementJoueur : NetworkBehaviour
{

    public float rotateSpeed = 20f;
    public float walkSpeed = 0.5f;

    private Vector3 targetPosition;
    private bool isMoving;

    public GameObject cubeCouleur;
    public GameObject PersonnageAnimated;
    public Camera cameraLocal;
    public Color couleurDefault;
    public Color couleurChange;

    void Start()
    {
        //desactive ou active la camera pour le reseau
        //myTransform.FindChild("Camera").Getcomponent<camera>().enabled = true;
        if (GetComponentInParent<NetworkIdentity>().isLocalPlayer)
        {
            cubeCouleur.GetComponent<Renderer>().material.color = Color.red;
            cameraLocal.GetComponent<Transform>().GetComponent<Camera>().enabled = true;
        }
        else
        {
            cubeCouleur.GetComponent<Renderer>().material.color = Color.green;
        }
        


    }
    void Update()
    {
        if (!GetComponentInParent<NetworkIdentity>().isLocalPlayer)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
            {
                isMoving = true;
                SetTargetPosition();
            }

            if (isMoving)
                MovePlayer();
        
        
    }
    
    private void MovePlayer()
    {
        Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, walkSpeed * Time.deltaTime);

        //fait marcher le personnage
        PersonnageAnimated.GetComponent<Animator>().SetBool("ConditionMarcher", true);

        if (transform.position == targetPosition)
        {
            isMoving = false;
            //fait marcher le personnage
            PersonnageAnimated.GetComponent<Animator>().SetBool("ConditionMarcher", false);
            PersonnageAnimated.GetComponent<Animator>().SetBool("ConditionIdle", true);

        }
    }

    private void SetTargetPosition()
    {
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        Ray ray = cameraLocal.ScreenPointToRay(Input.mousePosition);
        float hitdist = 0.0f;

        if (playerPlane.Raycast(ray, out hitdist))
            targetPosition = ray.GetPoint(hitdist);
    }
}
