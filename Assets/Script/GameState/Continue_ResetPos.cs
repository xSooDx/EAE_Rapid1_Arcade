
public class Continue_ResetPos : GameEndAction
{

    public Continue_ResetPos()
    {

    }

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
            gameController.TimerAction(true);
            gameController.SetGameTxt();
            gameController.SetPlayerPos();
            gameController.playerShip.SetCanMove();
            if (GameEventManager.gameEvent != null) GameEventManager.gameEvent.CancelFocus.Invoke(true);
        }
    }
}
