using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    static public ScoreManager scoreManager;

    private void Awake()
    {
        if (scoreManager == null)
        {
            scoreManager = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public float GameTime;
    public float Score;

    public List<Sprite> PrizeImg;

    public void SetImg(List<Sprite> _list)
    {
        this.PrizeImg = new List<Sprite>();
        this.PrizeImg.AddRange(_list);
    }
}
