using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkManager_custom : NetworkManager
{
    //StartHost
    public void StartupHost()
    {
        SetPort();
        NetworkManager.singleton.StartHost();
    }

    //JoinGame
    public void JoinGame()
    {
        SetIPAdress();
        SetPort();
        NetworkManager.singleton.StartClient();
    }

    //JoinGame
    public void StopGame()
    {
        NetworkManager.singleton.StopClient();
    }

    //JoinGame
    public void SetIPAdress()
    {
        string ipAdress = GameObject.Find("Ip_Adress").transform.Find("Text").GetComponent<Text>().text;
        NetworkManager.singleton.networkAddress = ipAdress;
    }

    //Port
    public void SetPort()
    {
        NetworkManager.singleton.networkPort = 7777;
    }



    //Bouton apparrait ou non en fonction de la scene

    void OnLevelWasLoaded(int level)
    {
        if(level==0)
        {
            SetupMenuSceneButtons();
        }
        else
        {
            SetupOtherSceneButtons();
        }

    }

    void SetupMenuSceneButtons()
    {
        GameObject.Find("Host").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("Host").GetComponent<Button>().onClick.AddListener(JoinGame);

        GameObject.Find("Connect").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("Connect").GetComponent<Button>().onClick.AddListener(JoinGame);
    }

    void SetupOtherSceneButtons()
    {
        GameObject.Find("Quit Game").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("Quit Game").GetComponent<Button>().onClick.AddListener(NetworkManager.singleton.StopHost);

    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

