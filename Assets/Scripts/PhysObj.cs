using UnityEngine;
using System.Collections;

/* A PhysObject grants physics to a GameObject */

/* For the physics to work properly, the object needs:
 * - A Rigidbody with kinetic set to true
 * - a Collider with Is Trigger enabled
 */

public class PhysObj : MonoBehaviour {


	public bool ActiveObject;
	public Vector2 InitialVelocity;

	private Vector2 vel			= new Vector2(0.0f, 0.0f);
	private Vector2 lastPos; // previous position;
	private float friction		= 0.0f; 
	private bool active 		= true;

	public  bool isGround 		= false;
	public float maxVelocity    = 1000.0f;
	public bool isGrounded = false;
	public bool isObstacle = false;
	public bool ignoreGravity = false;
	public bool ignoreObstacles = false;
	public bool ignoreGround = false;

	private GameObject lastFloor = null;
	private float preWallVel;
	private GameObject lastWall = null;


	/* Public Interface */

	//. Velocity Management

	// Adds a velocity component given a velocity amount and degree direction
	public void addVelocity(float velocity, float degrees) {
		vel.x += velocity * Mathf.Cos (degrees * (Mathf.PI / 180.0f));
		vel.y += velocity * Mathf.Sin (degrees * (Mathf.PI / 180.0f));
		if (vel.y > 0.0001) isGrounded = false;
	}

	// Adds a velocity component
	public void addVelocity(Vector2 newVel) {
		vel += newVel;
		if (vel.y > 0.0001) isGrounded = false;
	}

	// Explicitly sets the velocity to the specified value
	public void setVelocity(Vector2 newVel) {
		vel = newVel;
		if (vel.y > 0.0001) isGrounded = false;
	}

	// Returns the current Velocity
	public Vector2 getVelocity() {
		return vel;
	}

	// Sets a friction value. Friction reduces velocity every step by a percentage
	// thus, A friction value will be between 0.0f and 1.0f inclusive
	public void setFriction(float fr) {
		friction = fr;
	}
	public float getFriction() {
		return friction;
	}


	// Returns whether or not the object is falling
	public bool isFalling() {
		return vel.y < 0;
	}


	//. Positional handling

	// Returns the last position
	public Vector2 getLastPos() {
		return lastPos;
	}

	public void setLastPos(Vector2 v) {
		lastPos = v;
	}


	//. Inactive
	public void setActive(bool b) {
		active = b;
	}

	public bool isActive() {
		return active;
	}

	//. Collision 
	void OnTriggerEnter2D(Collider2D other) {

		PhysObj otherPhys = other.gameObject.GetComponent<PhysObj>();
		if (otherPhys)
			resolveCollision (otherPhys, true);
	}

	void OnTriggerStay2D(Collider2D other) {
		PhysObj otherPhys = other.gameObject.GetComponent<PhysObj>();
		if (otherPhys)
			resolveCollision (otherPhys, false);
	}

	void OnTriggerExit2D(Collider2D other) {


		if (lastFloor && other.gameObject.GetInstanceID () == lastFloor.GetInstanceID ()) {
			isGrounded = false;
			lastFloor = null;
		}


		if (lastWall && other.gameObject.GetInstanceID () == lastWall.GetInstanceID ()) {
			lastWall = null;
		}
	}

	private void resolveCollision(PhysObj other, bool enter) {

		if (!isActive()) return;

		// reset wall velocity is there is none to emulate preserving
		if (Mathf.Abs (vel.x) < .001 && Mathf.Abs (vel.y) < .001) {
			preWallVel = 0;
		}
		
		if (vel.y < 0 &&
			!ignoreGround && other.isGround && 
		     transform.position.y >= other.transform.position.y + other.GetComponent<BoxCollider2D>().bounds.extents.y/2) {

			//if (vel.y < 0) {
			//transform.position = getLastPos ();
			transform.position = new Vector3(
				transform.position.x, 
				getIdealContactFloorYPos(other),// added offset to account for rounding errors
				transform.position.z);

			// nullify y component
			setVelocity (new Vector2 (vel.x, 0.0f));
			//print ("landing");
			//}
			isGrounded = true;
			lastFloor = other.gameObject;
		}

		// prevent clipping through floor
		if (!ignoreGround && other.isGround && isGrounded && 
		    Mathf.Abs (Mathf.Abs(transform.position.y -       GetComponent<BoxCollider2D> ().GetComponent<Collider2D>().bounds.extents.y) -  
		         Mathf.Abs (other.transform.position.y + other.GetComponent<BoxCollider2D>().GetComponent<Collider2D>().bounds.extents.y)) < .2f &&
		    transform.position.y <
		    getIdealContactFloorYPos(other)) {
				transform.position = new Vector3(
					transform.position.x, 
					getIdealContactFloorYPos(other), 
					transform.position.z);
		} 



		if (other.isObstacle && !ignoreObstacles &&  enter) {
			//Debug.Log(other.gameObject);
			lastWall = other.gameObject;
			setVelocity (new Vector2(0.0f, vel.y)); 
		}

	}


	float getIdealContactFloorYPos(PhysObj other) {
		return other.transform.position.y + other.GetComponent<BoxCollider2D> ().GetComponent<BoxCollider2D>().bounds.extents.y + 
						                          GetComponent<BoxCollider2D> ().GetComponent<BoxCollider2D>().bounds.extents.y - .001f;
	}



	// Use this for initialization
	void Awake () {
		active = ActiveObject;
		vel = InitialVelocity;	

			
	}

	void Start() {
		PhysManager.register (this);
	}
	
	// Update is called once per frame
	void Update () {
		if (vel.x > maxVelocity) {
			vel.x = maxVelocity;
		}
		if (vel.y > maxVelocity) {
			vel.y = maxVelocity;
		}

		if (vel.x < -maxVelocity) {
			vel.x = -maxVelocity;
		}

		if (vel.y < -maxVelocity) {
			vel.y = -maxVelocity;
		}

	}

	void LateUpdate() {
		
	}
}
