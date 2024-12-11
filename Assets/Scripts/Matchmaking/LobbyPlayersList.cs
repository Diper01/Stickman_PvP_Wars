using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayersList : Photon.PunBehaviour {
    [SerializeField] GameObject playerInfoPref;
    [SerializeField] GameObject playerslistHolder;
    [SerializeField] Text connectedPlayersText; 

    private void OnEnable()
    {
        UpdateConnectdedPlayers();      
    }   

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        UpdateConnectdedPlayers();
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        UpdateConnectdedPlayers();
    }

    public override void OnPhotonCustomRoomPropertiesChanged(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(RoomProperty.BotList))
        {
            UpdateConnectdedPlayers();
        }
    }

    private void UpdateConnectdedPlayers() {
        foreach (Transform child in playerslistHolder.transform) {
            Destroy(child.gameObject);
        }

        foreach (var player in PhotonNetwork.playerList) {
            GameObject playerInfo = GameObject.Instantiate(playerInfoPref, playerslistHolder.transform);
            playerInfo.GetComponentInChildren<Text>().text = player.NickName;
        }

        var bots = BotsInformation.GetBotsDictionary();
        foreach (var key in bots.Keys)
        {
            GameObject playerInfo = GameObject.Instantiate(playerInfoPref, playerslistHolder.transform);
            playerInfo.GetComponentInChildren<Text>().text = bots[key];
        }

        connectedPlayersText.text = Localisation.GetString("Connected") + " " + (PhotonNetwork.playerList.Length + bots.Count) + "/6";
    }
   
}
