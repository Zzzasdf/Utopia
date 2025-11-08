// 灯光数据相关库
#ifndef CUSTOM_LIGHT_INCLUDED
#define CUSTOM_LIGHT_INCLUDED

#define MAX_DIRECTIONAL_LIGHT_COUNT 4

// 灯光的属性
struct Light
{
	float3 color;
	float3 direction;
	float attenuation;
};

// 方向光的数据
CBUFFER_START(_CustomLight)
	int _DirectionalLightCount;
	float4 _DirectionalLightColors[MAX_DIRECTIONAL_LIGHT_COUNT];
	float4 _DirectionalLightDirections[MAX_DIRECTIONAL_LIGHT_COUNT];
	// 阴影数据
	float4 _DirectionalLightShadowData[MAX_DIRECTIONAL_LIGHT_COUNT];
CBUFFER_END

// 获取方向光的数量
int GetDirectionalLightCount()
{
	return _DirectionalLightCount;
}

// 获取平行光阴影数据
DirectionalShadowData GetDirectionalShadowData(int lightIndex, ShadowData shadowData)
{
	DirectionalShadowData data;
	data.strength = _DirectionalLightShadowData[lightIndex].x * shadowData.strength;
	data.tileIndex = _DirectionalLightShadowData[lightIndex].y + shadowData.cascadeIndex;
	// 获取灯光的法线偏差值
	data.normalBias = _DirectionalLightShadowData[lightIndex].z;
	return data;
}

// 获取目标索引平行光的属性
Light GetDirectionalLight(int index, Surface surfaceWS, ShadowData shadowData)
{
	Light light;
	light.color = _DirectionalLightColors[index].rgb;
	light.direction = _DirectionalLightDirections[index].xyz;
	// 得到阴影数据
	DirectionalShadowData dirShadowData = GetDirectionalShadowData(index, shadowData);
	// 得到阴影衰减
	light.attenuation = GetDirectionalShadowAttenuation(dirShadowData, shadowData, surfaceWS);
	return light;
}

#endif