using BT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class BaseBT : MonoBehaviour
{
    private BaseNode root = null;

    // Start is called before the first frame update
    void Start()
    {
        root = InitializeBehaviourTree();
    }

    // Update is called once per frame
    void Update()
    {
        if (root != null)
        {
            root.Process();
        }
    }

    protected abstract BaseNode InitializeBehaviourTree();
}