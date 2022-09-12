using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class GameResultScript : MonoBehaviour
{
    public RectTransform ResultPanel;
    public float PanelShowingSpeed;

    public GameObject TimeLabel;
    public TextMeshProUGUI TimeTxt;

    public GameObject ScoreLabel;
    public TextMeshProUGUI ScoreTxt;

    public Transform SpawnPoint;
    [Range(0, 10)]
    public float SpawnRange;
    public float SpawnFreq;
    public GameObject PrizeItem;

    private void Start()
    {
        if (ResultPanel != null) ResultPanel.localScale = Vector2.zero;
        if (ScoreManager.scoreManager != null)
        {
            ShowResult();
        }
    }

    public void ShowResult()
    {
        ResultPanel.DOScale(Vector2.one, PanelShowingSpeed);
        if (TimeTxt != null) TimeTxt.text = (ScoreManager.scoreManager.GameTime / 60).ToString("00") + ":" + (ScoreManager.scoreManager.GameTime % 60).ToString("00");
        if (ScoreTxt != null) ScoreTxt.text = ScoreManager.scoreManager.Score.ToString("00");
        StartCoroutine(SpawnPrize());
    }

    IEnumerator SpawnPrize()
    {
        foreach (var prize in ScoreManager.scoreManager.PrizeImg)
        {
            Vector2 _pos = new Vector2(Random.Range(SpawnPoint.position.x - SpawnRange, SpawnPoint.position.x + SpawnRange), SpawnPoint.position.y);
            GameObject _obj = Instantiate(PrizeItem, _pos, Quaternion.identity);
            if (_obj.GetComponent<SpriteRenderer>() != null) _obj.GetComponent<SpriteRenderer>().sprite = prize;
            yield return new WaitForSeconds(SpawnFreq);
        }
    }

    public void ReturnTitle()
    {
        SceneManager.LoadScene(0);
    }
}
