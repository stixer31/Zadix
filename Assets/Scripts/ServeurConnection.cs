
/*#pragma strict
//aide https://www.youtube.com/watch?v=6A1Y-FVTbYo&t=276s



var adresse_ip : String;  
var port = 25565;
var NombreJoeur = 15;
var Pseudo : String;
var Chat : GameObject;


function Awake(){
DontDestroyOnLoad(gameObject);
}


function OnGUI()
{
    if(Network.peerType == NetworkPeerType.Disconnected)
    {
        GUI.Label(Rect(5, 10, 200, 20), "adresse ip du serveur : ");
        adresse_ip = GUI.TextField(Rect(150, 10, 100, 20), adresse_ip, 25);
        GUI.Label(Rect(5, 150, 400, 100), "adresse ip du serveur officiel : 78.212.108.22 \n (ouverture sur demande)");
        
        
        GUI.Label(Rect(260, 10, 200, 20), "Entre votre pseudo : ");
        Pseudo = GUI.TextField(Rect(380, 10, 100, 20), Pseudo, 25);
        
        //adresse_ip = "78.212.108.22"; 

        //Boutton Se connecter
        if(GUI.Button(Rect(50, 40, 100, 25), "se connecter") && adresse_ip.length != 0 && Pseudo != "" )//
        {
            
            Network.Connect(adresse_ip, port);
          
         	//Lance la scene apres etre connecte
            Application.LoadLevel("MultiJoueur");
        }

        //Bouton Creer un serveur
        if(GUI.Button(Rect(300, 40, 150, 25), "Creer un serveur") && Pseudo != "")
        {
            Network.InitializeServer(NombreJoeur, port, false);

            //Lance la scene apres etre connecte
            Application.LoadLevel("MultiJoueur");
            
        }
        
    }
    
}


function OnServerInitialized()
{
    print("serveur crée");
    Chat.SendMessage("Connecte", Pseudo);
    
}

function OnFailedToConnect()
{
    print("Impossible de se connecter à : "+adresse_ip);
}

function OnConnectedToServer()
{
    print("Connection réussis à : "+adresse_ip);
    Chat.SendMessage("Connecte", Pseudo);
}

function OnPlayerConnected()
{
    print("un joueur à rejoins");
}*/