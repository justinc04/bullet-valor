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

    [Header("Round Settings")]
    public float timeBetweenRounds;
    public int shopFrequency;

    [Header("Money Earnings")]
    [SerializeField] int baseKillMoney;
    [SerializeField] int baseDeathMoney;
    [SerializeField] float fastKillTime;
    [SerializeField] int fastKillMoney;
    [SerializeField] int losingStreakMoney;
    [SerializeField] int maxLosingStreakMoney;

    [Header("UI")]
    [SerializeField] GameObject killGraphic;
    [SerializeField] TMP_Text moneyText;

    [Header("Player")]
    public int score;
    public int money;
    public List<ItemInfo> items;

    private PhotonView pv;
    private int readyPlayers;

    private int round;
    private float roundTimer;
    private bool roundIsRunning;
    private int losingStreak;

    private void Awake()
    {
        Instance = this;
        pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        playerProperties["score"] = score;
        SyncItems();
    }

    private void Update()
    {
        if (roundIsRunning)
        {
            roundTimer += Time.deltaTime;
        }
    }

    public void OnSpawn()
    {
        roundIsRunning = true;
        roundTimer = 0;
        killGraphic.SetActive(false);
        AudioManager.Instance.audioListener.enabled = false;
    }

    public void OnDeath()
    {
        roundIsRunning = false;
        losingStreak++;
        ChangeMoney(CalculateDeathMoney());

        AudioManager.Instance.audioListener.enabled = true;     
        Invoke("StartNextRound", timeBetweenRounds);
    }

    public void OnKill()
    {
        roundIsRunning = false;
        losingStreak = 0;
        score++;
        playerProperties["score"] = score;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
        ChangeMoney(CalculateKillMoney());

        killGraphic.SetActive(true);
        AudioManager.Instance.Play("Kill");
        Invoke("StartNextRound", timeBetweenRounds);
    }

    int CalculateDeathMoney()
    {     
        int streakMoney = Mathf.Min(losingStreakMoney * (losingStreak - 1), maxLosingStreakMoney);
        return baseDeathMoney + streakMoney;
    }

    int CalculateKillMoney()
    {
        int timeMoney = roundTimer < fastKillTime ? fastKillMoney : 0;
        return baseKillMoney + timeMoney;
    }

    public void StartNextRound()
    {
        killGraphic.SetActive(false);
        Cursor.lockState = CursorLockMode.None;

        if (round > 0 && round % shopFrequency == 0)
        {
            ShopManager.Instance.OpenShop();
        }
        else
        {
            playerManager.CreateController();
        }

        round++;
    }

    public void AddPlayerItem(ItemInfo item)
    {
        items.Insert(0, item);
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

    public void ReadyToStart()
    {
        pv.RPC("RPC_ReadyToStart", RpcTarget.All);
    }

    [PunRPC]
    void RPC_ReadyToStart()
    {
        readyPlayers++;

        if (readyPlayers == PhotonNetwork.PlayerList.Length)
        {
            readyPlayers = 0;
            ShopManager.Instance.shop.SetActive(false);
            playerManager.CreateController();
        }
    }
}
