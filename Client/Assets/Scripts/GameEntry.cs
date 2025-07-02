using System.Collections.Generic;
using UnityEngine;
using ChatModule;
using ConditionModule;

public class GameEntry : MonoBehaviour
{
    public static GameEntry Instance;
    
    private List<IManager> managers;
    private List<IUpdate> updates;
    
    /// 条件
    public ConditionManager ConditionManager { get; private set; }
    /// 定时器
    public TimerManager TimerManager { get; private set; }
    /// 聊天
    public ChatManager ChatManager { get; private set; }
    
    /// Client 转换器
    public ClientConverterCollectorSystem ClientConverterCollectorSystem { get; private set; }
    /// Runtime 转换器
    public RuntimeConverterCollectorSystem RuntimeConverterCollectorSystem { get; private set; }
    /// Server 转换器
    public ServerConverterCollectorSystem ServerConverterCollectorSystem { get; private set; }
    
    private void Awake()
    {
        Instance = this;
        ConditionManager = new ConditionManager();
        TimerManager = new TimerManager();
        ChatManager = new ChatManager();
        
        ClientConverterCollectorSystem = new ClientConverterCollectorSystem();
        RuntimeConverterCollectorSystem = new RuntimeConverterCollectorSystem();
        ServerConverterCollectorSystem = new ServerConverterCollectorSystem();

        managers = new List<IManager>
        {
            ConditionManager,
            TimerManager,
            ChatManager,
            
            ClientConverterCollectorSystem,
            RuntimeConverterCollectorSystem,
            ServerConverterCollectorSystem,
        };
        updates = new List<IUpdate>
        {
            TimerManager,
        };
    }

    private void Update()
    {
        foreach (IUpdate item in updates)
        {
            item.Update();
        }
    }

    private void OnDestroy()
    {
        foreach (IManager item in managers)
        {
            item.Clear();
        }
    }
}
