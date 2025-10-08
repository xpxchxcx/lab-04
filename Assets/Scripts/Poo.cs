using System;
using UnityEngine;


public class Poo : MonoBehaviour
{
    public static event Action OnPooCollected;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnPooCollected.Invoke();
            AudioManager.I.PlayBigPoop();
            this.gameObject.SetActive(false);
        }
    }

}
