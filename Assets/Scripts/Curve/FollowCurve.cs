using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCurve : MonoBehaviour
{
	private Transform _transform;
	public Curve curve;

	public float speed = 0.1f;

	[Range(0f, 1f)]
	public float curvePosition;

    // Start is called before the first frame update
    void Start()
    {
		this._transform = this.transform;

        if (this.curve != null)
		{
			this._transform.position = this.curve.GetPoint(curvePosition);
			this._transform.LookAt(this._transform.position + this.curve.GetDirection(curvePosition));
		}
		
	}

	private void FixedUpdate()
	{
		if (Input.GetAxis("Vertical") != 0)
			this.Move(Input.GetAxis("Vertical") * this.speed);
	}

	public void Move(float value)
	{
		this.curvePosition = Mathf.Clamp01(this.curvePosition + (value * Time.fixedDeltaTime));
		if (this.curve != null)
		{
			this._transform.position = this.curve.GetPoint(curvePosition);
			this._transform.LookAt(this._transform.position + this.curve.GetDirection(curvePosition));
		}
	}
}
