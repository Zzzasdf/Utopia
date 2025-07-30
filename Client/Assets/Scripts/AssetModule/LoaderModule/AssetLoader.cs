using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AssetLoader<TObject> : MonoBehaviour
{
    private bool _isActivePlayLoop; // 是否激活了 Unity 的循环系统

    private AsyncOperationHandle<TObject> _handle;
    private string _resName; // 当前使用中的资源名
    private bool _completedCalled; // 资源是否被 Completed回调 处理过

    public void Load(string resName)
    {
        if (!_isActivePlayLoop)
        {
            Debug.LogError("未先激活 Unity 的循环系统！！");
            return;
        }
        if (string.IsNullOrEmpty(resName))
        {
            // 资源名为空，不加载，直接隐藏
            Hide();
            return;
        }
        // 新资源 加载
        if (resName != _resName)
        {
            _completedCalled = false;
            _resName = resName;
            LoadBuiltIn(resName);
            return;
        }
        // 与上次加载的资源相同，状态处理
        if (!_completedCalled)
        {
            if (_handle.IsValid() && _handle.IsDone) 
            {
                // 未处理 Completed 回调，但资源已加载完成
                // 说明 Completed 回调被取消了，直接赋值即可
                OnCompleted(_handle);
            }
            // 未处理 Completed 回调，且资源也未加载完成
            // 说明 Completed 还没触发，等待回调即可
            return;
        }
        // 都处理过，直接打开
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        // 取消完成回调，存在两种状态，一种执行过 Completed，一种 未执行过 Completed
        if (_handle.IsValid())
        {
            _handle.Completed -= OnCompleted;
        }
        gameObject.SetActive(false);
    }

    private void LoadBuiltIn(string resName)
    {
        gameObject.SetActive(false);
        UnLoadBuiltIn();
        _handle = Addressables.LoadAssetAsync<TObject>(resName);
        if (_handle.IsValid())
        {
            _handle.Completed += OnCompleted;
        }
        else
        {
            Debug.LogError($"无效资源名 => { resName }, 类型 => { typeof(TObject).Name }");
        }
    }
    private void UnLoadBuiltIn()
    {
        if (_handle.IsValid())
        {
            _handle.Completed -= OnCompleted;
            Addressables.Release(_handle);
        }
    }
    private void OnCompleted(AsyncOperationHandle<TObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.None
            || handle.Status == AsyncOperationStatus.Failed)
        {
            Debug.LogError($"资源加载失败！！ => {handle}");
            return;
        }
        // TODO ZZZ Something
        
        gameObject.SetActive(true);
        _completedCalled = true;
    }
    
    private void Awake()
    {
        _isActivePlayLoop = true;
    }
    private void OnDestroy()
    {
        UnLoadBuiltIn();
    }
}
