using System.Collections.Generic;
using UnityEngine;

namespace ReddotModule
{
    public delegate bool ReddotConditionByCommonDelegate();
    public interface IReddotNodeCommon
    {
        
    }
    
    public class Common: IReddotNode, IReddotNodeCommon
    {
        private EReddot current;
        private Dictionary<EReddot, IReddotNode> parents;
        private Dictionary<EReddot, IReddotNode> children;
        
        private LinkedList<IReddotComponent> components;

        public Common()
        {
            
        }
        public Common(EReddot eReddot)
        {
            current = eReddot;
        }
        void IReddotNode.SetReddot(EReddot eReddot)
        {
            current = eReddot;
        }

        EReddot IReddotNode.Linker(params EReddot[] childrenReddots)
        {
            throw new System.NotImplementedException();
        }

        void IReddotNode.AddParent(EReddot eReddot, IReddotNode reddotNode)
        {
            this.parents ??= new Dictionary<EReddot, IReddotNode>();
            if (!parents.TryAdd(eReddot, reddotNode))
            {
                Debug.LogError($"已存在该节点 => { eReddot } ");
            }
        }

        void IReddotNode.AddChild(EReddot eReddot, IReddotNode reddotNode)
        {
            this.children ??= new Dictionary<EReddot, IReddotNode>();
            if (!children.TryAdd(eReddot, reddotNode))
            {
                Debug.LogError($"已存在该节点 => { eReddot }");
            }
        }

        public void AddComponent(IReddotComponent reddotComponent)
        {
            components ??= new LinkedList<IReddotComponent>();
            if (components.Contains(reddotComponent))
            {
                return;
            }
            components.AddLast(reddotComponent);
        }

        void IReddotNode.Update()
        {
            
        }

        private void UpdateChildren(params int[] indexs)
        {
            // if (children == null || children.Count == 0)
            // {
            //     return;
            // }
            // List<int> childIndexList = new List<int>();
            // childIndexList.AddRange(indexs);
            // childIndexList.Add(-1);
            // foreach (var child in children)
            // {
            //     IReddotStatus childStatus = child.Value.GetStatus();
            //     for (int i = 0, count = childStatus.Count(indexs); i < count; i++)
            //     {
            //         childIndexList[indexs.Length] = i;
            //         int[] childIndexs = childIndexList.ToArray();
            //         child.Value.Update(childIndexs);   
            //     }
            // }
        }

        private void UpdateCurrent(params int[] indexs)
        {
            
        }

        private void UpdateParents(params int[] indexs)
        {
            
        }
    }
}
