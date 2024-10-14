// using UnityEngine;
//
// namespace ReddotModule
// {
//     public class StatusCollectionsLayer1 : IReddotStatus
//     {
//         private EReddot EReddot { get; set; }
//         private ReddotTree ReddotTree { get; set; }
//         
//         private ReddotConditionByCollectionsDelegate ConditionByFunc { get; set; }
//         private ReddotCountByCollectionsDelegate CountByFunc { get; set; }
//         private bool[] status { get; set; }
//
//         void IReddotStatus.InitRedTip(EReddot eReddot, ReddotTree reddotTree)
//         {
//             this.EReddot = eReddot;
//             this.ReddotTree = reddotTree;
//         }
//
//         public int Count(params int[] indexs)
//         {
//             return CountByFunc.Invoke();
//         }
//
//         bool IReddotStatus.Update(params int[] indexs)
//         {
//             if (indexs == null || indexs.Length > 1)
//             {
//                 Debug.LogError("代码错误");
//                 return false;
//             }
//             return ConditionByFunc.Invoke(indexs[0]);
//         }
//
//         public EReddot Linker(params EReddot[] childRedTips)
//         {
//             return EReddot.Linker(childRedTips);
//         }
//
//         public EReddot AddCondition(ReddotConditionByCollectionsDelegate conditionByFunc, ReddotCountByCollectionsDelegate countByFunc)
//         {
//             this.ConditionByFunc = conditionByFunc;
//             this.CountByFunc = countByFunc;
//             return EReddot;
//         }
//     }
// }