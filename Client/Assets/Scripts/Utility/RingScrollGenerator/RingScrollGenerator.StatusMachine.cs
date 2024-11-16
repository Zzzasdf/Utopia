using System;
using System.Collections.Generic;
using UnityEngine;

public partial class RingScrollGenerator
{
    /// 滚动状态
    private enum ScrollStatus
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
        private readonly Func<ScrollAni> scrollAniFunc;
        private readonly ItemContainer itemContainer;
        private readonly Func<int> onceScrollMillisecondFunc;
        private readonly Func<int> continuousScrollIntervalMillisecondFunc;
        private readonly float xAxisRadius;
        private readonly float yAxisRadius;

        private IScrollAni scrollAni;
        private List<Vector2> itemsOriginPos;
        private List<Vector2> itemsTargetPos;
        private float onceScrollProgress;
        private float continuousScrollIntervalProgress;
        private ScrollStatus scrollStatus;
        private Direction generatorDir;
        private Sequence scrollSequence;
        private int scrollDir;
        private int scrollTargetIndex;

        public ScrollAniInfo(Func<ScrollAni> scrollAniFunc, ItemContainer itemContainer, 
            Func<int> onceScrollMillisecondFunc, Func<int> continuousScrollIntervalMillisecondFunc,
            float xAxisRadius, float yAxisRadius)
        {
            this.scrollAniFunc = scrollAniFunc;
            this.itemContainer = itemContainer;
            this.onceScrollMillisecondFunc = onceScrollMillisecondFunc;
            this.continuousScrollIntervalMillisecondFunc = continuousScrollIntervalMillisecondFunc;
            this.xAxisRadius = xAxisRadius;
            this.yAxisRadius = yAxisRadius;
            scrollStatus = RingScrollGenerator.ScrollStatus.Freedom;
        }

        public ScrollStatus ScrollStatus()
        {
            return scrollStatus;
        }
        
        public void ResetStart(Direction generatorDir, Sequence scrollSequence, int scrollTargetIndex)
        {
            this.generatorDir = generatorDir;
            this.scrollSequence = scrollSequence;
            scrollDir = (generatorDir, scrollSequence) switch
            {
                (Direction.Clockwise, Sequence.Forward) => 1,
                (Direction.Clockwise, Sequence.Reverse) => -1,
                (Direction.Anticlockwise, Sequence.Forward) => -1,
                (Direction.Anticlockwise, Sequence.Reverse) => 1,
            };
            this.scrollTargetIndex = scrollTargetIndex;
            scrollStatus = RingScrollGenerator.ScrollStatus.Start;
        }

        public void ForceEnd()
        {
            onceScrollProgress = 0;
            continuousScrollIntervalProgress = 0;
            scrollStatus = RingScrollGenerator.ScrollStatus.Freedom;
        }
        
        public void Update(float deltaTime)
        {
            switch (scrollStatus)
            {
                case RingScrollGenerator.ScrollStatus.Start:
                {
                    UpdateScrollStart(deltaTime);
                    break;
                }
                case RingScrollGenerator.ScrollStatus.Rolling:
                {
                    UpdateScrollRolling(deltaTime);
                    break;
                }
                case RingScrollGenerator.ScrollStatus.Interval:
                {
                    UpdateScrollInterval(deltaTime);
                    break;
                }
                case RingScrollGenerator.ScrollStatus.End:
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
            if (!scrollAniMap.TryGetValue(scrollAniFunc.Invoke(), out IScrollAni scrollAni))
            {
                Debug.LogError($"未定义类型 => {scrollAniFunc.Invoke()}");
                return;
            }
            this.scrollAni = scrollAni;
            ItemsPosGenerator();
            scrollStatus = RingScrollGenerator.ScrollStatus.Rolling;
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
            itemContainer.SetCurSelectedIndex(itemContainer.CurSelectedIndex + (int)scrollSequence);
            if (itemContainer.CurSelectedIndex != scrollTargetIndex)
            {
                continuousScrollIntervalProgress = 0;
                scrollStatus = RingScrollGenerator.ScrollStatus.Interval;
                float extraTime = onceScrollSeconds * (onceScrollProgress - 1);
                Update(extraTime);
                return;
            }
            scrollStatus = RingScrollGenerator.ScrollStatus.End;
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
                ItemsPosGenerator();
                scrollStatus = RingScrollGenerator.ScrollStatus.Start;
                Update(extraIntervalSeconds);
            }
        }
        private void UpdateScrollEnd()
        {
            onceScrollProgress = 0;
            continuousScrollIntervalProgress = 0;
            scrollStatus = RingScrollGenerator.ScrollStatus.Freedom;
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
                itemsTargetPos.Add(items[(i - (int)scrollSequence + items.Count) % items.Count].transform.localPosition);
            }
        }
        private void UpdateItemsPos(float onceScrollProgress)
        {
            if (onceScrollProgress > 1) onceScrollProgress = 1;
            IReadOnlyList<RingScrollItem> items = itemContainer.Items();
            for (int i = 0; i < items.Count; i++)
            {
                RingScrollItem item = items[i];
                Vector2 originPos = itemsOriginPos[i];
                Vector2 targetPos = itemsTargetPos[i];
                scrollAni.MoveAni(item.transform, originPos, targetPos, onceScrollProgress, scrollDir, xAxisRadius, yAxisRadius);
            }
        }
    }
}
