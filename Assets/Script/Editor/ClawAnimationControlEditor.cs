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
            animationControl.Explosion(new Vector2(Random.Range(-10,10),Random.Range(1,10)));
        }

        if (GUILayout.Button("Reset"))
        {
            animationControl.Resetitem();
        }
    }
}
