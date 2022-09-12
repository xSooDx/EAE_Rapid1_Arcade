
using UnityEngine.SceneManagement;

public class GameEnd : GameEndAction
{
    public override void StartAction()
    {
        base.StartAction();
        if (gameController != null)
        {
            gameController.TimerAction(false);
        }

    }

    public override void EndAction()
    {
        base.EndAction();
        if (gameController != null)
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            //SceneManager.LoadScene(0);
            gameController.SetScore();
            SceneManager.LoadScene("GameResult");
        }
    }
}
