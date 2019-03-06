using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool detectedPlayer = true;

    private PhysObj phys;

    // Start is called before the first frame update
    void Start()
    {
         phys = this.gameObject.GetComponent<PhysObj>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
