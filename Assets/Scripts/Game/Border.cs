using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Border : MonoBehaviour {

    public bool isHorizontl = true;

    void OnDrawGizmosSelected()
    {
        Vector3 offset;
        if (isHorizontl) {
            offset = new Vector3(100, 0, 0);
        }
        else {
            offset = new Vector3(0, 100, 0);
        }

        Gizmos.color = Color.yellow;      
        Gizmos.DrawLine(transform.position - offset, transform.position + offset);        
    }
}
