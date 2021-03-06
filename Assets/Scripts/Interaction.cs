﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interaction : MonoBehaviour
{
    private Text TxtInfos;
    private GameObject objet;
    public Sprite myFirstImage;
    Image image = null;
    

    // Start is called before the first frame update
    void Start()
    {
        TxtInfos = GameObject.Find("MessageTXT").GetComponent<Text>();
        
        //TxtInfos = gameObject.transform.Find("MessageTXT").GetComponent<Text>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Verifie l'objet touché
    void OnTriggerEnter(Collider Col)
    {
        if (Col.gameObject.tag == "Equipements")
        {
            TxtInfos.text = "en contact avec " + Col.gameObject.name;
            //loot
            image = GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/Artefact Two").GetComponent<Image>();
            image.color = new Color(255, 255, 225, 100);
            //Le nom de l'objet equipement doit etre le meme que celui de l'image qui sert d'icone ds l'inventaire
            image.sprite = Resources.Load<Sprite>("Image/Equipement/" + Col.gameObject.name);

            //

            print(Col.gameObject.name);
        }
    }

    void OnTriggerExit(Collider Col)
    {
        if (Col.gameObject.tag == "Equipements")
        {
            TxtInfos.text = "";
            print("dehors");
        }
    }
}
