using System.Collections.Generic;

namespace ReddotModule
{
    public delegate bool ReddotConditionByCollectionsLayer1Delegate(int layer1Index);
    public delegate int ReddotCountByCollectionsLayer1Delegate();
    public interface IReddotNodeCollectionsLayer1
    {
        
    }
    public class Layer1 : IReddotNode, IReddotNodeCollectionsLayer1
    {
        private EReddot current;
        private Dictionary<EReddot, IReddotNodeCommon> parents;
        
        
        private IReddotNode reddotNodeImplementation;

        void IReddotNode.SetReddot(EReddot eReddot)
        {
            throw new System.NotImplementedException();
        }

        EReddot IReddotNode.Linker(params EReddot[] childrenReddots)
        {
            throw new System.NotImplementedException();
        }

        void IReddotNode.AddParent(EReddot eReddot, IReddotNode reddotNode)
        {
            throw new System.NotImplementedException();
        }

        void IReddotNode.AddChild(EReddot eReddot, IReddotNode reddotNode)
        {
            throw new System.NotImplementedException();
        }

        void IReddotNode.AddComponent(IReddotComponent reddotComponent)
        {
            throw new System.NotImplementedException();
        }

        void IReddotNode.Update()
        {
            throw new System.NotImplementedException();
        }
    }
}
