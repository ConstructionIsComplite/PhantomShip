using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorClose : MonoBehaviour
{
    [SerializeField]
    private Transform leftDoor;
    [SerializeField]
    private Transform rightDoor;
    
    private bool doorClose = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && doorClose == false)
        {
            doorClose = true;
            leftDoor.position = new Vector3(leftDoor.position.x, leftDoor.position.y+5, leftDoor.position.z);
            rightDoor.position = new Vector3(rightDoor.position.x, rightDoor.position.y+5, rightDoor.position.z);
        }
    }
}
