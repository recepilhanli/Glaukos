using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MainCharacter;

public class TutorialDialogHandler : MonoBehaviour
{
    public static bool SetFullFocus = false;
    public static bool TutBlockGetBack = false;
    public static bool TutBlockThrow = false;
    public static bool TutBlockAttack1 = false;
    public static bool TutBlockAttack2 = false;


    private int TutorialIndex = -1;
    private int CurrentIndex = 0;

    private float TutCoolDown = 0f;


    private void OnDestroy()
    {
        TutBlockGetBack = false;
        TutBlockThrow = false;
        TutBlockAttack1 = false;
        TutBlockAttack2 = false;
        SetFullFocus = false;
    }


    IEnumerator Start()
    {
        TutBlockGetBack = true;
        TutBlockThrow = true;
        TutBlockAttack1 = true;
        TutBlockAttack2 = true;
        SetFullFocus = false;

        yield return new WaitForSeconds(0.15f);
        Player.Instance.Focus = 0;
        Player.Instance.CanMove = false;
        yield return null;
    }

    private void Update()
    {
        if (TutorialIndex == -1 || TutorialIndex >= 10) return;
        if (Player.Instance.CanMove == false) return;
        if (TutCoolDown >= Time.time) return;

        switch (TutorialIndex)
        {
            case 0:
                {
                    if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)) OnDialogChanged(++CurrentIndex);
                    break;
                }

            case 1:
                {
                    if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)) OnDialogChanged(++CurrentIndex);
                    TutBlockAttack1 = false;
                    break;
                }

            case 2:
                {
                    if (Input.GetKeyDown(KeyCode.Mouse0)) OnDialogChanged(++CurrentIndex);
                    TutBlockAttack2 = false;
                    break;
                }


            case 3:
                {
                    if (Input.GetKeyDown(KeyCode.Mouse1)) OnDialogChanged(++CurrentIndex);
                    TutBlockAttack2 = true;
                    TutBlockThrow = false;
                    break;
                }


            case 4:
                {
                    if (Player.Instance._Spear.ThrowState == Spear.ThrowStates.STATE_THROWING) OnDialogChanged(++CurrentIndex);
                    TutBlockAttack2 = false;
                    TutBlockGetBack = false;
                    TutCoolDown = Time.time + 0.25f;
                    break;
                }

            case 5:
                {
                    if (Player.Instance._Spear.ThrowState == Spear.ThrowStates.STATE_GETTING_BACK) OnDialogChanged(++CurrentIndex);
                    Player.Instance.Health = 50;
                    Player.Instance.Focus = 75;
                    break;
                }


            case 6:
                {
                    if (Input.GetKey(KeyCode.H)) OnDialogChanged(++CurrentIndex);
                    Player.Instance.Focus = 75;
                    break;
                }

            case 7:
                {
                    if (Input.GetKey(KeyCode.Alpha1)) OnDialogChanged(++CurrentIndex);
                    Player.Instance.Focus = 75;
                    break;
                }

            case 8:
                {
                    if (Input.GetKey(KeyCode.Alpha2)) OnDialogChanged(++CurrentIndex);
                    Player.Instance.Focus = 100;
                    SetFullFocus = true;
                    break;
                }

            case 9:
                {
                    if (Player.Instance._Rage) OnDialogChanged(++CurrentIndex);
                    TutBlockAttack1 = false;
                    TutBlockAttack2 = false;
                    TutBlockGetBack = false;
                    TutBlockThrow = false;
                    break;
                }


            default:
                break;
        }

    }

    public void OnDialogChanged(int index)
    {
        if (TutorialIndex >= 10) return;


        CurrentIndex = index;
        Debug.Log("OnDialogChanged: " + index);

        const int _StartIndex = 17; // Baslangic Indexi (Tutorial) -> Start index of tutorial

        if (index < _StartIndex) return; //wait for the conversation

        TutorialIndex = index - _StartIndex;


        if (TutorialIndex >= 10)
        {
            Dialogue.PlayingInstance.gameObject.SetActive(false);
            return;
        }

        Invoke("UnPasuePlayer", 0.5f);
        Dialogue.PlayingInstance.Paused = true;
        Dialogue.PlayingInstance.GetLine(index);
    }


    void UnPasuePlayer()
    {
        Player.Instance.CanMove = true;
    }


}
