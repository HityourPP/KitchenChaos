using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Object = System.Object;

public class StoveCounter : BaseCounter
{
    [SerializeField] private FryingRecipe_SO[] fryingRecipeSo;
    [SerializeField] private GameObject fryingTimeUI;   //UI对象
    [SerializeField] private Image fryingTimeBar;       //UI条
    [SerializeField] private GameObject stoveOnVisual;  //烹饪的特效
    [SerializeField] private GameObject sizzlingParticles;
    [SerializeField] private KitchenObjects_SO cookedMeat;
    [SerializeField] private KitchenObjects_SO rawMeat;

    private AudioSource audioSource;
    private float startTime;
    private bool warning;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Hide();
    }
    
    private void Update()
    {
        if (HasKitchenObject())
        {
            if (CheckIfCanFry(kitchenObject.GetKitchenObjectsSO()))
            {
                FryingRecipe_SO fryingRecipe = GetFryingRecipeSo(kitchenObject.GetKitchenObjectsSO());
                fryingTimeBar.fillAmount = (Time.time - startTime) / fryingRecipe.fryingTimerMax;
                if ((Time.time - startTime) >= fryingRecipe.fryingTimerMax )
                {
                    kitchenObject.DestroySelf();
                    KitchenObject.SpawnNewKitchenObject(fryingRecipe.output, this);
                    startTime = Time.time;
                }                
            }
            else
            {
                Hide();
            }
        }
    }

    public override void Interact(PlayerController player)
    {
        if (!player.HasKitchenObject())//当玩家手上没有物体时
        {
            if (HasKitchenObject()) //若柜台上有物体
            {
                Hide();
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
        else if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject) && HasKitchenObject())    //当玩家手中携带盘子时
        {
            if(plateKitchenObject.TryAddKitchenObject(GetKitchenObject().GetKitchenObjectsSO())) //将柜台中的物品添加到盘子中
            {//若添加成功，则销毁物品
                Hide();
                GetKitchenObject().DestroySelf();//将柜台上的物体销毁
            } 
        } 
        //当玩家手中有物体，并且能烹饪时才能放在柜台上，这里的功能可选用
        else if(player.HasKitchenObject() && CheckIfCanFry(player.GetKitchenObject().GetKitchenObjectsSO()))
        {
            if (!HasKitchenObject())//若玩家手中有物体，但柜台上没有
            {
                player.GetKitchenObject().SetKitchenObjectParent(this);
                Show();
                startTime = Time.time;
            }
        }
    }
    private bool CheckIfCanFry(KitchenObjects_SO kitchenObjectsSo)
    {
        FryingRecipe_SO fryingRecipe = GetFryingRecipeSo(kitchenObjectsSo);
        return fryingRecipe != null;
    }
    private KitchenObjects_SO GetKitchenOnjectOutput(KitchenObjects_SO kitchenObjectsSo)
    {
        FryingRecipe_SO fryingRecipe = GetFryingRecipeSo(kitchenObjectsSo);
        if (fryingRecipe != null)
        {
            return fryingRecipe.output;
        }
        return null;
    }
    private FryingRecipe_SO GetFryingRecipeSo(KitchenObjects_SO kitchenObjectsSo)
    {
        foreach (var fryRecipe in fryingRecipeSo)
        {
            if (fryRecipe.input == kitchenObjectsSo)
            {
                return fryRecipe;
            }
        }
        return null;
    }

    private void Hide()
    {
        fryingTimeUI.SetActive(false);
        stoveOnVisual.SetActive(false);
        sizzlingParticles.SetActive(false);
        audioSource.Pause();
    }

    private void Show()
    {
        fryingTimeUI.SetActive(true);
        stoveOnVisual.SetActive(true);
        sizzlingParticles.SetActive(true);
        audioSource.Play();
    }

}
