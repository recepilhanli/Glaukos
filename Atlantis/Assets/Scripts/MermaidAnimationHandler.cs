using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MainCharacter;

public class MermaidAnimationHandler : MonoBehaviour
{

    public Mermaid _Mermaid;

    public void Wave()
    {
        _Mermaid.CreateWave();
    }

    public void HandleAttack()
    {
        _Mermaid.Attack(Player.Instance, 15f);
    }

    public void HandlePosion()
    {
        _Mermaid.CreatePosion();
    }

}
