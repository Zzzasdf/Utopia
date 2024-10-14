// using System;
// using System.Collections.Generic;
// using UnityEngine;
//
// namespace RedTipModule
// {
//
//     public interface IRedTipHandler
//     {
//         void AddParent(RedTip redTip);
//         void AddChild(RedTip redTip);
//         
//         void AddCondition(Func<RedTipIndexList, bool> condition);
//         void AddComponent(IRedTipComponent component);
//         
//         void Update(Dictionary<RedTip, IRedTipHandler> handlerMap, RedTipIndexList redTipIndexList);
//         void UpdateCurrent(RedTipIndexList redTipIndexList);
//
//         bool Status(RedTipIndexList redTipIndexList);
//     }
//
//     public class RedTipHandler: IRedTipHandler
//     {
//         private readonly RedTip current;
//         private List<RedTip> parents;
//         private List<RedTip> children;
//         
//         private Func<RedTipIndexList, bool> condition;
//         private Dictionary<RedTipIndexList, IRedTipHandlerUnit> uints;
//         
//         private LinkedList<IRedTipComponent> components;
//
//         public RedTipHandler(RedTip current)
//         {
//             this.current = current;
//             this.components = new LinkedList<IRedTipComponent>();
//         }
//
//         void IRedTipHandler.AddParent(RedTip parent)
//         {
//             parents ??= new List<RedTip>();
//             if (parents.Contains(parent))
//             {
//                 throw new Exception($"在 {current} 的 parents 里存在相同类型 => {parent}");
//             }
//             parents.Add(parent);
//         }
//
//         void IRedTipHandler.AddChild(RedTip child)
//         {
//             children ??= new List<RedTip>();
//             if (children.Contains(child))
//             {
//                 throw new Exception($"在 {current} 的 children 里存在相同类型 => {child}");
//             }
//             children.Add(child);
//         }
//
//         void IRedTipHandler.AddCondition(Func<RedTipIndexList, bool> condition)
//         {
//             this.condition = condition;
//         }
//
//         void IRedTipHandler.AddComponent(IRedTipComponent component)
//         {
//             RemoveNullComponents();
//             components.AddLast(component);
//         }
//
//         private void RemoveNullComponents()
//         {
//             LinkedListNode<IRedTipComponent> currentNode = components.First;  
//             while (currentNode != null)  
//             {  
//                 if (currentNode.Value == null)  
//                 {  
//                     LinkedListNode<IRedTipComponent> temp = currentNode;  
//                     currentNode = currentNode.Next;  
//                     components.Remove(temp); 
//                 }  
//                 else  
//                 {  
//                     currentNode = currentNode.Next;  
//                 }  
//             }  
//         }
//
//         void IRedTipHandler.Update(Dictionary<RedTip, IRedTipHandler> handlerMap, RedTipIndexList redTipIndexList)
//         {
//             bool lastStatus = Status(redTipIndexList);
//             UpdateChildren(handlerMap, redTipIndexList);
//             UpdateCurrent(redTipIndexList);
//             if (lastStatus == Status(redTipIndexList))
//             {
//                 return;
//             }
//             UpdateParents(handlerMap);
//         }
//
//         private void UpdateChildren(Dictionary<RedTip, IRedTipHandler> handlerMap, RedTipIndexList redTipIndexList)
//         {
//             if (children == null);
//             for (int i = 0; i < children.Count; i++)
//             {
//                 RedTip childRedTip = children[i];
//                 if (!handlerMap.TryGetValue(childRedTip, out IRedTipHandler childHandler))
//                 {
//                     continue;
//                 }
//                 RedTipIndexList childRedTipIndexList = new RedTipIndexList(redTipIndexList, i);
//                 childHandler.Update(handlerMap, childRedTipIndexList);
//             }
//         }
//
//         public void UpdateCurrent(RedTipIndexList redTipIndexList)
//         {
//             if (!uints.TryGetValue(redTipIndexList, out IRedTipHandlerUnit unit))
//             {
//                 return;
//             }
//             if (condition == null)
//             {
//                 unit.Update(false);
//                 return;
//             }
//             bool status = condition.Invoke(redTipIndexList);
//             unit.Update(status);
//             
//             // 更新组件
//             foreach (var component in components)
//             {
//                 if(component == null) continue;
//                 component.Update(redTipIndexList, status);
//             }
//         }
//
//         private void UpdateParents(Dictionary<RedTip, IRedTipHandler> handlerMap)
//         {
//             if (parents == null);
//             for (int i = 0; i < parents.Count; i++)
//             {
//                 RedTip parentRedTip = parents[i];
//                 if (!handlerMap.TryGetValue(parentRedTip, out IRedTipHandler parentHandler))
//                 {
//                     continue;
//                 }
//                 RedTipIndexList parentRedTipIndexList = new RedTipIndexList();
//                 parentHandler.UpdateCurrent(parentRedTipIndexList);
//             }
//         }
//         
//         public bool Status(RedTipIndexList redTipIndexList)
//         {
//             if (uints == null || condition == null)
//             {
//                 return false;
//             }
//             if (!uints.TryGetValue(redTipIndexList, out IRedTipHandlerUnit unit))
//             {
//                 Debug.LogError($"{current} 不存在索引类型 { redTipIndexList }");
//                 return false;
//             }
//             return unit.Status();
//         }
//     }
// }