Shader "Custom RP/Unlit"
{
    Properties
    {
        _BaseColor("Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _BaseMap("Texture", 2D) = "white" {}
        // 透明度测试的阈值
        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        [Toggle(_CLIPPING)] _Clipping("Alpha Clipping", Float) = 0
        
        // 设置混合模式
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Src Blend", Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Dst Blend", Float) = 0
        // 默认写入深度缓冲区
        [Enum(Off, 0, On, 1)] _ZWrite("Z Write", Float) = 1
        // 投影模式
        [KeywordEnum(On, Clip, Dither, Off)] _Shadows("Shadows", FLoat) = 0
    }
    SubShader 
    {
        Pass
        {
            // 定义混合模式
            Blend[_SrcBlend][_DstBlend]
            // 是否写入深度
            ZWrite[_ZWrite]
            
            HLSLPROGRAM
            #pragma shader_feature _CLIPPING
            #pragma multi_compile_instancing
            #pragma vertex UnlitPassVertex
            #pragma fragment UnlitPassFragment
            #include "UnlitPass.hlsl"
            ENDHLSL
        }
        Pass
        {
            Tags
            {
                "LightMode" = "ShadowCaster"
            }
            ColorMask 0
            
            HLSLPROGRAM
            #pragma target 3.5
            // #pragma shader_feature _CLIPPING
            #pragma shader_feature _ _SHADOWS_CLIP _SHADOWS_DITHER
            #pragma multi_compile_instancing
            #pragma vertex ShadowCasterPassVertex
            #pragma fragment ShadowCasterPassFragment
            #include "ShadowCasterPass.hlsl"
            ENDHLSL
        }
    }

    CustomEditor "CustomShaderGUI"
}
