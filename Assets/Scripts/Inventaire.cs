using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Inventaire : MonoBehaviour
{
    public GameObject Inventaire_Joueur;
    bool locked = false;
    // Start is called before the first frame update
    void Start()
    {
        //bouton dans inventaire
        GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/Head").GetComponent<Button>().onClick.AddListener(TaskOnClick);
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

    void ajouteInventaire()
    {

    }

    void TaskOnClick()
    {
        //Output this to console when Button1 or Button3 is clicked
        string StringObject = EventSystem.current.currentSelectedGameObject.name;
        GameObject btnGameObject = EventSystem.current.currentSelectedGameObject;

        //path = AssetDatabase.GetAssetPath(texture);
        string path = "Assets/Image/Equipement/";
        GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/Body").GetComponent<Image>().sprite = Resources.Load<Sprite>( path + btnGameObject.GetComponent<Image>().sprite.name);//btnGameObject.GetComponent<Image>().sprite.name;
        Debug.Log("You have clicked the button! " + StringObject + " " + btnGameObject.GetComponent<Image>().sprite.name);
        btnGameObject.GetComponent<Image>().sprite = null;
        //GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/Body").GetComponent<Image>().color = new Color(255, 255, 225, 100);
        GameObject.Find("Interface joueur/Equipement Panel/Body Slots Empty/Body").GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Equipement/iron sword");
    }




}
