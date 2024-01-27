using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


[System.Serializable]
public class TranslateObject
{
    public string key;
    public TextMeshProUGUI text;
}



public class LocalizationListener : MonoBehaviour
{
    public List<TranslateObject> translateObjects = new List<TranslateObject>();

    [SerializeField] bool DestroyOnAwake = false;
    public void Localize()
    {
        foreach (var item in translateObjects)
        {
            item.text.text = Translation.Translations[item.key].Get();
        }
    }


    void Awake()
    {
        Localize();
        if (DestroyOnAwake) Destroy(this);
    }

}
