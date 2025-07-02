using System.Collections.Generic;
using UnityEngine;

namespace Generator
{
    public class CommonGenerator : MonoBehaviour, IGenerator
    {
        [SerializeField] [Header("父节点")] private Transform parentTra;
        [SerializeField] [Header("Item模板")] private GameObject itemTmp;
        
        public List<GameObject> items { get; } = new();
        private Queue<GameObject> recycle = new();

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

        private GameObject GetItem()
        {
            GameObject result;
            if (recycle.Count == 0)
            {
                GameObject item = Instantiate(itemTmp, parentTra);
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

        private void Release(GameObject item)
        {
            item.gameObject.SetActive(false);
            recycle.Enqueue(item);
        }
    }
}
