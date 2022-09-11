
public class Continue_MaintainPos : GameEndAction
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
            gameController.TimerAction(true);
            gameController.SetGameTxt();
            gameController.playerShip.SetCanMove();
            if (GameEventManager.gameEvent != null) GameEventManager.gameEvent.CancelFocus.Invoke(true);
        }
    }
}
