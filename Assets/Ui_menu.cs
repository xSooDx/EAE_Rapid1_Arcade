using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Ui_menu : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(QuitButton);
    }

    public void StartButton()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitButton()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
