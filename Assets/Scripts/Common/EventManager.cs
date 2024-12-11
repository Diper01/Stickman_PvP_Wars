using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class EventManager {
    public static event Action<WeaponType> WeaponChaged;
    public static event Action<int> BulletsCountChaged;
    public static event Action<int> TeamSelectionTimerUpdate;
    public static event Action<int> RoundTimerUpdate;
    public static event Action<int> BreakTimerUpdate;
    public static event Action TeamSelectionStart;
    public static event Action RoundStart;
    public static event Action RoundFinished;
    public static event Action RoomDisconnected;
    public static event Action StartWatchingAds;
    public static event Action StopWatchingAds;
    public static event Action<Player> PlayerDie;
    public static event Action<PlayerBot> BotDie;
    public static event Action<string> PlayerJoinGame;
    public static event Action<string> PlayerLeaveGame;
    public static event Action LoadNextMap;
    public static event Action<Team> FlagCaptured;
    public static event Action<Team> ShowFlagCapturedMessage;
    public static event Action<byte, object, int> PhotonOfflineEvent;

    public static void OnWeaponChanged(WeaponType newType) {
        if (WeaponChaged != null) {
            WeaponChaged(newType);
        }
    }

    public static void OnBulletsCountChanged(int count) {
        if (BulletsCountChaged != null) {
            BulletsCountChaged(count);
        }
    }

    public static void OnTeamSelectionTimerUpdate(int timeSec) {
        if (TeamSelectionTimerUpdate != null) {
            TeamSelectionTimerUpdate(timeSec);
        }
    }

    public static void OnRoundTimerUpdate(int timeSec) {
        if (RoundTimerUpdate != null) {
            RoundTimerUpdate(timeSec);
        }
    }

    public static void OnBreakTimerUpdate(int timeSec) {
        if (BreakTimerUpdate != null) {
            BreakTimerUpdate(timeSec);
        }
    }

    public static void OnTeamSelectionStart() {
        if (TeamSelectionStart != null) {
            TeamSelectionStart();
        }
    }

    public static void OnRoundStart() {
        if (RoundStart != null) {
            RoundStart();
        }
    }

    public static void OnRoundFinished() {
        if (RoundFinished!= null) {
            RoundFinished();
        }
    } 

    public static void OnRoomDisconnected() {
        if(RoomDisconnected != null){
            RoomDisconnected();
        }
    }

    public static void OnPlayerDie(Player player) {
        if (PlayerDie != null) {
            PlayerDie(player);
        }
    }

    public static void OnBotDie(PlayerBot bot) {
        if (BotDie != null) {
            BotDie(bot);
        }
    }

    public static void OnPlayerJoinGame(string name) {
        if (PlayerJoinGame != null) {
            PlayerJoinGame(name);
        }
    }

    public static void OnPlayerLeaveGame(string name) {
        if (PlayerLeaveGame != null) {
            PlayerLeaveGame(name);
        }
    }

    public static void OnLoadNextMap() {
        if (LoadNextMap != null)
            LoadNextMap();
    }

    public static void OnFlagCaptured(Team team) {
        if (FlagCaptured != null)
            FlagCaptured(team);
    }

    public static void OnShowFlagCapturedMessage(Team team) {
        if (ShowFlagCapturedMessage != null)
            ShowFlagCapturedMessage(team);
    }

    public static void OnPhotonOfflineEvent(byte eventcode, object content, int senderid) {
        if (PhotonOfflineEvent != null) {
            PhotonOfflineEvent(eventcode, content, senderid);
        }
    }
}
