using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotAnimation : MonoBehaviour {

    [SerializeField] float lifeTime;

    private void Start()
    {
        StartCoroutine(DestroyObject());
    }

    private IEnumerator DestroyObject() {
        yield return new WaitForSeconds(lifeTime);
        Destroy(this.gameObject);
    }
}
