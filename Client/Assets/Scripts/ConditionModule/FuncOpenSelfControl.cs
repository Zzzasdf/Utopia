using UnityEngine;

namespace ConditionModule
{
    [DisallowMultipleComponent]
    public class FuncOpenSelfControl : MonoBehaviour
    {
        [SerializeField] private EFuncOpenId eFuncOpenId;
        private void Awake()
        {
            GameEntry.Instance.ConditionManager.Subscribe(eFuncOpenId.GetCondition(), gameObject.SetActive);
        }
        private void OnDestroy()
        {
            GameEntry.Instance.ConditionManager.Unsubscribe(eFuncOpenId.GetCondition(), gameObject.SetActive);
        }
    }
}