using System;
using UnityEngine;
using UnityEngine.Events;


public class Poo : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent pooCollected;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            pooCollected.Invoke();
            AudioManager.I.PlayBigPoop();
            this.gameObject.SetActive(false);
        }
    }

}
