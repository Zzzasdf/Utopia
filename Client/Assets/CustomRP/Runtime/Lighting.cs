using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using Vector2 = System.Numerics.Vector2;

public class Lighting 
{
    // 限制最大可见平行关数量为 4
    private const int maxDirLightCount = 4;
    
    private static int dirLightCountId = Shader.PropertyToID("_DirectionalLightCount");
    private static int dirLightColorsId = Shader.PropertyToID("_DirectionalLightColors");
    private static int dirLightDirectionsId = Shader.PropertyToID("_DirectionalLightDirections");
    private static int dirLightShadowDataId = Shader.PropertyToID("_DirectionalLightShadowData");

    // 存储可见光的颜色和方向
    private Vector4[] dirLightColors = new Vector4[maxDirLightCount];
    private Vector4[] dirLightDirections = new Vector4[maxDirLightCount];
    
    // 存储阴影数据
    private static Vector4[] dirLightShadowData = new Vector4[maxDirLightCount];

    private const string bufferName = "Lighting";
    
    CommandBuffer buffer = new CommandBuffer
    {
        name = bufferName,
    };
    
    // 存储相机踢除后的结果
    private CullingResults cullingResults;
    private Shadows shadows = new Shadows();
    
    public void SetUp(ScriptableRenderContext context, CullingResults cullingResults, ShadowSettings shadowSettings)
    {
        this.cullingResults = cullingResults;
        buffer.BeginSample(bufferName);
        // 传递阴影数据
        shadows.SetUp(context, cullingResults, shadowSettings);
        // 发送光源数据
        SetupLights();
        shadows.Render();
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
        buffer.SetGlobalVectorArray(dirLightShadowDataId, dirLightShadowData);
    }
    
    /// 将可见光的光照颜色和方向存储到数组
    private void SetupDirectionalLight(int index, ref VisibleLight visibleLight)
    {
        dirLightColors[index] = visibleLight.finalColor;
        dirLightDirections[index] = -visibleLight.localToWorldMatrix.GetColumn(2);
        // 存储阴影数据
        dirLightShadowData[index] = shadows.ReserveDirectionalShadows(visibleLight.light, index);
    }

    /// 释放阴影贴图 RT 内存
    public void Cleanup()
    {
        shadows.Cleanup();
    }
}
