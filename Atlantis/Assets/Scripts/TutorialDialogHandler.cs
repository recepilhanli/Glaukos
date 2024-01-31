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
    public GameObject objectToDisableAfterDialog;

    [SerializeField] GameObject _TutorialBlock1;

    [SerializeField] GameObject _TutorialBlock2;

    [SerializeField] GameObject _TutorialBlock3;

    private int TutorialIndex = -1;
    private int CurrentIndex = 0;

    private float TutCoolDown = 0f;

    private bool _SkipingLastDialog = false;

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
        Player.ResetRemainingLifes();
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
        if (TutorialIndex == -1 || TutorialIndex >= 11) return;
        if (Player.Instance.CanMove == false) return;
        if (TutCoolDown >= Time.time) return;




        switch (TutorialIndex)
        {
            case 0:
                {
                    if (Input.GetAxis("Vertical") != 0) OnDialogChanged(++CurrentIndex);
                    break;
                }

            case 1:
                {
                    if (Input.GetAxis("Horizontal") != 0) OnDialogChanged(++CurrentIndex);
                    TutBlockAttack1 = false;
                    TutBlockThrow = true;
                    break;
                }

            case 2:
                {
                    if (Input.GetKeyDown(KeyCode.Mouse0)) OnDialogChanged(++CurrentIndex);
                    TutBlockAttack2 = false;
                    TutBlockThrow = true;
                    break;
                }


            case 3:
                {
                    if (Input.GetKeyDown(KeyCode.Mouse2))
                    {
                        OnDialogChanged(++CurrentIndex);
                        TutBlockThrow = false;
                    }
                    TutBlockAttack2 = true;
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
            case 10:
                {
                    if (!_SkipingLastDialog)
                    {
                        Invoke("SkipLastDialog", 8f);
                        _SkipingLastDialog = true;
                    }

                    if (Input.GetKeyDown(KeyCode.Mouse0)) OnDialogChanged(++CurrentIndex);
                    TutBlockAttack2 = false;

                    break;
                }



            default:
                break;
        }

    }

    void SkipLastDialog()
    {
        OnDialogChanged(++CurrentIndex);
    }

    public void OnDialogChanged(int index)
    {
        if (TutorialIndex >= 11) return;



        if (index == 8)
        {
            _TutorialBlock1.SetActive(true);
        }
        if (index == 9)
        {
            Player.Instance.Focus = 35;
            _TutorialBlock2.SetActive(true);
            _TutorialBlock1.SetActive(false);
        }
        if (index == 10)
        {
            Player.Instance.Focus = 70;
            _TutorialBlock2.SetActive(false);
            _TutorialBlock3.SetActive(true);
        }
        if (index == 11)
        {
            Player.Instance.Focus = 0;
            _TutorialBlock1.SetActive(false);
            _TutorialBlock2.SetActive(false);
            _TutorialBlock3.SetActive(false);
        }


        CurrentIndex = index;
        Debug.Log("OnDialogChanged: " + index);

        const int _StartIndex = 11; // Baslangic Indexi (Tutorial) -> Start index of tutorial

        if (index < _StartIndex) return; //wait for the conversation

        TutorialIndex = index - _StartIndex;


        if (TutorialIndex >= 11)
        {
            Dialogue.PlayingInstance.gameObject.SetActive(false);
            Invoke("GetFirstLevel", 4f);
            if (objectToDisableAfterDialog != null)
            {
                objectToDisableAfterDialog.SetActive(false);
            }
            return;
        }

        Invoke("UnPasuePlayer", 0.5f);
        Dialogue.PlayingInstance.Paused = true;
        Dialogue.PlayingInstance.GetLine(index);
    }

    void GetFirstLevel()
    {
        Player.Instance.BossKillReward(PerfTable.perf_Level1, false);
    }


    void UnPasuePlayer()
    {
        Player.Instance.CanMove = true;
    }


}
