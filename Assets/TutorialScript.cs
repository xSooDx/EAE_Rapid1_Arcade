using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    public GameObject Present;
    public Transform SpawnPos;

    public ShipController _ship;
    private void Start()
    {
        SpawnPresent();
        _ship.SetCanMove();
        if (GameEventManager.gameEvent != null)
        {
            GameEventManager.gameEvent.PrizeLand.AddListener(RespawnPresentFunc);
        }
    }

    void SpawnPresent()
    {
        if (Present != null && SpawnPos != null)
            Instantiate(Present, SpawnPos);
    }

    void RespawnPresentFunc(string _str)
    {
        StartCoroutine(RespawnPresent());
    }

    IEnumerator RespawnPresent()
    {
        yield return new WaitForSeconds(4f);
        SpawnPresent();
    }
}
