using System.Collections.Generic;
using UnityEngine;

/// 补丁版本
public class PatchVersion
{
    /// 修复模块定义 eg!! 请勿修改值
    public enum EPatchModule
    {
        /// 拾取
        PickUp = 0,
    }

    /// 当前的角色 Id
    private static long RoleId => 7777777;

    /// <summary>
    /// 尝试注入补丁，修复版本不可变，起始为 1，后续递增
    /// </summary>
    /// <param name="ePatchModule">修复的模块</param>
    /// <param name="patchVersion">已修复的最低版本若低于此版本，触发修复</param>
    /// <returns></returns>
    public static bool TryInjectPatch(EPatchModule ePatchModule, int patchVersion)
    {
        string key = $"{RoleId}_{(int)ePatchModule}";
        // 已修复的最低版本
        int fixedLowestVersion = PlayerPrefs.GetInt(key, 0);
        if (patchVersion <= fixedLowestVersion)
        {
            // 已修复过，不在修复
            return false;
        }
        PlayerPrefs.SetInt(key, patchVersion);
        return true;
    }
}

// 补丁代码使用 partial 的 方式 写在下面
// 补丁代码 请勿轻易修改
public partial class Test
{
    /// 补丁：修复拾取数据
    private void PatchPickUp()
    {
        // 修复时间：2025-1-8
        // 拾取 数据 key 重复，只保留一个
        // 在服务器下发消息后，立即修复，并发送一份修改后的数据给服务器
        if (PatchVersion.TryInjectPatch(PatchVersion.EPatchModule.PickUp, 1))
        {
            List<long> fixedData = new List<long>()
            {
                (long)1 << 32 | 1 << 0, 
                (long)1 << 32 | 1 << 1, 
                (long)1 << 32 | 1 << 2, 
            };
            for (int i = 0; i < fixedData.Count; i++)
            {
                Debug.LogWarning(fixedData[i]);
            }
            Dictionary<int, int> temp = new Dictionary<int, int>();
            for (int i = 0; i < fixedData.Count; i++)
            {
                (int high, int low) = LongConvertDoubleInt(fixedData[i]);
                // 只留一个
                temp.TryAdd(high, low);
            }
            fixedData.Clear();
            foreach (var pair in temp)
            {
                fixedData.Add(DoubleIntCombineLong(pair.Key, pair.Value));
            }
            for (int i = 0; i < fixedData.Count; i++)
            {
                Debug.Log(fixedData[i]);
            }
            // TODO 发送给后台保存一次，避免玩家在本次游戏中没保存上，或闪退等情况
        }
        // // 二次修复 示例
        // // 修复时间：2025-1-9
        // // ...
        // if (PatchVersion.TryInjectPatch(PatchVersion.EPatchModule.PickUp, 2))
        // {
        //     // TODO Method Body ...  
        // }
    }
    
    private (int high, int low) LongConvertDoubleInt(long l)
    {
        return ((int)(l >> 32), (int)(l & 0xFFFFFFFF));
    }
    
    private long DoubleIntCombineLong(int high, int low)
    {
        return ((long)high << 32) | (low & 0xFFFFFFFFL);
    }
}