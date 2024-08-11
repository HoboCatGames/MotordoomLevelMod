using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(CurveArray))]
public class CurveArrayEditor : Editor
{
    private CurveArray curveArray;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        curveArray = target as CurveArray;
		//DrawDefaultInspector();

		if (GUILayout.Button("Arrange Objects"))
		{
			Undo.RecordObject(curveArray, "Add Curve");
			curveArray.ArrangeObjects();
			EditorUtility.SetDirty(curveArray);
		}
    }
}
