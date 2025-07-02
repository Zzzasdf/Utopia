// using System;
// using System.Collections.Generic;
// using UnityEngine;
//
// namespace Generator.Ring
// {
//     public partial class Generator
//     {
//         /// 滚动状态
//         private enum EScrollStatus
//         {
//             /// 自由状态
//             Freedom = 1 << 0,
//             /// 锁定状态
//             Locked = 1 << 1,
//             /// 开始
//             Start = 1 << 2 | Locked,
//             /// 滚动中
//             Rolling = 1 << 3 | Locked,
//             /// 间隔
//             Interval = 1 << 4 | Locked,
//             /// 结束
//             End = 1 << 5 | Locked,
//         }
//
//         private class ScrollAniInfo
//         {
//             private readonly ItemContainer itemContainer;
//             private readonly Func<EScrollAni> eScrollAniFunc;
//             private readonly Func<int> onceScrollMillisecondFunc;
//             private readonly Func<int> continuousScrollIntervalMillisecondFunc;
//
//             private IShape shape;
//             private int xAxisRadius;
//             private int yAxisRadius;
//
//             private IScrollAni scrollAni;
//             private List<Vector2> itemsOriginPos;
//             private List<Vector2> itemsTargetPos;
//             private float onceScrollProgress;
//             private float continuousScrollIntervalProgress;
//             private EScrollStatus eScrollStatus;
//             private ESequence eScrollSequence;
//             private int scrollDir;
//             private int scrollTargetIndex;
//
//             public ScrollAniInfo(ItemContainer itemContainer,
//                 Func<EScrollAni> eScrollAniFunc, Func<int> onceScrollMillisecondFunc, Func<int> continuousScrollIntervalMillisecondFunc)
//             {
//                 this.itemContainer = itemContainer;
//                 this.eScrollAniFunc = eScrollAniFunc;
//                 this.onceScrollMillisecondFunc = onceScrollMillisecondFunc;
//                 this.continuousScrollIntervalMillisecondFunc = continuousScrollIntervalMillisecondFunc;
//             }
//
//             public void Init(EShape eShape, int xAxisRadius, int yAxisRadius)
//             {
//                 if (!ShapeMap.TryGetValue(eShape, out IShape shape))
//                 {
//                     Debug.LogError($"未定义类型 => {eShape}");
//                     return;
//                 }
//                 this.shape = shape;
//                 this.xAxisRadius = xAxisRadius;
//                 this.yAxisRadius = yAxisRadius;
//                 eScrollStatus = Generator.EScrollStatus.Freedom;
//             }
//
//             public EScrollStatus EScrollStatus()
//             {
//                 return eScrollStatus;
//             }
//
//             public void ResetStart(EDirection eGeneratorDir, ESequence eScrollSequence, int scrollTargetIndex)
//             {
//                 this.eScrollSequence = eScrollSequence;
//                 scrollDir = (generatorDir: eGeneratorDir, scrollSequence: eScrollSequence) switch
//                 {
//                     (EDirection.Clockwise, ESequence.Forward) => 1,
//                     (EDirection.Clockwise, ESequence.Reverse) => -1,
//                     (EDirection.Anticlockwise, ESequence.Forward) => -1,
//                     (EDirection.Anticlockwise, ESequence.Reverse) => 1,
//                 };
//                 this.scrollTargetIndex = scrollTargetIndex;
//                 eScrollStatus = Generator.EScrollStatus.Start;
//             }
//
//             public void ForceEnd()
//             {
//                 onceScrollProgress = 0;
//                 continuousScrollIntervalProgress = 0;
//                 if (itemsOriginPos != null)
//                 {
//                     // 还原
//                     IReadOnlyList<Item> items = itemContainer.Items();
//                     for (int i = 0; i < itemsOriginPos.Count; i++)
//                     {
//                         Item item = items[i];
//                         Vector2 originPos = itemsOriginPos[i];
//                         item.transform.localPosition = originPos;
//                     }
//                     itemsOriginPos.Clear();
//                 }
//                 itemsTargetPos?.Clear();
//                 eScrollStatus = Generator.EScrollStatus.Freedom;
//             }
//
//             public void Update(float deltaTime)
//             {
//                 switch (eScrollStatus)
//                 {
//                     case Generator.EScrollStatus.Start:
//                     {
//                         UpdateScrollStart(deltaTime);
//                         break;
//                     }
//                     case Generator.EScrollStatus.Rolling:
//                     {
//                         UpdateScrollRolling(deltaTime);
//                         break;
//                     }
//                     case Generator.EScrollStatus.Interval:
//                     {
//                         UpdateScrollInterval(deltaTime);
//                         break;
//                     }
//                     case Generator.EScrollStatus.End:
//                     {
//                         UpdateScrollEnd();
//                         break;
//                     }
//                 }
//             }
//
//             private void UpdateScrollStart(float deltaTime)
//             {
//                 onceScrollProgress = 0;
//                 continuousScrollIntervalProgress = 0;
//                 if (!scrollAniMap.TryGetValue(eScrollAniFunc.Invoke(), out IScrollAni scrollAni))
//                 {
//                     Debug.LogError($"未定义类型 => {eScrollAniFunc.Invoke()}");
//                     return;
//                 }
//                 this.scrollAni = scrollAni;
//                 ItemsPosGenerator();
//                 eScrollStatus = Generator.EScrollStatus.Rolling;
//                 Update(deltaTime);
//             }
//
//             private void UpdateScrollRolling(float deltaTime)
//             {
//                 float onceScrollSeconds = onceScrollMillisecondFunc.Invoke() / 1000f;
//                 float addScrollProgress = 1;
//                 if (onceScrollSeconds > 0)
//                 {
//                     addScrollProgress = deltaTime / onceScrollSeconds;
//                 }
//                 onceScrollProgress += addScrollProgress;
//                 UpdateItemsPos(onceScrollProgress);
//                 if (onceScrollProgress < 1)
//                 {
//                     return;
//                 }
//                 itemsOriginPos.Clear();
//                 itemsTargetPos.Clear();
//                 itemContainer.SetCurSelectedIndex(itemContainer.CurSelectedIndex + (int)eScrollSequence);
//                 if (itemContainer.CurSelectedIndex != scrollTargetIndex)
//                 {
//                     continuousScrollIntervalProgress = 0;
//                     eScrollStatus = Generator.EScrollStatus.Interval;
//                     float extraTime = onceScrollSeconds * (onceScrollProgress - 1);
//                     Update(extraTime);
//                     return;
//                 }
//                 eScrollStatus = Generator.EScrollStatus.End;
//                 Update(0);
//             }
//
//             private void UpdateScrollInterval(float deltaTime)
//             {
//                 float continuousScrollIntervalSeconds = continuousScrollIntervalMillisecondFunc.Invoke() / 1000f;
//                 if (continuousScrollIntervalSeconds <= 0)
//                 {
//                     ExtraIntervalSeconds(deltaTime);
//                     return;
//                 }
//                 float addContinuousScrollIntervalProgress = deltaTime / continuousScrollIntervalSeconds;
//                 continuousScrollIntervalProgress += addContinuousScrollIntervalProgress;
//                 if (continuousScrollIntervalProgress < 1)
//                 {
//                     return;
//                 }
//                 ExtraIntervalSeconds(continuousScrollIntervalSeconds * (continuousScrollIntervalProgress - 1));
//                 return;
//
//                 void ExtraIntervalSeconds(float extraIntervalSeconds)
//                 {
//                     onceScrollProgress = 0;
//                     eScrollStatus = Generator.EScrollStatus.Start;
//                     Update(extraIntervalSeconds);
//                 }
//             }
//
//             private void UpdateScrollEnd()
//             {
//                 onceScrollProgress = 0;
//                 continuousScrollIntervalProgress = 0;
//                 eScrollStatus = Generator.EScrollStatus.Freedom;
//                 itemContainer.OnFinalSelectedCallback();
//             }
//
//             private void ItemsPosGenerator()
//             {
//                 itemsOriginPos ??= new List<Vector2>();
//                 itemsOriginPos.Clear();
//                 itemsTargetPos ??= new List<Vector2>();
//                 itemsTargetPos.Clear();
//                 IReadOnlyList<Item> items = itemContainer.Items();
//                 for (int i = 0; i < items.Count; i++)
//                 {
//                     itemsOriginPos.Add(items[i].transform.localPosition);
//                     itemsTargetPos.Add(items[(i - (int)eScrollSequence + items.Count) % items.Count].transform.localPosition);
//                 }
//             }
//
//             private void UpdateItemsPos(float onceScrollProgress)
//             {
//                 IReadOnlyList<Item> items = itemContainer.Items();
//                 if (onceScrollProgress < 1)
//                 {
//                     for (int i = 0; i < items.Count; i++)
//                     {
//                         Item item = items[i];
//                         Vector2 originPos = itemsOriginPos[i];
//                         Vector2 targetPos = itemsTargetPos[i];
//                         scrollAni.MoveAni(shape, item.transform, originPos, targetPos, onceScrollProgress, scrollDir, xAxisRadius, yAxisRadius);
//                     }
//                 }
//                 else // 去除误差
//                 {
//                     for (int i = 0; i < items.Count; i++)
//                     {
//                         Item item = items[i];
//                         Vector2 targetPos = itemsTargetPos[i];
//                         item.transform.localPosition = targetPos;
//                     }
//                 }
//             }
//         }
//     }
// }