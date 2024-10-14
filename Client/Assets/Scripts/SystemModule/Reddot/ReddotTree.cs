using System;
using System.Collections.Generic;

namespace ReddotModule
{
    public interface IReddotTree
    {
        /// 添加组件
        void AddComponent(EReddot current, IReddotComponent reddotComponent);
        /// 通用更新
        void Update(EReddot current);
        // /// 通用更新（性能较差，安全性低）
        // void Update(EReddot current, params int[] indexs);
        // /// 更新入口 => Common
        // void UpdateCommon<T>(EReddot current) where T : IReddotNodeCommon;
        // /// 更新入口 => Collections
        // void UpdateCollections<T>(EReddot current) where T : IReddotNodeCollections;
        // /// 更新入口 => Layer1
        // void UpdateLayer1<T>(EReddot current) where T : IReddotNodeCollectionsLayer1;
        // /// 更新入口 => Layer1，指定索引
        // void UpdateLayer1<T>(EReddot current, int layer1Index) where T : IReddotNodeCollectionsLayer1;
        // /// 更新入口 => Layer2
        // void UpdateLayer2<T>(EReddot current) where T : IReddotNodeCollectionsLayer2;
        // /// 更新入口 => Layer2，指定 layer1 索引
        // void UpdateLayer2<T>(EReddot current, int layer1Index) where T : IReddotNodeCollectionsLayer2;
        // /// 更新入口 => Layer2，指定 layer1, layer2 索引
        // void UpdateLayer2<T>(EReddot current, int layer1Index, int layer2Index) where T : IReddotNodeCollectionsLayer2;
    }
    
    public class ReddotTree: IReddotTree
    {
        /// 依赖型节点检测
        private List<EReddot> dependInspect = new List<EReddot>();
        /// 处理器映射
        private Dictionary<EReddot, IReddotNode> nodeMap = new Dictionary<EReddot, IReddotNode>();

        public ReddotTree(IReddotConfig reddotConfig)
        {
        }

        /// 添加组件
        void IReddotTree.AddComponent(EReddot current, IReddotComponent reddotComponent) => AddComponentBuiltIn(current, reddotComponent);
        private void AddComponentBuiltIn(EReddot current, IReddotComponent reddotComponent)
        {
            if (!nodeMap.TryGetValue(current, out IReddotNode node))
            {
                nodeMap.Add(current, node = new Common(current));
            }
            node.AddComponent(reddotComponent);
        }

        /// 通用更新
        void IReddotTree.Update(EReddot current) => UpdateBuiltIn(current);
        private void UpdateBuiltIn(EReddot current)
        {
            if (!nodeMap.TryGetValue(current, out IReddotNode node))
            {
                nodeMap.Add(current, node = new Common(current));
            }
            node.Update();
        }
    }
}