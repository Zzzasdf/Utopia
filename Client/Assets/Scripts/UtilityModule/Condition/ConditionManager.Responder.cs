using System;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace ConditionModule
{
    public interface IConditionResponder
    {
        void Fire();
        void Fire(EConditionType eConditionType);
    }
    /// 条件响应器
    public class ConditionResponder: IConditionResponder
    {
        /// key 映射 配置
        private Dictionary<IConditionTypeGroup, ConditionDataWrapper> key2Data;
        /// 条件类型 映射 配置集 （检测）
        private Dictionary<EConditionType, List<ConditionDataWrapper>> conditionType2Data;
        
        public ConditionResponder()
        {
            key2Data = new Dictionary<IConditionTypeGroup, ConditionDataWrapper>();
            conditionType2Data = new Dictionary<EConditionType, List<ConditionDataWrapper>>();
        }
        public void Clear()
        {
            conditionType2Data.Clear();
            key2Data.Clear();
        }
        
        public void Fire()
        {
            foreach (var pair in key2Data)
            {
                ConditionDataWrapper wrapper = pair.Value;
                wrapper.Fire();
            }
        }
        public void Fire(EConditionType eConditionType)
        {
            if (!conditionType2Data.TryGetValue(eConditionType, out List<ConditionDataWrapper> wrappers))
            {
                return;
            }
            foreach (var wrapper in wrappers)
            {
                wrapper.Fire();
            }
        }

        // 订阅
        public void Subscribe(IConditionTypeGroup key, Action<bool> callback)
        {
            if (!key2Data.TryGetValue(key, out ConditionDataWrapper wrapper))
            {
                AddData(key, out wrapper);
            }
            wrapper.Subscribe(callback);
            // 订阅时触发一次检测
            callback.Invoke(wrapper.Status());
        }
        /// 取消订阅
        public void Unsubscribe(IConditionTypeGroup key, Action<bool> callback)
        {
            if (!key2Data.TryGetValue(key, out ConditionDataWrapper wrapper))
            {
                return;
            }
            wrapper.Unsubscribe(callback);
            if (!wrapper.CheckNull()) return;
            RemoveData(wrapper);
        }
        /// 状态
        public bool Status(IConditionTypeGroup key)
        {
            if (!key2Data.TryGetValue(key, out ConditionDataWrapper wrapper))
            {
                AddData(key, out wrapper);
            }
            return wrapper.Status();
        }

        private void AddData(IConditionTypeGroup key, out ConditionDataWrapper wrapper)
        {
            wrapper = ConditionDataWrapperPool.Get(key);
            // 加入 id 映射
            key2Data.Add(key, wrapper);
            // 归入 条件类型检测字典
            List<EConditionType> conditionTypes = wrapper.EConditionTypes();
            for (int i = 0; i < conditionTypes.Count; i++)
            {
                EConditionType eConditionType = conditionTypes[i];
                if (!conditionType2Data.TryGetValue(eConditionType, out List<ConditionDataWrapper> wrappers))
                {
                    conditionType2Data.Add(eConditionType, wrappers = new List<ConditionDataWrapper>());
                }
                wrappers.Add(wrapper);
            }
            ListPool<EConditionType>.Release(conditionTypes);
        }
        private void RemoveData(ConditionDataWrapper wrapper)
        {
            // 从 条件类型字典 中删除
            List<EConditionType> conditionTypes = wrapper.EConditionTypes();
            for (int i = 0; i < conditionTypes.Count; i++)
            {
                EConditionType eConditionType = conditionTypes[i];
                List<ConditionDataWrapper> wrappers = conditionType2Data[eConditionType];
                for (int j = wrappers.Count - 1; j >= 0; j--)
                {
                    if(wrappers[j] != wrapper) continue;
                    wrappers.RemoveAt(j);
                    break;
                }
            }
            ListPool<EConditionType>.Release(conditionTypes);
            // 从 id 字典中 删除
            key2Data.Remove(wrapper.GetTypeGroup());
            ConditionDataWrapperPool.Release(wrapper);
        }
        
        private class ConditionDataWrapper
        {
            private IConditionTypeGroup typeGroup;
            private bool isDirty;
            private bool status;
            private Action<bool> callback;
        
            public void Init(IConditionTypeGroup list)
            {
                typeGroup = list;
                isDirty = true;
                status = false;
                callback = null;
            }
            public void Release()
            {
                callback = null;
                status = false;
                isDirty = true;
            }
            public List<EConditionType> EConditionTypes()
            {
                // TODO ZZZ 条件分类
                return ListPool<EConditionType>.Get();
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
                    // TODO ZZZ 检查条件
                    status = true;
                }
                return status;
            }
            // 检空
            public bool CheckNull()
            {
                return callback == null;
            }
            
            public IConditionTypeGroup GetTypeGroup()
            {
                return typeGroup;
            }
        }

        private class ConditionDataWrapperPool
        {
            private static readonly ObjectPool<ConditionDataWrapper> s_Pool = new ObjectPool<ConditionDataWrapper>(
                () => new ConditionDataWrapper(), 
                null, 
                x => x.Release());
            public static ConditionDataWrapper Get(IConditionTypeGroup key)
            {
                ConditionDataWrapper result = s_Pool.Get();
                result.Init(key);
                return result;
            }
            public static void Release(ConditionDataWrapper toRelease) => s_Pool.Release(toRelease);
        }
    }
}