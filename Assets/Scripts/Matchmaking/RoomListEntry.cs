using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListEntry : MonoBehaviour {
    public string RoomId { get; set; }
    public int ConnectedPlayers { get; set; }
    public bool IsHighlighted { get {
            return isHighlighted;
        } set {
            isHighlighted = value;
            if (isHighlighted) {
                entryBackground.color = highlightedColor;               
                gameTypeText.color = textHighlightedColor;
                roomIdText.color = textHighlightedColor;
                playersCountText.color = textHighlightedColor;
            }
            else {
                entryBackground.color = normalColor;              
                gameTypeText.color = textNormalColor;
                roomIdText.color = textNormalColor;
                playersCountText.color = textNormalColor;
            }
        }
    }
    public System.Action<string> OnClick;

    [SerializeField] Color normalColor;
    [SerializeField] Color highlightedColor;
    [SerializeField] Color textNormalColor;
    [SerializeField] Color textHighlightedColor;   
    [SerializeField] Text gameTypeText;
    [SerializeField] Text roomIdText;
    [SerializeField] Text playersCountText;
    [SerializeField] Image entryBackground;
    [SerializeField] Button button;
 
    private int maxPlayersPerRoom = 6;
    private bool isHighlighted = false;    

    private void Awake()
    {
        button.onClick.AddListener(OnClickButton);
        IsHighlighted = false;
    }

    public void SetEntryData(string gameType, string roomId, int connectedPlayers) {
        RoomId = roomId;
        ConnectedPlayers = connectedPlayers;       
        gameTypeText.text = gameType;
        roomIdText.text = roomId.ToString();
        playersCountText.text = connectedPlayers + "/" + maxPlayersPerRoom;
    }  

    private void OnClickButton() {
        if (OnClick != null) {
            OnClick(RoomId);
        }
    }

   

}
