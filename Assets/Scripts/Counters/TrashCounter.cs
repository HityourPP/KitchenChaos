using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCounter : BaseCounter
{
    public static event EventHandler OnAnyObjectDestroyed;
    public new static void ResetStaticData()
    {
        OnAnyObjectDestroyed = null;    //由于是静态资源，在加载场景的时候不会销毁，需要我们手动销毁
    }
    public override void Interact(PlayerController player)
    {
        if (player.HasKitchenObject())//将角色手中的物品摧毁
        {
            player.GetKitchenObject().DestroySelf();
            OnAnyObjectDestroyed?.Invoke(this, EventArgs.Empty);
        }        
    }
}
