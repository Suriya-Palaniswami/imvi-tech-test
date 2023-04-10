using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ExperienceBuilder))]
public class ObjectEditor : Editor
{
    ExperienceBuilder handler;
    public override void OnInspectorGUI()
    {
        handler = (ExperienceBuilder)target;
        base.OnInspectorGUI();
        if (handler.isNonLinear)
        {
            if (GUILayout.Button("Load All Sections", GUILayout.Height(25)))
            {
                handler.PopulateImviObjectsForEditor();
            }
            if (GUILayout.Button("Play", GUILayout.Height(25)))
            {
                handler.PlayOneByOne();
            }
        }
        

    }
}
