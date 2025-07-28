using System;
using System.Collections;
using UnityEngine;

public struct WaitForSecondsValue : IEnumerator
{
    private float _endTime;
    
    public WaitForSecondsValue(float seconds)
    {
        _endTime = Time.time + seconds;
    }
    
    public object Current => null;
    
    public bool MoveNext() => Time.time < _endTime;
    
    public void Reset() => throw new NotSupportedException();
}