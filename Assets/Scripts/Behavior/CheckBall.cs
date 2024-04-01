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

        if(other.tag == "GoldenSnitch")
        {
            transform.parent.parent.GetComponent<Role>().cachedGoldenSnitch = other.gameObject;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Quaffle")
        {
            transform.parent.parent.GetComponent<Role>().cachedQuaffle = null;
        }

        if(other.tag == "GoldenSnitch")
        {
            transform.parent.parent.GetComponent<Role>().cachedGoldenSnitch = null;
        }
    }
}
