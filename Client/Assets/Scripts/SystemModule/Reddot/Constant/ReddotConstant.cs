using System;

namespace ReddotModule
{
    /// 红点
    public enum EReddot
    {
        /// 最外层 Icon
        TestIcon,
        /// 界面层 Tab1
        TestTab1,
        /// 界面层 Tab2
        TestTab2,
        /// 数据列表层1
        TestDataList,
        /// 数据单元层
        TestDataUnit,
        /// 数据单元子层
        TestDataUnitChild,
    }
    
    public static class ReddotExtensions
    {
        private static IReddotConfig ReddotConfig { get; }
        private static IReddotTree ReddotTree { get; }

        static ReddotExtensions()
        {
            if (ReddotTreeManager.Instance is not IReddotTreeManager reddotTreeManager)
            {
                throw new NotImplementedException();
            }
            ReddotConfig = reddotTreeManager.ReddotConfig;
            ReddotTree = reddotTreeManager.ReddotTree;
        }

#region Config
        /// 连接器
        public static EReddot Linker(this EReddot parentEReddot, params EReddot[] childrenReddots)
        {
            return ReddotConfig.Linker(parentEReddot, childrenReddots);
        }

        /// 绑定节点 => Common
        public static EReddot BindNode(this EReddot eReddot)
        {
            return ReddotConfig.BindNode(eReddot);
        }
        /// 绑定节点 => Common + Condition
        public static EReddot BindNode(this EReddot eReddot, ReddotConditionByCommonDelegate reddotConditionByCommonDelegate)
        {
            return ReddotConfig.BindNode(eReddot, reddotConditionByCommonDelegate);
        }
        
        /// 绑定节点 => Layer1
        public static EReddot BindNodeLayer1<T>(this EReddot eReddot) where T: IReddotNodeCollectionsLayer1
        {
            return ReddotConfig.BindNodeLayer1<T>(eReddot);
        }
        /// 绑定节点 => Layer1 + Condition
        public static EReddot BindNodeLayer1<T>(this EReddot eReddot, ReddotConditionByCollectionsLayer1Delegate reddotConditionByCollectionsLayer1Delegate, ReddotCountByCollectionsLayer1Delegate reddotCountByCollectionsLayer1Delegate) where T: IReddotNodeCollectionsLayer1
        {
            return ReddotConfig.BindNodeLayer1<T>(eReddot, reddotConditionByCollectionsLayer1Delegate, reddotCountByCollectionsLayer1Delegate);
        }
        
        /// 绑定节点 => Layer2
        public static EReddot BindNodeLayer2<T>(this EReddot eReddot) where T: IReddotNodeCollectionsLayer2
        {
            return ReddotConfig.BindNodeLayer2<T>(eReddot);
        }
        /// 绑定节点 => Layer2 + Condition
        public static EReddot BindNodeLayer2<T>(this EReddot eReddot,  ReddotConditionByCollectionsLayer2Delegate reddotConditionByCollectionsLayer2Delegate, ReddotCountByCollectionsLayer2Delegate reddotCountByCollectionsLayer2Delegate) where T: IReddotNodeCollectionsLayer2
        {
            return ReddotConfig.BindNodeLayer2<T>(eReddot, reddotConditionByCollectionsLayer2Delegate, reddotCountByCollectionsLayer2Delegate);
        }
#endregion

#region Tree
        /// 添加组件
        public static void AddComponent(this EReddot current, IReddotComponent reddotComponent)
        {
            ReddotTree.AddComponent(current, reddotComponent);
        }

        /// 通用更新
        public static void Update(this EReddot current)
        {
            ReddotTree.Update(current);
        }

        // /// 通用更新（性能较差，安全性低）
        // public static void Update(this EReddot current, params int[] indexs)
        // {
        //     ReddotTree.Update(current, indexs);
        // }
        // /// 更新入口 => Common
        // public static void UpdateCommon<T>(this EReddot current)  where T: IReddotNodeCommon
        // {
        //     ReddotTree.UpdateCommon<T>(current);
        // }
        // /// 更新入口 => Collections
        // public static void UpdateCollections<T>(this EReddot current) where T : IReddotNodeCollections
        // {
        //     ReddotTree.UpdateCollections<T>(current);
        // }
        // /// 更新入口 => Layer1
        // public static void UpdateLayer1<T>(this EReddot current) where T : IReddotNodeCollectionsLayer1
        // {
        //     ReddotTree.UpdateLayer1<T>(current);
        // }
        // /// 更新入口 => Layer1，指定索引
        // public static void UpdateLayer1<T>(this EReddot current, int layer1Index) where T : IReddotNodeCollectionsLayer1
        // {
        //     ReddotTree.UpdateLayer1<T>(current, layer1Index);
        // }
        // /// 更新入口 => Layer2
        // public static void UpdateLayer2<T>(this EReddot current) where T : IReddotNodeCollectionsLayer2
        // {
        //     ReddotTree.UpdateLayer2<T>(current);
        // }
        // /// 更新入口 => Layer2，指定 layer1 索引
        // public static void UpdateLayer2<T>(this EReddot current, int layer1Index) where T : IReddotNodeCollectionsLayer2
        // {
        //     ReddotTree.UpdateLayer2<T>(current, layer1Index);
        // }
        // /// 更新入口 => Layer2，指定 layer1, layer2 索引
        // public static void UpdateLayer2<T>(this EReddot current, int layer1Index, int layer2Index) where T : IReddotNodeCollectionsLayer2
        // {
        //     ReddotTree.UpdateLayer2<T>(current, layer1Index, layer2Index);
        // }
#endregion
    }
}