using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Hashtable playerProperties = new Hashtable();
    [HideInInspector] public PlayerManager playerManager;

    [Header("Settings")]
    [SerializeField] int killMoney;
    public float timeBetweenRounds;

    [Header("UI")]
    [SerializeField] GameObject killGraphic;
    [SerializeField] TMP_Text moneyText;

    [Header("Player")]
    public int score;
    public int money;
    public List<ItemInfo> items;

    private int round;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerProperties["score"] = score;
        SyncItems();
    }

    public void OnSpawn()
    {
        killGraphic.SetActive(false);
        AudioManager.Instance.audioListener.enabled = false;
    }

    public void OnDeath()
    {
        AudioManager.Instance.audioListener.enabled = true;
        Invoke("StartNextRound", timeBetweenRounds);
    }

    public void OnKill()
    {
        score++;
        playerProperties["score"] = score;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);

        ChangeMoney(killMoney);

        killGraphic.SetActive(true);
        AudioManager.Instance.Play("Kill");
        Invoke("StartNextRound", timeBetweenRounds);
    }

    public void StartNextRound()
    {
        round++;

        if (round % 5 == 0)
        {
            ShopManager.Instance.shop.SetActive(true);
        }
        else
        {
            playerManager.CreateController();
        }
    }

    public void AddPlayerItem(ItemInfo item)
    {
        items.Clear();
        items.Add(item);
        SyncItems();
    }

    void SyncItems()
    {
        string[] itemsArray = new string[items.Count];

        for (int i = 0; i < itemsArray.Length; i++)
        {
            itemsArray[i] = items[i].itemName;
        }

        playerProperties["items"] = itemsArray;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
    }

    public void ChangeMoney(int amount)
    {
        money += amount;
        moneyText.text = money.ToString();
    }
}
