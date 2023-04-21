using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class ScoreboardItem : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text scoreText;
    public Slider scoreSlider;

    public void Initialize(Player player)
    {
        nameText.text = player.NickName;
        scoreSlider.maxValue = GameManager.Instance.winScore;
    }

    public void SetScore(int score)
    {
        scoreText.text = score.ToString();
        scoreSlider.value = score;
    }
}
