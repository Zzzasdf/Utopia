using System;
using System.Collections.Generic;
using Splitter.Core;
using UnityEngine;
using UnityEngine.Pool;

namespace Splitter.Mono
{
    /// 显示分离器
    public class DisplaySplitter : DisplaySplitterMonoBase
    {
        [SerializeField] public List<Group> groups;
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
                    for (int i = 0; i < groups.Count; i++)
                    {
                        int key = flags ? 1 << i : i;
                        List<GameObject> group = groups[i].Items;
                        List<GameObject> list = ListPool<GameObject>.Get();
                        list.AddRange(group);
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

        public static implicit operator DisplaySplitterCore(DisplaySplitter splitter) => splitter.Core;

        [Serializable]
        public class Group
        {
            public List<GameObject> Items;
        }
    }
}