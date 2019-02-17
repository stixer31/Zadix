using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventaire : MonoBehaviour
{
    public GameObject Inventaire_Joueur;
    bool locked = false;
    // Start is called before the first frame update
    void Start()
    {
        Inventaire_Joueur.active = false;
    }

    // Update is called once per frame
    void Update()
    {
        bool Inventaire = Input.GetButtonDown("Inventaire");
        bool Competences = Input.GetButton("Competences");
        bool Escape = Input.GetButtonUp("Escape");
        
        if (Inventaire)
        {
            Inventaire_Joueur.active = locked;
            locked = !locked;
        }
        else if (Competences)
        {
            
        }
        else if (Escape)
        {
            
        }
        else
        {
            
        }
    }
}
