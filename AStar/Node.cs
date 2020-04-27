using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Node : MonoBehaviour,IComparable {

    public int x;//横坐标
    public int y;//纵坐标
    public bool canWalk;//是否可通行

    public int g;//到起点的距离
    public int h;//到终点的距离
    public int f;//g+h
    public Node parent;//父节点

    public Node(int x,int y,bool canWalk)
    {
        this.x = x;
        this.y = y;
        this.canWalk = canWalk;
    }
    /// <summary>
    /// 设置变量
    /// </summary>
    /// <param name="g"></param>
    /// <param name="h"></param>
    /// <param name="parent"></param>
    public void SetInfo(int g,int h,Node parent)
    {
        this.g = g;
        this.h = h;
        this.f = g + h;
        this.parent = parent;
    }
    //排序接口，按照f排序
    public int CompareTo(object obj)
    {
        Node node = (Node)obj;
        if (this.f > node.f) return 1;
        else if (this.f < node.f) return -1;
        else return 0;
    }
}
