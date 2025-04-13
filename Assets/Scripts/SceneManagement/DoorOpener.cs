using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    [SerializeField]
    private RangedEnemy[] enemiesRange;
    [SerializeField] 
    private MeleeEnemy[] enemiesMelee;

    [SerializeField] private bool IsAllEnemiesDie;
    
    [SerializeField]
    private Transform leftDoor;
    [SerializeField]
    private Transform rightDoor;

    private void Start()
    {
        enemiesMelee = FindObjectsOfType<MeleeEnemy>(false);
        enemiesRange = FindObjectsOfType<RangedEnemy>(false);
    }

    private void Update()
    {
        if (ListIsEmpty(enemiesRange) && ListIsEmptyMelee(enemiesMelee))
        {
            IsAllEnemiesDie = true;
        }

        if (IsAllEnemiesDie)
        {
            leftDoor.position = new Vector3(leftDoor.position.x, leftDoor.position.y-5f, leftDoor.position.z);
            rightDoor.position = new Vector3(rightDoor.position.x, rightDoor.position.y-5f, rightDoor.position.z);
        }
    }

    private bool ListIsEmpty(RangedEnemy[] enemies)
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                return false;
            }
        }

        return true;
    }
    
    private bool ListIsEmptyMelee(MeleeEnemy[] enemies)
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                return false;
            }
        }

        return true;
    }
}
