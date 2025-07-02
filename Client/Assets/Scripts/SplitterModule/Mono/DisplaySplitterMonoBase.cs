using System.Collections.Generic;
using Splitter.Core;
using UnityEngine;

namespace Splitter.Mono
{
    /// 显示分离器：核心 Mono 形态 额外扩展
    public interface IDisplaySplitterCoreMonoExtensions
    {
        /// 自动回收
        void Release();
        /// 自动订阅
        void Subscribe((Dictionary<int, List<GameObject>> items, HashSet<int> allowReleases) model);
    }

    /// 显示分离器：核心 Mono 形态 基类
    public abstract class DisplaySplitterMonoBase : MonoBehaviour
    {
        [SerializeField] protected bool flags;
        public bool Flags => flags;
        public abstract DisplaySplitterCore Core { get; }
    }
}

namespace Splitter.Core
{
    public partial class DisplaySplitterCore : Mono.IDisplaySplitterCoreMonoExtensions
    {
        void Mono.IDisplaySplitterCoreMonoExtensions.Release() => Release();

        void Mono.IDisplaySplitterCoreMonoExtensions.Subscribe((Dictionary<int, List<GameObject>> items, HashSet<int> allowReleases) model)
        {
            (_items, _allowReleases) = model;
        }
    }
}