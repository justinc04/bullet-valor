using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Scoreboard : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform container;
    [SerializeField] GameObject scoreboardItemPrefab;

    private Dictionary<Player, ScoreboardItem> scoreboardItems = new Dictionary<Player, ScoreboardItem>();

    private void Start()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            AddScoreboardItem(player);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddScoreboardItem(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemoveScoreboardItem(otherPlayer);
    }

    void AddScoreboardItem(Player player)
    {
        ScoreboardItem item = Instantiate(scoreboardItemPrefab, container).GetComponent<ScoreboardItem>();
        item.Initialize(player);
        scoreboardItems[player] = item;
    }

    void RemoveScoreboardItem(Player player)
    {
        Destroy(scoreboardItems[player].gameObject);
        scoreboardItems.Remove(player);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        scoreboardItems[targetPlayer].SetScore((int)changedProps["score"]);

        if ((int)changedProps["score"] >= GameManager.Instance.winScore)
        {
            GameManager.Instance.gameOver = true;
        }
    }
}
