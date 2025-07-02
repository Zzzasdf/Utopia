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
    public class ConditionResponder<TList>: IConditionResponder
        where TList: List<uint>
    {
        /// id 映射 配置
        private Dictionary<TList, ConditionDataWrapper<TList>> key2Data;
        /// 条件类型 映射 配置集 （检测）
        private Dictionary<EConditionType, List<ConditionDataWrapper<TList>>> conditionType2Data;
        
        public ConditionResponder()
        {
            key2Data = new Dictionary<TList, ConditionDataWrapper<TList>>();
            conditionType2Data = new Dictionary<EConditionType, List<ConditionDataWrapper<TList>>>();
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
                ConditionDataWrapper<TList> wrapper = pair.Value;
                wrapper.Fire();
            }
        }
        public void Fire(EConditionType eConditionType)
        {
            if (!conditionType2Data.TryGetValue(eConditionType, out List<ConditionDataWrapper<TList>> wrappers))
            {
                return;
            }
            foreach (var wrapper in wrappers)
            {
                wrapper.Fire();
            }
        }

        // 订阅
        public void Subscribe(TList key, Action<bool> callback)
        {
            if (!key2Data.TryGetValue(key, out ConditionDataWrapper<TList> wrapper))
            {
                AddData(key, out wrapper);
            }
            wrapper.Subscribe(callback);
            // 订阅时触发一次检测
            callback.Invoke(wrapper.Status());
        }
        /// 取消订阅
        public void Unsubscribe(TList key, Action<bool> callback)
        {
            if (!key2Data.TryGetValue(key, out ConditionDataWrapper<TList> wrapper))
            {
                return;
            }
            wrapper.Unsubscribe(callback);
            if (!wrapper.CheckNull()) return;
            RemoveData(wrapper);
        }
        /// 状态
        public bool Status(TList key)
        {
            if (!key2Data.TryGetValue(key, out ConditionDataWrapper<TList> wrapper))
            {
                AddData(key, out wrapper);
            }
            return wrapper.Status();
        }

        private void AddData(TList key, out ConditionDataWrapper<TList> wrapper)
        {
            wrapper = ConditionDataWrapperPool<TList>.Get(key);
            // 加入 id 映射
            key2Data.Add(key, wrapper);
            // 归入 条件类型检测字典
            List<EConditionType> conditionTypes = wrapper.EConditionTypes();
            for (int i = 0; i < conditionTypes.Count; i++)
            {
                EConditionType eConditionType = conditionTypes[i];
                if (!conditionType2Data.TryGetValue(eConditionType, out List<ConditionDataWrapper<TList>> wrappers))
                {
                    conditionType2Data.Add(eConditionType, wrappers = new List<ConditionDataWrapper<TList>>());
                }
                wrappers.Add(wrapper);
            }
            ListPool<EConditionType>.Release(conditionTypes);
        }
        private void RemoveData(ConditionDataWrapper<TList> wrapper)
        {
            // 从 条件类型字典 中删除
            List<EConditionType> conditionTypes = wrapper.EConditionTypes();
            for (int i = 0; i < conditionTypes.Count; i++)
            {
                EConditionType eConditionType = conditionTypes[i];
                List<ConditionDataWrapper<TList>> wrappers = conditionType2Data[eConditionType];
                for (int j = wrappers.Count - 1; j >= 0; j--)
                {
                    if(wrappers[j] != wrapper) continue;
                    wrappers.RemoveAt(j);
                    break;
                }
            }
            ListPool<EConditionType>.Release(conditionTypes);
            // 从 id 字典中 删除
            key2Data.Remove(wrapper);
            ConditionDataWrapperPool<TList>.Release(wrapper);
        }
        
        private class ConditionDataWrapper<TList>
            where TList: IList<uint>
        {
            private TList list;
            private bool isDirty;
            private bool status;
            private Action<bool> callback;
        
            public void Init(TList list)
            {
                this.list = list;
                isDirty = true;
                status = false;
                callback = null;
            }
            public void Release()
            {
                callback = null;
                status = false;
                isDirty = true;
                list.Clear();
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
            
            public static implicit operator TList(ConditionDataWrapper<TList> wrapper)
            {
                return wrapper.list;
            }
        }

        private class ConditionDataWrapperPool<TList>
            where TList: IList<uint>
        {
            private static readonly ObjectPool<ConditionDataWrapper<TList>> s_Pool = new ObjectPool<ConditionDataWrapper<TList>>(
                () => new ConditionDataWrapper<TList>(), 
                null, 
                x => x.Release());
            public static ConditionDataWrapper<TList> Get(TList key)
            {
                ConditionDataWrapper<TList> result = s_Pool.Get();
                result.Init(key);
                return result;
            }
            public static void Release(ConditionDataWrapper<TList> toRelease) => s_Pool.Release(toRelease);
        }
    }
}