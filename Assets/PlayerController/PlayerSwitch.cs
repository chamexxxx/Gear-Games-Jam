using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[DefaultExecutionOrder(-2)]
public class PlayerSwitch : MonoBehaviour
{
    public int index;

    private PlayerController playerController;
    private List<GameObject> hierarchy;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Start()
    {
        // hierarchy.Add(GetComponentInChildren<CinemachineBrain>().gameObject);
        // hierarchy.Add(GetComponentInChildren<CinemachineCamera>().gameObject);
        var playerSwitch = FindAnyObjectByType<SwitchManager>();
        if (playerSwitch is null)
        {
            Debug.LogError("No SwitchManager registered");
        }
        else
        {
            index = playerSwitch.GetIndexAndRegister(gameObject);
            if (index != 0) PowerOff();
        }
    }
    
    public void PowerOn()
    {
        playerController.enabled = true;
        GetComponent<CharacterController>().enabled = true;
        hierarchy.ForEach(h => h.SetActive(true));
    }

    public void PowerOff()
    {
        playerController.enabled = false;
        GetComponent<CharacterController>().enabled = false;
        hierarchy.ForEach(h => h.SetActive(false));
    }
}
