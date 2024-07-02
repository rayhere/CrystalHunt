using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FocusOnSelectedObject : MonoBehaviour
{
    #if UNITY_EDITOR
    void Update()
    {
        if (EditorApplication.isPlaying && Selection.activeGameObject != null)
        {
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView != null)
            {
                // Focus the Scene view camera on the selected GameObject
                sceneView.LookAt(Selection.activeGameObject.transform.position);
            }
        }
    }
    #endif
}
