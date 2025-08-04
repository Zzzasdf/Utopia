using System;
using System.Collections.Generic;

namespace ConditionModule
{
    public interface IConditionResponder<T>
        where T: struct 
    {
        void Reset();
        void Destroy();
        
        void Fire();
        void Fire(T type);
        void FireNow();
        void FireNow(T type);

        void Subscribe(ICondition<T> types, Action<bool> callback);
        void Unsubscribe(ICondition<T> types, Action<bool> callback);
        bool Status(ICondition<T> types);
    }
    /// 条件响应器
    public class ConditionResponder<T>: IConditionResponder<T>
        where T: struct
    {
        /// kv 映射 配置
        private Dictionary<ICondition<T>, ConditionDataWrapper<T>> types2ConfigDict;
        /// 条件类型 映射 配置集 （检测）
        private Dictionary<T, PooledHashSet<ConditionDataWrapper<T>>> type2ConfigsDict;
        // 单线程仅需一个缓冲器
        private DelayBuffer<T> delayBuffer;
        
        public ConditionResponder()
        {
            types2ConfigDict = new Dictionary<ICondition<T>, ConditionDataWrapper<T>>();
            type2ConfigsDict = new Dictionary<T, PooledHashSet<ConditionDataWrapper<T>>>();
            delayBuffer = new DelayBuffer<T>(10, Fire, Fire, Fire);
        }
        void IConditionResponder<T>.Reset()
        {
            delayBuffer.Reset();
            Clear();
        }
        void IConditionResponder<T>.Destroy()
        {
            delayBuffer.Destroy();
            delayBuffer = null;
            Clear();
        }
        private void Clear()
        {
            foreach (var pair in type2ConfigsDict)
            {
                pair.Value.Clear();
                ((IDisposable)pair.Value).Dispose();
            }
            type2ConfigsDict.Clear();

            foreach (var pair in types2ConfigDict)
            {
                ((IDisposable)pair.Value).Dispose();
            }
            types2ConfigDict.Clear();
        }

        void IConditionResponder<T>.Fire() => delayBuffer.FireAllType();
        void IConditionResponder<T>.Fire(T type) => delayBuffer.FireType(type);
        void IConditionResponder<T>.FireNow() => delayBuffer.FireAllTypeNow();
        void IConditionResponder<T>.FireNow(T type) => delayBuffer.FireTypeNow(type);
        private void Fire()
        {
            foreach (var pair in types2ConfigDict)
            {
                ConditionDataWrapper<T> wrapper = pair.Value;
                wrapper.Fire();
            }
        }
        private void Fire(HashSet<T> types)
        {
            using (var wrappers = PooledHashSet<ConditionDataWrapper<T>>.Get())
            {
                foreach (var type in types)
                {
                    if (!type2ConfigsDict.TryGetValue(type, out PooledHashSet<ConditionDataWrapper<T>> hashSet))
                    {
                        continue;
                    }
                    wrappers.AddRange(hashSet);
                }
                foreach (var wrapper in wrappers)
                {
                    wrapper.Fire();
                }
            }
        }
        private void Fire(T type)
        {
            if (!type2ConfigsDict.TryGetValue(type, out PooledHashSet<ConditionDataWrapper<T>> wrappers))
            {
                return;
            }
            foreach (var wrapper in wrappers)
            {
                wrapper.Fire();
            }
        }

        // 订阅
        void IConditionResponder<T>.Subscribe(ICondition<T> types, Action<bool> callback)
        {
            if (!types2ConfigDict.TryGetValue(types, out ConditionDataWrapper<T> wrapper))
            {
                wrapper = ConditionDataWrapper<T>.Get(types);
                AddData(wrapper, types);
            }
            wrapper.Subscribe(callback);
            // 订阅时触发一次检测
            callback.Invoke(wrapper.Status());
        }
        /// 取消订阅
        void IConditionResponder<T>.Unsubscribe(ICondition<T> types, Action<bool> callback)
        {
            if (!types2ConfigDict.TryGetValue(types, out ConditionDataWrapper<T> wrapper))
            {
                return;
            }
            wrapper.Unsubscribe(callback);
            if (!wrapper.CheckNull()) return;
            RemoveData(wrapper);
        }
        /// 状态
        bool IConditionResponder<T>.Status(ICondition<T> types)
        {
            if (!types2ConfigDict.TryGetValue(types, out ConditionDataWrapper<T> wrapper))
            {
                AddData(wrapper, types);
            }
            return wrapper.Status();
        }

        private void AddData(in ConditionDataWrapper<T> wrapper, ICondition<T> types)
        {
            // 加入 kv 映射
            types2ConfigDict.Add(types, wrapper);
            // 归入 条件类型检测字典
            using (var conditionTypes = PooledHashSet<T>.Get())
            {
                wrapper.GetTypes(conditionTypes);
                foreach (var eConditionType in conditionTypes)
                {
                    if (!type2ConfigsDict.TryGetValue(eConditionType, out PooledHashSet<ConditionDataWrapper<T>> wrappers))
                    {
                        type2ConfigsDict.Add(eConditionType, wrappers = PooledHashSet<ConditionDataWrapper<T>>.Get());
                    }
                    wrappers.Add(wrapper);
                }
            }
        }
        private void RemoveData(ConditionDataWrapper<T> wrapper)
        {
            // 从 条件类型字典 中删除
            using (var conditionTypes = PooledHashSet<T>.Get())
            {
                wrapper.GetTypes(conditionTypes);
                foreach (var eConditionType in conditionTypes)
                {
                    type2ConfigsDict[eConditionType].Remove(wrapper);
                }
            }
            // 从 kv 字典中 删除
            types2ConfigDict.Remove(wrapper.GetTypeGroup());
            ((IDisposable)wrapper).Dispose();
        }
        
        private class ConditionDataWrapper<T>: IDisposable
            where T: struct
        {
            private static readonly MonitoredObjectPool.ObjectPool<ConditionDataWrapper<T>, T> s_Pool = 
                new("ConditionDataWrapper", () => new ConditionDataWrapper<T>(), 
                null,
                x =>
                {
                    x.callback = null;
                    x.status = false;
                    x.isDirty = true;
                });
            
            public static ConditionDataWrapper<T> Get(ICondition<T> types)
            {
                ConditionDataWrapper<T> result = s_Pool.Get();
                result.types = types;
                return result;
            }
            private ConditionDataWrapper() { }
            void IDisposable.Dispose() => s_Pool.Release(this);
            
#if !POOL_RELEASES
            ~ConditionDataWrapper() => s_Pool.FinalizeDebug();
#endif
            
            private ICondition<T> types;
            private bool isDirty = true;
            private bool status;
            private Action<bool> callback;
            
            public void GetTypes<TCollection>(TCollection eConditionTypes)
                where TCollection: ICollection<T>
            {
                types.GetConditionTypes(eConditionTypes);
            }

            public void Subscribe(Action<bool> callback)
            {
                this.callback += callback;
            }
            public void Unsubscribe(Action<bool> callback)
            {
                this.callback -= callback;
            }
            public void Fire()
            {
                isDirty = true;
                if (callback == null) return;
                bool lastStatus = status;
                if (lastStatus == Status()) return;
                callback.Invoke(status);
            }
            public bool Status()
            {
                if (isDirty)
                {
                    isDirty = !isDirty;
                    status = types.CheckCondition();
                }
                return status;
            }
            // 检空
            public bool CheckNull()
            {
                return callback == null;
            }
            
            public ICondition<T> GetTypeGroup()
            {
                return types;
            }
        }
    }
}