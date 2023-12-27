using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.Events;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;
    private int index;

    [SerializeField] bool _StartOnAwake = true;
    [SerializeField, ReadOnlyInspector] bool _IsPlaying = false;


    [SerializeField] UnityEvent OnDialogStart = new UnityEvent();
    [SerializeField] UnityEvent<int> OnLineChanged = new UnityEvent<int>();
    [SerializeField] UnityEvent OnDialogEnd = new UnityEvent();

    void Start()
    {
        textComponent.text = string.Empty;
        if (_StartOnAwake) StartDialogue();
    }

    void Update()
    {
        if (!_IsPlaying) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
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
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            OnLineChanged.Invoke(index);
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
