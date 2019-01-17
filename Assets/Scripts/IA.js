#pragma strict

var Distance = 0;
var TargetVieJoueur : Transform;
var lookAtDistance = 25.8;
var chaseRange = 15.0;
var attackRange = 1.5;
var moveSpeed = 5.0;
var Damping = 6.0;
var attackRepeatTime = 1;
var TheDammage = 0;
var VieEnnemi = 100;

private var attackTime : float;
var controller : CharacterController;
var gravity : float =20.0;
private var moveDirection : Vector3 = Vector3.zero;

var rotAngle : float;

var Missile : GameObject;
var spawnBall : GameObject;
var Explosion1 : GameObject;
var Explosion2 : GameObject; 
function Start () {
	attackTime = Time.time;
	//gere l'explosion du rombot
	Explosion1.SetActive(false);
	Explosion2.SetActive(false);
}

function Update () {

	Distance = Vector3.Distance(TargetVieJoueur.position, transform.position);

	if (Distance < lookAtDistance)
	{
		lookAt();
	}
	if (Distance > lookAtDistance)
	{
		//patrolRotate();
		//renderer.material.color = Color.green;
	}
	if (Distance < attackRange)
	{
		attack();
	}
	else if (Distance < chaseRange)
		{
			chase();
		}



}//Update () FIN

function patrolRotate()
{
	//renderer.material.color = Color.red;
	rotAngle += Time.deltaTime*100;
	transform.rotation = Quaternion.Euler(0, rotAngle, 0); //Slerp(transform.rotation, rotation, Time.deltaTime*Damping);

}

function lookAt()
{
	//renderer.material.color = Color.red;
	var rotation = Quaternion.LookRotation(TargetVieJoueur.position - transform.position);
	transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime*Damping);

}

function chase()
{
	//renderer.material.color = Color.red;

	moveDirection = transform.forward;
	moveDirection *= moveSpeed;

	moveDirection.y -= gravity*Time.deltaTime;
	controller.Move(moveDirection*Time.deltaTime);
}

function attack()
{
	if (Time.time > attackTime) 
	{
		TargetVieJoueur.SendMessage("ApplyDammage", TheDammage);
		//Debug.Log("The Enemy has attacked");
		attackTime = Time.time + attackRepeatTime;
		var go = Instantiate(Missile,spawnBall.transform.position, transform.rotation); 
		go.transform.TransformDirection(spawnBall.transform.position);

	}
}

function ApplyDammage()
{
	chaseRange += 30;
	moveSpeed = 2;
	lookAtDistance += 40;
}




/*****************************   Blessure  *********************************/

		function OnCollisionEnter(theCollision : Collision) {
		if (theCollision.gameObject.tag == "Bullet") {
		Destroy(theCollision.gameObject);
		VieEnnemi -= 10;

			if(VieEnnemi <= 0)
			{
			//fait disparaitre le robot
			var rendRobot = GetComponent.<Renderer>();
			rendRobot.enabled = false;
			//gere l'explosion du rombot
			Explosion1.SetActive(true);
			Explosion2.SetActive(true);
			Invoke("deleteEnnemi",1);
			}
		
		Debug.Log(" ennemi has been shot by a bullet"+VieEnnemi);
		}
	}

	function deleteEnnemi() {
	Destroy(this.gameObject);
}