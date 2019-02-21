using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interaction : MonoBehaviour
{
    private Text TxtInfos;

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
            TxtInfos.text = "en contact je eux agir sur l'objet";
            print("dedans");
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
