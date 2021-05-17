using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(CuteDuck))]

public class CuteDuckEditor : Editor
{

    private void OnSceneGUI()
    {
        CuteDuck fow = (CuteDuck)target;
        Handles.color = Color.black;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.viewRadius);

        Vector3 viewAngleA = fow.dirFromAngle(-fow.viewAngle / 2, false);
        Vector3 viewAngleB = fow.dirFromAngle(fow.viewAngle / 2, false);

        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.viewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.viewRadius);

        Handles.color = Color.red;
        foreach (Transform visibleObstacles in fow.visibleObstacles){
            Handles.DrawLine(fow.transform.position, visibleObstacles.position);
        }
    }
}
