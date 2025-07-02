using System.Collections.Generic;

public partial class RedPointTreeManager
{
    private RedPointTreeManager _instance;
    public RedPointTreeManager Instance => _instance ??= new RedPointTreeManager();

    /// 映射节点
    private Dictionary<ERedPoint, RedPointNode> nodeMap;
    /// 映射根节点
    private Dictionary<ERedPoint, HashSet<ERedPoint>> rootMap;
    
    private RedPointTreeManager()
    {
        nodeMap = new Dictionary<ERedPoint, RedPointNode>();
        rootMap = new Dictionary<ERedPoint, HashSet<ERedPoint>>();
        InitConfigure();
    }
}