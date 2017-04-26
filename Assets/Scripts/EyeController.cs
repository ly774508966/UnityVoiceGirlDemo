
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeController : MonoBehaviour
{

    public GameObject target;
    public Transform LeftEyePivot;
    public Transform RightEyePivot;

    private Quaternion originLeftEyeRot;
    private Quaternion originRightEyeRot;
    private Vector3 originLeftEyeDir;
    private Vector3 originRightEyeDir;

    void Start ()
	{
        originLeftEyeRot = LeftEyePivot.transform.rotation;
        originRightEyeRot = RightEyePivot.transform.rotation;
        originLeftEyeDir = LeftEyePivot.forward;
        originRightEyeDir = RightEyePivot.forward;
    }

    private float GetAngle(float angle)
    {
        if (angle < 180 && angle >= 0)
        {
            return angle;
        }
        else if (angle >= 180 && angle <= 360)
        {
            return Mathf.Abs(angle - 360);
        }
        return 0;
    }

    void Update () {
        if (LeftEyePivot != null && RightEyePivot != null)
        {
            SetCurrentAngle(LeftEyePivot, originLeftEyeDir, originLeftEyeRot);

            SetCurrentAngle(RightEyePivot,originRightEyeDir,originRightEyeRot);
        }
    }

    public void SetCurrentAngle(Transform eye,Vector3 originalEyeDir,Quaternion originRot)
    {
        Quaternion currentEyeAngle = Quaternion.FromToRotation(originalEyeDir,
                          target.transform.position - eye.transform.position);
        //Debug.Log(currentEyeAngle);
        //Debug.Log(currentEyeAngle.eulerAngles.x+"_________________"+ currentEyeAngle.eulerAngles.y);
        if (GetAngle(currentEyeAngle.eulerAngles.x) < 20 && GetAngle(currentEyeAngle.eulerAngles.y) < 30)
        {
            if(eye==LeftEyePivot)
            eye.transform.rotation = originRot * currentEyeAngle;
            else
            eye.transform.rotation = originRot * Quaternion.Inverse(currentEyeAngle) ;
        }
    }
    void LateUpdate()
    {
        
       
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(LeftEyePivot.transform.position,target.transform.position);
        Gizmos.DrawLine(RightEyePivot.transform.position,target.transform.position);
    }
}

