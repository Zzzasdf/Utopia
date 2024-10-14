using System.Collections.Generic;

namespace ReddotModule
{
    public delegate bool ReddotConditionByCollectionsLayer2Delegate(int layer1Index, int layer2Index);
    public delegate int ReddotCountByCollectionsLayer2Delegate(int layer1Index);
    public interface IReddotNodeCollectionsLayer2
    {
        
    }
    public class Layer2 : IReddotNode, IReddotNodeCollectionsLayer2
    {
        private EReddot current;
        private Dictionary<EReddot, IReddotNodeCollectionsLayer1> parents;

        
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
