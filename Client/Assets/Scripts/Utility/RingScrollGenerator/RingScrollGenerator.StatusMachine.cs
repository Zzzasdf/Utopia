using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public partial class RingScrollGenerator
{
    /// 滚动状态
    private enum EScrollStatus
    {
        /// 自由状态
        Freedom = 1 << 0,
        /// 锁定状态
        Locked = 1 << 1,
        /// 开始
        Start = 1 << 2 | Locked,
        /// 滚动中
        Rolling = 1 << 3 | Locked,
        /// 间隔
        Interval = 1 << 4 | Locked,
        /// 结束
        End = 1 << 5 | Locked,
    }

    private class ScrollAniInfo
    {
        private readonly ItemContainer itemContainer;
        private readonly Func<EScrollAni> eScrollAniFunc;
        private readonly Func<int> onceScrollMillisecondFunc;
        private readonly Func<int> continuousScrollIntervalMillisecondFunc;

        private IShape shape;
        private int xAxisRadius;
        private int yAxisRadius;

        private IScrollAni scrollAni;
        private List<Vector2> itemsOriginPos;
        private List<Vector2> itemsTargetPos;
        private float onceScrollProgress;
        private float continuousScrollIntervalProgress;
        private EScrollStatus eScrollStatus;
        private ESequence eScrollSequence;
        private int scrollDir;
        private int scrollTargetIndex;

        public ScrollAniInfo(ItemContainer itemContainer, 
            Func<EScrollAni> eScrollAniFunc, Func<int> onceScrollMillisecondFunc, Func<int> continuousScrollIntervalMillisecondFunc)
        {
            this.itemContainer = itemContainer;
            this.eScrollAniFunc = eScrollAniFunc;
            this.onceScrollMillisecondFunc = onceScrollMillisecondFunc;
            this.continuousScrollIntervalMillisecondFunc = continuousScrollIntervalMillisecondFunc;
        }

        public void Init(EShape eShape, int xAxisRadius, int yAxisRadius)
        {
            if (!shapeMap.TryGetValue(eShape, out IShape shape))
            {
                Debug.LogError($"未定义类型 => {eShape}");
                return;
            }
            this.shape = shape;
            this.xAxisRadius = xAxisRadius;
            this.yAxisRadius = yAxisRadius;
            eScrollStatus = RingScrollGenerator.EScrollStatus.Freedom;
        }

        public EScrollStatus EScrollStatus()
        {
            return eScrollStatus;
        }
        
        public void ResetStart(EDirection eGeneratorDir, ESequence eScrollSequence, int scrollTargetIndex)
        {
            this.eScrollSequence = eScrollSequence;
            scrollDir = (generatorDir: eGeneratorDir, scrollSequence: eScrollSequence) switch
            {
                (EDirection.Clockwise, ESequence.Forward) => 1,
                (EDirection.Clockwise, ESequence.Reverse) => -1,
                (EDirection.Anticlockwise, ESequence.Forward) => -1,
                (EDirection.Anticlockwise, ESequence.Reverse) => 1,
            };
            this.scrollTargetIndex = scrollTargetIndex;
            eScrollStatus = RingScrollGenerator.EScrollStatus.Start;
        }

        public void ForceEnd()
        {
            onceScrollProgress = 0;
            continuousScrollIntervalProgress = 0;
            if (itemsOriginPos != null)
            {
                // 还原
                IReadOnlyList<RingScrollItem> items = itemContainer.Items();
                for (int i = 0; i < itemsOriginPos.Count; i++)
                {
                    RingScrollItem item = items[i];
                    Vector2 originPos = itemsOriginPos[i];
                    item.transform.localPosition = originPos;
                }
                itemsOriginPos.Clear();
            }
            itemsTargetPos?.Clear();
            eScrollStatus = RingScrollGenerator.EScrollStatus.Freedom;
        }
        
        public void Update(float deltaTime)
        {
            switch (eScrollStatus)
            {
                case RingScrollGenerator.EScrollStatus.Start:
                {
                    UpdateScrollStart(deltaTime);
                    break;
                }
                case RingScrollGenerator.EScrollStatus.Rolling:
                {
                    UpdateScrollRolling(deltaTime);
                    break;
                }
                case RingScrollGenerator.EScrollStatus.Interval:
                {
                    UpdateScrollInterval(deltaTime);
                    break;
                }
                case RingScrollGenerator.EScrollStatus.End:
                {
                    UpdateScrollEnd();
                    break;
                }
            }
        }

        private void UpdateScrollStart(float deltaTime)
        {
            onceScrollProgress = 0;
            continuousScrollIntervalProgress = 0;
            if (!scrollAniMap.TryGetValue(eScrollAniFunc.Invoke(), out IScrollAni scrollAni))
            {
                Debug.LogError($"未定义类型 => {eScrollAniFunc.Invoke()}");
                return;
            }
            this.scrollAni = scrollAni;
            ItemsPosGenerator();
            eScrollStatus = RingScrollGenerator.EScrollStatus.Rolling;
            Update(deltaTime);
        }

        private void UpdateScrollRolling(float deltaTime)
        {
            float onceScrollSeconds = onceScrollMillisecondFunc.Invoke() / 1000f;
            float addScrollProgress = 1;
            if (onceScrollSeconds > 0)
            {
                addScrollProgress = deltaTime / onceScrollSeconds;
            }
            onceScrollProgress += addScrollProgress;
            UpdateItemsPos(onceScrollProgress);
            if (onceScrollProgress < 1)
            {
                return;
            }
            itemsOriginPos.Clear();
            itemsTargetPos.Clear();
            itemContainer.SetCurSelectedIndex(itemContainer.CurSelectedIndex + (int)eScrollSequence);
            if (itemContainer.CurSelectedIndex != scrollTargetIndex)
            {
                continuousScrollIntervalProgress = 0;
                eScrollStatus = RingScrollGenerator.EScrollStatus.Interval;
                float extraTime = onceScrollSeconds * (onceScrollProgress - 1);
                Update(extraTime);
                return;
            }
            eScrollStatus = RingScrollGenerator.EScrollStatus.End;
            Update(0);
        }

        private void UpdateScrollInterval(float deltaTime)
        {
            float continuousScrollIntervalSeconds = continuousScrollIntervalMillisecondFunc.Invoke() / 1000f;
            if (continuousScrollIntervalSeconds <= 0)
            {
                ExtraIntervalSeconds(deltaTime);
                return;
            }
            float addContinuousScrollIntervalProgress = deltaTime / continuousScrollIntervalSeconds;
            continuousScrollIntervalProgress += addContinuousScrollIntervalProgress;
            if (continuousScrollIntervalProgress < 1)
            {
                return;
            }
            ExtraIntervalSeconds(continuousScrollIntervalSeconds * (continuousScrollIntervalProgress - 1));
            return;
            
            void ExtraIntervalSeconds(float extraIntervalSeconds)
            {
                onceScrollProgress = 0;
                eScrollStatus = RingScrollGenerator.EScrollStatus.Start;
                Update(extraIntervalSeconds);
            }
        }
        private void UpdateScrollEnd()
        {
            onceScrollProgress = 0;
            continuousScrollIntervalProgress = 0;
            eScrollStatus = RingScrollGenerator.EScrollStatus.Freedom;
            itemContainer.OnFinalSelectedCallback();
        }

        private void ItemsPosGenerator()
        {
            itemsOriginPos ??= new List<Vector2>();
            itemsOriginPos.Clear();
            itemsTargetPos ??= new List<Vector2>();
            itemsTargetPos.Clear();
            IReadOnlyList<RingScrollItem> items = itemContainer.Items();
            for (int i = 0; i < items.Count; i++)
            {
                itemsOriginPos.Add(items[i].transform.localPosition);
                itemsTargetPos.Add(items[(i - (int)eScrollSequence + items.Count) % items.Count].transform.localPosition);
            }
        }
        private void UpdateItemsPos(float onceScrollProgress)
        {
            IReadOnlyList<RingScrollItem> items = itemContainer.Items();
            if (onceScrollProgress < 1)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    RingScrollItem item = items[i];
                    Vector2 originPos = itemsOriginPos[i];
                    Vector2 targetPos = itemsTargetPos[i];
                    scrollAni.MoveAni(shape, item.transform, originPos, targetPos, onceScrollProgress, scrollDir, xAxisRadius, yAxisRadius);
                }
            }
            else // 去除误差
            {
                for (int i = 0; i < items.Count; i++)
                {
                    RingScrollItem item = items[i];
                    Vector2 targetPos = itemsTargetPos[i];
                    item.transform.localPosition = targetPos;
                }
            }
        }
    }
}