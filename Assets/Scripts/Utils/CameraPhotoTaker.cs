#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace Utils
{
    public class CameraPhotoTaker : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        #if UNITY_EDITOR
        [MenuItem("Utils/Take Photo")]
        public static void TakePhoto()
        {
            var rt = new RenderTexture(1920, 1080, 24);
            var sc = new Texture2D(1920, 1080, TextureFormat.RGB24, false);
            var cpt = FindObjectOfType<CameraPhotoTaker>();
            cpt._camera.targetTexture = rt;
            RenderTexture.active = rt;
            cpt._camera.Render();
            sc.ReadPixels(new Rect(0,0,1920,1080), 0,0);
            cpt._camera.targetTexture = null;
            RenderTexture.active = null;
            var bytes = sc.EncodeToPNG();
            var filename = EditorUtility.SaveFilePanel("save", "", "", "");
            System.IO.File.WriteAllBytes(filename, bytes);
        }
        #endif
    }
}