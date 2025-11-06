using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Lighting 
{
    // 限制最大可见平行关数量为 4
    private const int maxDirLightCount = 4;
    
    private static int dirLightCountId = Shader.PropertyToID("_DirectionalLightCount");
    private static int dirLightColorsId = Shader.PropertyToID("_DirectionalLightColors");
    private static int dirLightDirectionsId = Shader.PropertyToID("_DirectionalLightDirections");
    
    // 存储可见光的颜色和方向
    private Vector4[] dirLightColors = new Vector4[maxDirLightCount];
    private Vector4[] dirLightDirections = new Vector4[maxDirLightCount];
    
    private const string bufferName = "Lighting";
    
    CommandBuffer buffer = new CommandBuffer
    {
        name = bufferName,
    };
    
    // 存储相机踢除后的结果
    private CullingResults cullingResults;

    public void SetUp(ScriptableRenderContext context, CullingResults cullingResults)
    {
        this.cullingResults = cullingResults;
        buffer.BeginSample(bufferName);
        // // 发送光源数据
        // SetupDirectionalLight();
        SetupLights();
        buffer.EndSample(bufferName);
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }
    
    // 发送多个光源数据
    private void SetupLights()
    {
        // 得到所有可见光
        NativeArray<VisibleLight> visibleLights = cullingResults.visibleLights;
        int dirLightCount = 0;
        for (int i = 0; i < visibleLights.Length; i++)
        {
            VisibleLight visibleLight = visibleLights[i];
            // 如果是方向光，我们才进行数据存储
            if (visibleLight.lightType == LightType.Directional)
            {
                // VisibleLight 结构很大，我们改为传递引用不是传递值，这样不会生成副本
                SetupDirectionalLight(dirLightCount++, ref visibleLight);
            }
            // 当超过灯光限制数量中止循环
            if (dirLightCount >= maxDirLightCount)
            {
                break;
            }
        }
        buffer.SetGlobalInt(dirLightCountId, dirLightCount);
        buffer.SetGlobalVectorArray(dirLightColorsId, dirLightColors);
        buffer.SetGlobalVectorArray(dirLightDirectionsId, dirLightDirections);
    }
    
    /// 将可见光的光照颜色和方向存储到数组
    private void SetupDirectionalLight(int index, ref VisibleLight visibleLight)
    {
        dirLightColors[index] = visibleLight.finalColor;
        dirLightDirections[index] = -visibleLight.localToWorldMatrix.GetColumn(2);
    }
}
