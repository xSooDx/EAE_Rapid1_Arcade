using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class GameResultScript : MonoBehaviour
{
    [Tooltip("The panel contains result")]
    public RectTransform ResultPanel;
    [Tooltip("The panel showing speed")]
    public float PanelShowingSpeed;
    [Tooltip("The Text shows the play time")]
    public TextMeshProUGUI TimeTxt;
    [Tooltip("The Text shows the score")]
    public TextMeshProUGUI ScoreTxt;
    [Tooltip("The spawn point of the prize")]
    public Transform SpawnPoint;
    [Tooltip("The range of spawning")]
    [Range(0, 10)]
    public float SpawnRange;
    [Tooltip("The frequent spawing prize")]
    public float SpawnFreq;
    [Tooltip("Spawning Item")]
    public GameObject PrizeItem;

    private void Start()
    {
        if (ResultPanel != null) ResultPanel.localScale = Vector2.zero;
        if (ScoreManager.scoreManager != null)
        {
            ShowResult();
        }
    }

    /// <summary>
    /// shows the panel
    /// </summary>
    public void ShowResult()
    {
        ResultPanel.DOScale(Vector2.one, PanelShowingSpeed);
        if (TimeTxt != null) TimeTxt.text = (ScoreManager.scoreManager.GameTime / 60).ToString("00") + ":" + (ScoreManager.scoreManager.GameTime % 60).ToString("00");
        if (ScoreTxt != null) ScoreTxt.text = ScoreManager.scoreManager.Score.ToString("00");
        StartCoroutine(SpawnPrize());
    }

    /// <summary>
    /// spawning prize
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// The function when return button press
    /// </summary>
    public void ReturnTitle()
    {
        SceneManager.LoadScene(0);
    }
    /// <summary>
    /// The function when retry button press
    /// </summary>
    public void RetryBtn()
    {
        SceneManager.LoadScene(1);
    }
}
