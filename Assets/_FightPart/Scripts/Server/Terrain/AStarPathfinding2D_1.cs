using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XianXia;
//脚本作者:Saber

//[Serializable]
//public class GridConfigurationInfo:IEquatable<GridConfigurationInfo>
//{
//    public int id;
//    public int movePrice = 1;
//    public bool slopeLimit = false;
//    public bool collisionLimit = false;
//    public bool edgeLimit = false;
//    public bool waterLimt = false;

//    public GridConfigurationInfo(GridItem gridItem)
//    {
//        this.id = gridItem.Id;
//        this.slopeLimit = gridItem.slopeLimit;
//        this.collisionLimit = gridItem.collisionLimit;
//        edgeLimit = gridItem.edgeLimit;
//        waterLimt = gridItem.waterLimt;
//        movePrice = gridItem.movePrice;
//        //chessStandLimit = gridItem.chessStandLimit;
//    }
//    public bool Equals(GridConfigurationInfo other)
//    {
//        if (this.id == other.id) return true;
//        else return false;
//    }
//}
//[Serializable]
//public class Node : IComparable<Node>
//{

//    public Node(Vector2 vector3, int id, AStarPathfinding2D gridMap, int movePrice = 1)
//    {
//        position = vector3;
//        this.id = id;
//        this.movePrice = movePrice;
//        this.gridMap = gridMap;
//    }
//    public GridBound bound;
//    #region 格子基本属性
//    [SerializeField]
//    int id;


//    [Header("坐标位置")]
//    public Vector2 position;
//    [Header("行走代价")]
//    public int movePrice = 1;
//    [Header("F G H")]
//    [HideInInspector]
//    public int F;
//    [HideInInspector]
//    public int G;
//    [HideInInspector]
//    public int H;
//    [Header("发现他的格子")]
//    [HideInInspector]
//    public Node parent;
//    [HideInInspector]
//    public AStarPathfinding2D gridMap;
//    [Space]
//    public bool chessStandLimit = false;
//    public bool canMove = true;
//    //[Space]
//    //public bool slopeLimit = false;
//    public bool collisionLimit = false;
//    public bool edgeLimit = false;
//    //public bool waterLimt = false;

//    //public bool IsCanMove => !waterLimt&&!edgeLimit&&!chessStandLimit&&!slopeLimit && !collisionLimit&& canMove;

//    public int Id { get => id; }

//    #endregion
//    //public GridConfigurationInfo PutConfiguration()
//    //{
//    //    return new GridConfigurationInfo(this);
//    //}
//    //public void GetConfiguration(GridConfigurationInfo gridItem)
//    //{
//    //    if (gridItem.id != this.id) return;
//    //    this.slopeLimit = gridItem.slopeLimit;
//    //    this.collisionLimit = gridItem.collisionLimit;
//    //    edgeLimit = gridItem.edgeLimit;
//    //    waterLimt = gridItem.waterLimt;
//    //    movePrice = gridItem.movePrice;
//    //    //chessStandLimit = gridItem.chessStandLimit;
//    //}
//    public void ResetSelf()
//    {
//        F = 0;
//        G = 0;
//        H = 0;
//        parent = null;
//    }
//    public int CompareTo(Node other)
//    {
//        if (F < other.F)
//        {
//            return -1;
//        }
//        if (F > other.F)
//        {
//            return 1;
//        }
//        return 0;
//    }
//}
//[Serializable]
////记录格子边缘四个点
//public class GridBound
//{
//    public Vector2[] points = new Vector2[4];

//    public GridBound(Vector2[] bounds = null)
//    {
//        if (bounds != null)
//            this.points = bounds;
//    }
//}
////[RequireComponent(typeof(Terrain))]
//public class AStarPathfinding2D : MonoBehaviour
//{
//    #region 公有字段
//    Vector3 StartPos=>transform.position;
//    public Vector2Int num;
//    public bool showGird = false;
//    //[OnValueChanged("ShowHelper")]
//    //public bool showHelper = false;
//    //public Terrain terrain;
//    //public GameObject gridHelperModel;

//    public float cellHalfSize = 0.75f;

//    public float rayHight = 100;
//    public LayerMask obstacleLayer;
//    public LayerMask terrainLayer;
//    //[Range(0,90)]
//    //public int slopeAngle = 45;
//    public int x;
//    public int y;
//        //public float agentHight = 2f;
//    #endregion

//    #region 私有字段
//    [SerializeField]
//    Node[] allGrids;

//    [HideInInspector]
//    [SerializeField]
//    Vector2 terrain_x;
//    [HideInInspector]
//    [SerializeField]
//    Vector2 terrain_z;

//    [HideInInspector]
//    public Vector3 cellSizeScale;

//    //[HideInInspector]
//    //[SerializeField]
//    //private bool[] HadConfigurations;


//    //[ShowInInspector]
//    //[SerializeField]
//    //private string mapDataID;
//    #endregion

//    #region 属性
//    public Node[] AllGrids { get => allGrids; }
//    public Vector2 Terrain_x { get => terrain_x; set => terrain_x = value; }
//    public Vector2 Terrain_z { get => terrain_z; set => terrain_z = value; }

//    //public string MapDataID { get => mapDataID; set => mapDataID = value; }

//    #endregion

//    #region 函数
//    //static GameObject GetAssetAtPath()
//    //{
//    //    return UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/WarChess/Prefabs/GridHelper.prefab");

//    //}
//    [Button]
//    private void Scan()
//    {
//        //terrain = terrain != null ? terrain : Terrain.activeTerrain;
//        //terrainLayer = 0;
//        //terrainLayer |= 1 << terrain.gameObject.layer;
//        cellSizeScale = new Vector3(cellHalfSize * 2, 0.1f, cellHalfSize * 2);
//        //obstacleLayer |= 0 << terrain.gameObject.layer;
//        //cellV3 = new Vector3(cellHalfSize, 0, cellHalfSize);
//        terrain_x = new Vector2(StartPos.x+cellHalfSize, StartPos.x + num.x*cellHalfSize-cellHalfSize);
//        terrain_z = new Vector2(StartPos.z+cellHalfSize, StartPos.z + num.y * cellHalfSize - cellHalfSize);
//        GetAllGridCells();
//        FindAllGridBound();
//        ScanObstable();
//        //GenerateGridHelper();
//        //GetGridMapConfigurationTable();
//    }
//    [Button]
//    private void ScanObstable()
//    {
//        foreach(var grid in allGrids)
//        {
//            Collider2D[] collider2Ds=new Collider2D[8];
//            int res=Physics2D.OverlapCircleNonAlloc(grid.position,cellHalfSize,collider2Ds,obstacleLayer);
//            Debug.Log("QQQ"+res);

//            foreach (var v in collider2Ds)
//            {
//                if(v==null||v.isTrigger) continue;
//                Debug.Log("QQQ");
//                grid.collisionLimit = true;
//                break;
//            }
//        }
//    }

//    /// <summary>
//    /// 根据地形生成所有的格子中心
//    /// </summary>
//    private void GetAllGridCells()
//    {
//        int x_Amount =num.x;
//        int z_Amount = num.y;
//        allGrids = new Node[x_Amount * z_Amount];
//        Vector2 cellPos = new Vector2(StartPos.x+cellHalfSize,StartPos.y+cellHalfSize);
//        Vector2 tempPos=cellPos;
//        //Vector3 temPos;
//        //cellPos.x += cellHalfSize;
//        //cellPos.y = terrain_z.x;
//        int index = 0;
//        x = 0;y = 0;
//        //bool x_F = false;
//        while (y < z_Amount)
//        {
//            x = 0;
//            //tempPos = cellPos;

//            while (x < x_Amount)
//            {
//                tempPos = cellPos + x * Vector2.right*cellHalfSize*2 + y * Vector2.up* cellHalfSize * 2;

//                allGrids[index] = new Node(tempPos, index, this);
//                x++;
//                index++;
//                Debug.Log(index);

//            }
//            y++;

//            //cellPos.x += cellHalfSize*2;
//        }
//        //while (cellPos.y < terrain_z.y)
//        //{
//        //    cellPos.x = terrain_x.x;
//        //    y++;
//        //    while (cellPos.x < terrain_x.y)
//        //    {
//        //        if(!x_F)x++;
//        //        //temPos = cellPos;
//        //        //Ray ray = new Ray(new Vector3(cellPos.x , rayHight, cellPos.z), Vector3.down );
//        //        //RaycastHit raycastHit;
//        //        ////射线获取
//        //        //if (Physics.Raycast(ray, out raycastHit, rayHight * 2,terrainLayer, QueryTriggerInteraction.Ignore))
//        //        //{
//        //        //    temPos = raycastHit.point;
//        //        //    //allGrids[index] = new GridItem(raycastHit.point,index,this);
//        //        //}
//        //        allGrids[index] = new GridItem(new Vector2(cellPos.x,cellPos.y), index, this);
//        //        ////判断是否有障碍物
//        //        //if(Physics.Raycast(ray, out raycastHit, rayHight, obstacleLayer, QueryTriggerInteraction.Ignore))
//        //        //{
//        //        //    allGrids[index].collisionLimit =true;
//        //        //    //Debug.Log("障碍五被限制" + index);
//        //        //}
//        //        index++;
//        //        cellPos.x += cellHalfSize * 2;
//        //    }
//        //    x_F = true;
//        //    cellPos.z += cellHalfSize * 2;
//        //}
//        //去除边界
//        Node[] tem = new Node[index];
//        Array.Copy(allGrids, tem, index);
//        allGrids = tem;
//        Debug.Log("生成完毕,总格子数目:"+ index);
//    }
//    /// <summary>
//    /// 计算所有格子的边缘顶点
//    /// </summary>
//    private void FindAllGridBound()
//    {
//        //Ray ray;
//        //RaycastHit raycastHit;
//        float x_offest = cellHalfSize;
//        float z_offest = cellHalfSize;
//        //边之间的高度偏移最大值
//        //float side_offest = Mathf.Tan(Mathf.Deg2Rad * slopeAngle)*cellHalfSize*2;
//        //对角线之间的高度偏移最大值
//        //float diagonal_offest = Mathf.Tan(Mathf.Deg2Rad * slopeAngle) * cellHalfSize * 2 * 1.4f;
//        //Debug.Log(side_offest+"as"+diagonal_offest);

//        for (int i = 0; i < allGrids.Length; i++)
//        {
//            if (allGrids[i] == null) continue;
//            allGrids[i].bound = new GridBound();
//            for(int j = 0; j < 4; j++)
//            {
//                //获取四个边缘点
//                //0为右上
//                //1为右下
//                //2为左下
//                //3为左上
//                switch (j)
//                {
//                    case 0:
//                        x_offest = cellHalfSize;
//                        z_offest = cellHalfSize;
//                        break;
//                    case 1:
//                        x_offest = cellHalfSize;
//                        z_offest = -cellHalfSize;
//                        break;
//                    case 2:
//                        x_offest = -cellHalfSize;
//                        z_offest = -cellHalfSize;
//                        break;
//                    case 3:
//                        x_offest = -cellHalfSize;
//                        z_offest = cellHalfSize;
//                        break;
//                }
//                //raycastHit =default;
//                //Debug.Log(allGrids[i]);
//                //Debug.Log(terrain);

//                allGrids[i].bound.points[j] = new Vector2(allGrids[i].position.x + x_offest, allGrids[i].position.y + z_offest);

//                //allGrids[i].bound.points[j] = allGrids[i].position;
//                ////射线获取
//                //ray = new Ray(new Vector3(allGrids[i].position.x + x_offest, rayHight, allGrids[i].position.y + z_offest), Vector3.down);
//                //if (Physics.Raycast(ray,out raycastHit, rayHight * 2, terrainLayer, QueryTriggerInteraction.Ignore))
//                //{
//                //}
//                //if (!allGrids[i].collisionLimit&&Physics.Raycast(ray, out raycastHit, rayHight, obstacleLayer, QueryTriggerInteraction.Ignore))
//                //{
//                //    //if(raycastHit.point.y - allGrids[i].bound.points[j].y> agentHight)
//                //    //Debug.Log("障碍五被限制" + i);
//                //    allGrids[i].collisionLimit = true;
//                //}
//            }
//            //计算格子上是否有障碍物或者是坡度过大
//            //坡度检测
//            //障碍检测在上面
//            //float pre_y = allGrids[i].bound.points[3].y;
//            //float low_y = allGrids[i].bound.points[3].y;
//            //float high_y = low_y;
//            //for (int j = 0; j < 4; j++)
//            //{
//            //    float y = allGrids[i].bound.points[j].y;
//            //    if (y < low_y) low_y = y;
//            //    if (high_y < y) high_y = y;
//            //}
//            //if (allGrids[i].slopeLimit) Debug.Log("坡度被限制");
//        }
//    }
//    #endregion
//    #region 静态函数
//    //public static void UpdateZone(UnitMovement go2D,int sizeX,int sizeY,GridMap gridMap,bool isCantStand)
//    //{
//    //    GridItem gridItem = GetGridByWorldPosition(go2D.feetPoint.position,gridMap);
//    //    if (gridItem == null) return;
//    //    if (go2D.standGridItem.Count != 0)
//    //    {
//    //        foreach(var v in go2D.standGridItem)
//    //        {
//    //            v.chessStandLimit = false;
//    //        }
//    //        go2D.standGridItem.Clear();
//    //    }
//    //    gridItem.chessStandLimit = isCantStand;
//    //    go2D.standGridItem.Add(gridItem);
//    //    //sizeX--;
//    //    //for(int i = -sizeX; i <= sizeX; i++)
//    //    //{
//    //    //    for(int j = 0; j <= sizeY; j++)
//    //    //    {
//    //    //        GridItem temp = GridUtility.IsEffectiveCoord(i, j, gridItem);
//    //    //        if (temp == null) continue;
//    //    //        temp.chessStandLimit = isCantStand;
//    //    //        go2D.standGridItem.Add(temp);
//    //    //    }
//    //    //}




//    //    //Stack<GridItem> list = new Stack<GridItem>();
//    //    //HashSet<GridItem> finded = new HashSet<GridItem>();
//    //    //list.Push(gridItem);
//    //    //while (list.Count > 0)
//    //    //{
//    //    //    gridItem=list.Pop();
//    //    //    if (!finded.Contains(gridItem))
//    //    //        finded.Add(gridItem);

//    //    //    if (go2D.bounds.Contains(gridItem.position))
//    //    //    {
//    //    //        Debug.Log("一个格子不能行走了！！");
//    //    //        gridItem.chessStandLimit = isCantStand;
//    //    //       // Vector2Int coord = GetCoordById(gridItem.Id, gridItem.gridMap);
//    //    //        for(int i = -1; i <= 1; i++)
//    //    //        {
//    //    //            for(int j = -1; j <= 1; j++)
//    //    //            {
//    //    //                GridItem temp = GridUtility.IsEffectiveCoord(i, j, gridItem);
//    //    //                if (temp == null||finded.Contains(temp)) continue;
//    //    //                list.Push(temp);
//    //    //            }
//    //    //        }
//    //    //    }
//    //    //}
//    //}
//    public Vector2 GetMapY()
//    {
//        return new Vector2(StartPos.y, StartPos.y + y * cellHalfSize * 2);
//    }
//    public Vector2 GetMapX()
//    {
//        return new Vector2(StartPos.x,StartPos.x + x *cellHalfSize * 2);
//    }
//    public static int CoordToId(Vector2Int coord, AStarPathfinding2D gridMap)
//    {
//        return CoordToId(coord.x, coord.y, gridMap);
//    }
//    public static int CoordToId(int x,int y,AStarPathfinding2D gridMap)
//    {
//        return (y - 1) * gridMap.x + (x - 1);
//    }
//    /// <summary>
//    /// 根据格子图坐标获取格子
//    /// </summary>
//    /// <param name="coord"></param>
//    /// <param name="gridFinding"></param>
//    /// <returns></returns>
//    public static Node GetGridByCoord(Vector2Int coord, AStarPathfinding2D gridFinding)
//    {
//        return GetGridByCoord(coord.x, coord.y, gridFinding);
//    }
//    /// <summary>
//    /// 根据格子图坐标获取格子
//    /// </summary>
//    /// <param name="x"></param>
//    /// <param name="y"></param>
//    /// <param name="gridFinding"></param>
//    /// <returns></returns>
//    public static Node GetGridByCoord(int x, int y, AStarPathfinding2D gridFinding)
//    {
//        //Debug.Log("1214" + x+"/"+y);

//        int index = (y - 1) * gridFinding.x + (x - 1);
//        //Debug.Log("1214_" + index+"+" +gridFinding.allGrids.Length);
//        //.Log("0302" + index + "/" + gridFinding.allGrids.Length);
//        if(gridFinding.allGrids.Length>index&&index>=0)
//            return gridFinding.allGrids[index];
//        else
//        { Debug.LogWarning("Index out range"); return null; }
//    }

//    /// <summary>
//    /// 通过世界坐标获取格子,可以配合射线检测使用
//    /// </summary>
//    /// <param name="worldPos"></param>
//    /// <param name="gridMap"></param>
//    /// <returns></returns>
//    public static Node GetGridByWorldPosition(Vector3 worldPos,AStarPathfinding2D gridMap)
//    {
//        float x = worldPos.x;
//        float y = worldPos.y;
//        //Debug.Log("开始" + gridMap.StartPos);
//        x -= gridMap.StartPos.x;
//        y -= gridMap.StartPos.y;
//        float radius = gridMap.cellHalfSize * 2;
//        x /= radius;
//        y /= radius;
//        //float f_x = x - (int)x;
//        //float f_y = y - (int)y;
//        //x = f_x < 0.5 ? x : x + 1;
//        //y = f_y < 0.5 ? y : y + 1;
//        int real_x = (int)x + 1;
//        int real_y = (int)y + 1;
//        //Debug.Log("1215 "+real_x+", "+real_y);
//        return GetGridByCoord(real_x, real_y, gridMap);

//    }
//    /// <summary>
//    /// 根据格子获取坐标
//    /// </summary>
//    /// <param name="gridItem"></param>
//    /// <param name="gridFinding"></param>
//    /// <returns></returns>
//    public static Vector2Int GetCoordByGrid(Node gridItem)
//    {
//        if (gridItem == null) return Vector2Int.one;
//        return GetCoordById(gridItem.Id, gridItem.gridMap);
//    }
//    /// <summary>
//    /// 根据ID获取坐标
//    /// </summary>
//    /// <param name="num"></param>
//    /// <param name="gridFinding"></param>
//    /// <returns></returns>
//    public static Vector2Int GetCoordById(int num, AStarPathfinding2D gridFinding)
//    {
//        int x = num % gridFinding.x + 1;
//        int y = num / gridFinding.x + 1;
//        return new Vector2Int(x, y);
//    }
//    #endregion
//    Stack<Node> cantMoveGridItem = new Stack<Node>();
//    Stack<Node> waterGridItem = new Stack<Node>();
//    Stack<Node> edgeGridItem = new Stack<Node>();


//    protected virtual void DrawGridBound(Node grid)
//    {
//        int size=1;
//        float offest_size = GLUtility.lineOffest * size * cellHalfSize*2;
//        Vector2 offest = Vector2.down * offest_size;
//        Vector2 pre = grid.bound.points[3];
//        for (int j = 0; j < 4; j++)
//        {
//            pre = new Vector3(pre.x + offest.x, pre.y+ offest.y,StartPos.z);
//            Vector3 pos = new Vector3(grid.bound.points[j].x + offest.x, grid.bound.points[j].y + offest.y, StartPos.z);
//            GLUtility.DrawLine(pre, pos,size);
//            pre = grid.bound.points[j];
//            switch (j)
//            {
//                case 0:
//                    offest = Vector2.left * offest_size; break;
//                case 1:
//                    offest = Vector2.up * offest_size; break;
//                case 2:
//                    offest = Vector2.right * offest_size; break;
//            }
//        }
//    }
//    protected virtual void OnDrawGizmos()
//    {
//        if (!Application.isEditor) return;
//        //if (!showed) return;
//        //ShowHelper();
//        if (showGird)
//        {
//            Gizmos.color = Color.green;
//            //Vector3 pre = Vector3.down * -100;
//            for (int i = 0; i < allGrids.Length; i++)
//            {
//                if (allGrids[i] == null || allGrids[i].bound == null) continue;
//                //不能移动的格子稍后绘制
//                if ((!allGrids[i].canMove || allGrids[i].edgeLimit))
//                {
//                    edgeGridItem.Push(allGrids[i]); continue;
//                }
//                else if ((allGrids[i].collisionLimit || allGrids[i].chessStandLimit))
//                {
//                    cantMoveGridItem.Push(allGrids[i]); continue;
//                }
//                //else if (allGrids[i].waterLimt)
//                //{
//                //    waterGridItem.Push(allGrids[i]); continue;
//                //}
//                else
//                    DrawGridBound(allGrids[i]);
//            }
//            //绘制障碍物与斜坡的格子
//            Gizmos.color = Color.red;
//            while (cantMoveGridItem.Count != 0)
//            {
//                Node gridItem = cantMoveGridItem.Pop();
//                DrawGridBound(gridItem);
//            }
//            //绘制海洋移动的格子
//            Gizmos.color = Color.blue;
//            while (waterGridItem.Count != 0)
//            {
//                Node gridItem = waterGridItem.Pop();
//                DrawGridBound(gridItem);
//            }
//            //绘制海洋移动的格子
//            Gizmos.color = Color.black;
//            while (edgeGridItem.Count != 0)
//            {
//                Node gridItem = edgeGridItem.Pop();
//                DrawGridBound(gridItem);
//            }
//        }
//        //helper
//    }
//}
