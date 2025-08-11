using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor 
{
    public partial class StateMachineEditorWindow
    {
        /// 绘制所有状态节点
        private void DrawStateNodes()
        {
            if (stateMachineData == null) return;
            foreach (var state in stateMachineData.states)
            {
                DrawStateNode(state);
            }
        }
        /// 绘制状态节点
        private void DrawStateNode(StateNode state)
        {
            GUIStyle style = new GUIStyle(GUI.skin.box);
            Color bgColor = state.isEntryState ? new Color(100 / 255f, 100 / 255f, 160 / 255f) : Color.gray;
            style.normal.background = MakeTex(2, 2, bgColor);
            GUIContent guiContent = new GUIContent(state.name);
            
            if(state.eNode == ENode.Branch)
            {
                float scaleAdapte = 1.5f;
                Vector2 center = state.rect.center;
                Vector3[] diamondPoints = new Vector3[4]
                {
                    center + new Vector2(0, -state.rect.height * 0.5f * scaleAdapte), // 上
                    center + new Vector2(state.rect.width * 0.5f * scaleAdapte, 0),  // 右
                    center + new Vector2(0, state.rect.height * 0.5f * scaleAdapte),  // 下
                    center + new Vector2(-state.rect.width * 0.5f * scaleAdapte, 0), // 左
                };
                // 绘制菱形背景
                Handles.BeginGUI();
                Handles.color = bgColor;
                Handles.DrawAAConvexPolygon(diamondPoints);
                Handles.EndGUI();
            }
            GUI.Box(state.rect, guiContent, style);

            // 允许拖拽节点
            if (Event.current.type == EventType.MouseDrag && state.rect.Contains(Event.current.mousePosition))
            {
                state.rect.position += Event.current.delta;
                Repaint();
            }
        }
        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

#region 添加 State
        private void AddState(Vector2 position, ENode eNode)
        {
            if (stateMachineData == null)
            {
                Debug.LogError("StateMachineData is null!");
                return;
            }
            if (stateMachineData.states == null)
            {
                stateMachineData.states = new List<StateNode>();
            }
            if (stateMachineData.states.Count == 0)
            {
                eNode = ENode.Branch;
            }
            StateNode newState = new StateNode
            {
                uniqueId = stateMachineData.UniqueIdGenerator(),
                eNode = eNode,
                name = stateMachineData.UniqueNameGenerator(),
                rect = new Rect(position.x, position.y, 100, 50),
                isEntryState = stateMachineData.states.Count == 0,
            };
            stateMachineData.states.Add(newState);
            EditorUtility.SetDirty(stateMachineData);
            AssetDatabase.SaveAssets();
            Repaint();
        }
#endregion

#region 移除 State
        private void DeleteStateNode(StateNode nodeToDelete)
        {
            if (stateMachineData == null || nodeToDelete == null) return;
    
            // 1. 从状态列表中移除节点
            stateMachineData.states.Remove(nodeToDelete);
    
            // 2. 清理所有相关过渡
            stateMachineData.transitions.RemoveAll(t => 
                t.fromUniqueId == nodeToDelete.uniqueId || t.toUniqueId == nodeToDelete.uniqueId);
    
            // 3. 处理入口状态
            if (nodeToDelete.isEntryState && stateMachineData.states.Count > 0)
            {
                // 自动设置第一个节点为新的入口
                stateMachineData.states[0].eNode = ENode.Branch;
                stateMachineData.states[0].isEntryState = true;
            }
    
            // 4. 保存更改
            EditorUtility.SetDirty(stateMachineData);
            AssetDatabase.SaveAssets();
            Repaint();
        }
#endregion

#region 详情 State
        private void DetailStateNode(StateNode selectedNode)
        {
            selectedElement = selectedNode;
        }
#endregion

        private bool TryGetStateNodeAtPosition(Vector2 position, out IElement result)
        {
            result = null;
            if (stateMachineData?.transitions == null) return false;
            
            foreach (var state in stateMachineData.states)
            {
                if (state.rect.Contains(position))
                {
                    result = state;
                    return true;
                }
            }
            return false;
        }
    }
}
