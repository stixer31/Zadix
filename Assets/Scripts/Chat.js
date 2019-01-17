#pragma strict


var ligneDeChat = new String[4];
var texteChat : String;
var PlayerName :String;
var texteModif : String;


function OnGUI () 
{
    
    if(Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
        {
        texteChat = GUI.TextField(Rect(0, Screen.height-20, 200, 20), texteChat);
    
        //chat
        GUI.Label(Rect(0, Screen.height-100, 200, 20), ligneDeChat[3]);
        GUI.Label(Rect(0, Screen.height-80, 200, 20), ligneDeChat[2]);
        GUI.Label(Rect(0, Screen.height-60, 200, 20), ligneDeChat[1]);
        GUI.Label(Rect(0, Screen.height-40, 200, 20), ligneDeChat[0]);


        if(GUI.Button(Rect(200, Screen.height-20, 100, 20), "Envoyer") && texteChat.length != 0)
            {
                texteModif = PlayerName + " : " + texteChat;
                GetComponent.<NetworkView>().RPC("RafraichirChat", RPCMode.All, texteModif);
                texteChat = ""; 
            }
        }


}

function Connecte(nom :String)
{
    PlayerName = nom;
    nom += " s'ext connecte";
    GetComponent.<NetworkView>().RPC("RafraichirChat", RPCMode.All, nom);
}

@RPC
function RafraichirChat(texte : String)
{
    ligneDeChat[3] = ligneDeChat[2];
    ligneDeChat[2] = ligneDeChat[1];
    ligneDeChat[1] = ligneDeChat[0];
    ligneDeChat[0] = texte;
}

