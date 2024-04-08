using UnityEditor;
using UnityEngine;

namespace Utils
{
    public class CameraPhotoTaker : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        [MenuItem("Utils/Take Photo")]
        public static void TakePhoto()
        {
            var cpt = FindObjectOfType<CameraPhotoTaker>();
        }
    }
}