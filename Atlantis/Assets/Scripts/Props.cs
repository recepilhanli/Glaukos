using System.Collections;
using System.Collections.Generic;
using MainCharacter;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class Props : MonoBehaviour
{
    [SerializeField] Sprite _BrokenSprite;

    [SerializeField] SpriteRenderer _Renderer;
    [SerializeField] GameObject _GivingItemAfterBreakingPrefab;

    [SerializeField] UnityEvent OnBreakProp = new UnityEvent();

    bool _Broken = false;

     private void Awake()
    {
      if(_BrokenSprite == null)  OnBreakProp?.AddListener(DestroyProp);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (!_Broken && IsDamaging(other.gameObject)) Break();
    }

    public void Break()
    {
        if (_Broken) return;
        _Broken = true;
        if(_BrokenSprite != null) _Renderer.sprite = _BrokenSprite;
        if (_GivingItemAfterBreakingPrefab != null)
        {
            Instantiate(_GivingItemAfterBreakingPrefab, transform.position, Quaternion.identity);
        }
        Player.Instance.CameraShake(2, 0.5f, 0.01f);
        var light = GetComponent<Light2D>();
        if (light != null) light.enabled = false;
        OnBreakProp?.Invoke();
    }

    bool IsDamaging(GameObject go)
    {
        if (!go.CompareTag("Weapon")) return false;

        var WepType = go.GetComponent<Weapons>().Type;

        switch (WepType)
        {
            case Weapons.Weapon_Types.Type_Spear:
                {
                    var spear = go.GetComponent<Spear>();
                    if (spear.ThrowState != 0) return true;
                    break;
                }

            default: break;
        }
        return false;
    }


    private void Update()
    {
        if (_Broken)
        {
            var color = _Renderer.color;
            color.a -= Time.deltaTime / 2.5f;
            _Renderer.color = color;
            if (color.a <= 0) Destroy(gameObject);
        }
    }


    void DestroyProp() //for events
    {
        Destroy(gameObject);
    }
}
