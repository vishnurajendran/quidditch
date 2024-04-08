using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FakeTextShadow : MonoBehaviour
{
    [SerializeField] private TMP_Text main;

    private TMP_Text me;

    private void Start()
    {
        me = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        me.text = main.text;
    }
}
