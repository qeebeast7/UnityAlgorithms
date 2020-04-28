using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarManager : MonoBehaviour
{
    public GameObject cube;//预设立方体
    public int minX;//最小x值
    public int maxX;//最大x值
    public int minZ;//最小z值
    public int maxZ;//最大z值

    private int row;//行
    private int col;//列
    private Node[,] nodes;//所有节点
    private Node start;//起点
    private Node end;//终点
    private List<GameObject> path;//最终路径
    // Use this for initialization
    void Start()
    {
        path = new List<GameObject>();
        //get row and col
        row = maxZ - minZ+1;
        col = maxX - minX+1;
        //init nodes
        nodes = new Node[row, col];
        //创建并寻路
        CreateMap();
        ResetMap();
    }

    // Update is called once per frame
    void Update()
    {
        //按下空格重置地图
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //reset map
            ResetMap();
        }
    }
    /// <summary>
    /// 创建地图
    /// </summary>
    void CreateMap()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                GameObject obj = Instantiate(cube, new Vector3(minX + j, 0, minZ + i), Quaternion.identity);
                Node node = obj.GetComponent<Node>();
                //设置坐标
                node.x = i;
                node.y = j;
                nodes[i, j] = node;
            }
        }
        ChangeCamera();
    }
    /// <summary>
    /// 重置地图
    /// </summary>
    void ResetMap()
    {
        path.Clear();

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                //初始化方块
                Node node = nodes[i, j];
                node.SetInfo(0, 0, null);
                GameObject obj = node.gameObject;
                ChangeColor(obj, "White");
                //20%概率为障碍
                if (Random.Range(0, 1f) < 0.8f)
                {
                    node.canWalk = true;
                }
                else
                {
                    node.canWalk = false;
                    ChangeColor(obj, "Red");
                }

                node.x = i;
                node.y = j;
                nodes[i, j] = node;
            }
        }
        //设置起点（除障碍和终点）
        start = nodes[Random.Range(0, row), Random.Range(0, col)];
        while (!start.canWalk)
        {
            start = nodes[Random.Range(0, row), Random.Range(0, col)];
        }
        while (start==end)
        {
            start = nodes[Random.Range(0, row), Random.Range(0, col)];
        }
        ChangeColor(start.gameObject, "Yellow");
        //设置终点（除障碍和终点）
        end = nodes[Random.Range(0, row), Random.Range(0, col)];
        while (!end.canWalk)
        {
            end = nodes[Random.Range(0, row), Random.Range(0, col)];
        }
        while (end==start)
        {
            end = nodes[Random.Range(0, row), Random.Range(0, col)];
        }
        ChangeColor(end.gameObject, "Blue");
        //初始化AStar
        AStar.Instance.InitNodes(nodes, start, end);
        //AStar寻路
        path = AStar.Instance.FindPath();
        //路径为绿色
        if (path.Count > 0)
        {
            foreach (GameObject obj in path)
            {
                ChangeColor(obj, "Green");
            }
        }
    }
    /// <summary>
    /// 更换颜色材质
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="color"></param>
    void ChangeColor(GameObject obj, string color)
    {
        obj.GetComponent<MeshRenderer>().material = (Material)Resources.Load("Materials/" + color);
    }
    /// <summary>
    /// 根据地图大小改变相机位置
    /// </summary>
    void ChangeCamera()
    {
        float x = minX + (float)col / 2 - 0.5f;
        float y = row > col ? row : col;
        float z = minZ + (float)row / 2 - 0.5f;
        Camera.main.transform.position = new Vector3(x, y, z);
    }
}
