// 公共方法库
#ifndef CUSTOM_COMMON_INCLUDED
#define CUSTOM_COMMON_INCLUDED

// 包含SRP核心库
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
#include "UnityInput.hlsl"

// 将自定义矩阵映射到Unity SRP预期的宏
#define UNITY_MATRIX_M      unity_ObjectToWorld
#define UNITY_MATRIX_I_M    unity_WorldToObject
#define UNITY_MATRIX_V      unity_MatrixV

#define UNITY_MATRIX_VP     unity_MatrixVP
#define UNITY_MATRIX_I_V	unity_MatrixIV
#define UNITY_MATRIX_P      glstate_matrix_projection
#define UNITY_PREV_MATRIX_M	unity_PrevObjectToWorld
#define UNITY_PREV_MATRIX_I_M	unity_PrevWorldToObject

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
// 包含空间变换函数（例如GetObjectToWorldMatrix）
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"

float Square(float v)
{
	return v * v;
}

#endif