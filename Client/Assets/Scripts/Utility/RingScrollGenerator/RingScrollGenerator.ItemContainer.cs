using System;
using System.Collections.Generic;
using UnityEngine;

public partial class RingScrollGenerator
{
    [Serializable]
    private class ItemContainer
    {
        private readonly Transform parentTra;
        private readonly RingScrollItem itemTmp;
        private readonly DirectionInfo directionInfo;
        private readonly Action<int> onItemClick;

        private Action<int, Transform> onItemRender;
        private Action<int> onItemSelected;
        private Action<int> onItemFinalSelected;

        public int Count { get; private set; }
        public int CurSelectedIndex { get; private set; }
        
        private List<RingScrollItem> items = new List<RingScrollItem>();
        private Queue<RingScrollItem> recycle = new Queue<RingScrollItem>();

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
            DirectionInfo directionInfo, Action<int> onItemClick)
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
        
        public void SetCount(int count, int selectedIndex, 
            float startAngle, float xAxisRadius, float yAxisRadius)
        {
            this.Count = count;
            SetItemCount();
            if (count == 0) return;
            CurSelectedIndex = Mathf.Clamp(selectedIndex, 0, count - 1);
            RefreshItemStatus(startAngle, directionInfo.GenerateDir(), xAxisRadius, yAxisRadius);
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
            
            void RefreshItemStatus(float startAngle, Direction generateDir, float xAxisRadius, float yAxisRadius)
            {
                // 布局
                float angularSpacing = (float)360 / count;
                int dir = generateDir == Direction.Clockwise ? -1 : 1;
                for (int i = 0; i < items.Count; i++)
                {
                    SetItem(items[i], i, startAngle + dir * angularSpacing * (i - CurSelectedIndex));
                }
                return;
                
                void SetItem(RingScrollItem item, int index, float angle)
                {
                    float angleRadians = angle * Mathf.Deg2Rad;
                    float x = xAxisRadius * Mathf.Cos(angleRadians);
                    float y = yAxisRadius * Mathf.Sin(angleRadians);
                    
                    item.transform.localPosition = new Vector3Int(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
                    item.Refresh(index);
                }
            }
        }

        /// 旋转到对应索引
        private void SetScrollUntilIndex(int index)
        {
            while (index < 0)
            {
                index += Count;
            }
            index %= Count;
            if (index == CurSelectedIndex) return;
            onItemClick?.Invoke(index);
        }
        
        /// 向 指定方向 旋转 次数
        public void SetScroll(Direction turnDir, int time)
        {
            SetScrollUntilIndex(directionInfo.GenerateDir() == turnDir ? 
                CurSelectedIndex - time : CurSelectedIndex + time);
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
    }
}
