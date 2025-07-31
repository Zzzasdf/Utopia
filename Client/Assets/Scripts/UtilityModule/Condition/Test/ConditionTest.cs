using ConditionModule;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ConditionTest : MonoBehaviour
{
    public class Condition: ICondition<EConditionType>
    {
        public void GetConditionTypes(in ICollection<EConditionType> types)
        {
            types.Add(EConditionType.Test1);
        }
        public bool CheckCondition()
        {
            return true;
        }
    }

    private Condition condition;
    private ConditionManager manager;

    [SerializeField][Header("迭代次数")] private int Iterations = 1_000_000; // 百万次迭代
    [SerializeField][Header("性能测试 KeyCode")] private KeyCode keyCode = KeyCode.Space;

    private void Awake()
    {
        manager = new ConditionManager();
        ((IInit)manager).OnInit();
        condition = new Condition();
        manager.Subscribe(condition, DebugLog);
    }
    private void DebugLog(bool isRight)
    {
        // Debug.LogError(isRight);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            Debug.Log("手动触发GC");
        }
        if (Input.GetKeyDown(keyCode))
        {
            PerformanceTest();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            UNormalTest();
        }
    }

    /// 性能测试
    private void PerformanceTest()
    {
        // long before = GC.GetTotalMemory(true);
        {
            for (int i = 0; i < Iterations; i++)
            {
                manager.Fire(EConditionType.Test1);
            }
        }
        // long after = GC.GetTotalMemory(true);
        // StringBuilder sb = new StringBuilder("PNormal");
        // sb.AppendLine($"GC分配总量: {after - before} bytes");
        // Debug.Log(sb.ToString());
    }

    
    private void UNormalTest()
    {
        UNoramlTestAsync().Forget();
    }
    private async UniTaskVoid UNoramlTestAsync()
    {
        // await UniTask.Delay(10);
    }
}
