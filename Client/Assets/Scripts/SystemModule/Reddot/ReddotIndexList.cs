using System;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;

namespace ReddotModule
{
    // public class ReddotIndexList : IEquatable<ReddotIndexList>
    // {
    //     private readonly LinkedList<int> linkedList;
    //
    //     public ReddotIndexList(params int[] indexList)
    //     {
    //         if (indexList == null || indexList.Length == 0)
    //         {
    //             linkedList = null;
    //             return;
    //         }
    //
    //         linkedList = new LinkedList<int>();
    //         foreach (int index in indexList)
    //         {
    //             linkedList.AddLast(index);
    //         }
    //     }
    //
    //     public ReddotIndexList(ReddotIndexList reddotIndexList, params int[] indexList)
    //     {
    //         if (reddotIndexList.linkedList == null || indexList == null || indexList.Length == 0)
    //         {
    //             linkedList = null;
    //             return;
    //         }
    //
    //         linkedList = new LinkedList<int>();
    //         linkedList.AddRange(reddotIndexList.linkedList);
    //         foreach (int index in indexList)
    //         {
    //             linkedList.AddLast(index);
    //         }
    //     }
    //
    //     public bool Equals(ReddotIndexList other)
    //     {
    //         if (ReferenceEquals(null, other)) return false;
    //         if (ReferenceEquals(this, other)) return true;
    //         LinkedListNode<int> thisCurrentNode = linkedList.First;
    //         LinkedListNode<int> otherCurrentNode = other.linkedList.First;
    //         while (true)
    //         {
    //             if (thisCurrentNode == null && otherCurrentNode == null)
    //             {
    //                 return true;
    //             }
    //
    //             if (thisCurrentNode == null || otherCurrentNode == null)
    //             {
    //                 return false;
    //             }
    //
    //             if (thisCurrentNode.Value != otherCurrentNode.Value)
    //             {
    //                 return false;
    //             }
    //
    //             thisCurrentNode = thisCurrentNode.Next;
    //             otherCurrentNode = otherCurrentNode.Next;
    //         }
    //     }
    //
    //     public override string ToString()
    //     {
    //         if (linkedList == null)
    //         {
    //             return "null";
    //         }
    //
    //         StringBuilder sb = new StringBuilder();
    //         int index = 0;
    //         foreach (var node in linkedList)
    //         {
    //             sb.AppendLine($"{index} => {node}");
    //             index++;
    //         }
    //
    //         return sb.ToString();
    //     }
    // }
}