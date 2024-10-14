// using UnityEngine;
//
// namespace ReddotModule
// {
//     public class StatusCollectionsLayer2 : IReddotStatus
//     {
//         private EReddot EReddot { get; set; }
//         private ReddotTree ReddotTree { get; set; }
//         
//         private ReddotConditionByCollectionsLayer1Delegate ConditionByCollectionsFunc { get; set; }
//         private ReddotCountByCollectionsLayer1Delegate CountByCollectionsFunc { get; set; }
//         private bool[][] status { get; set; }
//
//
//         void IReddotStatus.InitRedTip(EReddot eReddot, ReddotTree reddotTree)
//         {
//             this.EReddot = eReddot;
//             this.ReddotTree = reddotTree;
//         }
//         
//         public int Count(params int[] indexs)
//         {
//             return CountByCollectionsFunc.Invoke(indexs[0]);
//         }
//
//         bool IReddotStatus.Update(params int[] indexs)
//         {
//             if (indexs == null || indexs.Length > 2)
//             {
//                 Debug.LogError("代码错误");
//                 return false;
//             }
//
//             return ConditionByCollectionsFunc.Invoke(indexs[0], indexs[1]);
//         }
//
//         public EReddot Linker(params EReddot[] childRedTips)
//         {
//             return EReddot.Linker(childRedTips);
//         }
//
//         public EReddot AddCondition(ReddotConditionByCollectionsLayer1Delegate conditionByCollectionsFunc, ReddotCountByCollectionsLayer1Delegate countByCollectionsFunc)
//         {
//             this.ConditionByCollectionsFunc = conditionByCollectionsFunc;
//             this.CountByCollectionsFunc = countByCollectionsFunc;
//             return EReddot;
//         }
//     }
//
//     public class ListLayer3 : IReddotStatus
//     {
//         private EReddot EReddot { get; set; }
//         private ReddotTree ReddotTree { get; set; }
//         
//         private ReddotConditionByCollectionsLayer2Delegate ConditionByCollectionsFunc { get; set; }
//         private ReddotCountByCollectionsLayer2Delegate CountByCollectionsFunc { get; set; }
//         private bool[][][] status { get; set; }
//         
//         void IReddotStatus.InitRedTip(EReddot eReddot, ReddotTree reddotTree)
//         {
//             this.EReddot = eReddot;
//         }
//
//         public EReddot Linker(params EReddot[] childRedTips)
//         {
//             return EReddot.Linker(childRedTips);
//         }
//
//         public int Count(params int[] indexs)
//         {
//             return CountByCollectionsFunc.Invoke(indexs[0], indexs[1]);
//         }
//
//         bool IReddotStatus.Update(params int[] indexs)
//         {
//             if (indexs == null || indexs.Length > 3)
//             {
//                 Debug.LogError("代码错误");
//                 return false;
//             }
//             return ConditionByCollectionsFunc.Invoke(indexs[0], indexs[1], indexs[2]);
//         }
//
//         public EReddot AddCondition(ReddotConditionByCollectionsLayer2Delegate conditionByCollectionsFunc, ReddotCountByCollectionsLayer2Delegate countByCollectionsFunc)
//         {
//             this.ConditionByCollectionsFunc = conditionByCollectionsFunc;
//             this.CountByCollectionsFunc = countByCollectionsFunc;
//             return EReddot;
//         }
//     }
// }