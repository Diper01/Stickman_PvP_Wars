using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataManger {
    public static string PlayerName {
        get {
            return playerName;
        }
        set {
            playerName = value;
            PlayerPrefs.SetString("PlayerName", playerName);            
        }
    }

    static DataManger() {
        playerName = PlayerPrefs.GetString("PlayerName");
    }

    private static string playerName;
}
