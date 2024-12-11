using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamDeathMatchGameUI : Photon.PunBehaviour {
    [SerializeField] GameObject teamSelectionPanel;
    [SerializeField] GameObject breakePanel;
    [SerializeField] GameObject infoPnale;
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject fightMessage;
    [SerializeField] GameObject roundOverMessage;
    [SerializeField] GameObject loadingPanel;
    [SerializeField] GameObject connectionLost;
    [SerializeField] Text messageField;
    [SerializeField] GameObject buttons;
    [SerializeField] GameObject breakePanelFocusElement;

    private int openWindows = 0;
    private Queue<MessageInfo> messagesQueue = new Queue<MessageInfo>();
    private bool messageShowing = false;
    private GameManagerDeathmatch gameManager;

    private void Start()
    {
        gameManager = GameManagersHolder.Instance.GameManagerDeathmatch;
        OnJoystickAttachedChanged(GameOptions.JoystickAttached);
    }

    public void Update()
    {
        ProcessMessages();
        if (Input.GetButtonDown("Back"))
        {
            if (infoPnale.activeSelf)
            {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(breakePanelFocusElement);
                infoPnale.SetActive(false);
            }
            else if (menuPanel.activeSelf)
            {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(breakePanelFocusElement);
                menuPanel.SetActive(false);
            }
            else if(teamSelectionPanel.activeSelf == false && roundOverMessage.activeSelf == false)
            {
                menuPanel.SetActive(true);
            }
        }
        if (Input.GetButtonDown("Info"))
        {
            if (infoPnale.activeSelf)
            {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(breakePanelFocusElement);
                infoPnale.SetActive(false);
            }
            else if(teamSelectionPanel.activeSelf == false && roundOverMessage.activeSelf == false && breakePanel.activeSelf == false)
            {
                menuPanel.SetActive(false);
                infoPnale.SetActive(true);
            }
        }
    }

    private void OnEnable()
    {
        EventManager.TeamSelectionStart += OnTeamSelectionStart;
        EventManager.RoundStart += OnRoundStart;
        EventManager.RoundFinished += OnBreakStart;
        EventManager.RoomDisconnected += OnRoomDisconnected;
        EventManager.PlayerJoinGame += OnPlayerJoinGame;
        EventManager.PlayerLeaveGame += OnPlayerLeaveGame;
        EventManager.LoadNextMap += OnLoadNextMap;
        GameOptions.JoystickAttachedChanged += OnJoystickAttachedChanged;
    }

    private void OnDisable()
    {
        EventManager.TeamSelectionStart -= OnTeamSelectionStart;
        EventManager.RoundStart -= OnRoundStart;
        EventManager.RoundFinished -= OnBreakStart;
        EventManager.RoomDisconnected -= OnRoomDisconnected;
        EventManager.PlayerJoinGame -= OnPlayerJoinGame;
        EventManager.PlayerLeaveGame -= OnPlayerLeaveGame;
        EventManager.LoadNextMap -= OnLoadNextMap;
        GameOptions.JoystickAttachedChanged -= OnJoystickAttachedChanged;
    }


    private void OnTeamSelectionStart() {
        infoPnale.SetActive(false);
        menuPanel.SetActive(false);
        teamSelectionPanel.SetActive(true);
    }

    private void OnRoundStart()
    {
        StartCoroutine(RoundStart());
    }

    private void OnBreakStart()
    {
        StartCoroutine(ShowBreakPanel());
    }

    private IEnumerator RoundStart()
    {
        teamSelectionPanel.SetActive(false);
        breakePanel.SetActive(false);
        fightMessage.SetActive(true);
        yield return new WaitForSeconds(1f);
        fightMessage.SetActive(false);
    }

    private IEnumerator ShowBreakPanel()
    {
        teamSelectionPanel.SetActive(false);
        fightMessage.SetActive(false);
        yield return new WaitForSeconds(0.15f);       
        infoPnale.SetActive(false);
        menuPanel.SetActive(false);
        roundOverMessage.SetActive(true);
        yield return new WaitForSeconds(2.7f);
        roundOverMessage.SetActive(false);
        breakePanel.SetActive(true);
    }

    private IEnumerator LeaveGame()
    {
        loadingPanel.SetActive(true);
        yield return new WaitForSeconds(0.2f);      
        PhotonNetwork.LoadLevel(ScenesIndexes.Matchmaking);
    }

    private void OnRoomDisconnected()
    {
        connectionLost.SetActive(true);
    }

    private void OnPlayerJoinGame(string name)
    {
        messagesQueue.Enqueue(new MessageInfo() { MessageText = name + " " + Localisation.GetString("PlayerJoinGame"), TextColor = Color.green });
    }

    private void OnPlayerLeaveGame(string name)
    {
        messagesQueue.Enqueue(new MessageInfo() { MessageText = name + " " + Localisation.GetString("PlayerLeaveGame"), TextColor = Color.red });
    }

    private void OnLoadNextMap()
    {
        loadingPanel.SetActive(true);
    }

    private void ProcessMessages()
    {
        if (messagesQueue.Count != 0
            && !messageShowing)
        {
            StartCoroutine(ShowMessage(messagesQueue.Dequeue()));
        }
    }

    private IEnumerator ShowMessage(MessageInfo message)
    {
        messageShowing = true;
        yield return new WaitForSeconds(0.5f);
        messageField.color = message.TextColor;
        messageField.text = message.MessageText;
        yield return new WaitForSeconds(2.5f);
        messageField.text = "";
        messageShowing = false;
    }

    private void ClearPlayerData() {
        ExitGames.Client.Photon.Hashtable playerProp = new ExitGames.Client.Photon.Hashtable();
        foreach (var key in PhotonNetwork.player.CustomProperties.Keys)
        {
            playerProp.Add(key, null);
        }
        PhotonNetwork.player.SetCustomProperties(playerProp);
    }

    private void OnJoystickAttachedChanged(bool isAttached)
    {
        //if (isAttached)
        //{
        //    buttons.SetActive(false);
        //}
        //else
        //{
        //    buttons.SetActive(true);
        //}
    }

    #region BUTTONS HANDLERS

    public void OnMenuButton()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
        menuPanel.SetActive(true);
    }

    public void OnMenuCloseButton()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
        menuPanel.SetActive(false);
    }

    public void OnLeaveGameButton()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
        ClearPlayerData();
        PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.player);        
        PhotonNetwork.LeaveRoom();
        StartCoroutine(LeaveGame());
    }

    public void OnConnectionLostOkButton()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
        ClearPlayerData();
        PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.player);
        PhotonNetwork.LeaveRoom();
        loadingPanel.SetActive(true);
        PhotonNetwork.LoadLevel(ScenesIndexes.Start);
    }

    public void OnInfoButton()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
        infoPnale.SetActive(true);
    }

    public void OnInfoCloseButton()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
        infoPnale.SetActive(false);
    }

    #endregion

}
