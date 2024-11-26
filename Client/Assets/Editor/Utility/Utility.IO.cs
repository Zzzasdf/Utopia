using System;
using System.IO;
using UnityEngine;

namespace EditorModule
{
    public static partial class Utility
    {
        public static class IO
        {
            [Flags]
            public enum ECopyFolder
            {
                None = 0,
                /// 目标文件夹是否要覆盖
                OverrideTargetFolder = 1 << 0,
                /// 目标文件下文件是否要覆盖
                OverrideTargetFile = 1 << 2,
                /// 目标子文件夹是否要覆盖
                OverrideTargetSubFolder = 1 << 1,
                /// 目标子文件是否要覆盖
                OverrideTargetSubFile = 1 << 2,
                /// 覆盖所有
                OverrideAll = OverrideTargetFolder | OverrideTargetFile | OverrideTargetSubFolder | OverrideTargetSubFile,
            }
            
            /// 拷贝文件夹
            public static void CopyFolder(string sourceFolder, string targetFolder, ECopyFolder eCopyFolder)  
            {
                // 确保源目录存在  
                if (!Directory.Exists(sourceFolder))  
                {  
                    throw new DirectoryNotFoundException($"源文件夹不存在 => {sourceFolder}");  
                }
                // 文件夹
                {
                    if (Directory.Exists(targetFolder))
                    {
                        if (!eCopyFolder.HasFlag(ECopyFolder.OverrideTargetFolder))
                        {
                            Debug.LogError($"已存在目标文件夹 => {targetFolder}");
                            return;
                        }
                        Directory.Delete(targetFolder);
                        Directory.CreateDirectory(targetFolder);  
                    }
                    else
                    {
                        Directory.CreateDirectory(targetFolder);  
                    }
                }
                // 文件夹下文件
                {
                    FileInfo[] files = new DirectoryInfo(sourceFolder).GetFiles();  
                    foreach (FileInfo file in files)  
                    {  
                        string tempPath = Path.Combine(targetFolder, file.Name);
                        if (File.Exists(tempPath))
                        {
                            if (!eCopyFolder.HasFlag(ECopyFolder.OverrideTargetFile))
                            {
                                Debug.LogWarning($"已存在目标文件 => {tempPath}");
                                continue;
                            }
                        }
                        file.CopyTo(tempPath, true); 
                    }  
                }
                // 获取源目录中的子目录  
                DirectoryInfo[] dirs = new DirectoryInfo(sourceFolder).GetDirectories();  
                // 递归拷贝子目录  
                foreach (DirectoryInfo dir in dirs)  
                {  
                    string newDestinationSubDir = Path.Combine(targetFolder, dir.Name);  
                    CopySubFolder(dir.FullName, newDestinationSubDir, eCopyFolder);  
                }  
            }

            /// 拷贝子文件夹
            private static void CopySubFolder(string sourceFolder, string targetFolder, ECopyFolder eCopyFolder)
            {
                // 确保源目录存在  
                if (!Directory.Exists(sourceFolder))  
                {  
                    throw new DirectoryNotFoundException($"源文件夹不存在 => {sourceFolder}");  
                }
                // 文件夹
                {
                    if (Directory.Exists(targetFolder))
                    {
                        if (!eCopyFolder.HasFlag(ECopyFolder.OverrideTargetSubFolder))
                        {
                            Debug.LogWarning($"已存在目标文件夹 = > {targetFolder}");
                            return; 
                        }
                        Directory.Delete(targetFolder);
                        Directory.CreateDirectory(targetFolder); 
                    }
                    else
                    {
                        Directory.CreateDirectory(targetFolder);  
                    }
                }
                // 文件夹下文件
                {
                    FileInfo[] files = new DirectoryInfo(sourceFolder).GetFiles();  
                    foreach (FileInfo file in files)  
                    {  
                        string tempPath = Path.Combine(targetFolder, file.Name);
                        if (File.Exists(tempPath))
                        {
                            if (!eCopyFolder.HasFlag(ECopyFolder.OverrideTargetSubFile))
                            {
                                Debug.LogError($"已存在目标文件 => {tempPath}");
                                continue;
                            }
                        }
                        file.CopyTo(tempPath, true); 
                    }  
                }
                // 获取源目录中的子目录  
                DirectoryInfo[] dirs = new DirectoryInfo(sourceFolder).GetDirectories();  
                // 递归拷贝子目录  
                foreach (DirectoryInfo dir in dirs)  
                {  
                    string newDestinationSubDir = Path.Combine(targetFolder, dir.Name);  
                    CopySubFolder(dir.FullName, newDestinationSubDir, eCopyFolder);  
                }  
            }
        }
    }
}
