using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SerializerUtility : MonoBehaviour
{
    private void Serialize(byte[] bytes)
    {
        using (var stream = new MemoryStream(bytes))
        {
            
        }
    }
}
