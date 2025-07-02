using System.Collections.Generic;
using UnityEngine;

namespace Generator
{
    public class GeneratorBase<T> : MonoBehaviour, IGenerator
        where T: MonoBehaviour
    {
        [SerializeField] [Header("父节点")] private Transform parentTra;
        [SerializeField] [Header("Item模板")] private T itemTmp;
        
        public List<T> items { get; } = new();
        private Queue<T> recycle = new();

        private void Awake()
        {
            itemTmp.gameObject.SetActive(false);
        }

        public void SetCount(int count)
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
        }

        private T GetItem()
        {
            T result;
            if (recycle.Count == 0)
            {
                T item = Instantiate(itemTmp, parentTra);
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

        private void Release(T item)
        {
            item.gameObject.SetActive(false);
            recycle.Enqueue(item);
        }
    }
}
