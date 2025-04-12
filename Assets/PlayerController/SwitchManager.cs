using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-4)]
public class SwitchManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> characters = new();
    
    [SerializeField]
    private PlayerInput playerInput;
    
    [HideInInspector]
    public int currentIndex = 0;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.OnSwitchPlayer += SwitchPlayer;
    }

    public int GetIndexAndRegister(GameObject character)
    {
        characters.Add(character);
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
        characters[currentIndex].GetComponent<PlayerSwitch>().PowerOn();
    }
    
    private void OnDestroy()
    {
        if (playerInput != null)
            playerInput.OnSwitchPlayer -= SwitchPlayer;
    }
}
