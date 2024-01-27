using System.Collections;
using System.Collections.Generic;
using MainCharacter;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// manage the UI of the game
/// </summary>
public class UIManager : MonoBehaviour
{

    public static UIManager Instance = null;

    [SerializeField] Slider HealthBar;
    [SerializeField] Slider FocusBar;

    [SerializeField] Image FadeImage;
    [SerializeField] Image DeathFade;


    [SerializeField] Image[] Consumables = new Image[2];
    [SerializeField] TextMeshProUGUI _RemainingLifeText;

    [SerializeField] Color AvailableColor;
    [SerializeField] Color UnavailableColor;

    [SerializeField] Image FocusFillImage;
    [SerializeField] Image HealthFillImage;

    [SerializeField] Color _HealthNormalColor;
    [SerializeField] Color _HealthWarnColor;

    [SerializeField] Color FocusNormalColor;
    [SerializeField] Color FocusRageColor;

    [SerializeField] GameObject TitlePrefab;
    [SerializeField] AudioClip _TitleChillClip;

    public bool StopFading = false;

    private float fadeMultiplier;


    public void Fade(float r, float g, float b, float speed = 2)
    {
        if (StopFading) return;
        var color = new Color(r, g, b, 0.5f);
        FadeImage.color = color;
        fadeMultiplier = speed / 1.3f;
    }

    private void Start()
    {
        Instance = this;
        if (DeathFade != null) DeathFade.color = new Color(0, 0, 0, .95f);
    }

    void Update()
    {
        if (Player.Instance != null)
        {

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (Player.Instance.RewardSequence) SceneManager.LoadScene(PerfTable.perf_LevelThank);
                else if (Player.Instance.isDeath) SceneManager.LoadScene(PerfTable.perf_LevelDeath);
                else if (PauseMenu.instance != null) PauseMenu.instance.TogglePause(!PauseMenu.instance.isPaused);
            }

            if (FocusBar != null) FocusBar.value = Player.Instance.Focus / 100;
            if (FocusBar != null) HealthBar.value = Player.Instance.Health / 100;

            if (_RemainingLifeText.text == string.Empty) _RemainingLifeText.text = Player.RemainingLifes.ToString();

            if (Player.Instance.Focus >= 85) FocusFillImage.color = FocusRageColor;
            else FocusFillImage.color = FocusNormalColor;

            if (Player.Instance.Health <= 35) HealthFillImage.color = _HealthWarnColor;
            else HealthFillImage.color = _HealthNormalColor;


            if (Player.Instance.Focus >= 40 && Consumables[1].color != AvailableColor) Consumables[1].color = AvailableColor;
            else if (Player.Instance.Focus < 40 && Consumables[1].color != UnavailableColor) Consumables[1].color = UnavailableColor;

            if (Player.Instance.Focus >= 25 && Consumables[0].color != AvailableColor) Consumables[0].color = AvailableColor;
            else if (Player.Instance.Focus < 25 && Consumables[0].color != UnavailableColor) Consumables[0].color = UnavailableColor;

            if (Player.Instance.isDeath || Player.Instance.RewardSequence)
            {
                var color = DeathFade.color;
                color.a += Time.deltaTime / 2;
                DeathFade.color = color;
            }
            else if (DeathFade.color.a > 0)
            {
                var color = DeathFade.color;
                color.a -= Time.deltaTime / 2;
                DeathFade.color = color;
            }
        }

        if (FadeImage.color.a != 0 && !StopFading)
        {
            var color = FadeImage.color;
            color.a -= Time.deltaTime * fadeMultiplier;
            color.a = Mathf.Clamp(color.a, 0, 1);
            FadeImage.color = color;
        }

    }


    public void ShowTitle(string text)
    {
        var split = text.Split('$');
        if (split.Length > 1)
        {
            text = Translation.Translations[split[1]].Get();
        }
        
        var go = Instantiate(TitlePrefab);
        go.GetComponent<Title>().ShowTitle(text, .4f);
        LevelManager.PlaySound2D(_TitleChillClip, 0.3f);
    }
}
