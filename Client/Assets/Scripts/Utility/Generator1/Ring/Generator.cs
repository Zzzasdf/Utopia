﻿using System;
using UnityEngine;

namespace Generator.Ring111
{
    [DisallowMultipleComponent]
    public partial class Generator : MonoBehaviour
    {
        [Header("创建参数 ====================")] 
        [SerializeField] [Header("Item模板")] private Item itemTmp;
        [SerializeField] [Header("形状")] private EShape eShape = EShape.Circle;
        [SerializeField] [Header("x轴半径")] private int xAxisRadius = 500;
        [SerializeField] [Header("y轴半径")] private int yAxisRadius = 300;
        [SerializeField] [Header("起始角度：x轴正方向")] private int startAngle = 270;
        [SerializeField] [Header("生成方向")] private EDirection eGenerateDir = EDirection.Clockwise;

        [Header("动画参数：可播放时修改 ====================")] 
        [SerializeField] [Header("旋转动画")] private EScrollAni eScrollAni = EScrollAni.Linear;
        [SerializeField] [Header("单次滚动时长：毫秒")] private int onceScrollMillisecond = 500;
        [SerializeField] [Header("连续滚动停滞间隔：毫秒")] private int continuousScrollIntervalMillisecond = 200;
        
        [Header("零件：可选 ====================")] 
        [SerializeField] private OptionalParts optionalParts;

        private bool init;
        private ItemContainer itemContainer;
        private DirectionInfo directionInfo;
        private ScrollAniInfo scrollAniInfo;

        public int Count() => itemContainer?.Count ?? 0;
        public int CurSelectedIndex() => itemContainer?.CurSelectedIndex ?? 0;
        public void SetScrollUntilIndex(int index) => itemContainer?.SetScrollUntilIndex(index, null);

        public void SetIndex(int index)
        {
            if (itemContainer == null) return;
            scrollAniInfo?.ForceEnd();
            int onceScrollMillisecond = this.onceScrollMillisecond;
            int continuousScrollIntervalMillisecond = this.continuousScrollIntervalMillisecond;
            this.onceScrollMillisecond = 0;
            this.continuousScrollIntervalMillisecond = 0;
            itemContainer.SetScrollUntilIndex(index, null);
            Update();
            this.onceScrollMillisecond = onceScrollMillisecond;
            this.continuousScrollIntervalMillisecond = continuousScrollIntervalMillisecond;
        }

        private void Init()
        {
            if (init) return;
            init = !init;

            directionInfo = new DirectionInfo();

            itemContainer = new ItemContainer(transform, itemTmp,
                directionInfo, OnItemClick);

            scrollAniInfo = new ScrollAniInfo(itemContainer,
                () => eScrollAni, () => onceScrollMillisecond, () => continuousScrollIntervalMillisecond);

            optionalParts.Init(itemContainer);
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
            directionInfo.Init(eGenerateDir);
            scrollAniInfo.Init(eShape, xAxisRadius, yAxisRadius);
            itemContainer.SetCount(eShape, xAxisRadius, yAxisRadius, startAngle, eGenerateDir,
                count, selectedIndex);
        }

        public void ReRender()
        {
            itemContainer.ReRender();
        }

        private void OnItemClick(int index, EDirection? eDirection)
        {
            if (index == itemContainer.CurSelectedIndex)
            {
                return;
            }

            if (!scrollAniInfo.EScrollStatus().HasFlag(EScrollStatus.Freedom))
            {
                Debug.LogWarning("非自由态！！");
                return;
            }

            EDirection eGeneratorDir = directionInfo.EGenerateDir();
            ESequence eScrollESequence = directionInfo.EScrollSequence(index, itemContainer.CurSelectedIndex, itemContainer.Count, eDirection);
            scrollAniInfo.ResetStart(eGeneratorDir, eScrollESequence, index);
        }

        void Update()
        {
            if (scrollAniInfo == null || scrollAniInfo.EScrollStatus().HasFlag(EScrollStatus.Freedom)) return;
            scrollAniInfo.Update(Time.deltaTime);
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            // 布局
            int numPoints = 50;
            float angularSpacing = (float)360 / numPoints;
            if (!shapeMap.TryGetValue(eShape, out IShape shape))
            {
                Debug.LogError($"未定义类型 => {eShape}");
                return;
            }

            // 内置矩形
            {
                if (eShape != EShape.Circle)
                {
                    Gizmos.color = Color.green;
                    for (int i = 0; i < numPoints; i++)
                    {
                        DrawSphere(shape, xAxisRadius, yAxisRadius, angularSpacing * i);
                    }
                }
            }

            // 外框圆
            {
                Gizmos.color = eShape == EShape.Circle ? Color.green : Color.red;
                IShape ovalShape = shapeMap[EShape.Circle];
                for (int i = 0; i < numPoints; i++)
                {
                    DrawSphere(ovalShape, xAxisRadius, yAxisRadius, angularSpacing * i);
                }
            }
            return;

            void DrawSphere(IShape shape, int xAxisRadius, int yAxisRadius, float angle)
            {
                Vector3 localPoint = shape.GetLocalPos(xAxisRadius, yAxisRadius, angle);
                Gizmos.DrawSphere(transform.position + localPoint, 10);
            }
        }
#endif
    }
}