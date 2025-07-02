using UnityEngine;
using UnityEngine.UI;
using Wrapper;
using Object = UnityEngine.Object;

public class TestAwake : MonoBehaviour
{
    private Text txt => UtilityWrapper.LazyGet(ref _txt, transform, "Txt"); 
    private Text _txt; // 必须要有字段保存数据，目前看起来无解
}

public class MonoWrapper<T>
    where T: Object
{
    private T t;

    public MonoWrapper(ref Vector2 v)
    {
        
    }
}