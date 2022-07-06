using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    public Hashtable playerProperties = new Hashtable();
    [HideInInspector] public PlayerManager playerManager;

    [Header("Round Settings")]
    public float winScore;
    public float timeBetweenRounds;

    [Header("Money Earnings")]
    [SerializeField] int baseKillMoney;
    [SerializeField] int baseDeathMoney;
    [SerializeField] float fastKillTime;
    [SerializeField] int fastKillMoney;
    [SerializeField] int[] losingStreakMoney;

    [Header("UI")]
    [SerializeField] GameObject killGraphic;
    [SerializeField] TMP_Text moneyText;
    [SerializeField] GameObject gameOverMenu;
    [SerializeField] TMP_Text winLoseText;
    [SerializeField] GameObject loadingMenu;

    [Header("Player")]
    public int score;
    public int money;
    public List<ItemInfo> inventory;
    public List<ItemInfo> items;

    [Header("Components")]
    public GameObject cam;

    private PhotonView pv;
    private int readyPlayers;

    private float roundTimer;
    private bool roundIsRunning;
    private int losingStreak;

    [HideInInspector] public bool gameOver;

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
        cam.SetActive(false);
    }

    public void OnDeath()
    {
        roundIsRunning = false;
        losingStreak += (losingStreak < losingStreakMoney.Length - 1 ? 1 : 0);
        ChangeMoney(CalculateDeathMoney());

        cam.SetActive(true); 

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
        int streakMoney = losingStreakMoney[losingStreak];
        return baseDeathMoney + streakMoney;
    }

    int CalculateKillMoney()
    {
        int timeMoney = roundTimer < fastKillTime ? fastKillMoney : 0;
        return baseKillMoney + timeMoney;
    }

    void StartNextRound()
    {
        killGraphic.SetActive(false);
        Cursor.lockState = CursorLockMode.None;

        if (gameOver)
        {
            GameOver();
        }
        else
        {
            ShopManager.Instance.OpenShop();
            cam.SetActive(true);
        }
    }

    public void AddToInventory(ItemInfo item)
    {
        inventory.Add(item);
    }

    public void SelectPlayerItem(ItemInfo item)
    {
        items.RemoveAll(i => i.itemType == item.itemType);
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

    void GameOver()
    {
        gameOverMenu.SetActive(true);
        cam.SetActive(true);

        int highestScore = 0;
        Player[] playerList = PhotonNetwork.PlayerList;

        foreach (Player player in playerList)
        {
            if (player != PhotonNetwork.LocalPlayer && (int)player.CustomProperties["score"] > highestScore)
            {
                highestScore = (int)player.CustomProperties["score"];
            }
        }

        if (highestScore < score)
        {
            winLoseText.text = "VICTORY";
        }
        else if (highestScore > score)
        {
            winLoseText.text = "DEFEAT";
        }
        else
        {
            winLoseText.text = "DRAW";
        }

    }

    public void OnClickExit()
    {
        PhotonNetwork.Disconnect();
        loadingMenu.SetActive(true);
        RoomManager.Instance.photonView.ViewID = 0;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene(0);
    }
}
