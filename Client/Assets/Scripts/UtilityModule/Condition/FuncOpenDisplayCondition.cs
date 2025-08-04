using UnityEngine;

namespace ConditionModule
{
    [DisallowMultipleComponent]
    public class FuncOpenDisplayCondition : DisplayConditionBase<FuncOpenDisplayCondition>
    {
        [SerializeField] private EFuncOpenId eFuncOpenId;
        private void Start()
        {
            AppendSubscribe(eFuncOpenId.GetCondition());
        }
    }
}