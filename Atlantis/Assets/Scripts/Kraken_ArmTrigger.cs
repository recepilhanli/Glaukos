using System.Collections;
using System.Collections.Generic;
using MainCharacter;
using UnityEngine;

/// <summary>
/// kraken arm trigger class
/// </summary>
public class Kraken_ArmTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Weapon") || other.gameObject.CompareTag("Harmful"))
        {
            if (!Player.Instance.CanMove) return;
            Debug.Log("Kraken Takes damage trigger");
            Kraken.Instance.KrakenArm(transform, other.gameObject.transform, other.gameObject.tag);
        }
    }

    //create a method that will be called from the kraken script
    //this method will be called when the kraken arm hits the player
    //this method will be called when the kraken arm hits the player


    public void DropPlayer()
    {
        Kraken.Instance.SetAnimState(Kraken.Kraken_AnimStates.State_Idle);
        Player.Instance.transform.SetParent(null);
         Player.Instance.transform.transform.eulerAngles = Vector3.zero;
        Player.Instance.CanMove = true;
    }

    public void DamagePlayer(float dmg)
    {
        Player.Instance.OnTakeDamage(dmg);
    }

}
