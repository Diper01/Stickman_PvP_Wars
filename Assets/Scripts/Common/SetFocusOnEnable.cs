using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFocusOnEnable : MonoBehaviour {

    private void OnEnable()
    {
        StartCoroutine(SetFocus());
    }

    private IEnumerator SetFocus()
    {
        yield return new WaitForSeconds(0.1f);
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(this.gameObject);
    }
}
