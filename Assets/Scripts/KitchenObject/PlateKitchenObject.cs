using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{
    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
    public class OnIngredientAddedEventArgs:EventArgs
    {
        public KitchenObjects_SO kitchenObjectSo;
    }

    [SerializeField] private List<KitchenObjects_SO> validKitchenObjects; //可以放置在盘子上的物品
    private List<KitchenObjects_SO> plateKitchenObjects;    //保存放置在盘子中的物品

    private void Awake()
    {
        plateKitchenObjects = new List<KitchenObjects_SO>();
    }

    public bool TryAddKitchenObject(KitchenObjects_SO kitchenObjectSo)//获取柜台上的物品，添加到盘子中
    {
        if (CheckIfCanPlace(kitchenObjectSo))
        {
            if (plateKitchenObjects.Contains(kitchenObjectSo))//当列表中已经有时，不再进行添加
            {
                return false;
            }
            plateKitchenObjects.Add(kitchenObjectSo);
            OnIngredientAdded?.Invoke(this,new OnIngredientAddedEventArgs()
            {
                kitchenObjectSo = kitchenObjectSo
            });
            return true;
        }
        return false;
    }

    private bool CheckIfCanPlace(KitchenObjects_SO kitchenObjectSo) //判断是否可以放置到盘子中
    {
        if (validKitchenObjects.Contains(kitchenObjectSo))
        {
            return true;
        }
        return false;
    }

    public List<KitchenObjects_SO> GetKitchenObjectsList()
    {
        return plateKitchenObjects;
    }
}
