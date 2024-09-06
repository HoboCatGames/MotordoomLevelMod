using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gaps
{
    public class TransferGap : Gap
    {
        private void OnTriggerExit(Collider other)
        {
            CheckIfPlayer(other);
        }
        private void OnTriggerEnter(Collider other)
        {
            CheckIfPlayer(other);
        }

        private void CheckIfPlayer(Collider other)
        {
            if (other.tag == "Player")
            {
                Triggered();
            }
        }
    }
}

