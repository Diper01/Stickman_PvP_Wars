using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData {
    public bool IsBot;
    public PlayerBot PlayerBot { get; set; }
    public PhotonPlayer Player { get; set; }
    public string PlayerName { get; set; }
    public int PlayerId { get; set; }
    public int Kills { get; set; }
    public int Death { get; set; }
    public int RoundWon { get; set; }	
}
