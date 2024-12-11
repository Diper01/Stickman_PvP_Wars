using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class BotsInformation {            

    private static List<string> botsNames;
    private static System.Random rand;
    private static int baseBotId = 101;  

    static BotsInformation() {
        rand = new System.Random();       
        Reset();
    }

    public static void Reset() {
        baseBotId = 101;
        botsNames = new List<string>() {  "Гость", "Elf12", "Antonio", "Скорпион", "Бугатти", "Potter"
                                        , "Чёрный плащ", "Незнайка", "Чарли", "Марк", "Дима", "Гатлинг", "Убивашка"
                                        , "Диджей", "Саша", "Женя", "Бетмен", "Робин", "Ельф", "Олег99", "Патриот"
                                        , "Пират", "Король", "Князь", "Влад", "Оптимус", "Лаки Старр", "ЧеГеВара"
                                        , "Фидель", "ПулякаУбивака", "Torzak", "Hellraiser", "Bombermen", "БобрНеДобр"
                                        , "Naruto", "Терорист", "Hello_Kitty", "Enot", "Bumbershoot", "Mumpsimus"
                                        , "Comet", "Scoop", "Curles", "Hook", "Tank", "Blondie", "Shade", "Eagle"
                                        , "Buddy", "Dragon", "Ellidi", "sissor", "amammumi", "maloniga", "Gragak"
                                        , "Nark", "Pipina", "Salana", "Gagete", "Bautlan", "Eilgert", "S4murai"
                                        , "Comm4ndo", "ThrowEr", "Smurf", "BomBaRdier", "NINja", "Killsteal3r", "AssA5Sin"
                                        , "Gren4diEr", "Lurk3r", "C4Mper", "FragGEr", "D34Deye", "Expert", "S7abber"
                                        , "CarRy", "Burning", "ММА", "Kraken", "Colt", "Diablo", "Void", "Tempest"
                                        , "t4ty", "Мурчик", "Vasasa", "Gavufe", "Zyaal", "Beanyu", "Mydak", "Sii", "Real"
                                        , "Loz", "Syalon", "IIIIIIIIII", "iiiiiiiiiii", "wmwmw", "Psyduck", "Squirtle"
                                        , "sOs", "Parting", "Inovation", "Niko", "vYwYv", "Torres", "Бімба", "Goku"
                                        , "Школотрон", "Krab", "Росомаха", "Shtirlitz", "Fed_Cow", "AngryChicken", "lazy"};
    }

    public static Dictionary<int, string> GetBotsDictionary() {        
        if (PhotonNetwork.room != null && PhotonNetwork.room.CustomProperties.ContainsKey(RoomProperty.BotList))
        {
            string botsString = (string)PhotonNetwork.room.CustomProperties[RoomProperty.BotList];
            return BotsStringToDictionary(botsString);
        }
        else {
            return new Dictionary<int, string>();
        }
    }

    public static void AddBotToDictionary(int botId, string botName) {
        if (PhotonNetwork.room == null)
            return;

        Dictionary<int, string> bots = GetBotsDictionary();       
        bots.Add(botId, botName);

        string botsString = BotsDictionaryToString(bots);
        ExitGames.Client.Photon.Hashtable roomPrperties = new ExitGames.Client.Photon.Hashtable();
        roomPrperties.Add(RoomProperty.BotList, botsString);
        PhotonNetwork.room.SetCustomProperties(roomPrperties);
    }

    public static void RemoveBot(int botId) {
        if (PhotonNetwork.room == null)
            return;

        Dictionary<int, string> bots = GetBotsDictionary();
        bots.Remove(botId);

        string botsString = BotsDictionaryToString(bots);
        ExitGames.Client.Photon.Hashtable roomPrperties = new ExitGames.Client.Photon.Hashtable();
        roomPrperties.Add(RoomProperty.BotList, botsString);
        PhotonNetwork.room.SetCustomProperties(roomPrperties);
    }
      
    public static int AddBot()
    {
        int botId = baseBotId++;
        string botName = botsNames[rand.Next(0, botsNames.Count)];       
        botsNames.Remove(botName);       
        AddBotToDictionary(botId, botName);
        return botId;      
    }

    public static Team GetBotTeam(int botId) {
        if (PhotonNetwork.room != null && PhotonNetwork.room.CustomProperties.ContainsKey(RoomProperty.BotTeam + botId))
            return (Team)PhotonNetwork.room.CustomProperties[RoomProperty.BotTeam + botId];
        else
            return Team.RED;
    }

    public static void SetBotTeam(int botId, Team team) {
        if (PhotonNetwork.room != null) {
            ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable();
            roomProperties.Add(RoomProperty.BotTeam + botId, team);
            PhotonNetwork.room.SetCustomProperties(roomProperties);
        }
    }

    public static int GetBotRoundWon(int botId) {
        if (PhotonNetwork.room == null || !PhotonNetwork.room.CustomProperties.ContainsKey(RoomProperty.BotRoundWin + botId))
        {
            return 0;
        }

        return (int)PhotonNetwork.room.CustomProperties[RoomProperty.BotRoundWin + botId];
    }

    public static void SetBotRoundWon(int botId, int roundwon) {
        if (PhotonNetwork.room == null)
            return;

        ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable();
        roomProperties.Add(RoomProperty.BotRoundWin + botId, roundwon);
        PhotonNetwork.room.SetCustomProperties(roomProperties);
    }

    private static Dictionary<int, string> BotsStringToDictionary(string botsString)
    {                   
        Dictionary<int, string> botsDictionary = new Dictionary<int, string>();

        if (botsString.Length < 1)
            return botsDictionary;

        string[] mapSegments = botsString.Split('~');

        for (int i = 0; i < mapSegments.Length; ++i)
        {
            string[] botData = mapSegments[i].Split('#');          
            int key = Convert.ToInt32(botData[0]);
            string botName = botData[1];          
            botsDictionary.Add(key, botName);
        }

        return botsDictionary;
    }

    private static string BotsDictionaryToString(Dictionary<int, string> bots) {   
        string[] botSegments = new string[bots.Count];

        int i = 0;
        foreach (var key in bots.Keys)
        {
            botSegments[i] = key + "#" + bots[key];
            i++;
        }
        return string.Join("~", botSegments);
    }
    
}

