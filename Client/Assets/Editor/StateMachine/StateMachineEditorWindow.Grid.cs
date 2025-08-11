using UnityEngine;

namespace UnityEditor 
{
    public partial class StateMachineEditorWindow
    {
        /// 绘制网格背景
        private void DrawGrid(float spacing, float opacity, Color color)
        {
            int widthDivs = Mathf.CeilToInt(canvasSize.width / spacing);
            int heightDivs = Mathf.CeilToInt(canvasSize.height / spacing);

            Handles.BeginGUI();
            Handles.color = new Color(color.r, color.g, color.b, opacity);

            for (int i = 0; i < widthDivs; i++)
            {
                Handles.DrawLine(new Vector3(i * spacing, 0), new Vector3(i * spacing, canvasSize.height));
            }

            for (int j = 0; j < heightDivs; j++)
            {
                Handles.DrawLine(new Vector3(0, j * spacing), new Vector3(canvasSize.width, j * spacing));
            }

            Handles.EndGUI();
        }
    }
}
