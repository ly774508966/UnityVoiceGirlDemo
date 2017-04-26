//目前有个模型的眼珠是不动的,我想绕眼珠绕着某个点进行转,模仿人眼的转动
//但是要确定点,需要先建一个空的父物体,眼珠作为子物体,父物体不断调整位置
//但是父物体调整位置也就相对的把眼珠的初始位置给改了,所以还得重新调回眼珠的位置
//这个脚本是:只要父物体位置改变了,子物体变回初始的位置.
//父物体旋转,子物体跟随旋转.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TestParentPivot : MonoBehaviour
{

    private Vector3 originPos;

    public Transform child;
    private bool isChildNull = true;
    private Transform originParent;
    private Vector3 childPos;
    private Quaternion childRot;

    public void Start()
    {
        Debug.Log("Start");
    }

    private void Awake()
    {
        Debug.Log("Awake");

    }

    private void OnEnable()
    {
        Debug.Log("Enable");

    }




    void Update()
    {
        if(child==null)
            return;
           //初始化start
        if(isChildNull&&child!=null)
        {
            isChildNull = false;
            originPos = transform.position;
            childPos = child.position;
            childRot = child.rotation;
           
            if (child.parent == transform)
                originParent = transform.parent;
            else
                originParent = child.parent;
        }
        Debug.Log("child's origin postion is "+childPos);
        if (transform.position != originPos)
        {
            originPos = transform.position;
            child.position = childPos;
            child.rotation = childRot;
            child.parent =  originParent;
            return;
        }
        child.parent = transform;

    }

    
}
