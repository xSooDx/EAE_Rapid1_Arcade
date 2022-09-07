using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ClawAnimationControl))]
public class ClawAnimationControlEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ClawAnimationControl animationControl = (ClawAnimationControl)target;
        if (GUILayout.Button("Explotion"))
        {
            Debug.Log("Boom~~~");
            animationControl.Explosion();
        }

        if (GUILayout.Button("Reset"))
        {
            animationControl.Resetitem();
        }
    }
}
