using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* Object manager that resolves collisions */
public class PhysManager : MonoBehaviour {
	public static int wraithCount = 0;
	private static List<PhysObj> objs;
	private static Vector2 accConstant = new Vector2(0.0f, -.5f);
	static public bool isPaused = false;


	/* Public interface */

	// Adds an instance to manage and resolve collisions for
	public static void register(PhysObj phys) {
		objs.Add (phys);
	}

	public static Vector2 getAccelerationConstant() {
		return accConstant;
	}


	void Awake() {
		objs = new List<PhysObj> ();
	}

	// Update is called once per frame
	void LateUpdate () {
		if (isPaused)
						return;
		foreach (PhysObj co in objs) {
			if (!co.isActive ()) continue;

			if (co == null) continue;
			// Add gravity or (other constant) to the obj's velocity 
			if (!co.isGrounded && !co.ignoreGravity) {
				//print ("added velocity");
				co.addVelocity(accConstant);

			} 



			co.setLastPos(co.transform.position);
			Vector2 newPos = co.getLastPos () 
						 + (co.getVelocity ()) * 
						   (1.0f - co.getFriction ())*Time.fixedDeltaTime;

			co.transform.position = newPos;


		}
	}
}
