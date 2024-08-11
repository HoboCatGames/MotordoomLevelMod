using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveArray : MonoBehaviour
{
    public Curve curve;
    public bool neutralizeYRotation = false;
    public Transform[] objects;

    void Start(){
        Destroy(this);
    }

    public void ArrangeObjects()
    {
        if (this.curve != null && this.objects != null && this.objects.Length > 0)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] != null)
                {
                    objects[i].position = this.curve.GetPoint((float)i / (float)(this.objects.Length - 1f));
                    Vector3 direction = this.curve.GetDirection((float)i / (float)(this.objects.Length - 1f));
                    if (neutralizeYRotation)
                        direction.y = 0;
                    objects[i].forward = direction;
                }
            }
        }
    }
}
