using System;
using System.Collections.Generic;
using UnityEngine;

namespace Generator.Ring
{
    public partial class Generator
    {
        /// item 容器
        private class ItemContainer
        {
            private readonly Transform parentTra;
            private readonly Transform itemTmp;

            private Action<int, Transform> onItemRender;

            public int Count { get; private set; }

            private List<Transform> items = new();
            private Queue<Transform> recycle = new();

            public ItemContainer(Transform parentTra, Transform itemTmp)
            {
                this.parentTra = parentTra;
                this.itemTmp = itemTmp;
                this.itemTmp.gameObject.SetActive(false);
            }

            public void AddItemRender(Action<int, Transform> onItemRender)
            {
                this.onItemRender = onItemRender;
            }

            public void SetCount(EShape eShape, int xAxisRadius, int yAxisRadius, int startAngle, EDirection eGenerateDir, int count)
            {
                if (count < 0) return;
                SetItemCount(count);
                SetItemPos(eShape, xAxisRadius, yAxisRadius, startAngle, eGenerateDir, count);
            }

            /// 重新渲染
            public void ReRender()
            {
                for (int i = 0; i < items.Count; i++)
                {
                    onItemRender.Invoke(i, items[i]);
                }
            }
            
            private void SetItemCount(int count)
            {
                Count = count;
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

                Transform GetItem()
                {
                    Transform result;
                    if (recycle.Count == 0)
                    {
                        Transform item = Instantiate(itemTmp, parentTra);
                        item.transform.localScale = Vector3.one;
                        result = item;
                    }
                    else
                    {
                        result = recycle.Dequeue();
                    }
                    result.gameObject.SetActive(true);
                    return result;
                }

                void Release(Transform item)
                {
                    item.gameObject.SetActive(false);
                    recycle.Enqueue(item);
                }
            }

            private void SetItemPos(EShape eShape, int xAxisRadius, int yAxisRadius, int startAngle, EDirection eGenerateDir, int count)
            {
                float angularSpacing = (float)360 / count;
                int dir = eGenerateDir == EDirection.Clockwise ? -1 : 1;
                if (!ShapeMap.TryGetValue(eShape, out IShape shape))
                {
                    Debug.LogError($"未定义类型 => {eShape}");
                    return;
                }
                for (int i = 0; i < items.Count; i++)
                {
                    Transform item = items[i];
                    item.transform.localPosition = shape.GetLocalPos(xAxisRadius, yAxisRadius, startAngle + dir * angularSpacing * i);
                }
            }
        }
    }
}