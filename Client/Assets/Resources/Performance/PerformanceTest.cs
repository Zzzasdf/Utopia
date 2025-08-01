using System.Collections.Generic;
using UnityEngine;

public class PerformanceTest : MonoBehaviour
{
    [SerializeField][Header("迭代次数")] private int Iterations = 1_000_000; // 百万次迭代

    /// List 自带的 AddRange 不会产生新的枚举器
    private List<int> a = new List<int>();
    private List<int> b = new List<int>();

    /// Unity.VisualScripting.LinqUtility.AddRange 每次使用会产生一个新的枚举器
    private HashSet<int> c = new HashSet<int>();
    private HashSet<int> d = new HashSet<int>();
    private Dictionary<int, int> e = new Dictionary<int, int>();
    private Dictionary<int, int> f = new Dictionary<int, int>();
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            for (int i = 0; i < Iterations; i++)
            {
                a.Clear();
                b.Clear();
            
                b.Add(1);
                a.AddRange(b);
            }
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            for (int i = 0; i < Iterations; i++)
            {
                c.Clear();
                d.Clear();
            
                d.Add(1);
                // bad !! GC !!
                // Unity.VisualScripting.LinqUtility.AddRange
                // c.AddRange(d); 
                
                // good !! 0 GC
                // implement Extensions
                c.AddRange(d);
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            for (int i = 0; i < Iterations; i++)
            {
                e.Clear();
                f.Clear();
            
                f.Add(1, 1);
                // bad !! GC !!
                // Unity.VisualScripting.LinqUtility.AddRange
                // e.AddRange(f);
                
                // good !! 0 GC !! 
                // implement Extensions
                e.AddRange(f);
            }
        }
    }
}
