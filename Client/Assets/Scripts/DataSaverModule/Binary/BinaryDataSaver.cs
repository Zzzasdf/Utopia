using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Google.Protobuf.Collections;
using UnityEngine;

namespace DataSaver
{
    public class BinaryDataSaver : MonoBehaviour
    {
        private static string SaveDirectoryPath = Path.Combine(Application.persistentDataPath, nameof(BinaryDataSaver)); 
        
        public enum EFile
        {
            ClientConverterData = 1,
        }

        private static Dictionary<EFile, Type> pathMap = new Dictionary<EFile, Type>
        {
            [EFile.ClientConverterData] = typeof(RepeatedField<RepeatedField<long>>),
        };

        public static bool TrySaveData<T>(EFile eFile, T data)
        {
            if (!TryReadWrite<T>(eFile))
            {
                return false;
            }
            if (!Directory.Exists(SaveDirectoryPath))
            {
                Directory.CreateDirectory(SaveDirectoryPath);
            }
            string filePath = Path.Combine(SaveDirectoryPath, eFile.ToString());
            if (data == null)
            {
                File.Delete(filePath);
                return true;
            }
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(Path.Combine(SaveDirectoryPath, eFile.ToString()), FileMode.Create, FileAccess.Write, FileShare.None);
            try
            {
                formatter.Serialize(stream, data);
            }
            catch (SerializationException e)
            {
                Debug.LogError("序列化失败: " + e.Message);
            }
            finally
            {
                stream.Close();
            }
            return true;
        }
        
        public static bool TryLoadData<T>(EFile eFile, out T data)
        {
            if (!TryReadWrite<T>(eFile))
            {
                data = default;
                return false;
            }
            string filePath = Path.Combine(SaveDirectoryPath, eFile.ToString());
            if (!File.Exists(filePath))
            {
                data = default;
                return false;
            }
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = null;
            try
            {
                stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                data = (T)formatter.Deserialize(stream);
                return true;
            }
            catch (SerializationException e)
            {
                Debug.LogError("反序列化失败: " + e.Message);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
            data = default;
            return false;
        }

        private static bool TryReadWrite<T>(EFile eFile)
        {
            if (!pathMap.TryGetValue(eFile, out var value))
            {
                Debug.LogError($"该类型 未配置 ！！ => {eFile}");
                return false;
            }
            if (typeof(T) != value)
            {
                Debug.LogError($"数据类型 与 配置类型 不一致 ！！ EFile => {eFile}, Data => {typeof(T)}");
                return false;
            }
            return true;
        }
    }
}
