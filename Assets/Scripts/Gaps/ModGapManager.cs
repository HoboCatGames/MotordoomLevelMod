using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gaps
{
    public class ModGapManager : MonoBehaviour
    {
        public List<Gap> gaps;
        public string gapName;
        public int gapPoints;
        public UnityEvent[] eventsOnTrigger;
    }
}
