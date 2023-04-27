using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Chat;
using ExitGames.Client.Photon;
using TMPro;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    public static ChatManager Instance;

    [SerializeField] private string chatAppID;
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
            if (!messageInputField.isFocused)
            {
                messageInputField.Select();
            }
        }
    }

    public void SendChatMessage(string message)
    {
        chatClient.PublishMessage(PhotonNetwork.CurrentRoom.Name, message);
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        int msgCount = messages.Length;

        for (int i = 0; i < msgCount; i++)
        { 
            string sender = senders[i];
            string msg = messages[i].ToString();
            Debug.Log(sender +msg);
        }
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
