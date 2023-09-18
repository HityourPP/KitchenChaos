using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCounter : BaseCounter
{
    [SerializeField] private KitchenObjects_SO plateKitchenObjectsSo;
    [SerializeField] private GameObject[] platesVirtual;
    
    private float spawnPlateTime;
    private float spawnPlateTimeMax = 4f;
    private int currentPlateAmount;
    private int maxPlateAmount = 4;

    private void Start()
    {
        spawnPlateTime = 0f;
        currentPlateAmount = 0;
    }

    private void Update()
    {
        spawnPlateTime += Time.deltaTime;
        if (spawnPlateTime >= spawnPlateTimeMax)
        {
            spawnPlateTime = 0f;
            if (GameManager.Instance.IsGamePlaying() && currentPlateAmount < maxPlateAmount)
            {                
                platesVirtual[currentPlateAmount].SetActive(true);
                currentPlateAmount++;
            }
        }
    }
    public override void Interact(PlayerController player)
    {
        if (!player.HasKitchenObject())//当玩家手上没有物体时
        {
            if (currentPlateAmount > 0)
            {                    
                platesVirtual[currentPlateAmount-1].SetActive(false);
                currentPlateAmount--;
                KitchenObject.SpawnNewKitchenObject(plateKitchenObjectsSo, player);                
            }
        }
    }
}
