using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchmakingMenu : Photon.PunBehaviour {

    [SerializeField] GameObject tabs;
    [SerializeField] GameObject findRoomTab;
    [SerializeField] GameObject roomBrowserTab;
    [SerializeField] GameObject createRoomTab;
    [SerializeField] GameObject loadingPanel;
    [SerializeField] GameObject roomLobbyPanell;
    [SerializeField] GameObject findRoomTabFocusElement;
    [SerializeField] GameObject roomBrowserTabFocusElement;
    [SerializeField] GameObject createRoomTabFocusElement;

    private void Start()
    {
        if (!PhotonNetwork.connected) {
            ExitToStartMenu();
            return;
        }

        SoundManager.Instance.PlayMusicInLoop();
        ShowTabsPanle();       
    }

    private void Update()
    {
        if (Input.GetButtonDown("Back")) {
            if (loadingPanel.activeSelf) {
                //do nothing
            }
            else if (roomLobbyPanell.activeSelf) {
                PhotonNetwork.LeaveRoom();
                ShowTabsPanle();
            }
            else {
                StartCoroutine(ExitToStartMenuAds());
            }
            
        }
    }

    public void ShowTabsPanle() {
        tabs.GetComponent<RectTransform>().SetAsLastSibling();
        roomLobbyPanell.SetActive(false);
        loadingPanel.SetActive(false);       
        StartCoroutine(SetFocusOnTabs());

    }

    public void ShowRoomLobby() {
        tabs.GetComponent<RectTransform>().SetAsFirstSibling();
        roomLobbyPanell.SetActive(true);
        loadingPanel.SetActive(false);             
    }

    public void ShowLoading() {
        tabs.GetComponent<RectTransform>().SetAsFirstSibling();
        roomLobbyPanell.SetActive(false);
        loadingPanel.SetActive(true);       
    }

    private IEnumerator SetFocusOnTabs() {
        yield return new WaitForSeconds(0.15f);
        if (findRoomTab.activeSelf)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(findRoomTabFocusElement);
        }
        else if (roomBrowserTab.activeSelf)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(roomBrowserTabFocusElement);
        }
        else if (createRoomTab.activeSelf)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(createRoomTabFocusElement);
        }
    }


    public void OnFindRoomTab() {
        SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
        findRoomTab.SetActive(true);
        roomBrowserTab.SetActive(false);
        createRoomTab.SetActive(false);
    }

    public void OnRoomBrowserTab()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
        findRoomTab.SetActive(false);
        roomBrowserTab.SetActive(true);
        createRoomTab.SetActive(false);
    }

    public void OnCreateRoomTab() {
        SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
        findRoomTab.SetActive(false);
        roomBrowserTab.SetActive(false);
        createRoomTab.SetActive(true);
    }

    private IEnumerator ExitToStartMenuAds() {
        ShowLoading();
        yield return new WaitForSeconds(0.2f);        
        SceneManager.LoadScene(ScenesIndexes.Start);
    }

    public override void OnDisconnectedFromPhoton()
    {
        ExitToStartMenu();
    }

    private void ExitToStartMenu() {
        ShowLoading();
        SceneManager.LoadScene(ScenesIndexes.Start);
    }
}
