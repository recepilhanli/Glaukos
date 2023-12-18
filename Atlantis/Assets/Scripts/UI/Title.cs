using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

/// <summary>
/// This class is used to manage the title text of the game.
/// </summary>
public class Title : MonoBehaviour
{

    public string Text; //for testing

    [SerializeField] TextMeshProUGUI _TMP;

    private static Title PlayingTitle = null;

    public void ShowTitle(string text, float splittingTime, char seperator = ' ')
    {
        if (PlayingTitle != null) Destroy(PlayingTitle.gameObject);
        PlayingTitle = this;
        Text = text;
        var words = text.Split(seperator);

        StartCoroutine(ITitle(words, splittingTime));
    }

    IEnumerator ITitle(string[] words, float time)
    {
        foreach (var word in words)
        {
            _TMP.text += word + " ";
            yield return new WaitForSeconds(time);
        }
        yield return new WaitForSeconds(time * 3);

        for (int i = words.Length - 1; i > -1; i--)
        {
            var word = words[i];
           
            _TMP.text = _TMP.text.Replace(word + " ", string.Empty);
            yield return new WaitForSeconds(time / 5);
        }
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
        yield return null;
    }

}


[CustomEditor(typeof(Title))]
public class TitleEditorGUI : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        if (GUILayout.Button("Test"))
        {
            if (Application.isPlaying)
            {
                var title = FindObjectOfType<Title>();
                title.ShowTitle(title.Text, 0.25f);
            }
            else Debug.LogError("Try it in the play mode!");
        }

    }
}