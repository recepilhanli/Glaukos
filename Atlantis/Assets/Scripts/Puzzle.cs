using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour
{

    [SerializeField] Animator PuzzleAnimator;

    public void SetPuzzleActive()
    {
        PuzzleAnimator.SetBool("active", true);
    }


    public void SetPuzzleInActive()
    {
        PuzzleAnimator.SetBool("active", false);
    }



}
