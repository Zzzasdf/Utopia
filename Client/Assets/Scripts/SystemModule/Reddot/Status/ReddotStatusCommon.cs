// using UnityEngine;
//
// namespace ReddotModule
// {
//     /// 普通状态
//     public class StatusCommon : IReddotStatus
//     {
//         private EReddot EReddot { get; set; }
//         private ReddotTree ReddotTree { get; set; }
//         
//         private ReddotConditionByCommonDelegate ConditionByCommonFunc { get; set; }
//         private bool status { get; set; }
//
//         void IReddotStatus.InitRedTip(EReddot eReddot, ReddotTree reddotTree)
//         {
//             this.EReddot = eReddot;
//             this.ReddotTree = reddotTree;
//         }
//
//         public int Count(params int[] indexs)
//         {
//             return 1;
//         }
//
//         bool IReddotStatus.Update(params int[] indexs)
//         {
//             if (indexs != null)
//             {
//                 Debug.LogError("代码错误");
//                 return false;
//             }
//             return ConditionByCommonFunc.Invoke();
//         }
//
//         public EReddot Linker(params EReddot[] childRedTips)
//         {
//             return EReddot.Linker(childRedTips);
//         }
//
//         public EReddot AddCondition(ReddotConditionByCommonDelegate conditionByCommonFunc)
//         {
//             this.ConditionByCommonFunc = conditionByCommonFunc;
//             return EReddot;
//         }
//     }
// }