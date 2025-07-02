using System;

public partial class RedPointTreeManager
{
    private void InitConfigure()
    {
        // 新建一颗红点树
        {
            ERedPoint.Root
                .TreeLinker(() => true, ERedPointTree.Test1).Linker(
                    ERedPoint.Tab1.Condition(() => true).Linker(
                        ERedPoint.Item1.Condition(() => true)).Linker(
                        ERedPoint.Item2.Condition(() => true)))
                .TreeLinker(() => true, ERedPointTree.Test1).Linker(
                    ERedPoint.Tab2.Condition(() => true).Linker(
                        ERedPoint.Item3.Condition(() => true))).Linker(
                    ERedPoint.Item1.Condition(() => true))
                .Build(ERedPointTree.Root);
        }
        // ...
        {
            
        }
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

public static class ERedPointExtensions
{
    /// 连接节点
    public static ERedPoint Linker(this ERedPoint self, ERedPoint child)
    {
        return self;
    }
    /// 添加节点条件
    public static ERedPoint Condition(this ERedPoint self, Func<bool> display)
    {
        return self;
    }
    
    /// 连接子树
    public static ERedPoint TreeLinker(this ERedPoint self, Func<bool> enable, ERedPointTree eRedPointTree)
    {
        return self;
    }

    /// 构建树
    public static void Build(this ERedPoint self, ERedPointTree eRedPointTree)
    {
        
    }
}