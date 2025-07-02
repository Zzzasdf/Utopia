using System.Collections.Generic;
using Splitter.Core;
using UnityEngine;
using UnityEngine.Pool;

namespace Splitter.Mono
{
    /// 子节点 的 显示分离器
    public class DisplayChildSplitter : DisplaySplitterMonoBase
    {
        private DisplaySplitterCore _core;
        public override DisplaySplitterCore Core
        {
            get
            {
                if (_core == null)
                {
                    _core = DisplaySplitterCore.Get();
                    Dictionary<int, List<GameObject>> items = DictionaryPool<int, List<GameObject>>.Get();
                    HashSet<int> allowReleases = HashSetPool<int>.Get();
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        int key = flags ? 1 << i : i;
                        GameObject item = transform.GetChild(i).gameObject;
                        List<GameObject> list = ListPool<GameObject>.Get();
                        list.Add(item);
                        items.Add(key, list);
                        allowReleases.Add(key);
                    }
                    ((IDisplaySplitterCoreMonoExtensions)_core).Subscribe((items, allowReleases));
                }
                return _core;
            }
        }

        private void OnDestroy()
        {
            ((IDisplaySplitterCoreMonoExtensions)_core)?.Release();
        }

        public static implicit operator DisplaySplitterCore(DisplayChildSplitter splitter) => splitter.Core;
    }
}