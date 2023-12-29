using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repeater : MonoBehaviour
{
    [SerializeField] Sprite _Sprite;
    [SerializeField] Color _SpriteColor;
    [SerializeField] float _RepeatTime = 0.75f;

    IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitForSeconds(_RepeatTime);
            var go = new GameObject("Repeater");
            go.transform.position = transform.position + (transform.up * 0.5f);
            go.transform.rotation = transform.rotation;
            go.transform.localScale = transform.localScale;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = _Sprite;
            sr.color = _SpriteColor;
            go.AddComponent<RepeaterFade>();
            yield return null;
        }
    }

}
