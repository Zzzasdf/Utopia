using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
    public partial class StateMachineEditorWindow 
    {
        private float curveOffset = 0f;//50f;
        private Color transitionColor = Color.green;

        private float arrowSize = 20f;
        private Color arrowColor = Color.red;
        
        /// 绘制过渡曲线
        private void DrawTransitions()
        {
            if (stateMachineData == null) return;
            Handles.BeginGUI();
            foreach (var transition in stateMachineData.transitions)
            {
                StateNode fromState = stateMachineData.states.Find(s => s.uniqueId == transition.fromUniqueId);
                StateNode toState = stateMachineData.states.Find(s => s.uniqueId == transition.toUniqueId);

                if (fromState != null && toState != null)
                {
                   DrawBezier(fromState, toState);
                }
            }
            Handles.EndGUI();
        }
        /// 绘制贝塞尔曲线
        private void DrawBezier(StateNode fromState, StateNode toState)
        {
            Vector2 start = fromState.rect.center;
            start.y -= fromState.rect.height / 2;
            Vector2 end = toState.rect.center;
            end.y += toState.rect.height / 2;
                    
            // 计算控制点（使曲线呈弧形）
            Vector2 startTangent = start + Vector2.right * curveOffset;
            Vector2 endTangent = end - Vector2.right * curveOffset;
            using (new Handles.DrawingScope(transitionColor))
            {
                Handles.DrawBezier(start, end, startTangent, endTangent, transitionColor, null, 5);
            }
                    
            // 绘制条件标签
            Vector2 labelPos = (start + end) * 0.5f;
            Handles.Label(labelPos, "11111", EditorStyles.miniLabel);
                    
            // 计算箭头位置和方向
            Vector2 arrowDir = ((start - end) * 0.5f).normalized;
            Vector2 arrowBase = (end + start) * 0.5f - arrowDir * (arrowSize * 0.5f); // 箭头起点略微向内偏移

            // 绘制箭头
            DrawArrowHead(arrowBase, arrowDir, arrowSize, arrowColor);
        }
        // 绘制箭头（三角形）
        private void DrawArrowHead(Vector2 pos, Vector2 direction, float size, Color color)
        {
            Vector2 right = Quaternion.Euler(0, 0, 30) * direction * size;
            Vector2 left = Quaternion.Euler(0, 0, -30) * direction * size;
            using (new Handles.DrawingScope(color))
            {
                Handles.DrawAAConvexPolygon(pos, pos + right, pos + left);
            }
        }

#region 添加 Transition
        private StateNode dragStartNode; // 记录拖拽起始节点
        private void StartAddTransition(StateNode selectedNode)
        {
            dragStartNode = selectedNode;
            eEventStatus = EEventStatus.AddTransition;
        }
        private void EndAddTransition(StateNode selectedNode)
        {
            if (selectedNode != null)
            {
                if (selectedNode != dragStartNode)
                {
                    AddTransition(dragStartNode, selectedNode);
                }
            }
            dragStartNode = null;
            eEventStatus = EEventStatus.Normal;
        }
        private void AddTransition(StateNode fromState, StateNode toState)
        {
            if (stateMachineData.transitions == null)
            {
                stateMachineData.transitions = new List<Transition>();
            }
            if (stateMachineData.transitions.Exists(t => 
                t.fromUniqueId == fromState.uniqueId && t.toUniqueId == toState.uniqueId))
            {
                Debug.LogWarning("Transition already exists!");
                return;
            }

            var newTransition = new Transition()
            {
                fromUniqueId = fromState.uniqueId,
                toUniqueId = toState.uniqueId,
            };

            stateMachineData.transitions.Add(newTransition);
            EditorUtility.SetDirty(stateMachineData);
            AssetDatabase.SaveAssets();
            Repaint();
        }
#endregion

#region 删除 Transition
        private void DeleteTransition(Transition selectedTransition)
        {
            stateMachineData.transitions.Remove(selectedTransition);
            EditorUtility.SetDirty(stateMachineData);
            AssetDatabase.SaveAssets();
            Repaint();
        }
#endregion
        
        private bool TryGetTransitionAtPosition(Vector2 position, out IElement result)
        {
            result = null;
            if (stateMachineData?.transitions == null) return false;

            foreach (var transition in stateMachineData.transitions)
            {
                StateNode fromState = stateMachineData.GetStateByUniqueId(transition.fromUniqueId);
                StateNode toState = stateMachineData.GetStateByUniqueId(transition.toUniqueId);
        
                if (fromState == null || toState == null) continue;
        
                // 计算控制点（根据节点位置自动调整曲线方向）
                Vector2 start = fromState.rect.center;
                start.y -= fromState.rect.height / 2;
                Vector2 end = toState.rect.center;
                end.y += toState.rect.height / 2;
                    
                // // 计算控制点（使曲线呈弧形）
                // Vector2 startTangent = start + Vector2.right * curveOffset;
                // Vector2 endTangent = end - Vector2.right * curveOffset;
        
                // 计算箭头位置和方向
                Vector2 arrowDir = ((start - end) * 0.5f).normalized;
                Vector2 arrowBase = (end + start) * 0.5f - arrowDir * (arrowSize * 0.5f); // 箭头起点略微向内偏移
                
                Rect rect = new Rect(arrowBase.x- arrowSize, arrowBase.y - arrowSize, arrowSize * 2f, arrowSize * 2f);
                // GUIStyle style = new GUIStyle(GUI.skin.box);
                // style.normal.background = MakeTex(2, 2,Color.black);
                // GUI.Box(rect, "AAA", style);
                if (rect.Contains(position))
                {
                    result = transition;
                    return true;
                }

                // // 检测点是否在曲线附近
                // if (BezierUtils.IsPointNearBezier(
                //     start, 
                //     end, 
                //     startTangent, 
                //     endTangent, 
                //     position, 
                //     10f)) // 检测半径
                // {
                //     result = transition;
                //     return true;
                // }
            }
            return false;
        }
    }
}