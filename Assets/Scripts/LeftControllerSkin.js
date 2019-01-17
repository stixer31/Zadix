#pragma strict

var blo : GameObject;
function Start () {
	 Invoke("removeLeftController", 5.1);
}

function Update () {
	//bla = GameObject.Find("trigger");
}

function removeLeftController()
{
    blo = GameObject.Find("body");
	Destroy(blo);	
	blo = GameObject.Find("lgrip");
	Destroy(blo);
	blo = GameObject.Find("rgrip");
	Destroy(blo);
	blo = GameObject.Find("handgrip");
	Destroy(blo);
	blo = GameObject.Find("trackpad");
	Destroy(blo);
	Debug.Log("Left");
}