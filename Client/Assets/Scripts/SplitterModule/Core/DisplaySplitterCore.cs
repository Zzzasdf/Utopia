using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Splitter.Core
{
    /// 显示分离器：核心
    public partial class DisplaySplitterCore : IDisplaySplitterCore
    {
        private bool allowReleaseSelf;
        private Dictionary<int, List<GameObject>> _items;
        private Dictionary<int, List<GameObject>> items
        {
            get
            {
                if (_items == null)
                {
                    if (allowReleaseSelf) _items = DictionaryPool<int, List<GameObject>>.Get();
                    else _items = new Dictionary<int, List<GameObject>>();
                }

                return _items;
            }
        }
        private HashSet<int> _allowReleases;
        private HashSet<int> allowReleases => _allowReleases ??= HashSetPool<int>.Get();
        
        public static DisplaySplitterCore Get()
        {
            DisplaySplitterCore core = GenericPool<DisplaySplitterCore>.Get();
            core.allowReleaseSelf = true;
            return core;
        }
        public DisplaySplitterCore() : this(false) { }
        private DisplaySplitterCore(bool allowReleaseSelf) => this.allowReleaseSelf = allowReleaseSelf;

        void IDisplaySplitterCore.Release() => Release();
        private void Release()
        {
            if (_items != null)
            {
                if (_allowReleases != null)
                {
                    foreach (var pair in _items)
                    {
                        if (!_allowReleases.Contains(pair.Key))
                        {
                            continue;
                        }
                        ListPool<GameObject>.Release(pair.Value);
                    }
                    HashSetPool<int>.Release(_allowReleases);
                    _allowReleases = null;
                }
                if (allowReleaseSelf)
                {
                    DictionaryPool<int, List<GameObject>>.Release(_items);
                    _items = null;
                }
            }
            if (allowReleaseSelf) GenericPool<DisplaySplitterCore>.Release(this);
        }

        IDisplaySplitterCore IDisplaySplitterCore.Subscribe(int key, List<GameObject> items, bool allowRelease)
        {
            this.items.Add(key, items);
            if (allowRelease) allowReleases.Add(key);
            return this;
        }

        public void DisplayUnique(int key)
        {
            foreach (var pair in items)
            {
                bool isDisplay = pair.Key == key;
                foreach (var item in pair.Value)
                {
                    item.SetActive(isDisplay);
                }
            }
        }

        public void DisplaySet(List<int> keys, bool allowRelease = false)
        {
            foreach (var pair in items)
            {
                bool isDisplay = keys.Contains(pair.Key);
                foreach (var item in pair.Value)
                {
                    item.SetActive(isDisplay);
                }
            }
            if (allowRelease) ListPool<int>.Release(keys);
        }

        public void DisplayFlags(int keyFlags)
        {
            foreach (var pair in items)
            {
                bool isDisplay = (keyFlags & pair.Key) == pair.Key;
                foreach (var item in pair.Value)
                {
                    item.SetActive(isDisplay);
                }
            }
        }
    }
}