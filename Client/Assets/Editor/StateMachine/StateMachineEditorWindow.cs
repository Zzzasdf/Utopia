using UnityEngine;

namespace UnityEditor 
{
    public partial class StateMachineEditorWindow : EditorWindow 
    {
        [MenuItem("Tools/State Machine Editor")]
        public static void ShowWindow()
        {
            GetWindow<StateMachineEditorWindow>("State Machine Editor");
        }
        
        // Styles
        class Styles 
        {
            public GUIStyle header = "frameBox";
            public GUIStyle detail = "window";
        }
        
        // Constants
        
        // Static variables
        private static Styles s_Styles;

        // Member variables
        private StateMachineData stateMachineData;
        private Vector2 scrollPos;
        private Rect canvasSize = new Rect(0, 0, 5000, 5000); // 虚拟画布大小
        private EEventStatus eEventStatus;

        private void OnGUI()
        {
            if (s_Styles == null)
                s_Styles = new Styles();
            
            // 绘制滚动视图（支持拖拽画布）
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            {
                // 绘制网格背景
                DrawGrid(20, 0.2f, Color.gray);
                DrawGrid(100, 0.4f, Color.gray);

                using (new EditorGUILayout.VerticalScope())
                {
                    // 1.顶部工具栏（加载/保存状态机）
                    using (new EditorGUILayout.VerticalScope(s_Styles.header))
                    {
                        DrawToolbar();
                    }
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        // 2.检视面板
                        if (selectedElement != null)
                        {
                            using (new EditorGUILayout.VerticalScope(s_Styles.detail, GUILayout.MaxWidth(250)))
                            {
                                DrawInspector();
                            }
                        }
                        // 3、绘制元素
                        using (new EditorGUILayout.VerticalScope())
                        {
                            // 4. 绘制所有过渡曲线
                            DrawTransitions();
                            // 5. 绘制所有状态节点
                            DrawStateNodes();
                        }
                    }
                }
            }
            EditorGUILayout.EndScrollView();

            if (Event.current.type == EventType.MouseDown)
            {
                // 当前选中的元素
                IElement selectedElement;

                // 优先检测过渡线
                TryGetTransitionAtPosition(Event.current.mousePosition, out selectedElement);
                // 其次检测 State
                if (selectedElement == null) 
                    TryGetStateNodeAtPosition(Event.current.mousePosition, out selectedElement);
                
                // 事件分支
                switch(eEventStatus)
                {
                    case EEventStatus.Normal: // 普通操作
                    {
                        if (selectedElement == null)
                        {
                            if (Event.current.button == 1) // 菜单
                            {
                                DrawNullClickMenu();
                                Event.current.Use();
                            }
                        }
                        else
                        {
                            switch(selectedElement)
                            {
                                case StateNode stateNode: 
                                {
                                    if (Event.current.button == 0) // 选中编辑
                                        this.selectedElement = selectedElement;
                                    else if (Event.current.button == 1) // 菜单
                                    {
                                        DrawSelectedStateMenu(stateNode);
                                        Event.current.Use();
                                    }
                                    break;
                                }
                                case Transition transition:
                                {
                                    if (Event.current.button == 0) // 选中编辑
                                        this.selectedElement = selectedElement;
                                    else if (Event.current.button == 1) // 菜单
                                    {
                                        DrawSelectedTransitionMenu(transition);
                                        Event.current.Use();
                                    }
                                    break;
                                }
                            }
                        }
                        break;
                    }
                    case EEventStatus.AddTransition: // 添加过渡线操作
                    {
                        if (Event.current.button == 0 && selectedElement is StateNode stateNode)
                        {
                            EndAddTransition(stateNode);
                        }
                        else
                        {
                            dragStartNode = null;
                            eEventStatus = EEventStatus.Normal;
                        }
                        break;
                    }
                }
            }
        }
        
        private enum EEventStatus 
        {
            Normal,
            AddTransition,
        }
    }
}