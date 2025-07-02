// using System.Collections.Generic;
// using Selector.Linear;
// using UnityEngine;
// using UnityEngine.UI;
// using Generator;
// using UnityEngine.Serialization;
//
// public class TestCommonGenerator : MonoBehaviour
// {
//     [SerializeField] private CommonGenerator centerGenerator;
//     [FormerlySerializedAs("centerSelector")] [SerializeField] private CenterSelectorMono centerSelectorMono;
//     private int displayCenterMaxCount = 3;
//
//     [SerializeField] private CommonGenerator peakShapeGenerator;
//     [FormerlySerializedAs("peakShapeSelector")] [SerializeField] private PeakShapeSelectorMono peakShapeSelectorMono;
//     private int displayPeakShapeMaxCount = 6;
//     private List<int> peakShapeDatas = new List<int>();
//
//     private void Start()
//     {
//         centerSelectorMono.Subscribe(OnTabCenterRender, OnTabCenterSelected);//, OnItemDisableCondition);
//         peakShapeSelectorMono.Subscribe(OnContentRender, OnContentSelected);//, OnItemDisableCondition); 
//         
//         RefreshCenter();
//     }
//
//     private void RefreshCenter()
//     {
//         // 自适应 方式
//         int dataCount = 30;
//         int displayCount = dataCount < displayCenterMaxCount ? dataCount : displayCenterMaxCount;
//         if (centerGenerator.items.Count != displayCount)
//         {
//             centerGenerator.SetCount(displayCount);
//             centerSelectorMono.AddRangeItems(centerGenerator.items);
//         }
//         centerSelectorMono.SetCustomDataCount(dataCount).SetSelectedDataIndex(centerSelectorMono.CurrSelectedDataIndex);
//     }
//
//     private void RefreshPeakShape()
//     {
//         int centerSelectorIndex = centerSelectorMono.CurrSelectedDataIndex;
//         // 自适应 方式
//         int dataCount = 20;
//         int displayCount = dataCount < displayPeakShapeMaxCount ? dataCount : displayPeakShapeMaxCount;
//         if (peakShapeGenerator.items.Count != displayCount)
//         {
//             peakShapeGenerator.SetCount(displayCount);
//             peakShapeSelectorMono.AddRangeItems(peakShapeGenerator.items);
//         }
//         peakShapeDatas.Clear();
//         for (int i = 0; i < dataCount; i++)
//         {
//             peakShapeDatas.Add(i + centerSelectorIndex);
//         }
//         peakShapeSelectorMono.SetCustomDataCount(dataCount).SetSelectedDataIndex(peakShapeSelectorMono.CurrSelectedDataIndex);
//     }
//
//     private void OnTabCenterRender(int index, Transform itemTra)
//     {
//         Text lbIndex = itemTra.Find("lbIndex").GetComponent<Text>();
//         lbIndex.text = index.ToString();
//     }
//
//     private void OnTabCenterSelected(int index)
//     {
//         Debug.LogWarning(centerSelectorMono.CurrSelectedDataIndex);
//         RefreshPeakShape();
//     }
//
//     private void OnContentRender(int index, Transform itemTra)
//     {
//         Text lbIndex = itemTra.Find("lbIndex").GetComponent<Text>();
//         lbIndex.text = peakShapeDatas[index].ToString();
//     }
//
//     private void OnContentSelected(int index)
//     {
//         Debug.LogError(peakShapeDatas[index]);
//     }
//
//     private bool OnItemDisableCondition(int index)
//     {
//         return index == 1;
//     }
// }
