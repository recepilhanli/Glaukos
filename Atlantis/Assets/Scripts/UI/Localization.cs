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

    public static Dictionary<string, Translation> Translations { get; private set; } = new Dictionary<string, Translation>()
    {
        //Main Menu
        {"Menu_Start", new Translation("Start", "Başla")},
        {"Menu_Options", new Translation("Options", "Ayarlar")},
        {"Menu_Quit", new Translation("Quit", "Çıkış")},
        {"Menu_TutorialQuestion", new Translation("Would you like to skip the tutorial?", "Eğitimi atlamak ister misiniz?")},

        {"Menu_Paused", new Translation("Game Paused", "Oyun Durduruldu")},
        {"Menu_Continue", new Translation("Resume Game", "Devam Et")},
        {"Menu_GQuit1", new Translation("Quit Game", "Oyundan Çık")},
        {"Menu_GQuit2", new Translation("Leaving", "Çıkış")},
        {"Menu_GQuitD", new Translation("Are you sure you want to quit the game?\nAll your progress will be reset.", "Oyundan ayrılmak istediğine emin misin?\nTüm ilerlemen sıfırlanacak.")},


        {"Level_1_present", new Translation("The Warm Welcome of The Sea", "Denizin Sıcak Karşılaşaması") },
        {"Level_2_present", new Translation("The Detphs of The Sea", "Denizin Derinlikleri") },
        {"Level_3_present", new Translation("Kraken's Cave", "Kraken'in Magarası") },
        {"Level_5_present", new Translation("Now or Never", "Ya Şimdi Ya Da Asla") },

        {"Loading", new Translation("Loading", "Yukleniyor") },
        {"LoadingCompleted", new Translation("[For Continue Press F Key]", "[Devam Etmek Icin F Tusuna Basin]") },

        {"Thank_1", new Translation("Encounter successfully completed!", "Karşılaşma başarıyla tamamlandı!") },
        {"Thank_2", new Translation("Next Level", "Bir Sonraki Seviye") },

        {"Past", new Translation("Glaukos: Remember the past and do not make mistake..", "Glaukos: Geçmişi hatırla ve bir daha hata yapma..") },

        {"Okay", new Translation("Okay", "Tamam") },
        {"Yes", new Translation("Yes", "Evet") },
        {"No", new Translation("No", "Hayır") },

        {"Mermaid", new Translation("Mermaid", "Deniz Kızı") },

        //Death Scene
        {"DeathMessage1", new Translation("You got lost in the depths of the sea!","Denizin derinliklerinde kayboldun!")},
        {"DeathMessage2", new Translation("<color=red>You are now a fish for the rest of your life!","<color=red>Geri kalan hayatinda artik bir baliksin!")},
        {"RemainingLife", new Translation("Remaining Life:","Kalan Hayat:")},
        {"LastCheckpoint", new Translation("Last Checkpoint","Son Yükleme Noktası")},

        {"Remaining", new Translation("Remaining:","Kalan:")},    

        //Level Manager
        {"Checkpoint", new Translation("Checkpoint Reached","Kayit Noktasi Alindi")},

        //Prologue Dialogue
        {"pro_d_0", new Translation("Polyphemus Aetherion: Who forgot their belongings again?","Polyphemus Aetherion: Sahilde yine kim unuttu eşyalarını??")},
        {"pro_d_1", new Translation("Polyphemus Aetherion: Ah, irresponsible people..","Polyphemus Aetherion: Sorumsuz insanlar..")},

        //Tutorial Dialogue
        {"tut_d_0", new Translation("Glaukos: Are you new heir of my powers?","Glaukos: Güçlerimin yeni temsilcisi sen misin?")},

        {"tut_d_1", new Translation("Polyphemus Aetherion: Ah, my head.. Who the fuck are you? Where am i? WTF is this spear?",
        "Polyphemus Aetherion: Ah, başım... Sen kimsin? Burası neresi? Elimdeki bu mızrak ne?")},
        {"tut_d_2", new Translation("Glaukos: Listen to me. Do you know why are you here?","Glaukos: Soru sormayı bırak ve beni dinle. Neden burada olduğunu ve nasıl geldiğini bilmiyor musun?")},

        {"tut_d_3", new Translation("Polyphemus Aetherion: If what I just experienced is real, I know it more or less.",
        "Polyphemus Aetherion: Eğer biraz önce yaşadıklarım gerçekse, az çok biliyorum.")},

        {"tut_d_4", new Translation("Glaukos: Yes, it was real. Everything you just experienced was exactly the same as what I went through.",
        "Glaukos: Evet, gerçekti. Biraz önce yaşadığın her şey, benim yaşadıklarımın aynısıydı.")},

        {"tut_d_5", new Translation("Glaukos: Now listen to me carefully, your sole purpose for now is to lift the curse. To do that, you need to defeat the Mermaid, the Kraken, and Scylla. It will be child's play for you since you possess my memories.",
        "Glaukos: Şimdi beni iyi dinle, şimdilik tek amacın laneti kaldırmak. Bunun için Deniz Kızı, Kraken ve Scylla'yı yenmen gerekiyor. Benim hatıralarıma sahip olduğun için onları bulman çocuk oyuncağı olacak.")},

        {"tut_d_6", new Translation("Polyphemus Aetherion: I understand, but how will I deal with these creatures",
        "Polyphemus Aetherion: Anladım, ama bu yaratıklarla nasıl başa çıkacağım?")},

        {"tut_d_7", new Translation("Glaukos: For now, you have some of my powers. I will show you how to use them. But remember, you only have five lives, so be careful not to forget that.",
        "Glaukos: Şimdilik güçlerimin bir kısmına sahipsin. Sana bunları nasıl kullanacağını göstereceğim. Ama sakın unutma, sadece beş defa ölme hakkın var.")},



       {"tut_d_7_1", new Translation("Glaukos: The blue section represents your health, while the 'remaining' text indicates your remaining lives. As you lose lives, you'll consume them, and when you run out, the loop resets.",
        "Glaukos:  Bu mavi bölüm canını gösterir 'kalan' yazısı ise kalan hayatını, hayatlarını öldükçe harcarsın ve hepsini bitirdiğinde döngü başa sarar.")},

        {"tut_d_7_2", new Translation("Glaukos: The green section represents your focus points. You can earn focus by breaking objects or hitting creatures, which allows you to use your abilities.",
        "Glaukos: Bu yeşil kısım focus puanını gösterir focusu eşya kırarak veya yaratıklara vurarak kazanabilirsin bu da yeteneklerini kullanmanı sağlar.")},

        {"tut_d_7_3", new Translation("Glaukos: This section shows your abilities. When the focus bar turns purple, you can use the rage ability. I will show this at the end of the training.",
        "Glaukos: Bu bölümde yeteneklerini gösterir, focus barın mor olunca rage yeteneğini kullanabilirsin eğtimin sonunda buna değineceğim.")},

        {"tut_d_8", new Translation("Glaukos: Press the <sprite=8>  and  <sprite=9> keys to swim upward and downward.",
        "Glaukos:  <sprite=8>  ve  <sprite=9> tuşlarına basarak yukarı ve aşağı yüzebilirsin.")},

        {"tut_d_9", new Translation("Glaukos: Press the <sprite=7>  and  <sprite=6> keys to swim to the right and left.",
        "Glaukos:  <sprite=7>  ve  <sprite=6> tuşlarına basarak sağ ve sol tarafa doğru yüzebilirsin.")},

        {"tut_d_10", new Translation("Glaukos: Press the  <sprite=2>  key to perform a quick-close attack.",
        "Glaukos:  <sprite=2>  tuşuna basarak hızlı-yakın saldırıda bulunabilirsin.")},

        {"tut_d_11", new Translation("Glaukos: Press the  <sprite=10>  key to perform a heavy-close attack.",
        "Glaukos:  <sprite=10>  tuşuna basarak ağır-yakın saldırıda bulunabilirsin.")},

        {"tut_d_12", new Translation("Glaukos: First, press the  <sprite=3>  key, then press the  <sprite=2>  key to throw your spear.",
        "Glaukos: Önce  <sprite=3>  tuşuna basarak  sonra  <sprite=2>  tuşuna basarak mızrağını fırlatabilirsin.")},

        {"tut_d_13", new Translation("Glaukos: Press the  <sprite=3>  key to recall the spear you've thrown.",
        "Glaukos:  <sprite=3>  tuşuna basarak  fırlattığın mızrağı çağırabilirsin.")},

        {"tut_d_14", new Translation("Glaukos: Press the  <sprite=5>  key to replenish your health while your spear is with you. However, this will decrease your focus points.",
        "Glaukos:  <sprite=5> tuşuna basarak mızrağın sendeyken canını doldurabilirsin. Fakat bu odaklanma puanının azalmasına neden olacaktır.")},

        {"tut_d_15", new Translation("Glaukos: Press the  <sprite=0>  key to use the tentacle attack ability. Keep in mind that this will decrease your focus points.",
         "Glaukos:  <sprite=0> tuşuna basarak hortum saldırısı yeteneğini kullanabilirsin. Bu da sahip olduğun  odaklanma puanını azaltıcaktır.")},

        {"tut_d_16", new Translation("Glaukos: Press the  <sprite=1>  key to unleash the spear rain attack ability. Note that this will decrease your focus points.",
        "Glaukos:  <sprite=1> tuşuna basarak mızrak yağmuru saldırısı yeteneğini kullanabilirsin. Bu da sahip olduğun  odaklanma puanını azaltıcaktır.")},

        {"tut_d_17", new Translation("Glaukos: Press the  <sprite=4>  key to activate the rage mode. In this mode, you move very quickly, take no damage, and each attack you make replenishes your health instead of focus points. However, yes, this also decreases your focus points.",
        "Glaukos:  <sprite=4> tuşuna basarak öfke modunu açabilirsin. Bu mod ile çok hızlı hareket edersin, hasar almazsın ve attığın her saldırı sana odaklanma puanı olarak değil, can olarak döner. Ve evet, bu da odaklanma puanını düşürür.")},

        {"tut_d_18", new Translation("Glaukos: Not bad, at least it will serve us until the curse is lifted. You better start on your way.",
        "Glaukos:  Fena değil en azından laneti kaldırana kadar işimizi görür. Yola çıkmaya başlasan iyi olur.")},




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

