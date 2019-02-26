using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Reflection;



public class Inventaire : MonoBehaviour
{

    /*------------------------------------------------------------------------------- REQUIS -------------------------------------------------------------------------
     *Ajouter un Equipements pour qu'il soit gérer automatiquement dans l'inventaire et sur le perosnnage:
     * - Ajouter l'equipement dans la List<FoodItem> avec un nom précis (array)
     * - Ajouter un loot avec le même nom sur la scene à partir du prefab
     * - Ajouter le loot avec le même nom dans le "Personage" Prefab au chemin /Human Blend Low poly Animated/rig/root/MCH-.... et le dossier main, pied, tete, etc... à choisir 
     * - Ajouter une image avec le même nom dans le dossier Image/Equipement/
     *-----------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    public GameObject Inventaire_Joueur;//(Equipement Panel)
    bool locked = false;
    //Liste de l'Equipement/Armure/Arme/Potion/Etc...
    //Liste Position Inventaire: Head | Body | Right Arm | Left Arm | Belt | Right Hand | Left Hand | Left Leg | Right Leg | Right Weapon | Left Weapon | Left Boot | Right Boot | Left Ear | Left Finger | Right Finger | Right Ear | Artefact Two | Artefact Three | Artefact One | Scroll Two | Scroll Three | Scroll One
    List<FoodItem> list = new List<FoodItem>
        {
         new FoodItem { ID = 1, Name = "iron_sword", Position = "Left Arm" },
         new FoodItem { ID = 2, Name = "apple", Position = "kiwi" },
        };
    //Class qui permet de récuperer dans la liste de l'equipement les infos voir List<FoodItem> list = new List<FoodItem>
    class FoodItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Position { get; set; } 
    }


    //Efface l'équipement porté par le joueur quand il touche à l'inventaire
    void RemouveEquipementSurPersonage(string NameEquipements, bool TrueFalse)
    {
        GameObject originalGameObject = GameObject.Find("Human Blend Low poly Animated");
        GameObject child = originalGameObject.transform.GetChild(2).GetChild(0).gameObject;
        //Debug.Log("OKKK" + child);
        foreach (Transform childer in child.GetComponentsInChildren<Transform>())
        {
            if (childer.name == NameEquipements)
            {
                //Debug.Log("trouvé " + childer.name);

                for (int i = 0; i < childer.transform.childCount; i++)
                {
                    childer.GetChild(i).gameObject.active = TrueFalse;
                } 
            }
        }
        //GameObject gun = GameObject.Find("Group1").SetActive(false);
        //Transform ammo = gun.transform.Find("Group1");
        //gun.GetComponent<Renderer>().enabled = false;
        //GameObject.Find("iron_sword").active = false;
        //FindChild("childname").GetComponent<Transform>().enabled = false;
    }



        

    void Start()
    {
        //Remouve l'equipement sur le joueur Avatar des le debut
        RemouveEquipementSurPersonage("iron_sword",false);

        //bouton Equipement
        GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/Head").GetComponent<Button>().onClick.AddListener(RechercheInventaire);
        GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/Body").GetComponent<Button>().onClick.AddListener(RechercheInventaire);
        GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/Right Arm").GetComponent<Button>().onClick.AddListener(RechercheInventaire);
        GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/Left Arm").GetComponent<Button>().onClick.AddListener(RechercheInventaire);
        GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/Belt").GetComponent<Button>().onClick.AddListener(RechercheInventaire);
        GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/Right Hand").GetComponent<Button>().onClick.AddListener(RechercheInventaire);
        GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/Left Hand").GetComponent<Button>().onClick.AddListener(RechercheInventaire);
        GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/Left Leg").GetComponent<Button>().onClick.AddListener(RechercheInventaire);
        GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/Right Leg").GetComponent<Button>().onClick.AddListener(RechercheInventaire);
        GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/Right Weapon").GetComponent<Button>().onClick.AddListener(RechercheInventaire);
        GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/Left Weapon").GetComponent<Button>().onClick.AddListener(RechercheInventaire);
        GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/Left Boot").GetComponent<Button>().onClick.AddListener(RechercheInventaire);
        GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/Right Boot").GetComponent<Button>().onClick.AddListener(RechercheInventaire);
        GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/Left Ear").GetComponent<Button>().onClick.AddListener(RechercheInventaire);
        GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/Left Finger").GetComponent<Button>().onClick.AddListener(RechercheInventaire);
        GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/Right Finger").GetComponent<Button>().onClick.AddListener(RechercheInventaire);
        GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/Right Ear").GetComponent<Button>().onClick.AddListener(RechercheInventaire);
        //Bouton Inventaire General
        GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/Artefact Two").GetComponent<Button>().onClick.AddListener(RechercheEquipement);
        GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/Artefact Three").GetComponent<Button>().onClick.AddListener(RechercheEquipement);
        GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/Artefact One").GetComponent<Button>().onClick.AddListener(RechercheEquipement);
        GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/Scroll Two").GetComponent<Button>().onClick.AddListener(RechercheEquipement);
        GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/Scroll Three").GetComponent<Button>().onClick.AddListener(RechercheEquipement);
        GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/Scroll One").GetComponent<Button>().onClick.AddListener(RechercheEquipement);
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

    }//update

    /*---------------------------------------------*/
    //Recherche la position de l'equipement dans la liste des Equipements
      void RechercheEquipement()
    {
        //Recherche le nom de l'image dans la liste pour recuperer la position
        foreach (var Lists in list)
        {
            //Si le nom de l'image est egal au nom de la liste de l'objet alors recuperer position
            if (Lists.Name == EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite.name)
            {
                Debug.Log(EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite.name + " " +list[0].Position + " ok");
                //Deplace l'image dans le slot prévu pour elle
                ajouteDsEquipement(list[0].Position);
            }
            
        }   
    }

    //Positione l'equipemnts a la bonne place une foi la recherche de la position faite
    private void ajouteDsEquipement(string Slot)
    {
        //string StringObject = EventSystem.current.currentSelectedGameObject.name;
        GameObject btnGameObject = EventSystem.current.currentSelectedGameObject;
        string path = "Image/Equipement/";
        GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/" + Slot).GetComponent<Image>().sprite = Resources.Load<Sprite>(path + btnGameObject.GetComponent<Image>().sprite.name);
        btnGameObject.GetComponent<Image>().sprite = null;
        //Ajoute l'equipement sur le joueur Avatar
        RemouveEquipementSurPersonage("iron_sword", true);

    }



    /*---------------------------------------------*/
    //Retire un éléments de l'equipements pour l'ajouter dans l'inventaire general
    void RechercheInventaire()
    {
        string Slot = "Artefact Two";//emplacement de l'inventaire
        ajouteInventaire(Slot);
    }
    //Ajout dans l'inventaire une foi le slot trouvé
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
        //Remouve l'equipement sur le joueur Avatar
        RemouveEquipementSurPersonage("iron_sword", false);
    }




}
