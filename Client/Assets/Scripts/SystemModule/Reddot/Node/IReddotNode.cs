using ReddotModule;

public interface IReddotNode
{
    void SetReddot(EReddot eReddot);
    EReddot Linker(params EReddot[] childrenReddots);
    
    void AddParent(EReddot eReddot, IReddotNode reddotNode);
    void AddChild(EReddot eReddot, IReddotNode reddotNode);

    void AddComponent(IReddotComponent reddotComponent);
    void Update();
}