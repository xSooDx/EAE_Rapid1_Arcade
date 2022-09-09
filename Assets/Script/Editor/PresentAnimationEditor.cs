using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PresentAnimationControl))]
public class PresentAnimationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PresentAnimationControl animationControl = (PresentAnimationControl)target;
        if (GUILayout.Button("Open"))
        {
            Debug.Log("Yee");
            animationControl.OpenPresent();
        }

        
    }
}
