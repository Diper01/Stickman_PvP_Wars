using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimation : MonoBehaviour {
    public List<Sprite> sprites;
    public bool loop;
    public float animTime;

    private SpriteRenderer sprRenderer;
    private float updateTime;

    private void Start()
    {
        if (sprites.Count == 0) {
            return;
        }

        sprRenderer = GetComponent<SpriteRenderer>();
        updateTime = animTime / sprites.Count;
        StartCoroutine(Animate());
    }

    private IEnumerator Animate() {
        do
        {
            foreach (var spr in sprites)
            {
                sprRenderer.sprite = spr;
                yield return new WaitForSeconds(updateTime);
            }
        } while (loop);
    }
}
