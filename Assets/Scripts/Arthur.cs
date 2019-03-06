/*using UnityEngine;
using System.Collections;

public class Arthur : MonoBehaviour {
	public static GUIText scoreGT;
	public GameObject Sprite;

	// Use this for initialization
	public static Vector3 arthurPos = new Vector3 (-100, -100, -100);
	public static int lives = 2;
	public static int weaponCount;
	public static float sides;
	public static PhysObj arthurPhys;

	
	static bool gameStarted = false;

	public GameObject WeaponPrefab;

	public int health;
	public static WeaponType weapon;
	private PhysObj thisPhys;
	private GameObject arthurObject;
	private WeaponType priorWeapon;

	private bool crouching = false;
	public static bool jumping = false;
	private bool isHit = false;
	private bool wall = false;
	private bool gibsonMode = false;
	private float speed = 2.4f;
	private float jumpVel = 10f;
	private float weaponThrownWaitTime = .2f; // how long to wait after thrown weapon before able to move again
	private int weaponLimit = 2; //amount of weapon permitted on screen
	private Color origColor;
	private float isHitTimer = 0;
	private float platformSpeed = 0;
	private bool isHitOnGround = false; // When hit, it does a special jump that does not show the invincibility amount
	private bool invincibleVisual;
	private char hitSide;
	private bool onLadder = false;
	private bool onLadderTop = false;
	private bool upLadder = false;
	private bool stepUp = false;
	private bool stop = false;

	private bool timeGibson = false;

	private Vector3 crouchState1 = new Vector3(1f, 0.75f, 1f);
	private Vector3 crouchState2 = new Vector3(0f, -0.125f, 0f);
	private Vector3 standState1 = new Vector3(1f, 1f, 1f);
	private Vector3 standState2 = new Vector3(0f, 0f, 0f);
	private Vector3 ladderVec;
	private float verticalWeaponSpawn = 0.5f;
	private BoxCollider boxCollider;

	private float weaponWaiting = 0;


	private bool isDying = false;
	public bool jumpOverTombLeft = false;
	public bool jumpOverTombRight = false;

	public Sprite Stand1;
	public Sprite Move1;
	public Animator Move12;
	public Sprite Crouch1;
	public Sprite Jump1;
	public Sprite Shoot1;
	public Sprite ShootC1;
	public Sprite Climb1;
	public Sprite Stand2;
	public Sprite Move2;
	public Sprite Move22;
	public Sprite Crouch2;
	public Sprite Jump2;
	public Sprite Shoot2;
	public Sprite ShootC2;
	public Sprite Climb2;
	public Sprite Hit;
	public Sprite Dead; 

	public Sprite Used; 



	
	void Awake() {
		arthurPos = new Vector3 (-111, -111, -111);
		origColor = GetComponent<MeshRenderer> ().material.color;
		arthurPhys = GetComponent<PhysObj> ();
	}

	void OnDestroy() {
		arthurPos = new Vector3 (-111, -111, -111);
	}


	
	void Start() {
		Sprite = GameObject.FindWithTag ("spritePlayer");
		GameObject scoreGo = GameObject.Find ("ScoreCounter");
		scoreGT = scoreGo.GetComponent<GUIText> ();
		scoreGT.text = "0";
		thisPhys = this.gameObject.GetComponent<PhysObj>(); 
		boxCollider = this.gameObject.GetComponent<Collider>() as BoxCollider;
		arthurObject = this.gameObject;
		health = 2;
		crouching = false;
		//priorWeapon = weapon;
		sides = 1f;
		//Debug.Log(Sprite.GetComponent<SpriteRenderer> ().sprite);

	}

	void changeSide(bool sides) {
		if (sides) {
			Vector3 theScale = transform.localScale;
			theScale.z *= -1;
			transform.localScale = theScale;
		}
	}

	void Update () {

		if (Input.GetKeyDown (KeyCode.Return)) {
			Time.timeScale = (Time.timeScale>0?0:1);
			PhysManager.isPaused = !PhysManager.isPaused;
		}

		if (upLadder) {
			if (health == 2)
				Used = Climb2;
			else 
				Used = Climb1;
			Sprite.GetComponent<SpriteRenderer> ().sprite = Used;
			climbUp ();
			return;
		}

		//If standing
		if (health == 2)
			Used = Stand2;
		else 
			Used = Stand1;

		//If Jumping
		if (!thisPhys.isGrounded) {
			if (health == 2)
				Used = Jump2;
			else 
				Used = Jump1;
		}
		else {
			Vector3 t = Sprite.transform.localPosition;
			t.y = 0;
			Sprite.transform.localPosition = t;
		}

		if (jumping && thisPhys.isGrounded) {
			if (!isHitOnGround) {
				isHitOnGround = true;
				print ("Grounded but hit");
			}
			print ("No longer jumping");
			jumping = false;
			jumpOverTombLeft = false;
			jumpOverTombRight = false;
		}

		weaponWaiting -= Time.deltaTime;

		if (!jumping)
			thisPhys.setVelocity (new Vector2(0f, thisPhys.getVelocity().y));
		
		if (isDying) {
			Vector3 t = Sprite.transform.localPosition;
			t.y = -0.2f;
			Sprite.transform.localPosition = t;
			return;
		}

		//If I am pressing up while on the latter
		if (commandUp() && !crouching && thisPhys.isGrounded && !jumping && onLadder) 
		{
			//Debug.Log("going up");
			thisPhys.isGrounded = false;
			upLadder = true;
			stepUp = true;
		}

		//If I press down while on top of the ladder
		if (commandDown () && onLadderTop) 
		{
			onLadder = true;
			//Debug.Log("going up");
			thisPhys.isGrounded = false;
			upLadder = true;
			stepUp = true;
			Vector3 temp = transform.position;
			temp.x = ladderVec.x;
			temp.y =  transform.position.y-1f;
			transform.position = temp;
		}
		
		if (commandB () && weaponCount < weaponLimit && weaponWaiting < 0) 
		{
			//arthurObject.scale
			weaponCount++;
			//Debug.Log ("weapon on " + weaponCount);
			if (weaponCount < 0)
				weaponCount = 0;
			GameObject weaponObj = Instantiate (WeaponPrefab) as GameObject;
			GetComponent<AudioSource>().Play();
			if (weapon == WeaponType.XBOW) {
				weaponCount++;
				GameObject weaponObj2 = Instantiate (WeaponPrefab) as GameObject;
				Weapon weaponComp2 = weaponObj2.GetComponent<Weapon>();
				weaponComp2.thisArthur = this.GetComponent<Arthur>();
				weaponComp2.angle = 10f;
				//Debug.Log (weaponComp.thisArthur.weaponCount);
				weaponComp2.weapon = weapon;
				weaponComp2.sides = sides;
				weaponObj2.transform.position = new Vector2 (transform.position.x+sides, transform.position.y + verticalWeaponSpawn + 0.5f); 
			}
			Weapon weaponComp = weaponObj.GetComponent<Weapon>();
			weaponComp.thisArthur = this.GetComponent<Arthur>();
			//Debug.Log (weaponComp.thisArthur.weaponCount);
			weaponComp.angle = 0f;
			weaponComp.weapon = weapon;
			weaponComp.sides = sides;
			weaponObj.transform.position = new Vector2 (transform.position.x+sides, transform.position.y + verticalWeaponSpawn); 
			weaponWaiting = weaponThrownWaitTime;
			if (weapon == WeaponType.FIREBALL)  {
				weaponObj.transform.localScale = new Vector3 (0.3f,0.3f,1f);
			}
		}

		if (commandLeft () && !upLadder) 
		{
			sides = -1f;
		}
		if (commandRight() && !upLadder) 
		{
			sides = 1f;
		}
		if (commandLeft () && !crouching && thisPhys.isGrounded && !jumping && hitSide != 'l' && weaponWaiting < 0) 
		{
			if (health == 2)
				Used = Move2;
			else 
				Used = Move1;
			thisPhys.addVelocity(-speed, 0f);
		}

		if (commandRight () && !crouching && thisPhys.isGrounded && !jumping && hitSide != 'r' && weaponWaiting < 0)
		{
			if (health == 2)
				Used = Move2;
			else 
				Used = Move1;
			thisPhys.addVelocity(speed, 0f);
		}	

		if (!jumping && commandA () && !crouching && !isDying && thisPhys.isGrounded)
		{
			jumping = true;
			print ("XVel: " + thisPhys.getVelocity ().x);			
			thisPhys.addVelocity (jumpVel, 90);
			crouching = false;


			if (commandLeft ()) {
				print ("jumping over the tombstone");
				jumpOverTombLeft = true;
			}
			if (commandRight ()) {
				print ("jumping over the tombstone");
				jumpOverTombRight = true;
			} 

		}

		if (Input.GetKeyDown (KeyCode.G)) {
			gibsonMode = !gibsonMode;

		}

		if (commandDown () && !jumping && thisPhys.isGrounded)
		{
			crouching = true;
			verticalWeaponSpawn = 0.2f;
			//Debug.Log (crouching);
		}
		if (commandDownRelease() && !jumping)
		{
			crouching = false;
			verticalWeaponSpawn = 0.5f;

			//Debug.Log (crouching);
		}

		if (crouching) {
			if (!stop) {
				boxCollider.center = crouchState2;
				boxCollider.size = crouchState1;
				Vector3 t = Sprite.transform.localPosition;
				t.y = -0.11f;
				Sprite.transform.localPosition = t;
				if (health == 2)
					Used = Crouch2;
				else 
					Used = Crouch1;
			}
			else {
				crouching = false;
				if (thisPhys.isGrounded) {
					stop = false;
				}
			}
		}
		else {
			boxCollider.center = standState2;	
			boxCollider.size = standState1;

			Vector3 t = Sprite.transform.localPosition;
			t.y = 0;
			Sprite.transform.localPosition = t;
		}
		
		if (weaponWaiting > 0 && !crouching) {
			Vector3 t = Sprite.transform.localPosition;
			t.y = 0f;
			Sprite.transform.localPosition = t;
			if (health == 2)
				Used = Shoot2;
			else 
				Used = Shoot1;
		}
		
		if (weaponWaiting > 0 && crouching) {
			Vector3 t = Sprite.transform.localPosition;
			t.y = -0.11f;
			Sprite.transform.localPosition = t;
			if (health == 2)
				Used = ShootC2;
			else 
				Used = ShootC1;
		}



		if ( (sides == -1f && Sprite.transform.localScale.x > 0) || 
		    (sides == 1f && Sprite.transform.localScale.x < 0 )){
			Vector3 t = Sprite.transform.localScale;
			t.x *= -1f;
			Sprite.transform.localScale = t;
		}


		Sprite.GetComponent<SpriteRenderer> ().sprite = Used;


	}

	void LateUpdate() {
		arthurPos = transform.position;
	}

	void FixedUpdate() {
		if (isHitTimer < 0) {
			isHit = false;
			Sprite.GetComponent<SpriteRenderer>().GetComponent<Renderer>().enabled = true;
			stop = false;
		} else {
			
			isHitTimer -= Time.deltaTime;
			if (isHitOnGround) {
				drawInvincibleVisual();
			}
		}
		Vector3 pos = transform.position;
		pos.x += platformSpeed * Time.deltaTime;
		transform.position = pos;
		GameObject TimeGo = GameObject.Find ("TimeCounter");
		if (gibsonMode && !timeGibson) {
			TimeGo.GetComponent<Score> ().gibson = true;
			timeGibson = true;
		}
		else if (!gibsonMode && timeGibson){
			TimeGo.GetComponent<Score> ().gibson = false;
			timeGibson = false;
		}
		if (TimeGo.GetComponent<GUIText> ().text == "0:0") {
			isDying = true;
			Invoke ("die",3);
		}
	}


	void drawInvincibleVisual() {
		if (isDying) return;
		if (invincibleVisual) {
			Sprite.GetComponent<SpriteRenderer>().GetComponent<Renderer>().enabled = true;
		} else {
			Sprite.GetComponent<SpriteRenderer>().GetComponent<Renderer>().enabled = false;
		}
		invincibleVisual = !invincibleVisual;
	}

	void OnTriggerEnter(Collider coll){
		//Find out what hit this basket
		GameObject collidedWith = coll.gameObject;


		if (collidedWith.tag == "Item") {
			//Debug.Log("item received");
			ItemType received = collidedWith.GetComponent<Items>().get();
			Destroy(collidedWith);
			if (received == ItemType.LANCE)
				weapon = WeaponType.LANCE;
			if (received == ItemType.KNIFE)
				weapon = WeaponType.KNIFE;
			if (received == ItemType.FIREBALL)
				weapon = WeaponType.FIREBALL;
			if (received == ItemType.XBOW)
				weapon = WeaponType.XBOW;
			if (received == ItemType.MONEY) {
				int score = int.Parse (scoreGT.text);
				score += 500;
				scoreGT.text = score.ToString ();
			}
			if (received == ItemType.ARMOR) {
				if (health == 1) {
					health++;
				}
				else {
					int score = int.Parse (scoreGT.text);
					score += 200;
					scoreGT.text = score.ToString ();
				}
			}

		}
		if (collidedWith.tag == "Wall" && collidedWith.GetComponent<PhysObj>().isObstacle) {
			//Debug.Log ("Wallhit");
			if (collidedWith.transform.position.x  > this.transform.position.x)
				hitSide = 'r';
			if (collidedWith.transform.position.x  < this.transform.position.x)
				hitSide = 'l';
			//Debug.Log (hitSide);
		}
		if (collidedWith.tag == "Ladder") {
			//Debug.Log ("near ladder");
			onLadder = true;
		}
		if ((collidedWith.tag == "Ground" || collidedWith.tag == "Platform") && upLadder && !commandUp () && !onLadderTop) {
			//Debug.Log ("hitFloor");
			upLadder = false;
			onLadder = true;
		}
			if (collidedWith.tag == "Wall" && upLadder) {
				upLadder = false;
				onLadder = true;
			}
		if (collidedWith.tag == "LadderTop") {
			//Debug.Log ("near ladderTop");
			onLadderTop = true;
			ladderVec = collidedWith.transform.position;
		}
		if (collidedWith.tag == "hazard") {
			isDying = true;
			Invoke ("die", 3);
		}
	}

	void OnTriggerStay(Collider coll){
		GameObject collidedWith = coll.gameObject;
		if (collidedWith.tag == "Ladder" && upLadder && stepUp) {
			//Debug.Log ("LadderOn");
			stepUp = false;
			transform.position = new Vector3(collidedWith.transform.position.x,
			   transform.position.y+0.1f,collidedWith.transform.position.z);
		}
		if (collidedWith.tag == "LadderTop" && upLadder && stepUp) {
			//Debug.Log ("LadderOn");
			stepUp = false;
			transform.position = new Vector3(collidedWith.transform.position.x,
			                                 transform.position.y+0.1f,collidedWith.transform.position.z);
		}
		if (collidedWith.tag == "Hostile" && !isHit) {
			if (collidedWith.GetComponent<Enemy>().ready)
				takeHit();
		}
		if (collidedWith.tag == "Platform" && !jumping) {
			//Debug.Log("on platform");
			platformSpeed = collidedWith.GetComponent<MovingPlatform>().speed; 			
		}
	}
		
	void OnTriggerExit(Collider coll){
		GameObject collidedWith = coll.gameObject;
		if (collidedWith.tag == "Wall") {
			//Debug.Log ("WallOff");
			hitSide = 'n';
			if (jumpOverTombLeft) thisPhys.addVelocity(-speed, 0f);
			if (jumpOverTombRight) thisPhys.addVelocity(speed, 0f);
		}
		if (collidedWith.tag == "Ladder") {
			//Debug.Log ("LadderOff");
			onLadder = false;
			upLadder = false;
		}
		if (collidedWith.tag == "LadderTop") {
			//Debug.Log ("off ladderTop");
			onLadderTop = false;
		}
		if (collidedWith.tag == "Platform") {
			//Debug.Log("off platform");
			if (!jumping && !onLadder) {
				Vector3 Pos = transform.position;
				Pos.x += 0.4f*sides;
				transform.position = Pos;
			}
			platformSpeed = 0f;
		}
	}

	void climbUp() {
		thisPhys.setVelocity (new Vector2(0f, 0.5f));
		if (commandUp ()) {
			//Debug.Log ("up");
			thisPhys.addVelocity (speed, 90f);
		}
		if (commandDown ()) {
			Debug.Log (thisPhys.isGrounded);
			if (thisPhys.isGrounded) {
				//Debug.Log ("grounded");
				upLadder = false;
			}
			//Debug.Log ("down");
			thisPhys.addVelocity (-speed, 90f);
		}

	}

	// take a hit
	void takeHit() {
		if (gibsonMode) return;

		print ("Ouch!");
		isHit = true;
		isHitTimer = 3.0f;
		isHitOnGround = false;
		upLadder = false;
		stop = true;
		Vector3 hitVel = new Vector3 (-sides*speed, jumpVel, 0);
		jumping = true;

		thisPhys.setVelocity (hitVel);
		health--;
		if (health == 0) {
			isDying = true;
			GetComponent<MeshRenderer>().material.color = new Color(255, 0, 0, 255);
			Sprite.GetComponent<SpriteRenderer> ().sprite = Dead;
			Invoke ("die", 3);
		}
	}

	void die() {
		Destroy (this.gameObject);
		//weapon = priorWeapon;
		Application.LoadLevel ("gameOver");
	}





	bool commandA() {
		if (PhysManager.isPaused)
						return false;
		return Input.GetKeyDown (KeyCode.X) ||
			   Input.GetKeyDown (KeyCode.Period);
	}

	bool commandUp() {
		if (PhysManager.isPaused)
			return false;
		return Input.GetKey (KeyCode.UpArrow) ||
			   Input.GetKey (KeyCode.W);
	}

	bool commandDown() {
		if (PhysManager.isPaused)
			return false;
		return Input.GetKey (KeyCode.DownArrow) ||
			   Input.GetKey (KeyCode.S);
	}

	bool commandDownRelease() {
		if (PhysManager.isPaused)
			return false;
		return Input.GetKeyUp (KeyCode.DownArrow) ||
			   Input.GetKeyUp (KeyCode.S);
	}

	bool commandLeft() {
		if (PhysManager.isPaused)
			return false;
		return Input.GetKey (KeyCode.LeftArrow) ||
			   Input.GetKey (KeyCode.A);
	}

	bool commandRight() {
		if (PhysManager.isPaused)
			return false;
		return Input.GetKey (KeyCode.RightArrow) ||
			   Input.GetKey (KeyCode.D);
	}

	bool commandB() {
		if (PhysManager.isPaused)
			return false;
		return Input.GetKeyDown (KeyCode.Z)||
			   Input.GetKeyDown (KeyCode.Comma);
	}




}

*/