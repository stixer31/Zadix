using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Inventaire : MonoBehaviour
{
    public GameObject Inventaire_Joueur;//(Equipement Panel)
    bool locked = false;
    //Equipement
    //object iron_sword = new { ProductId = "iron_sword", Type = "Head" };
    Dictionary<int, string> iron_sword = new Dictionary<int, string>()
                                            {
                                                {1,"iron_sword"},
                                                {2, "Head"},
                                                {3,"3"}
                                            };

    void Start()
    {
        


        //bouton dans inventaire
        GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/Head").GetComponent<Button>().onClick.AddListener(RechercheInventaire);
        GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/Body").GetComponent<Button>().onClick.AddListener(RechercheEquipement);
        Inventaire_Joueur.active = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        //Bouton pour ouvrir l'inventaire
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

    /*---------------------------------------------*/
    public string ExampleFunction(string Variable)
    {
        return Variable;
    }
    void RechercheEquipement()
    {
        string test = "";
        /*foreach (KeyValuePair<int, string> item in iron_sword)
        {
            //Debug.Log("Key: {0}, Value: {1}", item.Key, item.Value);
            test = item.Value;
        }*/
        //var newVar : int = this.GetType().GetField("iron_sword" + "[2]").GetValue(this);

        Debug.Log(iron_sword[2]);  //iron_sword[2]
        //chercher le moyen de changer le nom par un string pour pointer automatiquement sur le bon equipement "iron_sword"[2]
        ICI
        string Slot = iron_sword[2];// "Head";
        ajouteDsEquipement(Slot);
    }
    private void ajouteDsEquipement(string Slot)
    {
        string StringObject = EventSystem.current.currentSelectedGameObject.name;
        GameObject btnGameObject = EventSystem.current.currentSelectedGameObject;
        string path = "Image/Equipement/";
        GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/" + Slot).GetComponent<Image>().sprite = Resources.Load<Sprite>(path + btnGameObject.GetComponent<Image>().sprite.name);
        btnGameObject.GetComponent<Image>().sprite = null;
    }

    /*---------------------------------------------*/
    void RechercheInventaire()
    {
        string Slot = "Body";
        ajouteInventaire(Slot);
    }
    void ajouteInventaire(string Slot)
    {
        //Output this to console when Button1 or Button3 is clicked
        string StringObject = EventSystem.current.currentSelectedGameObject.name;
        GameObject btnGameObject = EventSystem.current.currentSelectedGameObject;
        string path = "Image/Equipement/";
        //Debug.Log("You have clicked the button! " + StringObject + " " + btnGameObject.GetComponent<Image>().sprite.name);
        //GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/Body").GetComponent<Image>().color = new Color(255, 255, 225, 100);
        //GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/Body").GetComponent<Image>().sprite = Resources.Load<Sprite>( path + "iron sword" );
        GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/" + Slot).GetComponent<Image>().sprite = Resources.Load<Sprite>(path + btnGameObject.GetComponent<Image>().sprite.name);//btnGameObject.GetComponent<Image>().sprite.name;
        btnGameObject.GetComponent<Image>().sprite = null;
    }




}
