using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[DefaultExecutionOrder(-4)]
public class SwitchManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> characters = new();
    
    [SerializeField]
    private PlayerInput playerInput;
    
    [HideInInspector]
    public int currentIndex = 0;

    [FormerlySerializedAs("camera")] [ReadOnly]
    public CinemachineCamera cnCamera;

    private void Awake()
    {
        cnCamera = FindAnyObjectByType<CinemachineCamera>();
        playerInput = GetComponent<PlayerInput>();
        playerInput.OnSwitchPlayer += SwitchPlayer;
    }

    public int GetIndexAndRegister(GameObject character)
    {
        characters.Add(character);
        if (characters.Count == 1) cnCamera.Target.TrackingTarget = character.transform;
        return characters.Count - 1;
    }

    public PlayerInput GetPlayerInput()
    {
        return playerInput;
    }
    
    private void SwitchPlayer()
    {
        if (characters.Count < 2) return;
        characters[currentIndex].GetComponent<PlayerSwitch>().PowerOff();
        currentIndex = (currentIndex + 1) % characters.Count;
        cnCamera.Target.TrackingTarget = characters[currentIndex].transform;
        characters[currentIndex].GetComponent<PlayerSwitch>().PowerOn();
    }
    
    private void OnDestroy()
    {
        if (playerInput != null)
            playerInput.OnSwitchPlayer -= SwitchPlayer;
    }
}
