using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using TMPro;

public class ScoreboardItem : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text scoreText;

    public void Initialize(Player player)
    {
        nameText.text = player.NickName;
    }

    public void SetScore(int score)
    {
        scoreText.text = score.ToString();
    }
}
