using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStateMachine", menuName = "State Machine")]
public class StateMachineData : ScriptableObject
{
    public List<StateNode> states = new List<StateNode>();
    public List<Transition> transitions = new List<Transition>();

    /// 唯一 id 查找 node
    public StateNode GetStateByUniqueId(int uniqueId)
    {
        foreach (var state in states)
        {
            if (state.uniqueId != uniqueId) continue;
            return state;
        }
        return null;
    }

    /// 唯一 id 生成器
    public int UniqueIdGenerator()
    {
        int result = 0;
        while (ContainsUniqueId(result))
        {
            result++;
        }
        return result;
    }
    /// 是否 包含 唯一 id
    public bool ContainsUniqueId(int uniqueId)
    {
        return states.Any(x => x.uniqueId == uniqueId);
    }

    /// 唯一 名称 生成器
    public string UniqueNameGenerator()
    {
        string result;
        int i = 0;
        do
        {
            result = string.Format($"New State ({i++})");
        } while (ContainsName(result));
        return result;
    }
    /// 是否 包含 名称
    public bool ContainsName(string name)
    {
        return states.Any(x => x.name == name);
    }

    public void Export()
    {
        Debug.LogError("导出");
    }
}

public interface IElement 
{
    
}

[System.Serializable]
public class StateNode: IElement
{
    public int uniqueId;
    public ENode eNode;
    public string name;
    public Rect rect; // 节点在窗口中的位置
    public bool isEntryState; // 是否是入口状态
}

public enum ENode 
{
    Normal,
    Branch,
}

[System.Serializable]
public class Transition: IElement
{
    public int fromUniqueId;
    public int toUniqueId;
}
