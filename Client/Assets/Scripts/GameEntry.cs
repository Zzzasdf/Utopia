using System.Collections.Generic;
using UnityEngine;
using ChatModule;
using ConditionModule;

public class GameEntry : MonoBehaviour
{
    /// 条件
    public static ConditionManager ConditionManager { get; private set; }
    /// 定时器
    public static TimerManager TimerManager { get; private set; }
    /// 聊天
    public static ChatManager ChatManager { get; private set; }

    /// Client 转换器
    public static ClientConverterCollectorSystem ClientConverterCollectorSystem { get; private set; }
    /// Server 转换器
    public static ServerConverterCollectorSystem ServerConverterCollectorSystem { get; private set; }

    private void Awake()
    {
        lifeCycles = new LifeCycleCollections();
        lifeCycles.Add(ConditionManager = new ConditionManager());
        lifeCycles.Add(TimerManager = new TimerManager());
        lifeCycles.Add(ChatManager = new ChatManager());
        lifeCycles.Add(ClientConverterCollectorSystem = new ClientConverterCollectorSystem());
        lifeCycles.Add(ServerConverterCollectorSystem = new ServerConverterCollectorSystem());

        lifeCycles.OnInit();
        lifeCycles.OnSubscribe();
        lifeCycles.OnEnable();
    }

    /// 生命周期集合
    private LifeCycleCollections lifeCycles;
    private void Update() => lifeCycles.OnUpdate(Time.deltaTime);
    private void LateUpdate() => lifeCycles.OnLateUpdate(Time.deltaTime);
    private void FixedUpdate() => lifeCycles.OnFixedUpdate(Time.fixedTime);

    private class LifeCycleCollections
    {
        private HashSet<IInit> inits;
        private HashSet<IReset> resets;
        private HashSet<IDestroy> destroys;
        private HashSet<IEnable> enables;
        private HashSet<IDisable> disables;
        private HashSet<ISubscribe> subscribes;
        private HashSet<IUnsubscribe> unsubscribes;
        private HashSet<IUpdate> updates;
        private HashSet<ILateUpdate> lateUpdates;
        private HashSet<IFixedUpdate> fixedUpdates;
        
        public LifeCycleCollections()
        {
            inits = new HashSet<IInit>();
            resets = new HashSet<IReset>();
            destroys = new HashSet<IDestroy>();
            enables = new HashSet<IEnable>();
            disables = new HashSet<IDisable>();
            subscribes = new HashSet<ISubscribe>();
            unsubscribes = new HashSet<IUnsubscribe>();
            updates = new HashSet<IUpdate>();
            lateUpdates = new HashSet<ILateUpdate>();
            fixedUpdates = new HashSet<IFixedUpdate>();
        }
        
        public void Add<T>(T o) where T: class
        {
            if (o is IInit init) inits.Add(init);
            if (o is IReset reset) resets.Add(reset);
            if (o is IDestroy destroy) destroys.Add(destroy);
            if (o is IEnable enable) enables.Add(enable);
            if (o is IDisable disable) disables.Add(disable);
            if (o is ISubscribe subscribe) subscribes.Add(subscribe);
            if (o is IUnsubscribe unsubscribe) unsubscribes.Add(unsubscribe);
            if (o is IUpdate update) updates.Add(update);
            if (o is ILateUpdate lateUpdate) lateUpdates.Add(lateUpdate);
            if (o is IFixedUpdate fixedUpdate) fixedUpdates.Add(fixedUpdate);
        }

        public void OnInit()
        {
            foreach (var init in inits) init.OnInit();
        }
        public void OnReset()
        {
            foreach (var reset in resets) reset.OnReset();
        }
        public void OnDestroy()
        {
            foreach (var destroy in destroys) destroy.OnDestroy();
        }
        public void OnEnable()
        {
            foreach (var enable in enables) enable.OnEnable();
        }
        public void OnDisable()
        {
            foreach (var disable in disables) disable.OnDisable();
        }
        public void OnSubscribe()
        {
            foreach (var subscribe in subscribes) subscribe.OnSubscribe();
        }
        public void OnUnsubscribe()
        {
            foreach (var unsubscribe in unsubscribes) unsubscribe.OnUnsubscribe();
        }
        public void OnUpdate(float deltaTime)
        {
            foreach (var update in updates) update.OnUpdate(deltaTime);
        }
        public void OnLateUpdate(float deltaTime)
        {
            foreach (var lateUpdate in lateUpdates) lateUpdate.OnLateUpdate(deltaTime);
        }
        public void OnFixedUpdate(float fixedTime)
        {
            foreach (var fixedUpdate in fixedUpdates) fixedUpdate.OnFixedUpdate(fixedTime);
        }
    }
}
