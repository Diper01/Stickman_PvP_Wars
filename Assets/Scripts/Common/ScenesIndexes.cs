using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScenesIndexes {
    public static int Splash = 0;
    public static int Start = 1; 
    public static int Matchmaking = 2;
    public static int LoadNextMap = 12;  

    public static int GetMapSceneIndex(Maps map) {       
        switch (map)
        {
            case Maps.CITY_FFA:
                return 3;               
            case Maps.JUNGLE_FFA:
                return 4;               
            case Maps.FACTORY_FFA:
                return 5;
            case Maps.UNDERWATER_TEAM:
                return 6;
            case Maps.ROCKET_TEAM:
                return 7;
            case Maps.FACTORY_TEAM:
                return 8;
            case Maps.CITY_CTF:
                return 9;
            case Maps.JUNGLE_CTF:
                return 10;
            case Maps.UNDERWATER_CTF:
                return 11;
                
        }
        return 3;
    }
}
