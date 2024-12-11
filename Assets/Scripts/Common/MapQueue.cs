using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapQueue {
   
    public static List<MapQueueEntry> MapQueueList {
        get {
            if (PhotonNetwork.room == null
                || PhotonNetwork.room.CustomProperties == null
                || PhotonNetwork.room.CustomProperties.ContainsKey(RoomProperty.MapQueue) == false)
            {
                return new List<MapQueueEntry>();
            }
            else
            {
                string mapQueueString = (string)PhotonNetwork.room.CustomProperties[RoomProperty.MapQueue];
                return StringToList(mapQueueString);
            }
        }
        set {
            if (PhotonNetwork.room != null)
            {
                string mapQueueString = ListToString(value);
                ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable();
                roomProperties.Add(RoomProperty.MapQueue, mapQueueString);
                PhotonNetwork.room.SetCustomProperties(roomProperties);
            }
        }
    }

    public static int MapQueueIndex {
        get {
            if (PhotonNetwork.room == null
               || PhotonNetwork.room.CustomProperties == null
               || PhotonNetwork.room.CustomProperties.ContainsKey(RoomProperty.MapQueueIndex) == false)
            {
                Debug.Log("Cant find map queue index");
                return 0;
            }
            else
            {
                return (int)PhotonNetwork.room.CustomProperties[RoomProperty.MapQueueIndex];
            }
        }
        set {
            if (PhotonNetwork.room != null)
            {
                ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable();
                roomProperties.Add(RoomProperty.MapQueueIndex, value);
                PhotonNetwork.room.SetCustomProperties(roomProperties);
            }
        }
    }

	public static string ListToString(List<MapQueueEntry> mapQueue)
    {        
        string[] mapSegments = new string[mapQueue.Count];     
        for (int i = 0; i < mapQueue.Count; ++i)
        {
            mapSegments[i] = EntryToString(mapQueue[i]);
        }       
        return string.Join("~", mapSegments);
    }
    
    public static List<MapQueueEntry> StringToList(string mapQueueString)
    {
        List<MapQueueEntry> mapQueue = new List<MapQueueEntry>();
      
        string[] mapSegments = mapQueueString.Split('~');

        for (int i = 0; i < mapSegments.Length; ++i)
        {           
            MapQueueEntry newQueueEntry = StringToEntry(mapSegments[i]);

            mapQueue.Add(newQueueEntry);
        }

        return mapQueue;
    }
   
    public static MapQueueEntry StringToEntry(string mapQueueEntryString)
    {
        string[] mapData = mapQueueEntryString.Split('#');

        MapQueueEntry queueEntry = new MapQueueEntry
        {
            Map = (Maps)(int.Parse(mapData[0])),
            Mode = (GameMode)(int.Parse(mapData[1]))
        };

        return queueEntry;
    }
  
    public static string EntryToString(MapQueueEntry entry)
    {
        return (int)entry.Map + "#" + (int)entry.Mode;
    }              

}
