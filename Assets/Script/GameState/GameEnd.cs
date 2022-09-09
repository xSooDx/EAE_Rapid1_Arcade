
using UnityEngine.SceneManagement;

public class GameEnd : GameEndAction
{
    public override void StartAction()
    {
        if (gameController != null)
        {
            gameController.TimerAction(false);
        }

    }

    public override void EndAction()
    {
        if (gameController != null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
