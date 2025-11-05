using UnityEngine;
using UnityEngine.Rendering;

public partial class CameraRender 
{
    private static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");
    
    private const string bufferName = "Render Camera";
    private CommandBuffer buffer = new CommandBuffer
    {
        name = bufferName,
    };
    
    private ScriptableRenderContext context;

    private Camera camera;

    public void Render(ScriptableRenderContext context, Camera camera)
    {
        this.context = context;
        this.camera = camera;
        PrepareBuffer();
        // 在 Game 视图绘制的几何体也绘制在 Scene 视图中
        PrepareForSceneWindow();
        
        if (!Cull())
        {
            return;
        }

        Setup();
        // 绘制几何体
        DrawVisibleGeometry();
        // 绘制 SRP 不支持的着色器类型
        DrawUnsupportedShaders();
        // 绘制 Gizmos
        DrawGizmos();
        Submit();
    }

    /// 存储剔除后的结果数据
    private CullingResults cullingResults;

    /// 剔除
    private bool Cull()
    {
        ScriptableCullingParameters p;
        if (camera.TryGetCullingParameters(out p))
        {
            cullingResults = context.Cull(ref p);
            return true;
        }
        return false;
    }
    
    /// 设置相机的属性和矩阵
    private void Setup()
    {
        context.SetupCameraProperties(camera);
        // 得到相机的 clear flags
        CameraClearFlags flags = camera.clearFlags;
        // 设置相机清除状态
        buffer.ClearRenderTarget(flags <= CameraClearFlags.Depth, flags == CameraClearFlags.Color,
            flags == CameraClearFlags.Color ? camera.backgroundColor.linear : Color.clear);
        buffer.BeginSample(SampleName);
        ExecuteBuffer();
    }
    
    /// 绘制几何体
    private void DrawVisibleGeometry()
    {
        // 设置绘制顺序和指定渲染相机
        var sortingSettings = new SortingSettings(camera)
        {
            criteria = SortingCriteria.CommonOpaque,
        };
        // 设置渲染的 Shader Pass 和排序模式
        var drawingSettings = new DrawingSettings(unlitShaderTagId, sortingSettings);
        // 只绘制 RenderQueue 为 opaque 不透明的物体
        var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
        // 1、绘制不透明物体
        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
        
        // 2、绘制天空盒
        context.DrawSkybox(camera);

        sortingSettings.criteria = SortingCriteria.CommonTransparent;
        drawingSettings.sortingSettings = sortingSettings;
        // 只绘制 RenderQueue 为 transparent 透明的物体
        filteringSettings.renderQueueRange = RenderQueueRange.transparent;
        // 3、绘制透明物体
        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
    }

    private partial void PrepareBuffer();
    private partial void PrepareForSceneWindow();
    private partial void DrawUnsupportedShaders();
    private partial void DrawGizmos();
    
    /// 提交缓冲区渲染命令
    private void Submit()
    {
        buffer.EndSample(SampleName);
        ExecuteBuffer();
        context.Submit();
    }

    private void ExecuteBuffer()
    {
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }
}
