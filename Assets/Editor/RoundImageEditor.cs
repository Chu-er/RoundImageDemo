using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(RoundImage), true)]
public class RoundImageEditor : ImageEditor
{

    SerializedProperty m_Raius;
    SerializedProperty m_TriangleNum;
    SerializedProperty m_Sprite;


    protected override void OnEnable()
    {
        base.OnEnable();
        m_Raius = serializedObject.FindProperty("Radius");
        m_TriangleNum = serializedObject.FindProperty("TrinagleNum");
        m_Sprite = serializedObject.FindProperty("m_Sprite");
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        SpriteGUI();
        AppearanceControlsGUI();
        RaycastControlsGUI();
        bool showNativeSize = m_Sprite.objectReferenceValue != null;
        m_ShowNativeSize.target = showNativeSize;
        NativeSizeButtonGUI();
        EditorGUILayout.PropertyField(m_Raius);
        EditorGUILayout.PropertyField(m_TriangleNum);
        serializedObject.ApplyModifiedProperties();

    }

}

public class ProgressShaderGUI : ShaderGUI
{
    MaterialEditor _materialEditor;
    MaterialProperty[] _materalProperty;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        base.OnGUI(materialEditor, properties);
        this._materialEditor = materialEditor;
        this._materalProperty = properties;
        MaterialProperty fillAmount_1 = FindProperty("_FillAmountOne", properties);
        MaterialProperty fillAmount_2 = FindProperty("_FillAmountTwo", properties);
        MaterialProperty fillAmount_3 = FindProperty("_FillAmountThree", properties);

        float v1 = fillAmount_1.floatValue;
        float v2 = fillAmount_2.floatValue;
        float v3 = fillAmount_3.floatValue;

        //TODO 三个值互相影响
        int count = 0;
        if (v1!=0) count++;
        if (v2 != 0) count++;
        if (v3 != 0) count++;

        if (count>1)
        {



        }
    }
}
