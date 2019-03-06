using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class CameraMovements : MonoBehaviour
{
    public float zRotationRange = 30.0f;
    private Transform initialTransform;
    public float rotationSpeed = 0.1f;
    private float curSpeed;
    public float initx;
    public float currentRotation = 0.0f;

    void Start()
    {
        initialTransform = this.transform;
        curSpeed = rotationSpeed;
        initx = initialTransform.eulerAngles.x;
    }
    //
    // Update is called once per frame
    void FixedUpdate()
    {
        if(currentRotation >= zRotationRange)
        {
            curSpeed = -1* rotationSpeed;
        }
        else if(currentRotation <= 0)
        {
            curSpeed = rotationSpeed;
        }

        currentRotation += curSpeed;
        transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, this.transform.eulerAngles.y, currentRotation + initx);
        
    }
}
