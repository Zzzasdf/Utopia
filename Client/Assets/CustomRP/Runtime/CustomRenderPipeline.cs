using UnityEngine;
using UnityEngine.Rendering;

public class CustomRenderPipeline : RenderPipeline 
{
    private CameraRender renderer = new CameraRender();
    private bool useDynamicBatching, useGPUInstancing;

    private ShadowSettings shadowSettings;
    
    /// 测试 SRP 合批启用
    public CustomRenderPipeline(bool useDynamicBatching, bool useGPUInstancing, bool useSRPBather, ShadowSettings shadowSettings)
    {
        this.shadowSettings = shadowSettings;
        this.useDynamicBatching = useDynamicBatching;
        this.useGPUInstancing = useGPUInstancing;
        GraphicsSettings.useScriptableRenderPipelineBatching = useSRPBather;
        // 灯光使用线性强度
        GraphicsSettings.lightsUseLinearIntensity = true;
    }
    
    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        foreach (Camera camera in cameras)
        {
            renderer.Render(context, camera, useDynamicBatching, useGPUInstancing, shadowSettings);
        }        
    }
}
