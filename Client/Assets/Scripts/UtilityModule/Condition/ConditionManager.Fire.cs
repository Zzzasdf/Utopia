using UnityEngine;

namespace ConditionModule
{
    public partial class ConditionManager : IUpdate // TODO ZZZ Test
    {
        private static bool result;
        
        public void OnUpdate(float deltaTime)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                result = !result;
                GameEntry.ConditionManager.Fire(EConditionType.Test1);
                GameEntry.ConditionManager.Fire(EConditionType.Test2);
                GameEntry.ConditionManager.Fire(EConditionType.Test3);
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                GameEntry.ConditionManager.Fire(EConditionType.Test1);
                GameEntry.ConditionManager.Fire(EConditionType.Test2);
                GameEntry.ConditionManager.Fire(EConditionType.Test3);
            }
        }
    }
}
