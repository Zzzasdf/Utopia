using System;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public partial class RingScrollGenerator : MonoBehaviour
{
    [SerializeField] private RingScrollItem itemTmp;
    [SerializeField] [Header("x轴半径")] private float xAxisRadius = 500f;
    [SerializeField] [Header("y轴半径")] private float yAxisRadius = 300f;
    [SerializeField] [Header("起始角度：x轴正方向")] private float startAngle = 270f;
    [SerializeField] [Header("生成方向")] private Direction generateDir = Direction.Clockwise;
    [SerializeField] [Header("向顺时针旋转一次 (可不用)")] private Button btnClockwiseScrollOnce;
    [SerializeField] [Header("向逆时针旋转一次 (可不用)")] private Button btnAnticlockwiseScrollOnce;
    
    [Header("可播放动画时修改 ====================")]
    [SerializeField] [Header("旋转动画")] private ScrollAni scrollAni = ScrollAni.Linear; 
    [SerializeField] [Header("单次滚动时长：毫秒")] private int onceScrollMillisecond = 500;
    [SerializeField] [Header("连续滚动停滞间隔：毫秒")] private int continuousScrollIntervalMillisecond = 200;
    private bool init;
    private ItemContainer itemContainer;
    private DirectionInfo directionInfo;
    private ScrollAniInfo scrollAniInfo;

    private void Init()
    {
        if (init) return;
        init = !init;
        
        directionInfo = new DirectionInfo(() => generateDir);
        
        itemContainer = new ItemContainer(transform, itemTmp,
            directionInfo, OnItemClick);

        scrollAniInfo = new ScrollAniInfo(() => scrollAni, itemContainer, 
            () => onceScrollMillisecond, () => continuousScrollIntervalMillisecond,
            xAxisRadius, yAxisRadius);
        
        if(btnClockwiseScrollOnce) btnClockwiseScrollOnce.onClick.AddListener(() => itemContainer.SetScroll(Direction.Clockwise, 1));
        if(btnAnticlockwiseScrollOnce) btnAnticlockwiseScrollOnce.onClick.AddListener(() => itemContainer.SetScroll(Direction.Anticlockwise, 1));
    }
    
    public void AddListener(Action<int, Transform> onItemRender, Action<int> onItemSelected)
    {
        AddListener(onItemRender, onItemSelected, null);
    }
    public void AddListener(Action<int, Transform> onItemRender, Action<int> onItemSelected, Action<int> onItemFinalSelected)
    {
        Init();
        itemContainer.AddListener(onItemRender, onItemSelected, onItemFinalSelected);
    }

    public void SetCount(int count)
    {
        SetCount(count, 0);
    }
    public void SetCount(int count, int selectedIndex)
    {
        Init();
        scrollAniInfo.ForceEnd();
        itemContainer.SetCount(count, selectedIndex, 
            startAngle, xAxisRadius, yAxisRadius);
    }
    
    private void OnItemClick(int index)
    {
        if (index == itemContainer.CurSelectedIndex)
        {
            return;
        }
        if (!scrollAniInfo.ScrollStatus().HasFlag(ScrollStatus.Freedom))
        {
            Debug.LogError("非自由态！！");
            return;
        }
        Direction generatorDir = directionInfo.GenerateDir();
        Sequence scrollSequence = directionInfo.ScrollSequence(index, itemContainer.CurSelectedIndex, itemContainer.Count);
        scrollAniInfo.ResetStart(generatorDir, scrollSequence, index);
    }
    
    void Update()
    {
        if (scrollAniInfo == null || scrollAniInfo.ScrollStatus().HasFlag(ScrollStatus.Freedom)) return;
        scrollAniInfo.Update(Time.deltaTime);
    }
    
    private Color gizmoColor = Color.red; // Gizmo颜色
    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        int numPoints = 50;
        for (int i = 0; i < numPoints; i++)
        {
            float theta = 2 * Mathf.PI * i / numPoints;
            float x = transform.position.x + xAxisRadius * Mathf.Cos(theta);
            float y = transform.position.y + yAxisRadius * Mathf.Sin(theta);
            Gizmos.DrawSphere(new Vector3(x, y, transform.position.z), 10f);
        }
    }
}
