using System;
using System.Collections.Generic;

public partial class RedPointTreeManager
{
    private void InitConfigure()
    {
        // // 新建一颗红点树
        // {
        //     ERedPoint.Root.NewTree(ERedPointTree.Root)
        //         .TreeLinker(() => true, ERedPointTree.Test1).Linker(
        //             ERedPoint.Tab1.Condition(() => true).Linker(
        //                 ERedPoint.Item1.Condition(() => true)).Linker(
        //                 ERedPoint.Item2.Condition(() => true)))
        //         .TreeLinker(() => true, ERedPointTree.Test1).Linker(
        //             ERedPoint.Tab2.Condition(() => true).Linker(
        //                 ERedPoint.Item3.Condition(() => true))).Linker(
        //             ERedPoint.Item1.Condition(() => true))
        //         .Build();
        // }
        // // ...
        // {
        //     
        // }
    }
}

public enum ERedPointTree
{
    Root = 1,
    Test1 = 2,
}

public enum ERedPoint
{
    Root = 1,
    Tab1 = 2,
    Tab2 = 3,
    Item1 = 4,
    Item2 = 5,
    Item3 = 6,
}

// public static class ERedPointExtensions
// {
//     private static Dictionary<ERedPoint, RedPointNode> nodeDict;
//     private static Dictionary<ERedPointTree, RedPointTree> treeDict;
//
//     /// 新建树
//     public static ERedPoint NewTree(this ERedPoint self, ERedPointTree belongToTree)
//     {
//         nodeDict = new Dictionary<ERedPoint, RedPointNode>();
//         treeDict = new Dictionary<ERedPointTree, RedPointTree>();
//         
//         RedPointNode nodeSelf = new RedPointNode(self, belongToTree);
//         nodeDict.Add(self, nodeSelf);
//         
//         RedPointTree redPointTree = new RedPointTree(belongToTree, self);
//         treeDict.Add(belongToTree, redPointTree);
//
//         return self;
//     }
//     /// 构建完成，导入 manager
//     public static void Build(this ERedPoint self)
//     {
//         
//     }
//     
//     /// 连接节点
//     public static ERedPoint Linker(this ERedPoint self, ERedPoint child)
//     {
//         if (!nodeDict.TryGetValue(current, out RedPointNode nodeCurrent))
//         {
//             Debug.LogError($"未先初始化 {current}");
//             return;
//         }
//         RedPointNode nodeChild = new RedPointNode(child);
//         nodeDict.Add(child, nodeChild);
//         
//         nodeChild.AddParentNode(nodeCurrent);
//         nodeCurrent.AddChildNode(nodeChild);
//         return self;
//     }
//     /// 添加节点条件
//     public static ERedPoint Condition(this ERedPoint self, Func<bool> displayCondition)
//     {
//         if (!nodeDict.TryGetValue(current, out RedPointNode nodeCurrent))
//         {
//             nodeDict.Add(current, nodeCurrent = new RedPointNode(current));
//         }
//         nodeCurrent.AddCondition(displayCondition);
//         return self;
//     }
//     
//     /// 连接子树
//     public static ERedPoint TreeLinker(this ERedPoint self, Func<bool> enable, ERedPointTree eRedPointTree)
//     {
//         configRedPointTree ??= new RedPointTree(self);
//         return self;
//     }
// }