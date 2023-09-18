using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObject : MonoBehaviour
{
    [SerializeField] private KitchenObjects_SO kitchenObjectsSo;

    private IKitchenObjectParent kitchenObjectParent;
    
    public KitchenObjects_SO GetKitchenObjectsSO()
    {
        return this.kitchenObjectsSo;
    }
 
    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent)
    {
        if (this.kitchenObjectParent != null)  //将该物品放置的原本的柜台所包含的物品清空
        {
            this.kitchenObjectParent.ClearKitchenObject();
        }

        if (kitchenObjectParent.HasKitchenObject())
        {   
            Debug.Log("无法放置");
        }
        this.kitchenObjectParent = kitchenObjectParent;    //新的柜台
        
        kitchenObjectParent.SetKitchenObject(this);
        transform.parent = kitchenObjectParent.GetTopCounterPos().transform;    //将该物品放置在新柜台上
        transform.localPosition = Vector3.zero;
    }

    public IKitchenObjectParent GetKitchenObjectParent()
    {
        return this.kitchenObjectParent;
    }

    public void DestroySelf()
    {
        kitchenObjectParent.ClearKitchenObject();//清空父对象
        Destroy(gameObject);
    }

    public static KitchenObject SpawnNewKitchenObject(KitchenObjects_SO kitchenObjectSO,IKitchenObjectParent kitchenObjectParent)
    {//生成一个物品
        Transform kitchenObjectSpawn = Instantiate(kitchenObjectSO.prefabs);
        KitchenObject kitchenObject = kitchenObjectSpawn.GetComponent<KitchenObject>();
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
        return kitchenObject;
    }

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject)
    {
        if (this is PlateKitchenObject)
        {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        }
        plateKitchenObject = null;
        return false;
    }
    
}
