using UnityEngine;

public class PerObjectMaterialProperties : MonoBehaviour
{
    private static int bastColorId = Shader.PropertyToID("_BaseColor");
    private static int cutoffId = Shader.PropertyToID("_Cutoff");
    private static int metallicId = Shader.PropertyToID("_Metallic");
    private static int smoothnessId = Shader.PropertyToID("_Smoothness");
    private static MaterialPropertyBlock block;

    [SerializeField] private Color baseColor = Color.white;
    [SerializeField, Range(0f, 1f)] private float cutoff = 0.5f;
    
    // 定义金属度和光滑度
    [SerializeField, Range(0f, 1f)] private float metallic = 0;
    [SerializeField, Range(0f, 1f)] private float smoothness = 0.5f;
    
    private void OnValidate()
    {
        if (block == null)
        {
            block = new MaterialPropertyBlock();
        }
        // 设置材质属性
        block.SetColor(bastColorId, baseColor);
        block.SetFloat(cutoffId, cutoff);
        block.SetFloat(metallicId, metallic);
        block.SetFloat(smoothnessId, smoothness);
        GetComponent<Renderer>().SetPropertyBlock(block);
    }

    private void Awake()
    {
        OnValidate();
    }
}
