using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.Events;

public class Dialogue : MonoBehaviour
{
    public static Dialogue PlayingInstance = null;
    public bool Paused = false;
    public TextMeshProUGUI textComponent;
    [SerializeField, Tooltip("it can be null")] Animator _Animator;
    public string[] lines;
    public float textSpeed;
    private int index;

    [SerializeField] bool _StartOnAwake = true;
    [SerializeField, ReadOnlyInspector] bool _IsPlaying = false;
    [SerializeField] UnityEvent OnDialogStart = new UnityEvent();
    [SerializeField] UnityEvent<int> OnLineChanged = new UnityEvent<int>();
    [SerializeField] UnityEvent OnDialogEnd = new UnityEvent();

    private string[] CharacterNames = new string[2];




    void Start()
    {
        PlayingInstance = this;

        CharacterNames[0] = string.Empty;
        CharacterNames[1] = string.Empty;
        GetCharacters();
        textComponent.text = string.Empty;

        if (_StartOnAwake) StartDialogue();
    }

    void Update()
    {
        if (!_IsPlaying || Paused) return;
        if (PauseMenu.instance != null)
        {
            if (PauseMenu.instance.isPaused) return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else GetCurrentLine();
        }
    }


    public void GetCurrentLine()
    {
        StopAllCoroutines();
        textComponent.text = lines[index];
    }

    public void GetLine(int index)
    {
        StopAllCoroutines();
        textComponent.text = lines[index];
    }


    public void StartDialogue()
    {
        _IsPlaying = true;
        index = 0;
        OnDialogStart.Invoke();
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {

        foreach (char c in lines[index].ToCharArray())
        {

            // //if it's paused, wait until it's unpaused
            // while (Paused)
            // {
            //     yield return null;
            // }

            if (PauseMenu.instance != null)
            {
                while (PauseMenu.instance.isPaused)
                {
                    yield return null;
                }
            }


            if (Paused) break;
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        yield return null;
    }



    void GetCharacters()
    {
        if (_Animator == null) return;



        foreach (var item in lines)
        {
            string name = item.Split(':')[0];


            if (CharacterNames[0] != string.Empty && name != string.Empty && CharacterNames[0] != name)
            {
                CharacterNames[1] = name;
                Debug.Log("Last: " + name);
                break;
            }
            else if (name != string.Empty)
            {
                CharacterNames[0] = name;
            }

        }

        foreach (var item in CharacterNames)
        {
            Debug.Log(item);
        }

    }

    void SwitchBetweenCharacters()
    {
        if (_Animator == null) return;

        if (CharacterNames[0] != string.Empty && CharacterNames[1] != string.Empty)
        {
            string character = lines[index].Split(':')[0];

            Debug.Log("Line Turn: " + character);

            if (CharacterNames[0] == character)
            {
                _Animator.SetInteger("Character", 1);
            }
            else if (CharacterNames[1] == character)
            {
                _Animator.SetInteger("Character", 2);
            }
        }

    }

    void NextLine()
    {
        if (Paused) return;

        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            OnLineChanged.Invoke(index);
            SwitchBetweenCharacters();
            Debug.Log("Line Changed");
            StartCoroutine(TypeLine());
        }
        else
        {
            Debug.Log("End of Dialogue");
            OnDialogEnd.Invoke();
            _IsPlaying = false;
            gameObject.SetActive(false);
        }
    }
}
