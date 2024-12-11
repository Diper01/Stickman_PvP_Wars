using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopFlameEmission : MonoBehaviour {

    public float timeDelay;
	
	IEnumerator Start () {
        yield return new WaitForSeconds(timeDelay);      
        GetComponent<ParticleSystem>().Stop();

    }
	
	
}
