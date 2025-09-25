using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class MacroDefinerWindow : EditorWindow
{
    [MenuItem("Tools/设置全局宏")]
    private static void Open()
    {
        GetWindow<MacroDefinerWindow>("设置全局宏", true);
    }
    private void OnEnable()
    {
        PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, out string[] _defines);
        defines = _defines.ToList();
        Init();
        EditorApplication.update += Repaint;
    }
    private void OnDisable()
    {
        EditorApplication.update -= Repaint;
    }

    public enum EMacroDefiner
    {
        DOTWEEN = 1,
        POOL_RELEASES = 2,
        // Test = 3,
    }
    private static Dictionary<EMacroDefiner, DefineNode> dict = new Dictionary<EMacroDefiner,DefineNode>
    {
        [EMacroDefiner.DOTWEEN] = new DefineNode(
            "DOTWEEN", "Tween"),
        [EMacroDefiner.POOL_RELEASES] = new DefineNode(
            "POOL_RELEASES", "对象池发布模式"),
        // [EMacroDefiner.Test] = new DefineNode(
        //     "Test", "测试", new DefineNode(
        //         "Test1", "测试1", new DefineNode(
        //             "Test1_1", "测试1_1")), new DefineNode(
        //         "Test2", "测试2")),
    };

    /// 固定
    private List<EMacroDefiner> fixedDefines;
    // 可选
    private List<EMacroDefiner> optionalDefines;
    
    // 开发模式
    private List<EMacroDefiner> developmentDefines;
    
    // 发布模式
    private List<EMacroDefiner> releasesDefines;
    
    private void Init()
    {
        // 固定
        fixedDefines = new List<EMacroDefiner>
        {
            EMacroDefiner.DOTWEEN,
        };
        // 可选
        optionalDefines = new List<EMacroDefiner>
        {
            EMacroDefiner.POOL_RELEASES,
            // EMacroDefiner.Test,
        };
        
        // 开发者模式
        developmentDefines = new List<EMacroDefiner>
        {
        };
        
        // 发布模式
        releasesDefines = new List<EMacroDefiner>
        {
            EMacroDefiner.POOL_RELEASES,
        };
    }

    // 前宏定义
    private List<string> defines;

    private Vector2 scroll;
    private List<DefineNode> defineNodes = new List<DefineNode>();

    public void OnGUI()
    {
        scroll = EditorGUILayout.BeginScrollView(scroll);
        using (new EditorGUILayout.VerticalScope())
        {
            // 固定 隐藏
            {
                EditorGUILayout.LabelField("固定宏");
                defineNodes.Clear();
                {
                    SetDefineNodes(defineNodes, fixedDefines);
                    GUI.enabled = false;
                    ShowDefines(defineNodes);
                    GUI.enabled = true;
                    if (TrySetAllNode(defineNodes, true))
                    {
                        PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines.ToArray());
                    }
                }
            }
            // 可选
            {
                EditorGUILayout.LabelField("可选宏");
                defineNodes.Clear();
                SetDefineNodes(defineNodes, optionalDefines);
                ShowDefines(defineNodes);
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("一键开发模式"))
                {
                    defines.Clear();
                    defineNodes.Clear();
                    {
                        SetDefineNodes(defineNodes, fixedDefines);
                        TrySetAllNode(defineNodes, true);
                    }
                    defineNodes.Clear();
                    {
                        SetDefineNodes(defineNodes, developmentDefines);
                        TrySetAllNode(defineNodes, true);
                    }
                }
                if (GUILayout.Button("一键发布模式"))
                {
                    defines.Clear();
                    defineNodes.Clear();
                    {
                        SetDefineNodes(defineNodes, fixedDefines);
                        TrySetAllNode(defineNodes, true);
                    }
                    defineNodes.Clear();
                    {
                        SetDefineNodes(defineNodes, releasesDefines);
                        TrySetAllNode(defineNodes, true);
                    }
                }
            }
            if (GUILayout.Button("保存"))
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines.ToArray());
            }
        }
        EditorGUILayout.EndScrollView();
    }

    private void SetDefineNodes(in List<DefineNode> defineNodes, List<EMacroDefiner> eMacroDefiners)
    {
        for (int i = 0; i < eMacroDefiners.Count; i++)
        {
            EMacroDefiner eMacroDefiner = eMacroDefiners[i];
            DefineNode defineNode = dict[eMacroDefiner];
            defineNodes.Add(defineNode);
        }
    }

    private void ShowDefines(List<DefineNode> defineNodes)
    {
        using (new EditorGUILayout.VerticalScope())
        {
            if (defineNodes == null || defineNodes.Count == 0) return;
            for (int i = 0; i < defineNodes.Count; i++)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Space(20);
                    DefineNode defineNode = defineNodes[i];
                    using (new EditorGUILayout.VerticalScope())
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.LabelField($"{defineNode.Current} => {defineNode.FuncIntro}", GUILayout.Width(250));
                            GUILayout.FlexibleSpace();
                            bool toggle = EditorGUILayout.Toggle(defines.Contains(defineNode.Current));
                            if (toggle && !defines.Contains(defineNode.Current))
                            {
                                // 父节点必须是 Add 的状态
                                AddAllParentNode(defineNode);
                                defines.Add(defineNode.Current);
                            }
                            else if (!toggle && defines.Contains(defineNode.Current))
                            {
                                // 子节点必须是 Remove 的状态
                                RemoveAllChildren(defineNode);
                                defines.Remove(defineNode.Current);
                            }
                        }
                        ShowDefines(defineNode.Children);
                    }
                }
            }
        }
    }

    private bool TrySetAllNode(List<DefineNode> defineNodes, bool isSelected)
    {
        bool isDirty = false;
        for (int i = 0; i < defineNodes.Count; i++)
        {
            if (TrySetAllNode(defineNodes[i], isSelected))
            {
                isDirty = true;
            }
        }
        return isDirty;
    }
    private bool TrySetAllNode(DefineNode defineNode, bool isSelected)
    {
        bool result = false;
        if (defineNode == null) return result;
        if (isSelected)
        {
            if (TrySetAllNode(defineNode.Parent, isSelected))
            {
                result = true;
            }
            if (!defines.Contains(defineNode.Current))
            {
                defines.Add(defineNode.Current);
                result = true;
            }
        }
        else
        {
            for (int i = 0; i < defineNode.Children.Count; i++)
            { 
                if (TrySetAllNode(defineNode.Children[i], isSelected))
                {
                    result = true;
                }
            }
            if (defines.Contains(defineNode.Current))
            {
                defines.Remove(defineNode.Current);
                result = true;
            }
        }
        return result;
    }
    private void AddAllParentNode(DefineNode defineNode)
    {
        if (defineNode.Parent == null) return;
        if (!defines.Contains(defineNode.Parent.Current))
        {
            AddAllParentNode(defineNode.Parent);
            defines.Add(defineNode.Parent.Current);
        }
    }
    private void RemoveAllChildren(DefineNode defineNode)
    {
        if (defineNode.Children == null || defineNode.Children.Count == 0) return;
        for (int i = 0; i < defineNode.Children.Count; i++)
        {
            RemoveAllChildren(defineNode.Children[i]);
            if (defines.Contains(defineNode.Children[i].Current))
            {
                defines.Remove(defineNode.Children[i].Current);
            }
        }
    }

    public class DefineNode 
    {
        public DefineNode Parent;
        public string Current;
        public string FuncIntro;
        public List<DefineNode> Children;

        public DefineNode(string current, string funcIntro, params DefineNode[] defineNodes)
        {
            this.Current = current;
            this.FuncIntro = funcIntro;
            for (int i = 0; i < defineNodes.Length; i++)
            {
                defineNodes[i].Parent = this;
            }
            this.Children = defineNodes.ToList();
        }
    }
}