using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> enemies = new List<GameObject>();
    [SerializeField]
    private List<Transform> spawnPoints = new List<Transform>();
    
    
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private GameObject green_1;
    [SerializeField] private GameObject green_2;
    [SerializeField] private GameObject red_1;
    [SerializeField] private GameObject red_2;
    
    private bool coroutineRunning = false;
    
    [SerializeField]
    private float timerInSeconds;

    private void Update()
    {
        if (IsAllDie())
        {
            if (!coroutineRunning)
            {
                ReturnOnPositions();
                StartCoroutine(StartSpawnTimer());
                coroutineRunning = true;
            }
        }
    }

    private bool IsAllDie()
    {
        foreach (var enemy in enemies)
        {
            if (enemy.activeInHierarchy)
            {
                return false;
            }
        }

        return true;
    }

    private void ReturnOnPositions()
    {
        foreach (var enemy in enemies)
        {
            enemy.transform.localPosition = Vector3.zero;
            enemy.GetComponent<Health>()?.Heal(100);
        }
    }

    private void ActivateEnemies()
    {
        foreach (var enemy in enemies)
        {
            enemy.SetActive(true);
        }
    }

    
    
    private IEnumerator StartSpawnTimer()
    {
        yield return new WaitForSeconds(timerInSeconds);
        
        coroutineRunning = false;
        ActivateEnemies();
        StartCoroutine(ChangeColorTimer());
    }
    
    private void SetColorGreen()
    {
        green_1.SetActive(true);
        green_2.SetActive(true);
        red_1.SetActive(false);
        red_2.SetActive(false);
    }

    private void SetColorRed()
    {
        audioSource.PlayOneShot(audioSource.clip);
        green_1.SetActive(false);
        green_2.SetActive(false);
        red_1.SetActive(true);
        red_2.SetActive(true);
    }

    private IEnumerator ChangeColorTimer()
    {
        SetColorRed();
        yield return new WaitForSeconds(3f);
        SetColorGreen();
    }
}
