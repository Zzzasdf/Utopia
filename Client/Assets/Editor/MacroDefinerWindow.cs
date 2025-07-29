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
        EditorApplication.update += Repaint;
    }
    private void OnDisable()
    {
        EditorApplication.update -= Repaint;
    }

    private Vector2 scroll;

    // 前宏定义
    private List<string> defines;
    /// 固定
    private List<DefineNode> fixedDefines = new List<DefineNode>
    {
        new DefineNode("DOTWEEN", "Tween"),
    };
    // 可选
    private List<DefineNode> optionalDefines = new List<DefineNode>
    {
        new DefineNode("POOL_RELEASES", "对象池发布模式",
            new DefineNode("POOL_PERFORNANCE", "对象池性能模式")),
    };
    
    // // 开发模式
    // private List<(string, string)> developmentDefines = new List<(string, string)>
    // {
    //     ("POOL_RELEASE", "对象池发布模式"),  
    // };
    // // 发布模式
    // private List<(string, string)> releasesDefines = new List<(string, string)>
    // {
    //      
    // };
    
    public void OnGUI()
    {
        scroll = EditorGUILayout.BeginScrollView(scroll);
        // 固定 隐藏
        {
            EditorGUILayout.LabelField("固定宏");
            GUI.enabled = false;
            ShowDefines(fixedDefines);
            GUI.enabled = true;
            bool isDirtyFixed = false;
            for (int i = 0; i < fixedDefines.Count; i++)
            {
                if (TrySetAllNode(fixedDefines[i], true))
                {
                    isDirtyFixed = true;
                }
            }
            if (isDirtyFixed)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines.ToArray());
            }
        }
        // 可选
        {
            EditorGUILayout.LabelField("可选宏");
            ShowDefines(optionalDefines);
        }
        if (GUILayout.Button("保存"))
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines.ToArray());
        }
        EditorGUILayout.EndScrollView();
    }

    private void ShowDefines(List<DefineNode> defineNodes)
    {
        using (new EditorGUILayout.VerticalScope())
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(20);
                if (defineNodes == null || defineNodes.Count == 0) return;
                for (int i = 0; i < defineNodes.Count; i++)
                {
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
    private bool TrySetAllNode(DefineNode defineNode, bool isSelected)
    {
        bool result = false;
        if (defineNode == null) return result;
        if (!defines.Contains(defineNode.Current))
        {
            defines.Add(defineNode.Current);
            result = true;
        }
        if (isSelected)
        {
            if (TrySetAllNode(defineNode.Parent, isSelected))
            {
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