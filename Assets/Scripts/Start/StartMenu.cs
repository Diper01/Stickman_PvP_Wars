using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class StartMenu : Photon.PunBehaviour {
    //[SerializeField] InputField inputField;
    [SerializeField] Button startButton;
    [SerializeField] GameObject IncorrectName;
    [SerializeField] Toggle soundToggle;
    [SerializeField] Toggle musicToggle;
    [SerializeField] Toggle vibroToggle;
    [SerializeField] Dropdown controllDropdown;
    [SerializeField] Dropdown regionDropdown;
    [SerializeField] Text regionOptionsText;
    [SerializeField] Text regionConnectionFailedText;

    [SerializeField] GameObject selectRegionPanel;
    [SerializeField] GameObject startMenuPanel;
    [SerializeField] GameObject connectingPanel;
    [SerializeField] GameObject connectionFailedPanel;
    [SerializeField] GameObject optionsPanel;   
    [SerializeField] GameObject exitPanel;
    [SerializeField] GameObject loadingPanel; 

    private Regex nickNameFilter = new Regex(@"^[\p{L}][\p{L}0-9\\_]{2,11}$");
    private bool waitingConnection = false;
    private bool reconnecting = false;

    private void Start()
    {        
        Init();
    }

    private void Update()
    {
        CheckConnection();
        CheckEscapeButton();
    }

    private void Init()
    {
        //if (GameOptions.SelectedServer == CloudRegionCode.none)
        //{
        //    ShowSelectRegionPanel();
        //}
        ////else if (!PhotonConnector.Instance.IsConnected)
        ////{
        ////    ShowConnectingPanle();
        ////    waitingConnection = true;
        ////    PhotonConnector.Instance.Connect();
        ////}
        //else {

			PhotonNetwork.offlineMode = true;
			ShowStartPanle();
        //}
    }

    private void CheckEscapeButton() {
        if (Input.GetButtonDown("Back"))
        {
            if (!PhotonNetwork.connected || loadingPanel.activeSelf)
                return;

            if (optionsPanel.activeSelf)
            {
                OnCloseOptions();
            }
            else if (exitPanel.activeSelf)
            {
                OnExitNoButton();
            }
            else if (!connectingPanel.activeSelf && !connectionFailedPanel.activeSelf)
            {
                ShowExitPanle();
            }
        }
    }

    private void CheckConnection() {
        if (PhotonConnector.Instance.IsConnected && connectingPanel.activeSelf && waitingConnection)
        {
            waitingConnection = false;
            ShowStartPanle();
        }

    }

    private bool IsNickNameMach(string nickName) {
        return nickNameFilter.IsMatch(nickName);
    }  

    #region PHOTON CALLBACKS

    public override void OnDisconnectedFromPhoton()
    {
        if (reconnecting)
        {
            reconnecting = false;
            waitingConnection = true;
            PhotonConnector.Instance.Connect();
        }
        else {
            ShowConnectionFailedPanel();
        }        
    }

    public override void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        print("OnFailedToConnectToPhoton to server failed. Caise: " + cause);
        //ShowConnectionFailedPanel();
        PhotonNetwork.offlineMode = true;
    }

    public override void OnConnectionFail(DisconnectCause cause)
    {
        print("OnConnectionFail to server failed. Caise: " + cause);
        //ShowConnectionFailedPanel();
        PhotonNetwork.offlineMode = true;
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        print("Join random room faield");
    }

    #endregion

    #region PANELS

    private void ShowSelectRegionPanel()
	{
		ShowStartPanle();
		return;
		selectRegionPanel.SetActive(true);
        startMenuPanel.SetActive(false);
        connectingPanel.SetActive(false);
        connectionFailedPanel.SetActive(false);
        optionsPanel.SetActive(false);        
        exitPanel.SetActive(false);
        loadingPanel.SetActive(false);      
        SetupSelectRegionPanel();       
    }

    private void SetupSelectRegionPanel() {
        regionDropdown.options = new List<Dropdown.OptionData>() {
              new Dropdown.OptionData(Localisation.GetString("Europe"))
            , new Dropdown.OptionData(Localisation.GetString("USAEast"))
            , new Dropdown.OptionData(Localisation.GetString("Asia"))
            , new Dropdown.OptionData(Localisation.GetString("Japan"))
            , new Dropdown.OptionData(Localisation.GetString("Australia"))
            , new Dropdown.OptionData(Localisation.GetString("USAWest"))
            , new Dropdown.OptionData(Localisation.GetString("SouthAmerica"))
            , new Dropdown.OptionData(Localisation.GetString("CanadaEast"))
            , new Dropdown.OptionData(Localisation.GetString("SouthKorea"))
            , new Dropdown.OptionData(Localisation.GetString("India"))
            , new Dropdown.OptionData(Localisation.GetString("Russia"))
        };
        if (GameOptions.SelectedServer == CloudRegionCode.none) {
            GameOptions.SelectedServer = CloudRegionCode.eu;
        }
        if (GameOptions.SelectedServer <= CloudRegionCode.jp)
        {
            regionDropdown.value = (int)GameOptions.SelectedServer;
        }
        else {
            regionDropdown.value = (int)GameOptions.SelectedServer - 1;
        }     
    }

    private void ShowStartPanle() {       
        selectRegionPanel.SetActive(false);
        startMenuPanel.SetActive(true);
        connectingPanel.SetActive(false);
        connectionFailedPanel.SetActive(false);
        optionsPanel.SetActive(false);     
        exitPanel.SetActive(false);
        loadingPanel.SetActive(false);       
        SetupStartPanel();               
    }

    private void SetupStartPanel() {
        regionOptionsText.text = Localisation.GetString("Server") + " " + GetServerLocalizedName(GameOptions.SelectedServer);
        controllDropdown.options = new List<Dropdown.OptionData>() {
              new Dropdown.OptionData(Localisation.GetString("Buttons") + " A")
            , new Dropdown.OptionData(Localisation.GetString("Buttons") + " B")
            , new Dropdown.OptionData(Localisation.GetString("Joystick")) };
        controllDropdown.value = (int)GameOptions.ControllType;
        if (DataManger.PlayerName != "")
        {
            //inputField.text = DataManger.PlayerName;
        }
    }

    private void ShowConnectingPanle() {
		selectRegionPanel.SetActive(false);
        startMenuPanel.SetActive(false);
        connectingPanel.SetActive(true);
        connectionFailedPanel.SetActive(false);
        optionsPanel.SetActive(false);     
        exitPanel.SetActive(false);
        loadingPanel.SetActive(false);        
    }

    private void ShowConnectionFailedPanel() {
		ShowStartPanle();
		return;
        selectRegionPanel.SetActive(false);
        startMenuPanel.SetActive(false);
        connectingPanel.SetActive(false);
        connectionFailedPanel.SetActive(true);
        optionsPanel.SetActive(false);       
        exitPanel.SetActive(false);
        loadingPanel.SetActive(false);       
        SetupConnectionFailedPanel();
    }

    private void SetupConnectionFailedPanel() {
        regionConnectionFailedText.text = Localisation.GetString("Server") + " " + GetServerLocalizedName(GameOptions.SelectedServer);
    }

    private void ShowOptionsPanle() {
		
        selectRegionPanel.SetActive(false);
        startMenuPanel.SetActive(false);
        connectingPanel.SetActive(false);
        connectionFailedPanel.SetActive(false);
        optionsPanel.SetActive(true);       
        exitPanel.SetActive(false);
        loadingPanel.SetActive(false);
        SetupOptionPanel();
    }

    private void SetupOptionPanel() {
        soundToggle.isOn = GameOptions.Sound;
        musicToggle.isOn = GameOptions.Music;
        vibroToggle.isOn = GameOptions.Vibro;
    }   

    private void ShowExitPanle() {     
        selectRegionPanel.SetActive(false);
        startMenuPanel.SetActive(false);
        connectingPanel.SetActive(false);
        connectionFailedPanel.SetActive(false);
        optionsPanel.SetActive(false);       
        exitPanel.SetActive(true);
        loadingPanel.SetActive(false);       
    }

    private void ShowLoadnigPanle() {       
        selectRegionPanel.SetActive(false);
        startMenuPanel.SetActive(false);
        connectingPanel.SetActive(false);
        connectionFailedPanel.SetActive(false);
        optionsPanel.SetActive(false);       
        exitPanel.SetActive(false);
        loadingPanel.SetActive(true);       
    }       

    private string GetServerLocalizedName(CloudRegionCode region) {
        switch (region)
        {
            case CloudRegionCode.eu:
                return Localisation.GetString("Europe");              
            case CloudRegionCode.us:
                return Localisation.GetString("USAEast");              
            case CloudRegionCode.asia:
                return Localisation.GetString("Asia");
            case CloudRegionCode.jp:
                return Localisation.GetString("Japan");              
            case CloudRegionCode.au:
                return Localisation.GetString("Australia");               
            case CloudRegionCode.usw:
                return Localisation.GetString("USAWest");               
            case CloudRegionCode.sa:
                return Localisation.GetString("SouthAmerica");               
            case CloudRegionCode.cae:
                return Localisation.GetString("CanadaEast");              
            case CloudRegionCode.kr:
                return Localisation.GetString("SouthKorea");               
            case CloudRegionCode.@in:
                return Localisation.GetString("India");              
            case CloudRegionCode.ru:
                return Localisation.GetString("Russia");              
            default:
                return "";      
        }       
    }

    #endregion

    #region UI EVENTS HANDLERS

    public void OnStartButton()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
       
            PhotonNetwork.player.NickName = Localisation.GetString("You");           
            ShowLoadnigPanle();
            SceneManager.LoadScene(ScenesIndexes.Matchmaking);
            IncorrectName.SetActive(true);
    }

    public void OnChangeRegionButton() {
        ShowSelectRegionPanel();
    }

    public void OnConnectButton() {
        if (PhotonNetwork.connectionState == ConnectionState.Disconnected)
        {
            ShowConnectingPanle();
            waitingConnection = true;
            PhotonConnector.Instance.Connect();
        }
        else if (PhotonNetwork.CloudRegion != GameOptions.SelectedServer)
        {
            ShowConnectingPanle();
            reconnecting = true;
            PhotonNetwork.Disconnect();
        }
        else {
            ShowStartPanle();
        }
    }    

    public void OnInputNameValueChange()
    {
            IncorrectName.SetActive(false);           
            DataManger.PlayerName = Localisation.GetString("You");
    }

    public void OnTryRecconetButton() {      
        SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
        waitingConnection = true;
        ShowConnectingPanle();
        PhotonConnector.Instance.Connect();
    }

    public void OnExitGameButton() {      
        SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
        Application.Quit();
    }

    public void OnExitNoButton() {       
        SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
        ShowStartPanle();
    }

    public void OnBotTestButton() {      
        SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
        PhotonNetwork.CreateRoom("BotRoom");
    }

    public void OnOptionsButton() {       
        SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
        ShowOptionsPanle();
    }

    public void OnSoundToggle() {      
        SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
        GameOptions.Sound = soundToggle.isOn;
    }

    public void OnMusicToggle() {      
        SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
        GameOptions.Music = musicToggle.isOn;
    }

    public void OnVibroToggle() {        
        SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
        GameOptions.Vibro = vibroToggle.isOn;
    }

    public void OnControllDropdown()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
        GameOptions.ControllType = (ControllType)controllDropdown.value;
    }

    public void OnSelectedRegionDropdown() {
        SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
        if ((CloudRegionCode)regionDropdown.value <= CloudRegionCode.jp)
        {
            GameOptions.SelectedServer = (CloudRegionCode)regionDropdown.value;
        }
        else {
            GameOptions.SelectedServer = (CloudRegionCode)(regionDropdown.value + 1);
        }
    }

    public void OnCloseOptions() {        
        SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
        ShowStartPanle();
    }

    public void OnRateOkButton() {
        SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
        PlayerPrefs.SetInt("GameRated", 1);       
        ShowStartPanle();
        Application.OpenURL("market://details?id=" + Application.identifier);
    }

    public void OnRateCloseButton() {
        SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
        ShowStartPanle();
    }

    #endregion
}
