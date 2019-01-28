using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInteractions : MonoBehaviour
{

    public GameObject startMenu;
    public bool showingMenu;
    public float axisCancel;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        axisCancel = Input.GetAxisRaw("Cancel");

        if (axisCancel == 1)
            {
            showingMenu = !showingMenu;
            startMenu.SetActive(showingMenu);
        }
    }
}