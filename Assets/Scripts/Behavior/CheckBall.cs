using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBall : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Quaffle") {
            transform.parent.parent.GetComponent<Role>().cachedQuaffle = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Quaffle")
        {
            transform.parent.parent.GetComponent<Role>().cachedQuaffle = null;
        }
    }
}
