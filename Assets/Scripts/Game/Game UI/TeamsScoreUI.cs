using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;

public class TeamsScoreUI : Photon.PunBehaviour {

    [SerializeField] Text redTeamScore;
    [SerializeField] Text blueTeamScore;

    private void Start()
    {
        SetupTeamsScore();
    }

    public override void OnPhotonCustomRoomPropertiesChanged(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        SetupTeamsScore();
    }

    private void SetupTeamsScore() {
        if (PhotonNetwork.room.CustomProperties.ContainsKey(RoomProperty.RedTeamScore))
            redTeamScore.text = ((int)PhotonNetwork.room.CustomProperties[RoomProperty.RedTeamScore]).ToString();
        else
            redTeamScore.text = "0";

        if (PhotonNetwork.room.CustomProperties.ContainsKey(RoomProperty.BlueTeamScore))
            blueTeamScore.text = ((int)PhotonNetwork.room.CustomProperties[RoomProperty.BlueTeamScore]).ToString();
        else
            blueTeamScore.text = "0";
    }
}
