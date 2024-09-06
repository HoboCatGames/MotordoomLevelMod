using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gaps
{ 
    public class Gap : MonoBehaviour
    {
        public bool triggered = false;

        public delegate void GapHandler();
        public event GapHandler GapTriggered;
        public virtual void Triggered()
        {
            triggered = true;
            GapTriggered?.Invoke();
        }
    }
}

