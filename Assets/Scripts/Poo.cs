using System;
using UnityEngine;
using UnityEngine.Events;


public class Poo : MonoBehaviour
{

    public UnityEvent OnPooPooCollected;

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
        Debug.Log($"colldied with: {collision.name}");
        if (collision.CompareTag("Player"))
        {

            OnPooPooCollected.Invoke();
            Debug.Log("POOPOOCOLLECTED! raised from poo!");
            //AudioManager.I.PlayBigPoop();
            this.gameObject.SetActive(false);
        }
    }

}
