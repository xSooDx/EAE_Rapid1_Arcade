using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndAction
{
    protected MainGameController gameController;

    public GameEndAction()
    {
        if(MainGameController.gameController!=null)
        gameController = MainGameController.gameController;
    }

    public virtual void StartAction()
    {
        if (gameController == null && MainGameController.gameController != null) gameController = MainGameController.gameController;
        //Debug.Log("I love Disgaea");
    }

    public virtual void EndAction()
    {
        if (gameController == null && MainGameController.gameController != null) gameController = MainGameController.gameController;
        //Debug.Log("Yee");
    }
}
