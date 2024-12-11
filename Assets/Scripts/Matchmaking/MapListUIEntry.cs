using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MapListUIEntry : MonoBehaviour {
    public Action<int> RemoveButtonClick;
    public int EntryIndex { get; set; }

    [SerializeField] Text mapText;

    public void SetupEntry(MapQueueEntry entry) {
        string mapName = "";
        switch (entry.Map)
        {            
            case Maps.CITY_FFA:
                mapName = Localisation.GetString("CityMap");
                break;
            case Maps.JUNGLE_FFA:
                mapName = Localisation.GetString("JungleMap");
                break;
            case Maps.FACTORY_FFA:
                mapName = Localisation.GetString("FactoryMap");
                break;
            case Maps.UNDERWATER_TEAM:
                mapName = Localisation.GetString("UnderwaterBase");
                break;
            case Maps.ROCKET_TEAM:
                mapName = Localisation.GetString("RocketBase");
                break;
            case Maps.FACTORY_TEAM:
                mapName = Localisation.GetString("FactoryMap");
                break;
            case Maps.CITY_CTF:
                mapName = Localisation.GetString("CityMap");
                break;
            case Maps.JUNGLE_CTF:
                mapName = Localisation.GetString("JungleMap");
                break;
            case Maps.UNDERWATER_CTF:
                mapName = Localisation.GetString("UnderwaterBase");
                break;
        }        
        mapText.text = mapName;
    }

    public void OnRemoveButton() {
        if (RemoveButtonClick != null) {
            RemoveButtonClick(EntryIndex);
        }
    }


}
