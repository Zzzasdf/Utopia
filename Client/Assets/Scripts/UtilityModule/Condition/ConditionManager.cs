using System;
using System.Collections.Generic;
using UnityEngine;

namespace ConditionModule
{
    public interface ICondition<T>
        where T: struct
    {
        void GetConditionTypes(in ICollection<T> types);
        bool CheckCondition();
    }
    
    public partial class ConditionManager: IManager
    {
        /// 条件响应器
        private IConditionResponder<EConditionType> commonResponder;
        
        void IInit.OnInit()
        {
            commonResponder = new ConditionResponder<EConditionType>();
        }
        void IReset.OnReset()
        {
            commonResponder.Reset();
        }
        void IDestroy.OnDestroy()
        {
            commonResponder.Destroy();
            commonResponder = null;
        }
        
        public void Subscribe(ICondition<EConditionType> types, Action<bool> callback) => commonResponder.Subscribe(types, callback);
        public void Unsubscribe(ICondition<EConditionType> types, Action<bool> callback) => commonResponder.Unsubscribe(types, callback);
        public bool Status(ICondition<EConditionType> types) => commonResponder.Status(types);
        
        public void Fire(EConditionType eConditionType) => commonResponder.Fire(eConditionType);
        public void Fire() => commonResponder.Fire();
        
        public void FireNow(EConditionType eConditionType) => commonResponder.FireNow(eConditionType);
        public void FireNow() => commonResponder.FireNow();

        public void GetConditionTypes(in ICollection<EConditionType> types, IList<uint> list)
        {
            int index = 0;
            while (index < list.Count)
            {
                int addCount = (int)list[index];
                if (list.Count <= index + addCount * 2)
                {
                    Debug.LogError($"条件 {list}，配置有问题");
                    return;
                }
                for (int i = index + 1; i < index + addCount * 2; i += 2)
                {
                    // 只取第一个类型
                    types.Add((EConditionType)list[i]);
                }
                index += 1 + addCount * 2;
            }
        }
        public bool CheckCondition(IList<uint> list)
        {
            // TODO ZZZ
            return true;
        }
    }
    
    public enum EConditionType
    {
        Test1 = 1,
        Test2 = 2,
        Test3 = 3,
    }
    
    public enum EFuncOpenId
    {
        Test1 = 1,
        Test2 = 2,
    }
    public static class EFuncOpenIdExtensions
    {
        public static ICondition<EConditionType> GetCondition(this EFuncOpenId eFuncOpenId)
        {
            // TODO ZZZ 返回一个已存在的常驻条件
            throw new NotImplementedException();
        }
    }
}