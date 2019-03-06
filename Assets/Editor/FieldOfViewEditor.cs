using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    void OnSceneGUI()
    {

        FieldOfView fov = (FieldOfView)target;
        Handles.color = Color.black;
        Vector3 viewAngleA = fov.DirFromAngle(-fov.viewAngle /2, false);
        Vector3 viewAngleB = fov.DirFromAngle(fov.viewAngle / 2, false);
        Vector3 viewAngleC = fov.DirFromAngle(fov.viewAngle, false);
        //float Angle = Vector3.Angle(viewAngleB, viewAngleC);
        Debug.Log(viewAngleA);
        Debug.Log(viewAngleB);

        Debug.Log(viewAngleC);

        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.viewRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.viewRadius);
        // Handles.DrawLine(fov.transform.position, fov.transform.position * fov.viewRadius);

        Color flashLight = Color.yellow;
        flashLight.a = 0.1f;
        Handles.color = flashLight;
       // Handles.DrawSolidArc(fov.transform.position, Vector3.Angle, Vector3.right, fov.viewAngle, fov.viewRadius);

        foreach (Transform visibleTarget in fov.visibleTargets)
        {
            Handles.DrawLine(fov.transform.position, visibleTarget.position);
        }
    }
}
