using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Chat;
using ExitGames.Client.Photon;
using TMPro;
using DG.Tweening;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    public static ChatManager Instance;

    [SerializeField] private string chatAppID;
    [SerializeField] private Transform messageBox;
    [SerializeField] private TMP_Text messagePrefab;
    [SerializeField] private TMP_InputField messageInputField;
    [SerializeField] private RectTransform chatScrollView;
    [SerializeField] private ScrollRect chatScrollRect;
    [SerializeField] private Scrollbar chatScrollbar;
    [SerializeField] private Image chatScrollbarImage;
    [SerializeField] private float chatHiddenHeight;
    [SerializeField] private float chatExpandedHeight;
    [SerializeField] private float chatHiddenAlpha;
    [SerializeField] private float chatExpandedAlpha;

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
                HideChat(false);
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(null);
                HideChat(true);

                if (!string.IsNullOrEmpty(messageInputField.text))
                {
                    SendChatMessage(messageInputField.text);
                    messageInputField.text = "";
                }
            }
        }
    }

    public void HideChat(bool hide)
    {
        if (hide)
        {
            chatScrollView.sizeDelta = new Vector2(chatScrollView.sizeDelta.x, chatHiddenHeight);
            chatScrollbarImage.enabled = false;
            chatScrollbar.value = 0;
            chatScrollView.GetComponent<Image>().DOFade(chatHiddenAlpha, .05f).SetEase(Ease.Linear);
            messageInputField.image.DOFade(chatHiddenAlpha, .05f).SetEase(Ease.Linear);
        }
        else
        {
            chatScrollView.sizeDelta = new Vector2(chatScrollView.sizeDelta.x, chatExpandedHeight);
            chatScrollbarImage.enabled = true;
            chatScrollView.GetComponent<Image>().DOFade(chatExpandedAlpha, .05f).SetEase(Ease.Linear);
            messageInputField.image.DOFade(chatExpandedAlpha, .05f).SetEase(Ease.Linear);
        }
    }

    public void OnGUI()
    {
        if (chatScrollbarImage.enabled && Event.current.type == EventType.ScrollWheel)
        {
            float scrollDelta = Event.current.delta.y * chatScrollRect.scrollSensitivity;
            Vector2 normalizedPosition = chatScrollRect.normalizedPosition;
            normalizedPosition.y -= scrollDelta / chatScrollRect.content.rect.height;
            normalizedPosition.y = Mathf.Clamp(normalizedPosition.y, 0f, 1f);
            chatScrollRect.normalizedPosition = normalizedPosition;
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
