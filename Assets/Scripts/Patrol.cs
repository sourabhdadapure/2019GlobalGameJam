using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    //Determine enemy will patrol a route
    public bool isComplexPatrol = true;
    public List<string> PatternList;
    public List<float> patternDurations;

    private int patternInd = -1;
    private float patternTimer = 0.0f;

    public bool isSimplePatrol = true;
    public float patrolLen = 4f;
    public float speed = 2f;
    public float rotationSpeed = 0f;

    bool isStopped = false;

    public float fastMod = 1.5f;
    public bool isFast = false;
    
    private bool movingRight = true;
    private Vector3 initialPosition;

    private CameraMovements flashLight;

    void Start()
    {
        initialPosition = this.transform.position;
    }

    void Update()
    {
        flashLight = this.GetComponentInChildren<CameraMovements>();

       // RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, 2f);
        //flashLight.rotationSpeed = rotationSpeed;
        if (isComplexPatrol)
        {
            if (patternTimer <= 0.0f)
            {
                if (PatternList.Count-1 == patternInd)
                {
                    patternInd = 0;
                }
                else
                {
                    patternInd++;
                }
                patternTimer = patternDurations[patternInd];
            }
            if (PatternList[patternInd] != "stop")
            {
                transform.Translate(Vector3.right * speed * Time.deltaTime * (isFast == true ? fastMod : 1.0f));
            }


            string patternName = PatternList[patternInd];
            if(patternName == "left")
            {
                transform.eulerAngles = new Vector3(0, -180, 0);
            }
            else if (patternName == "right")
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else
            {

            }


            patternTimer -= Time.deltaTime;
        }
        else if(isSimplePatrol)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime * (isFast == true ? fastMod : 1.0f));

            Vector3 currentPos = this.transform.position;
            Debug.Log(currentPos.x);
            Debug.Log(initialPosition.x);

            if (initialPosition.x+patrolLen <= currentPos.x)
            {
                transform.eulerAngles = new Vector3(0, -180, 0);
                movingRight = false;
            }
            else if(initialPosition.x-patrolLen >= currentPos.x)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                movingRight = true;
            }
        }

        /*if (groundInfo.collider== false)
        {
            if (movingRight == true)
            {
                transform.eulerAngles = new Vector3(0, -180, 0);
                movingRight = false;
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                movingRight = true;
            }
        }*/
    }

}
