using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class GizmoUtil : MonoBehaviour
{
   [SerializeField] private Color _color;
   [SerializeField] private float _radius = 1;
   [SerializeField] private string _labelStr = "";
   [SerializeField] private bool _useName = true;
   
   private void OnDrawGizmos()
   {
      #if UNITY_EDITOR
      Handles.Label(transform.position, _useName?this.name:_labelStr);
      #endif

      Gizmos.color = _color;
      Gizmos.DrawSphere(transform.position, _radius);
   }
}
