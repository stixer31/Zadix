#pragma strict

var joueur : Transform;
var equipe : String;
var estMort = true;
var spawn : GameObject;


function OnGUI() {
    
            
    
    if(Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
    {
        if(equipe == "")
        {
            GUI.Box(Rect(Screen.width/2-125, Screen.height/2-50, 250, 75), "Choisisser votre equipe ");
                if(GUI.Button(Rect(Screen.width/2-100, Screen.height/2-15, 75, 30), "Ange" ))
                   {
                   equipe = "ROUGE";
                   }
                if(GUI.Button(Rect(Screen.width/2+25, Screen.height/2-15, 75, 30), "Demon" ))
                   {
                   equipe = "BLEU";
                   }
        }
    }
    
    if(equipe != "" && estMort == true)
        {
            if(GUI.Button(Rect(Screen.width/2-50, Screen.height/2-15, 100, 30), "Apparaître"))
                {
                   Network.Instantiate (joueur, spawn.transform.position, spawn.transform.rotation, 0);
                   estMort = false;
                }
        }
    
}


