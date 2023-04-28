using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Chat;
using ExitGames.Client.Photon;
using TMPro;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    public static ChatManager Instance;

    [SerializeField] private string chatAppID;
    [SerializeField] private Transform messageBox;
    [SerializeField] private TMP_Text messagePrefab;
    [SerializeField] private TMP_InputField messageInputField;

    private ChatClient chatClient;
    private AuthenticationValues authValues;

    private void Awake()
    {
        Instance = this;
        authValues = new AuthenticationValues();
    }

    private void Start()
    {
        chatClient = new ChatClient(this, ConnectionProtocol.Tcp);
        chatClient.ChatRegion = "US";
        authValues.UserId = PhotonNetwork.NickName;
        chatClient.Connect(chatAppID, Application.version, authValues);
    }

    private void Update()
    {
        if (chatClient != null)
        {
            chatClient.Service();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!EventSystem.current.currentSelectedGameObject == messageInputField)
            {
                messageInputField.Select();
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(null);

                if (!string.IsNullOrEmpty(messageInputField.text))
                {
                    SendChatMessage(messageInputField.text);
                    messageInputField.text = "";
                }
            }
        }
    }

    public void SendChatMessage(string message)
    {
        chatClient.PublishMessage(PhotonNetwork.CurrentRoom.Name, message);
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < messages.Length; i++)
        { 
            CreateMessage(senders[i], messages[i].ToString());
        }
    }

    private void CreateMessage(string sender, string message)
    {
        Instantiate(messagePrefab, messageBox).text = sender + ": " + message;
    }

    public void OnConnected()
    {
        chatClient.Subscribe(new string[] { PhotonNetwork.CurrentRoom.Name });
    }

    public void DisconnectChat()
    {
        if (chatClient != null)
        {
            chatClient.Disconnect();
        }
    }

    public void DebugReturn(DebugLevel level, string message) { }

    public void OnChatStateChange(ChatState state) { }

    public void OnPrivateMessage(string sender, object message, string channelName) { }

    public void OnSubscribed(string[] channels, bool[] results) { }

    public void OnUnsubscribed(string[] channels) { }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) { }

    public void OnUserSubscribed(string channel, string user) { }

    public void OnUserUnsubscribed(string channel, string user) { }

    public void OnDisconnected() { }
}
