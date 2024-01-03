using System.Collections;
using System.Collections.Generic;
using MainCharacter;
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

    [SerializeField] GameObject Focus_HealthIcon;
    [SerializeField] GameObject Focus_RageIcon;

    [SerializeField] Image FadeImage;
    [SerializeField] Image DeathFade;

    [SerializeField] Image[] Consumables = new Image[3];

    [SerializeField] Color AvailableColor;
    [SerializeField] Color UnavailableColor;

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
                if (Player.Instance.isDeath) SceneManager.LoadScene("Death");
                else SceneManager.LoadScene("Menu");
            }

            if (FocusBar != null) FocusBar.value = Player.Instance.Focus / 100;
            if (FocusBar != null) HealthBar.value = Player.Instance.Health / 100;


            if (Player.Instance.Focus >= 10 && Focus_HealthIcon.activeInHierarchy == false) Focus_HealthIcon.SetActive(true);
            else if (Player.Instance.Focus < 10 && Focus_HealthIcon.activeInHierarchy == true) Focus_HealthIcon.SetActive(false);

            if (Player.Instance.Focus >= 85 && Focus_RageIcon.activeInHierarchy == false) Focus_RageIcon.SetActive(true);
            else if (Player.Instance.Focus < 85 && Focus_RageIcon.activeInHierarchy == true) Focus_RageIcon.SetActive(false);

            if (Player.Instance.Focus >= 40 && Consumables[1].color != AvailableColor) Consumables[1].color = AvailableColor;
            else if (Player.Instance.Focus < 40 && Consumables[1].color != UnavailableColor) Consumables[1].color = UnavailableColor;

            if (Player.Instance.Focus >= 25 && Consumables[0].color != AvailableColor) Consumables[0].color = AvailableColor;
            else if (Player.Instance.Focus < 25 && Consumables[0].color != UnavailableColor) Consumables[0].color = UnavailableColor;

            if (Player.Instance.isDeath)
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
        var go = Instantiate(TitlePrefab);
        go.GetComponent<Title>().ShowTitle(text, .4f);
        LevelManager.PlaySound2D(_TitleChillClip, 0.3f);
    }
}
