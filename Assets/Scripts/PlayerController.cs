using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public bool onLadderTop = false;
    public bool onLadder = false;
    public bool upLadder = false;
    public bool stepUp = false;

    public bool crouching = false;
    private PhysObj thisPhys;
    public float speed;
    public float sides = 1.0f;

    private bool stop = false;
    private BoxCollider2D boxCollider2D;
    private Vector3 crouchState1 = new Vector3(1f, 0.75f, 1f);
    private Vector3 crouchState2 = new Vector3(0f, -0.125f, 0f);
    private Vector3 standState1 = new Vector3(1f, 1f, 1f);
    private Vector3 standState2 = new Vector3(0f, 0f, 0f);
    private Vector3 ladderVec;

    private char hitSide;

    void Start()
    {
        thisPhys = this.gameObject.GetComponent<PhysObj>();
        boxCollider2D = this.gameObject.GetComponent<BoxCollider2D>();
    }
 
    void Update()
    {
        thisPhys.setVelocity(new Vector2(0f, thisPhys.getVelocity().y));

        if (upLadder)
        {
            /*if (health == 2)
                Used = Climb2;
            else
                Used = Climb1;
            Sprite.GetComponent<SpriteRenderer>().sprite = Used;*/
            climbUp();
            return;
        }
        /* Sprite states

//If standing
if (health == 2)
    Used = Stand2;
else
    Used = Stand1;

//If Jumping
if (!thisPhys.isGrounded)
{
    if (health == 2)
        Used = Jump2;
    else
        Used = Jump1;
}*/

        //If I am pressing up while on the latter
        if (commandUp() && !crouching && thisPhys.isGrounded && onLadder)
        {
            //Debug.Log("going up");
            thisPhys.isGrounded = false;
            upLadder = true;
            stepUp = true;
        }

        //If I press down while on top of the ladder
        if (commandDown() && onLadderTop)
        {
            onLadder = true;
            //Debug.Log("going up");
            thisPhys.isGrounded = false;
            upLadder = true;
            stepUp = true;
            Vector3 temp = transform.position;
            temp.x = ladderVec.x;
            temp.y = transform.position.y - 1f;
            transform.position = temp;
        }


        if (commandLeft() && !upLadder)
        {
            sides = -1f;
        }
        if (commandRight() && !upLadder)
        {
            sides = 1f;
        }
        if (commandLeft() && !crouching && thisPhys.isGrounded && hitSide != 'l')
        {
            thisPhys.addVelocity(-speed, 0f);
        }

        if (commandRight() && !crouching && thisPhys.isGrounded && hitSide != 'r')
        {
            thisPhys.addVelocity(speed, 0f);
        }

        if (commandDown() && thisPhys.isGrounded)
        {
            crouching = true;
            Debug.Log (crouching);
        }
        if (commandDownRelease())
        {
            crouching = false;
            Debug.Log (crouching);
        }
        if (crouching)
        {
            if (!stop)
            {
                boxCollider2D.offset = crouchState2;
                boxCollider2D.size = crouchState1;

            }
            else
            {
                crouching = false;
                if (thisPhys.isGrounded)
                {
                    stop = false;
                }
            }
        }
        else
        {
            boxCollider2D.offset = standState2;
            boxCollider2D.size = standState1;
        }


    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        GameObject collidedWith = coll.gameObject;
        if (collidedWith.tag == "Wall" && collidedWith.GetComponent<PhysObj>().isObstacle)
        {
            if (collidedWith.transform.position.x > this.transform.position.x)
                hitSide = 'r';
            if (collidedWith.transform.position.x < this.transform.position.x)
                hitSide = 'l';
            Debug.Log (hitSide);
        }
        if (collidedWith.tag == "Ladder")
        {
            onLadder = true;
        }
        if ((collidedWith.tag == "Ground" || collidedWith.tag == "Platform") && upLadder && !commandUp() && !onLadderTop)
        {
            upLadder = false;
            onLadder = true;
        }
        if (collidedWith.tag == "Wall" && upLadder)
        {
            upLadder = false;
            onLadder = true;
        }
        if (collidedWith.tag == "LadderTop")
        {
            onLadderTop = true;
            ladderVec = collidedWith.transform.position;
        }
        if (collidedWith.tag == "Enemy")
        {
            
        }
    }

    void OnTriggerStay2D(Collider2D coll)
    {
        GameObject collidedWith = coll.gameObject;
        if (collidedWith.tag == "Ladder" && upLadder && stepUp)
        {
            //Debug.Log ("LadderOn");
            stepUp = false;
            transform.position = new Vector3(collidedWith.transform.position.x,
               transform.position.y + 0.1f, collidedWith.transform.position.z);
        }
        if (collidedWith.tag == "LadderTop" && upLadder && stepUp)
        {
            //Debug.Log ("LadderOn");
            stepUp = false;
            transform.position = new Vector3(collidedWith.transform.position.x,
                                             transform.position.y + 0.1f, collidedWith.transform.position.z);
        }
        if (collidedWith.tag == "Hostile")
        {
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        GameObject collidedWith = coll.gameObject;
        if (collidedWith.tag == "Wall")
        {
            //Debug.Log ("WallOff");
            hitSide = 'n';
        }
        if (collidedWith.tag == "Ladder")
        {
            //Debug.Log ("LadderOff");
            onLadder = false;
            upLadder = false;
        }
        if (collidedWith.tag == "LadderTop")
        {
            //Debug.Log ("off ladderTop");
            onLadderTop = false;
        }
    }

    bool commandUp()
    {
        if (PhysManager.isPaused)
            return false;
        return Input.GetKey(KeyCode.UpArrow) ||
               Input.GetKey(KeyCode.W);
    }

    bool commandDown()
    {
        if (PhysManager.isPaused)
            return false;
        return Input.GetKey(KeyCode.DownArrow) ||
               Input.GetKey(KeyCode.S);
    }

    bool commandDownRelease()
    {
        if (PhysManager.isPaused)
            return false;
        return Input.GetKeyUp(KeyCode.DownArrow) ||
               Input.GetKeyUp(KeyCode.S);
    }

    bool commandLeft()
    {
        if (PhysManager.isPaused)
            return false;
        return Input.GetKey(KeyCode.LeftArrow) ||
               Input.GetKey(KeyCode.A);
    }

    bool commandRight()
    {
        if (PhysManager.isPaused)
            return false;
        return Input.GetKey(KeyCode.RightArrow) ||
               Input.GetKey(KeyCode.D);
    }

    void climbUp()
    {
        thisPhys.setVelocity(new Vector2(0f, 0.5f));
        if (commandUp())
        {
            Debug.Log ("up");
            thisPhys.addVelocity(speed, 90f);
        }
        if (commandDown())
        {
            Debug.Log(thisPhys.isGrounded);
            if (thisPhys.isGrounded)
            {
                //Debug.Log ("grounded");
                upLadder = false;
            }
            Debug.Log ("down");
            thisPhys.addVelocity(-speed, 90f);
        }

    }
}


