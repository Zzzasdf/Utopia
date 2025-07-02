using System;
using UnityEngine;

public class PooledListTest1 : MonoBehaviour
{
    private PooledList<int> T;

    private void Awake()
    {
        T = PooledList<int>.Get();
    }
    
    private void OnDestroy()
    {
        ((IDisposable)T).Dispose();
    }
}
