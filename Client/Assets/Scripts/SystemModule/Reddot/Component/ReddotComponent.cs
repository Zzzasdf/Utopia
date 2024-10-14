using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace ReddotModule
{
    public interface IReddotComponent
    {
        void Update(bool status, params int[] indexs);
    }

    public class ReddotComponent : MonoBehaviour, IReddotComponent
    {
        [FormerlySerializedAs("reddot")] [SerializeField] [ReadOnly] private EReddot eReddot;
        private int[] indexs;

        private void Awake()
        {
            eReddot.AddComponent(this);
        }

        public void UpdateParam(int[] indexs)
        {
            this.indexs = indexs;
        }

        void IReddotComponent.Update(bool status, params int[] indexs)
        {
            if (!this.indexs.Equals(indexs))
            {
                return;
            }
            gameObject.SetActive(status);
        }
    }
}