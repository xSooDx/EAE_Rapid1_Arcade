using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameResultScript))]
public class GameResultScript_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GameResultScript grs = (GameResultScript)target;
        if (GUILayout.Button("Show Result"))
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
#endif
            {
                grs.ResultPanel.localScale = Vector2.zero;
                grs.ShowResult();
            }
        }


    }
}
