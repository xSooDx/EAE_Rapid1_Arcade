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
    /// add score
    /// </summary>
    public UnityEvent<int> AddScore;

    public UnityEvent<Transform> ClosePlanet;

    public UnityEvent LeavePlanet;

    public UnityEvent<Vector2> PlayerCrash;
}
