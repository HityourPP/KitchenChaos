using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour,IKitchenObjectParent
{
    public static PlayerController Instance { get; private set;}//设置单例

    public event EventHandler OnPickedSomething;    //设置玩家拿物品声音
    
    private Animator anim;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private float speed = 8f;
    [SerializeField] private LayerMask counters;
    [SerializeField] private Transform kitchenObjectHoldPoint;
    
    public bool isWalking;
    private Vector3 lastDir;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;
    
    //创建包含类OnSelectedCounter的事件
    public event EventHandler<OnSelectedCounter> OnSelectedCounterChanged;
    public class OnSelectedCounter:EventArgs    //作为事件的参数
    {
        public BaseCounter selectedCounter;
    }
    private void Awake()
    {
        if (Instance != null)//设置单例模式
        {
            Destroy(Instance);
        }
        Instance = this;
    }

    private void Start()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
        inputManager.OnInteractionAction += InteractionAction;//为之前创建的空事件添加函数
        inputManager.OnInteractionAlternateAction += InteractionAlternateAction;
    }
    private void Update()
    {
        MoveController();
        SwitchAnim();
        Interaction();
    }
    
    private void InteractionAlternateAction(object sender, EventArgs e) //用于交互，执行切菜等命令
    {
        if (!GameManager.Instance.IsGamePlaying())//游戏开始时才能进行交互
            return;
        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }  
    }

    private void InteractionAction(object sender, EventArgs e)  //用于交互，更改持有物品
    {
        if (!GameManager.Instance.IsGamePlaying())//游戏开始时才能进行交互
            return;
        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }

    private void Interaction()
    {
        Vector2 dir = inputManager.GetMovementDir();
        Vector3 moveDir = new Vector3(dir.x, 0f, dir.y);
        if (moveDir != Vector3.zero)
        {
            lastDir = moveDir;
        }
        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, lastDir, out RaycastHit hit, interactDistance,counters))
        {
            if (hit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                SetSelectedCounter(baseCounter);
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }
    }
    private void MoveController()
    {
        // float h = Input.GetAxis("Horizontal");
        // float v = Input.GetAxis("Vertical");
        // Vector2 dir = new Vector2(h, v).normalized;
        // Vector3 moveDir = new Vector3(dir.x, 0f, dir.y);
        Vector2 dir = inputManager.GetMovementDir();
        Vector3 moveDir = new Vector3(dir.x, 0f, dir.y);    
        //判断是否在移动
        isWalking = dir != Vector2.zero;
        //设置插值实现其平滑的旋转
        float switchSpeed = 15f;
        if(moveDir != Vector3.zero)
            transform.forward = Vector3.Slerp(transform.forward, moveDir, switchSpeed * Time.deltaTime);
        //设置碰撞检测
        float playerHeight = 2f;
        float playerRadius = 0.65f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight,
            playerRadius, moveDir, speed * Time.deltaTime);
        if (canMove)
        {
            transform.position += speed * moveDir * Time.deltaTime;
        }
        else if (!canMove)
        {//无法在当前方向移动
            //拆分方向，判断在x轴是否能移动
            moveDir = new Vector3(dir.x, 0f, 0f);
            //moveDir.x != 0是为了避免点击两个方向时选中了物体，但角色没有转向，结合下面的moveDir.z != 0一起使用
            canMove = moveDir.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight,
                playerRadius, moveDir, speed * Time.deltaTime);
            if(canMove)transform.position += speed * moveDir * Time.deltaTime;
            else
            {//判断在z轴是否能够移动
                moveDir = new Vector3(0f, 0f, dir.y);
                canMove = moveDir.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight,
                    playerRadius, moveDir, speed * Time.deltaTime);
                if(canMove)transform.position += speed * moveDir * Time.deltaTime;
            }
        }
    }

    private void SwitchAnim()
    {
        anim.SetBool("isWalking", isWalking);
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)//更改事件中的类中的参数，并执行该事件
    {
        this.selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this,new OnSelectedCounter
        {
            selectedCounter = this.selectedCounter
        });//初始化该事件的参数，并执行该事件，并且当该事件包含函数不为空时
    }
    //实现接口函数
    public Transform GetTopCounterPos()
    {
        return this.kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
        if (kitchenObject != null)  //若不为空
        {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);            
        }
    }

    public KitchenObject GetKitchenObject()
    {
        return this.kitchenObject;
    }

    public void ClearKitchenObject() 
    {
        this.kitchenObject = null;
    }

    public bool HasKitchenObject()  //判断是否有物体
    {
        return this.kitchenObject != null;
    }
}
