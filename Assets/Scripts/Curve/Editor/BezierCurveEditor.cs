using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierCurve))]
public class BezierCurveEditor : Editor
{

    private BezierCurve curve;
    private Transform handleTransform;
    private Quaternion handleRotation;

    private const int lineSteps = 10;

    private const float handleSize = 0.4f;
    private const float pickSize = 0.6f;

    private int selectedIndex = -1;

	private const float directionScale = 0.2f;


	public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        curve = target as BezierCurve;
        if (GUILayout.Button("Add Curve"))
        {
            Undo.RecordObject(curve, "Add Curve");
            curve.AddCurve();
            EditorUtility.SetDirty(curve);
        }
    }

    private void OnSceneGUI()
    {
        curve = target as BezierCurve;
        handleTransform = curve.transform;
        handleRotation = Tools.pivotRotation == PivotRotation.Local ?
            handleTransform.rotation : Quaternion.identity;

        if (curve.points.Length <= 0)
            return;

        Vector3 p0 = ShowPoint(0);


        for (int i = 1; i < curve.points.Length; i += 2)
        {
            if (curve.points.Length <= i + 1 || curve.points.Length <= i)
                return;
            Vector3 p1 = ShowPoint(i);
            Vector3 p2 = ShowPoint(i+1);

            Handles.color = Color.grey;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p1, p2);

            Vector3 lineStart = curve.GetPoint(i-1, 0f);
            for (int j = 1; j <= lineSteps; j++)
			{
				Handles.color = Color.white;
				Vector3 lineEnd = curve.GetPoint(i-1,j / (float)lineSteps);
                Handles.DrawLine(lineStart, lineEnd);


				Handles.color = Color.green;
				Handles.DrawLine(lineStart, lineStart + curve.GetDirection(i / (float)lineSteps) * directionScale);

				lineStart = lineEnd;
            }
            p0 = p2;
        }

		//Handles.DrawWireCube(curve.GetPoint(curve.t), Vector3.one);
	}

	private void ShowDirections()
	{
		Handles.color = Color.green;
		Vector3 point = curve.GetPoint(0f);
		Handles.DrawLine(point, point + curve.GetDirection(0f) * directionScale);
		for (int i = 1; i <= lineSteps; i++)
		{
			point = curve.GetPoint(i / (float)lineSteps);
			Handles.DrawLine(point, point + curve.GetDirection(i / (float)lineSteps) * directionScale);
		}
	}

	private Vector3 ShowPoint(int index)
    {
        Vector3 point = handleTransform.TransformPoint(curve.points[index]);
        Handles.color = Color.red;
        if (Handles.Button(point, handleRotation, handleSize, pickSize, Handles.CubeHandleCap))
        {
            selectedIndex = index;
        }
        if (selectedIndex == index)
        {
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(curve, "Move Point");
                EditorUtility.SetDirty(curve);
                curve.points[index] = handleTransform.InverseTransformPoint(point);
            }
        }
        return point;
    }
}