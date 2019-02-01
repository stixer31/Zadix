using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showMenu : MonoBehaviour
{
    public GameObject panel;

    // Start is called before the first frame update
    void Start()
    {
        panel = GameObject.Find("Start Menu Panel");
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown("escape"))
        {
            panel.SetActive(true);
        }
    }
}