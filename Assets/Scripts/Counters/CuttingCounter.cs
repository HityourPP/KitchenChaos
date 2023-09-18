using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CuttingCounter : BaseCounter
{
    [SerializeField] private CuttingRecipe_SO[] cuttingRecipeArray;
    // [SerializeField] private Image cuttingProgressBar;  //直接实现UI滑动条
    private float cuttingProgress;
    //使用事件实现UI滑动条
    public event EventHandler<OnProgressChangedArgs> OnProgressChanged;
    public class OnProgressChangedArgs : EventArgs
    {
        public float progressNormalize;
    }
    //使用事件实现切割动画
    public event EventHandler OnCuttingVirtual;
    public static EventHandler OnAnyCut;

    public new static void ResetStaticData()
    {
        OnAnyCut = null;    //由于是静态资源，在加载场景的时候不会销毁，需要我们手动销毁
    }
    public override void Interact(PlayerController player)
    {
        if (!player.HasKitchenObject())//当玩家手上没有物体时
        {
            if (HasKitchenObject()) //若柜台上有物体
            {
                GetKitchenObject().SetKitchenObjectParent(player);
                cuttingProgress = 0;
                OnProgressChanged?.Invoke(this,new OnProgressChangedArgs()
                {
                    progressNormalize = 0
                });
            }
        }            
        else if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)&& HasKitchenObject())    //当玩家手中携带盘子时
        {
            if(plateKitchenObject.TryAddKitchenObject(GetKitchenObject().GetKitchenObjectsSO())) //将柜台中的物品添加到盘子中
            {//若添加成功，则销毁物品
                GetKitchenObject().DestroySelf();//将柜台上的物体销毁
            } 
        } 
        //当玩家手中有物体，并且能切割时才能放在柜台上，这里的功能可选用
        else if(player.HasKitchenObject() && CheckIfCanCut(player.GetKitchenObject().GetKitchenObjectsSO()))
        {
            if (!HasKitchenObject())//若玩家手中有物体，但柜台上没有
            {
                player.GetKitchenObject().SetKitchenObjectParent(this);
                cuttingProgress = 0;
                OnProgressChanged?.Invoke(this,new OnProgressChangedArgs()
                {
                    progressNormalize = cuttingProgress / GetCuttingRecipeSo(kitchenObject.GetKitchenObjectsSO()).cuttingProgressMax
                });
            }
        }
    }

    public override void InteractAlternate(PlayerController player)
    {//在切菜前需要在柜台上有物品,并且玩家手上没有物品
        if (!player.GetKitchenObject()&&HasKitchenObject() && GetOutputKitchenObjectsSo(kitchenObject.GetKitchenObjectsSO())) 
        {
            cuttingProgress++;
            CuttingRecipe_SO cuttingRecipeSo = GetCuttingRecipeSo(kitchenObject.GetKitchenObjectsSO());
            //添加切割动画
            OnCuttingVirtual?.Invoke(this, EventArgs.Empty);
            //添加切割声音
            OnAnyCut?.Invoke(this, EventArgs.Empty);
            // cuttingProgressBar.fillAmount = cuttingProgress / cuttingRecipeSo.cuttingProgressMax;//直接调用修改UI条
            OnProgressChanged?.Invoke(this,new OnProgressChangedArgs()//使用委托事件方式修改UI条
            {
                progressNormalize = cuttingProgress / GetCuttingRecipeSo(kitchenObject.GetKitchenObjectsSO()).cuttingProgressMax
            });
            if (cuttingProgress >= cuttingRecipeSo.cuttingProgressMax)
            {
                KitchenObjects_SO outputKitchenObjectsSo = GetOutputKitchenObjectsSo(kitchenObject.GetKitchenObjectsSO());
                //销毁之前的物品，切为片
                kitchenObject.DestroySelf();
                //生成切片
                // Transform objectSpawn = Instantiate(cuttingKitchenObjectSO.prefabs);
                // objectSpawn.GetComponent<KitchenObject>().SetKitchenObjectParent(this); 
                KitchenObject.SpawnNewKitchenObject(outputKitchenObjectsSo, this);                
            }
        }
    }

    private bool CheckIfCanCut(KitchenObjects_SO kitchenObjectSo)
    {
        var cuttingRecipeSo = GetCuttingRecipeSo(kitchenObjectSo);
        return cuttingRecipeSo != null;
    }
    
    private KitchenObjects_SO GetOutputKitchenObjectsSo(KitchenObjects_SO kitchenObjectSo)
    {
        CuttingRecipe_SO cuttingRecipeSo = GetCuttingRecipeSo(kitchenObjectSo);
        if (cuttingRecipeSo != null)
        {
            return cuttingRecipeSo.output;
        }
        return null;//与下面有同样的功能，增强代码复用性
        // foreach (var cuttingRecipe in cuttingRecipeArray)
        // {
        //     if (cuttingRecipe.input == kitchenObjectSo)
        //     {
        //         return cuttingRecipe.output;
        //     }
        // }
        // return null;
    }

    private CuttingRecipe_SO GetCuttingRecipeSo(KitchenObjects_SO kitchenObjectSo)  //获取对应菜谱的SO数据文件
    {
        foreach (var cuttingRecipe in cuttingRecipeArray)
        {
            if (cuttingRecipe.input == kitchenObjectSo)
            {
                return cuttingRecipe;
            }
        }
        return null;
    }
}
