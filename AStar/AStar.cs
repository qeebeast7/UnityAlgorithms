using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar {
    //单例
    private static AStar instance;
    public static AStar Instance {
        get
        {
            if (instance == null) instance = new AStar();
            return instance;
        }
    }
    
    public Node[,] nodes;//所有节点
    private int row;//行数
    private int col;//列数
    public List<Node> openList=new List<Node>();//开启列表
    public List<Node> closeList=new List<Node>();//关闭列表
    public Node start;//起点
    public Node end;//终点

    private Node curNode;//当前点
    List<GameObject> path = new List<GameObject>();//最终路径

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="nodes"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    public void InitNodes(Node[,] nodes, Node start, Node end)
    {
        //init nodes
        row = nodes.GetLength(0);
        col = nodes.GetLength(1);
        this.nodes = new Node[row,col];
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                this.nodes[i, j] = nodes[i, j];
            }
        }
        //init start and end
        this.start = start;
        this.end = end;
        if (!IsAllow(start))
        {
            Debug.Log("start is not allow");
        }
        if (!IsAllow(end))
        {
            Debug.Log("end is not allow");
        }
    }
    /// <summary>
    /// AStar寻路
    /// </summary>
    /// <returns></returns>
    public List<GameObject> FindPath()
    {
        //clear info
        closeList.Clear();
        openList.Clear();
        path.Clear();
        start.SetInfo(0, 0, null);
        //将起点加入开启列表
        openList.Add(start);
        //start path-finding
        //若没有到达终点则继续
        while (curNode != end)
        {
            //寻找开启列表中f值最小点
            openList.Sort();
            //该点设为当前点
            curNode = openList[0];
            //开启列表存放备选点
            openList.Remove(curNode);
            //关闭列表存放已寻路的父节点
            closeList.Add(curNode);
            //寻找当前点的周围点，若满足条件则放进开启列表中
            List<Node> neighbours = GetNeighbours();
            for (int i = 0; i < neighbours.Count; i++)
            {
                Node node = neighbours[i];
                //去除不在范围内、不可通行、已寻过的点
                if (!IsAllow(node) || closeList.Contains(node))
                    continue;
                //计算g(parent.g+self_g)
                int g = curNode.g + GetG(node);
                //计算h
                int h = GetH(curNode);
                //若较优或未寻过则更新信息放进开启列表中
                if (g < node.g || !openList.Contains(node))
                {
                    node.SetInfo(g,h,curNode);
                    if (!openList.Contains(node))
                    {
                        openList.Add(node);
                    }
                }
            }
            //死路（所有点已寻完）
            if (openList.Count <= 0)
            {
                Debug.Log("can't reach");
                break;
            }
        }
        //到达终点后返回路径（除起点和终点）
        end = end.parent;
        while (end != start && end != null)
        {
            path.Add(end.gameObject);
            end = end.parent;//回溯
        }
        path.Reverse();
        return path;
    }
    /// <summary>
    /// 是否在范围内及是否可通行
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    bool IsAllow(Node node)
    {
        if (node.x < 0 || node.x >= row
            || node.y < 0 || node.y >= col
            || !node.canWalk) return false;
        else return true;
    }
    /// <summary>
    /// 寻找周围节点
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public List<Node> GetNeighbours()
    {
        List<Node> neighbours = new List<Node>();
        //八方向
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                //除去自身节点
                if (i == 0 && j == 0)
                {
                    continue;
                }

                int x = curNode.x + i;
                int y = curNode.y + j;
                //范围校验
                if (x>=0 && x<row && y>=0 && y<col)
                {
                    neighbours.Add(nodes[x, y]);
                }
            }
        }
        return neighbours;
    }
    /// <summary>
    /// 利用对角线算法计算G值
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    int GetG(Node node)
    {
        int disX = Mathf.Abs(curNode.x - node.x);
        int disY = Mathf.Abs(curNode.y - node.y);
        ////斜向(14)+横向/竖向(10)
        //if (disX > disY)
        //{
        //    return 14 * disY + 10 * (disX - disY);
        //}
        //else
        //{
        //    return 14 * disX + 10 * (disY - disX);
        //}
        if (disX >= 1 && disY >= 1) return 14;//对角线
        else return 10;//横竖
    }
    /// <summary>
    /// 利用曼哈顿算法计算H值
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    int GetH(Node node)
    {
        return 10 * (Mathf.Abs(end.x - node.x) + Mathf.Abs(end.y - node.y));//横竖差值*10
    }
}
