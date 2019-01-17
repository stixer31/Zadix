#pragma strict

var bla : GameObject;
function Start () {
	 Invoke("removeRightController", 5);
}

function Update () {
	//bla = GameObject.Find("trigger");
}

function removeRightController()
{
    bla = GameObject.Find("body");
	Destroy(bla);	
	bla = GameObject.Find("lgrip");
	Destroy(bla);
	bla = GameObject.Find("rgrip");
	Destroy(bla);
	bla = GameObject.Find("handgrip");
	Destroy(bla);
	bla = GameObject.Find("trackpad");
	Destroy(bla);
	Debug.Log("Right");
}