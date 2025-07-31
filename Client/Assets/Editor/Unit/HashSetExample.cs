using NPOI.Util.Collections;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

[TestFixture]
public class HashSetExample 
{
    [Test]
    public void Foo()
    {
        HashSet<int> a = new HashSet<int>()
        {
            1,2
        };
        HashSet<int> b = new HashSet<int>()
        {
            2,3
        };
        a.AddRange(b);
        foreach (var _a in a)
        {
            Debug.Log(_a);
        }
    }
}