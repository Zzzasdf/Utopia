using System;
using System.Collections.Generic;
using Generator.Ring.Helper;
using UnityEngine;
using Component = Generator.Ring.Helper.Component;

namespace Generator.Ring
{
    [DisallowMultipleComponent]
    public partial class Generator : MonoBehaviour
    {
        [SerializeField] [Header("Item模板")] private Transform itemTmp;
        [SerializeField] [Header("形状")] private EShape eShape = EShape.Circle;
        [SerializeField] [Header("x轴半径")] private int xAxisRadius = 500;
        [SerializeField] [Header("y轴半径")] private int yAxisRadius = 300;
        [SerializeField] [Header("起始角度：x轴正方向")] private int startAngle = 270;
        [SerializeField] [Header("生成方向")] private EDirection eGenerateDir = EDirection.Clockwise;

        private ItemContainer _itemContainer;
        private ItemContainer itemContainer => _itemContainer ??= new ItemContainer(transform, itemTmp);
        
        public void AddItemRender(Action<int, Transform> onItemRender)
        {
            itemContainer.AddItemRender(onItemRender);
        }

        public new T GetComponent<T>()
        {
            return GetComponent<T>();
        }

        public void SetCount(int count)
        {
            itemContainer.SetCount(eShape, xAxisRadius, yAxisRadius, startAngle, eGenerateDir, count);
        }

        public void ReRender()
        {
            itemContainer.ReRender();
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            // 布局
            int numPoints = 50;
            float angularSpacing = (float)360 / numPoints;
            if (!ShapeMap.TryGetValue(eShape, out IShape shape))
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
                IShape ovalShape = ShapeMap[EShape.Circle];
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