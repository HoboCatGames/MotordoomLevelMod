using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(CubicBezierSpline))]
public class CubicBezierSplineEditor : Editor
{
	private bool _drawDefaultInspector = false;
	private CubicBezierSpline spline;
	private Transform handleTransform;
	private Quaternion handleRotation;
	private const float handleSize = 0.4f;
	private const float pickSize = 0.6f;

	private int selectedIndex = -1;

	private const float directionScale = 0.5f;

	private static Color[] modeColors = {
		Color.white,
		Color.yellow,
		Color.cyan
	};

	public override void OnInspectorGUI()
	{
		spline = target as CubicBezierSpline;
		this._drawDefaultInspector = GUILayout.Toggle(this._drawDefaultInspector, "Draw Default Inspector");

		if (this._drawDefaultInspector)
			DrawDefaultInspector();

		EditorGUI.BeginChangeCheck();
		/*bool loop = EditorGUILayout.Toggle("Loop", spline.Loop);
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(spline, "Toggle Loop");
			EditorUtility.SetDirty(spline);
			spline.Loop = loop;
		}
		*/
		if (selectedIndex >= 0 && selectedIndex < spline.ControlPointCount)
		{
			DrawSelectedPointInspector();
			Repaint();
		}

		if (GUILayout.Button("Add Point"))
		{
			Undo.RecordObject(spline, "Add Point");
			spline.AddPoint();
			EditorUtility.SetDirty(spline);
		}
		if (GUILayout.Button("Remove Point"))
		{
			Undo.RecordObject(spline, "Remove Point");
			spline.RemoveLastPoint();
			EditorUtility.SetDirty(spline);
		}
	}

	private void OnSceneGUI()
	{
		spline = target as CubicBezierSpline;
		handleTransform = spline.transform;
		handleRotation = Tools.pivotRotation == PivotRotation.Local ?
			handleTransform.rotation : Quaternion.identity;

		Vector3 p0 = ShowPoint(0);
		for (int i = 1; i < spline.ControlPointCount; i += 3)
		{
			Vector3 p1 = ShowPoint(i);
			Vector3 p2 = ShowPoint(i + 1);
			Vector3 p3 = ShowPoint(i + 2);

			Handles.color = Color.gray;
			Handles.DrawLine(p0, p1);
			Handles.DrawLine(p2, p3);

			Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
			p0 = p3;
		}
		//ShowDirections();
	}

	private void ShowDirections()
	{
		Handles.color = Color.green;
		Vector3 point = spline.GetPoint(0f);
		Handles.DrawLine(point, point + spline.GetDirection(0f) * directionScale);
		int steps = spline.arcSegments * spline.CurveCount;
		for (int i = 1; i <= steps; i++)
		{
			point = spline.GetPoint(i / (float)steps);
			Handles.DrawLine(point, point + spline.GetDirection(i / (float)steps) * directionScale);
		}
	}


	private void DrawSelectedPointInspector()
	{
		GUILayout.Label("Selected Point");
		EditorGUI.BeginChangeCheck();
		Vector3 point = EditorGUILayout.Vector3Field("Position", spline.GetControlPoint(selectedIndex));
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(spline, "Move Point");
			EditorUtility.SetDirty(spline);
			spline.SetControlPoint(selectedIndex, point);
		}

		EditorGUI.BeginChangeCheck();
		BezierControlPointMode mode = (BezierControlPointMode)
			EditorGUILayout.EnumPopup("Mode", spline.GetControlPointMode(selectedIndex));
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(spline, "Change Point Mode");
			spline.SetControlPointMode(selectedIndex, mode);
			EditorUtility.SetDirty(spline);
		}
	}

	private Vector3 ShowPoint(int index)
	{
		Vector3 point = handleTransform.TransformPoint(spline.GetControlPoint(index));
		Handles.color = modeColors[(int)spline.GetControlPointMode(index)];
		if (Handles.Button(point, handleRotation, handleSize, pickSize, Handles.DotHandleCap))
		{
			selectedIndex = index;
		}
		if (selectedIndex == index)
		{
			EditorGUI.BeginChangeCheck();
			point = Handles.DoPositionHandle(point, handleRotation);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(spline, "Move Point");
				EditorUtility.SetDirty(spline);
				spline.SetControlPoint(index, handleTransform.InverseTransformPoint(point));

				/*if (!spline.freeHandles)
				{
					//no the start or end point
					int handleType = (index - 1) % 3;
					int otherHandleIndex;
					int centerHandleIndex;
					float direnctionLength;
					Vector3 lastPosition;

					switch (handleType)
					{
						case 0:
							//first handle
							otherHandleIndex = index - 2;
							centerHandleIndex = index - 1;
							if (otherHandleIndex > 0)
							{
								direnctionLength = Vector3.Distance(spline.GetControlPoint(centerHandleIndex), spline.GetControlPoint(otherHandleIndex));
								spline.SetControlPoint(index, handleTransform.InverseTransformPoint(point));
								spline.SetControlPoint(otherHandleIndex, spline.GetControlPoint(centerHandleIndex) + ((spline.GetControlPoint(centerHandleIndex) - spline.GetControlPoint(index)).normalized * direnctionLength));
							}
							spline.SetControlPoint(index, handleTransform.InverseTransformPoint(point));
							break;
						case 1:
							//second handle
							otherHandleIndex = index + 2;
							centerHandleIndex = index + 1;
							if (otherHandleIndex < spline.ControlPointCount)
							{
								direnctionLength = Vector3.Distance(spline.GetControlPoint(centerHandleIndex), spline.GetControlPoint(otherHandleIndex));
								spline.SetControlPoint(index, handleTransform.InverseTransformPoint(point));
								spline.SetControlPoint(otherHandleIndex, spline.GetControlPoint(centerHandleIndex) + ((spline.GetControlPoint(centerHandleIndex) - spline.GetControlPoint(index)).normalized * direnctionLength));
							}
							spline.SetControlPoint(index, handleTransform.InverseTransformPoint(point));
							break;
						case 2:
							//end point
							otherHandleIndex = index - 1;
							centerHandleIndex = index + 1;
							lastPosition = spline.GetControlPoint(index);
							spline.SetControlPoint(index, handleTransform.InverseTransformPoint(point));

							spline.SetControlPoint(otherHandleIndex, spline.GetControlPoint(otherHandleIndex) + spline.GetControlPoint(index) - lastPosition);
							if (centerHandleIndex < spline.ControlPointCount)
								spline.SetControlPoint(centerHandleIndex, spline.GetControlPoint(centerHandleIndex) + spline.GetControlPoint(index) - lastPosition);
							break;
						case -1:
							//first point in spline
							otherHandleIndex = index + 1;
							lastPosition = spline.GetControlPoint(index);
							spline.SetControlPoint(index, handleTransform.InverseTransformPoint(point));
							spline.SetControlPoint(otherHandleIndex, spline.GetControlPoint(otherHandleIndex) + spline.GetControlPoint(index) - lastPosition);
							break;
					}
				}
				else
					spline.SetControlPoint(index, handleTransform.InverseTransformPoint(point));*/
			}
		}
		return point;
	}
}
