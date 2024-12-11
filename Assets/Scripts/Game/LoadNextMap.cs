using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextMap : Photon.PunBehaviour {

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        if (PhotonNetwork.isMasterClient)
        {
            int nextMapSceneIndex = ScenesIndexes.GetMapSceneIndex(MapQueue.MapQueueList[MapQueue.MapQueueIndex].Map);
            PhotonNetwork.LoadLevel(nextMapSceneIndex);
        }            
    }

    public override void OnDisconnectedFromPhoton()
    {        
        SceneManager.LoadScene(ScenesIndexes.Start);
    }
}
