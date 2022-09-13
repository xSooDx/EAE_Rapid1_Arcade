using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager gameEvent;

    private void Awake()
    {
        gameEvent = this;
    }


    /// <summary>
    /// Event of game over
    /// </summary>
    [HideInInspector]
    public UnityEvent<string, string, GameEndAction> GameOver;

    /// <summary>
    /// Event of camera focus on ship when it is low altitude
    /// </summary>
    [HideInInspector]
    public UnityEvent StartFocus;

    /// <summary>
    /// Event of camera cancel focus on ship
    /// </summary>
    [HideInInspector]
    public UnityEvent<bool> CancelFocus;

    /// <summary>
    /// Trigger when prize land perfectly
    /// </summary>
    public UnityEvent<string> PrizeLand;

    /// <summary>
    /// Trigger when you need to crash a prize
    /// </summary>
    public UnityEvent<string> PrizeCrash;

    /// <summary>
    /// add score
    /// </summary>
    public UnityEvent<string,int> AddScore;

    /// <summary>
    /// Trigger when player close to a planet
    /// </summary>
    public UnityEvent<Transform> ClosePlanet;

    /// <summary>
    /// Trigger when player leave the planet
    /// </summary>
    public UnityEvent LeavePlanet;

    /// <summary>
    /// trigger when you want to crash player
    /// </summary>
    public UnityEvent<Vector2> PlayerCrash;

    /// <summary>
    /// trigger when you want camera focus on player
    /// </summary>
    public UnityEvent<bool> FocusPlayer;

    /// <summary>
    /// trigger to show warning
    /// </summary>
    public UnityEvent<string,bool> SetWarning;
}
