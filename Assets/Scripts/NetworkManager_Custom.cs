using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkManager_Custom : NetworkManager {

	public void StartUpHost()
	{
		SetPort ();
		//NetworkManager.singleton.StopClient (); 
		NetworkManager.singleton.StopHost ();
		NetworkManager.singleton.StartHost();
	}


	public void JoinGame()
	{
		SetIpAddress ();
		SetPort ();
		NetworkManager.singleton.StartClient();
	}


	/*************************/
	/*****   METHODE  ********/
	/*************************/

	void SetIpAddress(){
		string ipAddress = GameObject.Find ("inputFieldIPAddress").transform.Find ("Text").GetComponent<Text> ().text;
		NetworkManager.singleton.networkAddress = ipAddress;
	}


	void SetPort(){
		NetworkManager.singleton.networkPort = 7777;
	}

	void Update ()
	{
	// Create a temporary reference to the current scene.
	Scene currentScene = SceneManager.GetActiveScene ();
	// Retrieve the name of this scene.
	string sceneName = currentScene.name;


	if (sceneName == "Menu") 
	{
		SetupMenuSceneButtons ();
			Debug.Log (sceneName+" teswwwwwwwwwwwwwwwwwwwt");
	}
	else if (sceneName == "MultiJoueur")
	{
		SetupOtherSceneButtons ();
			Debug.Log (sceneName+" xxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
	}

	}//Update




	void SetupMenuSceneButtons(){
		GameObject.Find ("ButtonStartHost").GetComponent<Button> ().onClick.RemoveAllListeners ();
		GameObject.Find ("ButtonStartHost").GetComponent<Button> ().onClick.AddListener (StartUpHost);//StartUpHost recuperer dans le onclick du bouton ButtonStartHost Canvas
	
		GameObject.Find ("ButtonJoinGame").GetComponent<Button> ().onClick.RemoveAllListeners ();
		GameObject.Find ("ButtonJoinGame").GetComponent<Button> ().onClick.AddListener (JoinGame);//JoinGame recuperer dans le onclick du bouton ButtonJoinGame Canvas

	}

	void SetupOtherSceneButtons(){
		GameObject.Find ("ButtonDisconnect").GetComponent<Button> ().onClick.RemoveAllListeners ();
		GameObject.Find ("ButtonDisconnect").GetComponent<Button> ().onClick.AddListener (NetworkManager.singleton.StopHost);//JoinGame recuperer dans le onclick du bouton ButtonJoinGame Canvas
	}

}
