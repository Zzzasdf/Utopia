using System;
using System.Collections.Generic;
using UnityEditor.Playables;

namespace ReddotModule
{
    public interface IReddotConfig
    {
        /// 连接器
        EReddot Linker(EReddot parentEReddot, params EReddot[] childRedTips);
        /// 绑定节点 => Common
        EReddot BindNode(EReddot eReddot);
        /// 绑定节点 => Common + Condition
        EReddot BindNode(EReddot eReddot, ReddotConditionByCommonDelegate reddotConditionByCommonDelegate);
        /// 绑定节点 => Layer1
        EReddot BindNodeLayer1<T>(EReddot eReddot) where T : IReddotNodeCollectionsLayer1;
        /// 绑定节点 => Layer1 + Condition
        EReddot BindNodeLayer1<T>(EReddot eReddot, ReddotConditionByCollectionsLayer1Delegate reddotConditionByCollectionsLayer1Delegate, ReddotCountByCollectionsLayer1Delegate reddotCountByCollectionsLayer1Delegate) where T : IReddotNodeCollectionsLayer1;
        /// 绑定节点 => Layer2
        EReddot BindNodeLayer2<T>(EReddot eReddot) where T: IReddotNodeCollectionsLayer2;
        /// 绑定节点 => Layer2 + Condition
        EReddot BindNodeLayer2<T>(EReddot eReddot, ReddotConditionByCollectionsLayer2Delegate reddotConditionByCollectionsLayer2Delegate, ReddotCountByCollectionsLayer2Delegate reddotCountByCollectionsLayer2Delegate) where T : IReddotNodeCollectionsLayer2;
    }

    public partial class ReddotConfig: IReddotConfig
    {
        private Dictionary<EReddot, IReddotNode> reddotNodeMap = new Dictionary<EReddot, IReddotNode>();
        
        public ReddotConfig()
        {
            List<EReddot> detectionList = ModelList();
            foreach (var redTip in detectionList)
            {
                if (!ClosedLoopInspection(redTip))
                {
                    continue;
                }
                return;
            }
        }

        /// 模型列表
        private partial List<EReddot> ModelList();

        /// 闭环检测 (快慢指针)
        private bool ClosedLoopInspection(EReddot eReddot)
        {
            // TODO ZZZ => 检测，抛出异常点
            return false;
        }

        EReddot IReddotConfig.BindNode(EReddot eReddot)
        {
            throw new NotImplementedException();
        }

        EReddot IReddotConfig.BindNode(EReddot eReddot, ReddotConditionByCommonDelegate reddotConditionByCommonDelegate)
        {
            throw new NotImplementedException();
        }

        EReddot IReddotConfig.BindNodeLayer1<T>(EReddot eReddot)
        {
            throw new NotImplementedException();
        }

        EReddot IReddotConfig.BindNodeLayer1<T>(EReddot eReddot, ReddotConditionByCollectionsLayer1Delegate reddotConditionByCollectionsLayer1Delegate, ReddotCountByCollectionsLayer1Delegate reddotCountByCollectionsLayer1Delegate)
        {
            throw new NotImplementedException();
        }

        EReddot IReddotConfig.BindNodeLayer2<T>(EReddot eReddot)
        {
            throw new NotImplementedException();
        }

        EReddot IReddotConfig.BindNodeLayer2<T>(EReddot eReddot, ReddotConditionByCollectionsLayer2Delegate reddotConditionByCollectionsLayer2Delegate, ReddotCountByCollectionsLayer2Delegate reddotCountByCollectionsLayer2Delegate)
        {
            throw new NotImplementedException();
        }

        EReddot IReddotConfig.Linker(EReddot parentEReddot, params EReddot[] childRedTips)
        {
            throw new NotImplementedException();
        }
    }
}