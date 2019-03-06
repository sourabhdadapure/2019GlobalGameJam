using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    private Transform handle;
    //detection behavior
    public const float detectedTimeThreshold = 1.2f;

    private bool detected = false;
    private float detectedTimer = 0.0f;


    void Start()
    {
        handle = this.transform.parent.transform;
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Enemy Found Player ");
        }
    }
    
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (detectedTimer >= detectedTimeThreshold)
            {
                SceneManager.LoadScene("GameOverScreen");
            }
            detectedTimer += Time.deltaTime;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Enemy Exit Player ");
            detectedTimer = 0;
        }
    }

}
