using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Academy.HoloToolkit.Unity;

[Serializable]
public class FacialStruct
{
    //BlendShapeValues
    public float _SapphiArtChanFacial_Eye_L_Happy;
    public float _SapphiArtChanFacial_Eye_R_Happy;
    public float _SapphiArtChanFacial_Eye_L_Closed;
    public float _SapphiArtChanFacial_Eye_R_Closed;
    public float _SapphiArtChanFacial_Eye_L_Wide;
    public float _SapphiArtChanFacial_Eye_R_Wide;
    
    public float _SapphiArtChanFacial_Mouth_Sad;
    public float _SapphiArtChanFacial_Mouth_Puff;
    public float _SapphiArtChanFacial_Mouth_Smile;
   
    public float _SapphiArtChanFacial_Eyebrow_L_Up;
    public float _SapphiArtChanFacial_Eyebrow_R_Up;
    public float _SapphiArtChanFacial_Eyebrow_L_Angry;
    public float _SapphiArtChanFacial_Eyebrow_R_Angry;
    public float _SapphiArtChanFacial_Eyebrow_L_Sad;
    public float _SapphiArtChanFacial_Eyebrow_R_Sad;
   
    public float _SapphiArtChanFacial_Mouth_E;
    public float _SapphiArtChanFacial_Mouth_O;
    public float _SapphiArtChanFacial_Mouth_JawOpen;
    public float _SapphiArtChanFacial_Mouth_Extra01;
    public float _SapphiArtChanFacial_Mouth_Extra02;
    public float _SapphiArtChanFacial_Mouth_Extra03;
    public float _SapphiArtChanFacial_Mouth_BottomTeeth;
   
    public bool _SapphiArtChanFacial_Mouth_TopTeeth;
    public bool _SapphiArtChanFacial_Mouth_Tongue;

}


public class CharacterManager : Singleton<CharacterManager>
{

    private SkinnedMeshRenderer _SapphiArtChanRenderer_Face;        //Character Skin Mesh Renderer for Face
    private SkinnedMeshRenderer _SapphiArtChanRenderer_Brow;        //Character Skin Mesh Renderer for Eyebrows
    private SkinnedMeshRenderer _SapphiArtChanRenderer_BottomTeeth;        //Character Skin Mesh Renderer for Bottom Teeth
    private SkinnedMeshRenderer _SapphiArtChanRenderer_Tongue;        //Character Tongue Skinned Mesh Renderer
    private SkinnedMeshRenderer _SapphiArtChanRenderer_TopTeeth;      //Character Top Teeth Skinned Mes

    public FacialStruct fs ;
    public bool isSpeaking=false;

    private bool isEyeClose = false;//防止眼睛闭上之后发现来音频合不上了
    private bool isMouthDown = false;
    void Start()
    {
        Transform[] SapphiArtchanChildren = GetComponentsInChildren<Transform>();

        foreach (Transform t in SapphiArtchanChildren)
        {
            if (t.name == "face")
                _SapphiArtChanRenderer_Face = t.gameObject.GetComponent<SkinnedMeshRenderer>();
            if (t.name == "brow")
                _SapphiArtChanRenderer_Brow = t.gameObject.GetComponent<SkinnedMeshRenderer>();
            if (t.name == "BottomTeeth")
                _SapphiArtChanRenderer_BottomTeeth = t.gameObject.GetComponent<SkinnedMeshRenderer>();
            if (t.name == "tongue")
                _SapphiArtChanRenderer_Tongue = t.gameObject.GetComponent<SkinnedMeshRenderer>();
            if (t.name == "TopTeeth")
                _SapphiArtChanRenderer_TopTeeth = t.gameObject.GetComponent<SkinnedMeshRenderer>();
        }
        fs._SapphiArtChanFacial_Eye_L_Happy = 50;
        fs._SapphiArtChanFacial_Mouth_Smile = 100;
        fs._SapphiArtChanFacial_Mouth_E = 10;
        fs._SapphiArtChanFacial_Mouth_Extra02 = 15;
        InvokeRepeating("RightEyeClose", 1, 5f);
        InvokeRepeating("RightEyeOpen", 1.5f, 5f);
        InvokeRepeating("MouthDown", 0, 0.5f);
        InvokeRepeating("MouthUp", 0.3f, 0.5f);
    }


    void LateUpdate()
    {
        //解决animator覆盖blendshape的问题.
        SetFacial();
        
    }

    public void MouthDown()
    {
        if (isSpeaking)
        {
            fs._SapphiArtChanFacial_Mouth_O = 30;
            isMouthDown = true;
        }
        
    }
    public void MouthUp()
    {
        if (isSpeaking||isMouthDown==true)
        {
            fs._SapphiArtChanFacial_Mouth_O = 0;
            isMouthDown = false;
        }
      
    }


    public void RightEyeClose()
    {
        if (isSpeaking == false)
        {
            fs._SapphiArtChanFacial_Eye_R_Closed = 100;
            isEyeClose = true;
        }
            
        //yield return new WaitForSeconds(0.1f);
        //fs._SapphiArtChanFacial_Eye_R_Closed = 0;
    }
    public void RightEyeOpen()
    {
        if (isSpeaking == false||isEyeClose==true)//眼睛闭上了一定要再睁开,不要来音频就不睁开了
        {
            fs._SapphiArtChanFacial_Eye_R_Closed = 0;
            isEyeClose = false;
        }
       
        //yield return new WaitForSeconds(0.1f);
        //fs._SapphiArtChanFacial_Eye_R_Closed = 0;
    }

    //设置表情参数
    void SetFacial()
    {
        _SapphiArtChanRenderer_Face.SetBlendShapeWeight(0, fs._SapphiArtChanFacial_Eye_L_Happy);
        _SapphiArtChanRenderer_Face.SetBlendShapeWeight(1, fs._SapphiArtChanFacial_Eye_R_Happy);
        _SapphiArtChanRenderer_Face.SetBlendShapeWeight(4, fs._SapphiArtChanFacial_Eye_L_Closed);
        _SapphiArtChanRenderer_Face.SetBlendShapeWeight(5, fs._SapphiArtChanFacial_Eye_R_Closed);
        _SapphiArtChanRenderer_Face.SetBlendShapeWeight(2, fs._SapphiArtChanFacial_Eye_L_Wide);
        _SapphiArtChanRenderer_Face.SetBlendShapeWeight(3, fs._SapphiArtChanFacial_Eye_R_Wide);
                                                          
        _SapphiArtChanRenderer_Brow.SetBlendShapeWeight(0, fs._SapphiArtChanFacial_Eyebrow_L_Up);
        _SapphiArtChanRenderer_Brow.SetBlendShapeWeight(1, fs._SapphiArtChanFacial_Eyebrow_R_Up);
        _SapphiArtChanRenderer_Brow.SetBlendShapeWeight(2, fs._SapphiArtChanFacial_Eyebrow_L_Angry);
        _SapphiArtChanRenderer_Brow.SetBlendShapeWeight(3, fs._SapphiArtChanFacial_Eyebrow_R_Angry);
        _SapphiArtChanRenderer_Brow.SetBlendShapeWeight(4, fs._SapphiArtChanFacial_Eyebrow_L_Sad);
        _SapphiArtChanRenderer_Brow.SetBlendShapeWeight(5, fs._SapphiArtChanFacial_Eyebrow_R_Sad);
                                                          
        _SapphiArtChanRenderer_Face.SetBlendShapeWeight(6, fs._SapphiArtChanFacial_Mouth_E);
        _SapphiArtChanRenderer_Face.SetBlendShapeWeight(8, fs._SapphiArtChanFacial_Mouth_O);
        _SapphiArtChanRenderer_Face.SetBlendShapeWeight(7, fs._SapphiArtChanFacial_Mouth_JawOpen);
        _SapphiArtChanRenderer_Face.SetBlendShapeWeight(12,fs. _SapphiArtChanFacial_Mouth_Extra01);
        _SapphiArtChanRenderer_Face.SetBlendShapeWeight(13,fs. _SapphiArtChanFacial_Mouth_Extra02);
        _SapphiArtChanRenderer_Face.SetBlendShapeWeight(14,fs. _SapphiArtChanFacial_Mouth_Extra03);
                                                          
        _SapphiArtChanRenderer_Face.SetBlendShapeWeight(9, fs._SapphiArtChanFacial_Mouth_Sad);
        _SapphiArtChanRenderer_Face.SetBlendShapeWeight(10,fs. _SapphiArtChanFacial_Mouth_Puff);
        _SapphiArtChanRenderer_Face.SetBlendShapeWeight(11,fs. _SapphiArtChanFacial_Mouth_Smile);

        if (_SapphiArtChanRenderer_BottomTeeth.isVisible)
            _SapphiArtChanRenderer_BottomTeeth.SetBlendShapeWeight(0, fs._SapphiArtChanFacial_Mouth_BottomTeeth);
        _SapphiArtChanRenderer_TopTeeth.enabled = fs._SapphiArtChanFacial_Mouth_TopTeeth;
        _SapphiArtChanRenderer_Tongue.enabled = fs._SapphiArtChanFacial_Mouth_Tongue;

    }
}

