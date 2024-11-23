// using System;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace Generator.Ring
// {
//     public partial class Generator
//     {
//         /// 可选零件
//         [Serializable]
//         private class OptionalParts
//         {
//             [SerializeField] [Header("向顺时针旋转一次")] private Button btnClockwiseScrollOnce;
//             [SerializeField] [Header("向逆时针旋转一次")] private Button btnAnticlockwiseScrollOnce;
//
//             private ItemContainer itemContainer;
//
//             public void Init(ItemContainer itemContainer)
//             {
//                 this.itemContainer = itemContainer;
//                 if (btnClockwiseScrollOnce) btnClockwiseScrollOnce.onClick.AddListener(OnBtnClockwiseScrollOnceClick);
//                 if (btnAnticlockwiseScrollOnce) btnAnticlockwiseScrollOnce.onClick.AddListener(OnBtnAnticlockwiseScrollOnceClick);
//             }
//
//             private void OnBtnClockwiseScrollOnceClick()
//             {
//                 itemContainer.SetScroll(EDirection.Clockwise, 1);
//             }
//
//             private void OnBtnAnticlockwiseScrollOnceClick()
//             {
//                 itemContainer.SetScroll(EDirection.Anticlockwise, 1);
//             }
//         }
//     }
// }