using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used to manage the localization of the game.
/// </summary>



[System.Serializable]
public class Translation
{
    public string key;
    public string Lang_EN;
    public string Lang_TR;

    public static Dictionary<string, Translation> Translations {get; private set;} = new Dictionary<string, Translation>()
    {
        //Main Menu
        {"Menu_Start", new Translation("Start", "Başla") },
        {"Menu_Continue", new Translation("Continue", "Devam Et") },
        {"Menu_Options", new Translation("Options", "Ayarlar") },
        {"Menu_Quit", new Translation("Quit", "Çıkış") },
        {"Menu_TutorialQuestion", new Translation("Would you like to skip the tutorial?", "Eğitimi atlamak ister misiniz?") },
        
        {"Yes", new Translation("Yes", "Evet") },
        {"No", new Translation("No", "Hayır") },
        
        //Level Manager
        {"Checkpoint", new Translation("Checkpoint Reached","Kayit Noktasi Alindi")},


    };

    public Translation(string Lang_EN, string Lang_TR)
    {
        this.Lang_EN = Lang_EN;
        this.Lang_TR = Lang_TR;
    }

    public string Get()
    {
        if (PlayerPrefs.HasKey(PerfTable.perf_Language))
        {
            string lang = PlayerPrefs.GetString(PerfTable.perf_Language);
            if (lang == "TR")
            {
                return Lang_TR;
            }   
        }
        return Lang_EN;
    }

}

