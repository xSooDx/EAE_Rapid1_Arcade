using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Ui_menu : MonoBehaviour

{
    public GameObject control;
    // Start is called before the first frame update
    private void Start()
    {
        if (this.GetComponent<Button>() != null)
            this.GetComponent<Button>().onClick.AddListener(QuitButton);
    }

    public void StartButton()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public void ControlScreen()
    {
        control.SetActive(true);
    }
}
