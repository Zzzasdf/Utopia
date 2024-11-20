using System;
using System.Collections.Generic;
using UnityEngine;

public partial class RingScrollGenerator
{
    /// item 容器
    private class ItemContainer
    {
        private readonly Transform parentTra;
        private readonly RingScrollItem itemTmp;
        private readonly DirectionInfo directionInfo;
        private readonly Action<int, EDirection?> onItemClick;

        private Action<int, Transform> onItemRender;
        private Action<int> onItemSelected;
        private Action<int> onItemFinalSelected;
        
        public int Count { get; private set; }
        public int CurSelectedIndex { get; private set; }
        
        private List<RingScrollItem> items = new();
        private Queue<RingScrollItem> recycle = new();

        public IReadOnlyList<RingScrollItem> Items() => items;

        public void SetCurSelectedIndex(int index)
        {
            while (index < 0)
            {
                index += Count;
            }
            index %= Count;
            CurSelectedIndex = index;
            SetRingSelectedEffect();
            onItemSelected?.Invoke(index);
        }

        public void OnFinalSelectedCallback()
        {
            onItemFinalSelected?.Invoke(CurSelectedIndex);
        }
        
        public ItemContainer(Transform parentTra, RingScrollItem itemTmp, 
            DirectionInfo directionInfo, Action<int, EDirection?> onItemClick)
        {
            this.parentTra = parentTra;
            this.itemTmp = itemTmp; 
            this.itemTmp.gameObject.SetActive(false);
            this.directionInfo = directionInfo;
            this.onItemClick = onItemClick;
        }
        
        public void AddListener(Action<int, Transform> onItemRender, Action<int> onItemSelected, Action<int> onItemFinalSelected)
        {
            this.onItemRender = onItemRender;
            this.onItemSelected = onItemSelected;
            this.onItemFinalSelected = onItemFinalSelected;
        }
        
        public void SetCount(EShape eShape, int xAxisRadius, int yAxisRadius, int startAngle, EDirection eGenerateDir,
            int count, int selectedIndex)
        {
            this.Count = count;
            SetItemCount();
            if (count < 0) return;
            CurSelectedIndex = Mathf.Clamp(selectedIndex, 0, count - 1);
            RefreshItemStatus(eShape, xAxisRadius, yAxisRadius, startAngle, eGenerateDir);
            SetRingSelectedEffect();
            onItemSelected?.Invoke(CurSelectedIndex);
            onItemFinalSelected?.Invoke(CurSelectedIndex);
            return;
            
            void SetItemCount()
            {
                // 不足生成
                for (int i = items.Count; i < count; i++)
                {
                    items.Add(GetItem());
                }
                // 超出回收
                for (int i = items.Count - 1; i >= count; i--)
                {
                    Release(items[i]);
                    items.RemoveAt(i);
                }
                return;
            
                RingScrollItem GetItem()
                {
                    RingScrollItem result;
                    if (recycle.Count == 0)
                    {
                        RingScrollItem item = Instantiate(itemTmp, parentTra);
                        item.transform.localScale = Vector3.one;
                        item.Init(onItemRender, onItemClick);
                        result = item;
                    }
                    else
                    {
                        result = recycle.Dequeue();
                    }
                    result.gameObject.SetActive(true);
                    return result;
                }
       
                void Release(RingScrollItem item)
                {
                    item.gameObject.SetActive(false);
                    recycle.Enqueue(item);
                }
            }
            
            void RefreshItemStatus(EShape eShape, int xAxisRadius, int yAxisRadius, int startAngle, EDirection eGenerateDir)
            {
                // 布局
                float angularSpacing = (float)360 / count;
                int dir = eGenerateDir == EDirection.Clockwise ? -1 : 1;
                if (!shapeMap.TryGetValue(eShape, out IShape shape))
                {
                    Debug.LogError($"未定义类型 => {eShape}");
                    return;
                }
                for (int i = 0; i < items.Count; i++)
                {
                    SetItem(items[i], i, shape, xAxisRadius, yAxisRadius, startAngle + dir * angularSpacing * (i - CurSelectedIndex));
                }
                return;
                
                void SetItem(RingScrollItem item, int index, IShape shape, int xAxisRadius, int yAxisRadius, float angle)
                {
                    item.transform.localPosition = shape.GetLocalPos(xAxisRadius, yAxisRadius, angle);
                    item.Refresh(index);
                }
            }
        }

        /// 旋转到对应索引
        public void SetScrollUntilIndex(int index, EDirection? eScrollDir)
        {
            if (Count == 0) return;
            while (index < 0)
            {
                index += Count;
            }
            index %= Count;
            if (index == CurSelectedIndex) return;
            onItemClick?.Invoke(index, eScrollDir);
        }
        
        /// 向 指定方向 旋转 次数
        public void SetScroll(EDirection eScrollDir, int time)
        {
            SetScrollUntilIndex(directionInfo.EGenerateDir() == eScrollDir ? 
                CurSelectedIndex - time : CurSelectedIndex + time, eScrollDir);
            // SetTurnUntilIndex((generateDirInfo.Direction, turnDir) switch
            // {
            //     (Direction.Clockwise, Direction.Clockwise) => curSelectedIndex - time,
            //     (Direction.Clockwise, Direction.Anticlockwise) => curSelectedIndex + time,
            //     (Direction.Anticlockwise, Direction.Clockwise) => curSelectedIndex + time,
            //     (Direction.Anticlockwise, Direction.Anticlockwise) => curSelectedIndex - time
            // });
        }
        
        private void SetRingSelectedEffect()
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].SetSelectedEffect(i == CurSelectedIndex);
            }
        }

        /// 重新渲染
        public void ReRender()
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].Refresh(i);
            }
        }
    }
}