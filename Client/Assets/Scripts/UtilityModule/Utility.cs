using UnityEngine;

public class Utility
{
    /// 懒加载
    public static T LazyGet<T>(ref T source, in Transform parent, in string path)
        where T: Object
    {
        if (source == null)
        {
            source = parent.Find(path).GetComponent<T>();
        }
        return source;
    }
}
