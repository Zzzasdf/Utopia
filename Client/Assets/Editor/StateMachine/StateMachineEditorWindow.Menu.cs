using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
namespace UnityEditor 
{
    public partial class StateMachineEditorWindow 
    {
        private string folderPath = string.Empty;

        private string[] filePaths;
        private string[] fileDisplayNames;
        private int inRefIndex = 0;
        
        /// 顶部工具栏（加载/保存状态机）
        private void DrawToolbar()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    string folderPath = SetFolderPath(this.folderPath, "指定资源文件夹");
                    if (!string.IsNullOrEmpty(folderPath))
                    {
                        this.folderPath = folderPath;
                    }
                    if (GUILayout.Button("Scan"))
                    {
                        DirectoryInfo directoryInfo = new DirectoryInfo(this.folderPath);
                        FileInfo[] fileInfos = directoryInfo.GetFiles();
                        List<(string path, string displayName)> validFiles = new List<(string, string)>();
                        for (int i = 0; i < fileInfos.Length; i++)
                        {
                            FileInfo fileInfo = fileInfos[i];
                            if(fileInfo.Extension != ".asset") continue;
                            string path = fileInfo.FullName[fileInfo.FullName.IndexOf("Assets", StringComparison.Ordinal)..];
                            string displayName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                            validFiles.Add((path, displayName));
                        }
                        validFiles.Sort((x,y) => string.Compare(x.displayName, y.displayName, StringComparison.Ordinal));
                        filePaths = validFiles.Select(x => x.path).ToArray();
                        fileDisplayNames = validFiles.Select(x => x.displayName).ToArray();
                        LoadFile(0);
                    }
                }
                if (fileDisplayNames == null || fileDisplayNames.Length == 0)
                {
                    stateMachineData = null;
                    return;
                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    int changeIndex = EditorGUILayout.Popup(inRefIndex, fileDisplayNames);
                    if (changeIndex != inRefIndex)
                    {
                        inRefIndex = changeIndex;
                        LoadFile(inRefIndex);
                    }
                    if (GUILayout.Button("Export"))
                    {
                        if (stateMachineData != null)
                        {
                            stateMachineData.Export();
                        }
                    }
                }
            }
        }
        private string SetFolderPath(string showPath, string defaultName)
        {
            string tempPath = string.Empty;
            using (new EditorGUILayout.HorizontalScope())
            {
                if (string.IsNullOrEmpty(showPath))
                {
                    showPath = defaultName;
                }
                GUI.enabled = false;
                EditorGUILayout.TextField("", showPath);
                GUI.enabled = true;
                if (GUILayout.Button("指定资源文件夹"))
                {
                    tempPath = EditorUtility.OpenFolderPanel("选取资源文件夹", "", string.Empty);
                    return tempPath;
                }
            }
            return tempPath;
        }
        private void LoadFile(int index)
        {
            if (index >= filePaths.Length) return;
            string path = filePaths[index];
            if (!string.IsNullOrEmpty(path))
            {
                stateMachineData = AssetDatabase.LoadAssetAtPath<StateMachineData>(path);
            }
        }
        
#region 绘制 Inspector
        private IElement _selectedElement;
        private IElement selectedElement 
        {
            get => _selectedElement;
            set 
            {
                GUI.FocusControl(null);
                _selectedElement = value;
            }
        }
        private void DrawInspector()
        {
            if (stateMachineData == null) return;
            if (selectedElement == null) return;
            switch(selectedElement)
            {
                case StateNode stateNode:
                {
                    if (!stateMachineData.states.Contains(stateNode)) return;
                    EditorGUILayout.LabelField("StateNode");
                    GUI.enabled = false;
                    EditorGUILayout.IntField("uniqueId", stateNode.uniqueId);
                    EditorGUILayout.EnumPopup("eNode", stateNode.eNode, EditorStyles.popup);
                    GUI.enabled = true;
                    stateNode.name = EditorGUILayout.TextField("name", stateNode.name);
                    break;
                }
                case Transition transition: 
                {
                    if (!stateMachineData.transitions.Contains(transition)) return;
                    EditorGUILayout.LabelField("Transition");
                    GUI.enabled = false;
                    EditorGUILayout.IntField("fromUniqueId", transition.fromUniqueId);
                    EditorGUILayout.IntField("toUniqueId", transition.toUniqueId);
                    GUI.enabled = true;
                    break;
                }
            }
        }
#endregion   

#region 绘制 空点击 菜单
        private void DrawNullClickMenu()
        {
            Vector2 mousePos = Event.current.mousePosition;
            GenericMenu menu = new GenericMenu();
            // 确保 stateMachineData 存在
            if (stateMachineData == null)
            {
                menu.AddDisabledItem(new GUIContent("Load a StateMachineData first!"));
            }
            else
            {
                if (stateMachineData.states.Count == 0)
                {
                    menu.AddItem(new GUIContent("Add Entry"), false, () => AddState(mousePos, ENode.Branch));
                }
                else
                {
                    menu.AddItem(new GUIContent("Add State"), false, () => AddState(mousePos, ENode.Normal));
                    menu.AddItem(new GUIContent("Add Branch"), false, () => AddState(mousePos, ENode.Branch));
                }
            }
            menu.ShowAsContext();
        }
#endregion

#region 绘制选中 State 菜单
        private void DrawSelectedStateMenu(StateNode selectedNode)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Detail State"), false, () => DetailStateNode(selectedNode));
            menu.AddItem(new GUIContent("Delete State"), false, () => DeleteStateNode(selectedNode));
            menu.AddItem(new GUIContent("Add Transition"), false, () => StartAddTransition(selectedNode));
            menu.ShowAsContext();
        }
#endregion

#region 绘制选中 Transition 菜单
        private void DrawSelectedTransitionMenu(Transition selectedTransition)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Delete Transition"), false, () => DeleteTransition(selectedTransition));
            menu.ShowAsContext();
        }
#endregion
    }
}