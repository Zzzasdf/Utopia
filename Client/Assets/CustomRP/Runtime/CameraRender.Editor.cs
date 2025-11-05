using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
public partial class CameraRender 
{
#if UNITY_EDITOR
    // SRP 不支持的着色器标签类型
    private static ShaderTagId[] legacyShaderTagIds =
    {
        new ShaderTagId("Always"),
        new ShaderTagId("ForwardBase"),
        new ShaderTagId("PrepassBase"),
        new ShaderTagId("Vertex"),
        new ShaderTagId("VertexLMRGBM"),
        new ShaderTagId("VertexLM"),
    };
    /// 绘制成使用错误材质的粉红颜色
    private static Material errorMaterial;

    /// 在 Game 视图绘制的几何体也绘制在 Scene 视图中
    private partial void PrepareForSceneWindow()
    {
        if (camera.cameraType == CameraType.SceneView)
        {
            // 如果切换到了 Scene 视图，调用此方法完成绘制
            ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
        }
    }
    
    /// 绘制 SRP 不支持的着色器类型
    private partial void DrawUnsupportedShaders()
    {
        if (legacyShaderTagIds.Length == 0) return;

        // 不支持的 ShaderTag 类型我们使用错误材质专用 Shader 来渲染（粉红颜色）
        if (errorMaterial == null)
        {
            errorMaterial = new Material(Shader.Find("Hidden/InternalErrorShader"));
        }
        
        // 数组第一个元素用来构造 DrawingSettings 对象的时候设置
        var drawingSettings = new DrawingSettings(legacyShaderTagIds[0], new SortingSettings(camera))
        {
            overrideMaterial = errorMaterial,
        };
        for (int i = 1; i < legacyShaderTagIds.Length; i++)
        {
            // 遍历数组逐个设置着色器的 PassName，从 i = 1 开始
            drawingSettings.SetShaderPassName(i, legacyShaderTagIds[i]);
        }
        // 使用默认设置即可，反正画出来的都是不支持的
        var filteringSettings = FilteringSettings.defaultValue;
        // 绘制不支持的 ShaderTag 类型的物体
        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
    }

    /// 绘制 DrawGizmos
    private partial void DrawGizmos()
    {
        if (Handles.ShouldRenderGizmos())
        {
            context.DrawGizmos(camera, GizmoSubset.PreImageEffects);
            context.DrawGizmos(camera, GizmoSubset.PostImageEffects);
        }
    }

    private string SampleName { get; set; }
    private partial void PrepareBuffer()
    {
        // 设置一下只有在编辑器模式下才分配内存
        Profiler.BeginSample("Editor Only");
        buffer.name = SampleName = camera.name;
        Profiler.EndSample();
    }
#else
    private const string SampleName = bufferName;
#endif
}
