using Saber.Camp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using XianXia.Terrain;
namespace XianXia
{
//    public static class GridUtility 
//    {
//        /// <summary>
//        /// �ж�����һ���߽�����Ƿ���Ч
//        /// </summary>
//        /// <param name="i"></param>
//        /// <param name="j"></param>
//        /// <param name="center"></param>
//        /// <param name="gridCalculateType"></param>
//        /// <param name="moveType"></param>
//        /// <param name="closeList"></param>
//        /// <param name="gridMap"></param>
//        /// <returns></returns>
//        public static bool IsEffectiveGrid(int i, int j, Node center, out Node gridItem, GridType moveType, List<Node> closeList)
//        {
//            gridItem = IsEffectiveCoord(i, j, center);
//            if (gridItem == null) return false;
//            return IsEffectiveGrid(gridItem, moveType, closeList);
//        }
//        public static Node IsEffectiveCoord(int i, int j, Node center, GridCalculateType gridCalculateType = GridCalculateType.Eight)
//        {
//            Node res = null;
//            //�Ƿ���center
//            if (i == 0 && j == 0)
//                return res;
//            //�ж����ĸ���ʾ���ǰ˸���ʾ
//            if ((i == j || i == -j) && gridCalculateType == GridCalculateType.Four)
//                return res;
//            Vector2Int centerV2 = center.Position;
//            AStarPathfinding2D gridMap = center.gridMap;
//            int center_x = (int)centerV2.x;
//            int center_y = (int)centerV2.y;
//            //��ֹԽ��
//            int x = center_x + i;
//            int y = center_y + j;
//            if (x < 1 || x > gridMap.x)
//                return res;
//            if (y < 1 || y > gridMap.y)
//                return res;
//            return res = AStarPathfinding2D.GetGridByCoord(x, y, gridMap);
//        }
//        private static bool IsEffectiveGrid(Node center, GridType moveType, List<Node> closeList)
//        {
//            //�Ƿ��Ѿ�����center
//            if (closeList.Contains(center))
//                return false;
//            //�Ƿ����ϰ���
//            //bool ob = IsObstacleToThisGrid(moveType, gridItem);
//            //Debug.Log("�ϰ��ж�Ϊ" + ob);
//            if (IsObstacleToThisGrid(moveType, center))
//                return false;
//            return true;
//        }
//        /// <summary>
//        /// �����ƶ������ж�һ�������Ƿ����ϰ���
//        /// </summary>
//        /// <param name="moveType"></param>
//        /// <param name="grid"></param>
//        /// <returns></returns>
//        public static bool IsObstacleToThisGrid(GridType moveType, Node grid)
//        {
//            if (moveType == GridType.none) return false;
//            //�ø������ڱ߽���߱�����Ϊ�����ƶ�״̬,������ʲô�ƶ����Ͷ����ܿ�Խ
//            if (grid.canMove == false) return true;
//            if (grid.edgeLimit) return true;
//            switch (moveType)
//            {
//                case GridType.Ground:
//                    if (grid.collisionLimit) return true;
//                    break;
//            }
//            return false;
//        }

//        /// <summary>
//        /// �����ж϶��������������ж�
//        /// </summary>
//        /// <param name="startGrid"></param>
//        /// <param name="center"></param>
//        /// <param name="gridMap"></param>
//        /// <param name="gridCalculateType"></param>
//        /// <param name="moveType"></param>
//        /// <param name="closeList"></param>
//        /// <param name="nextList"></param>
//        private static void GridGroundScreen(Node startGrid, Node center, AStarPathfinding2D gridMap, GridCalculateType gridCalculateType, GridType moveType, List<Node> closeList, List<Node> nextList)
//        {
//            for (int i = -1; i <= 1; i++)
//            {
//                for (int j = -1; j <= 1; j++)
//                {
//                    #region            �ж��˸����Ƿ��Թ�
//                    //if (!IsEffectiveGrid(i, j, center,out GridItem item, gridCalculateType, moveType, closeList, gridMap)) continue;
//                    Node item = IsEffectiveCoord(i, j, center);
//                    if (item == null || closeList.Contains(item)) continue;
//                    if (!IsEffectiveGrid(item, moveType, closeList)) continue;
//                    //if (standedList != null && standedList.Contains(item)) continue;
//                    //����������ӵ�ежԵ�λ������center���Ѿ��е�λ��
//                    //�жϵж�
//                    //if (ChessUtility.RelationOfTwoGrids(item, startGrid) == CampRelation.hostile) continue;
//                    #endregion
//                    //���ҵ��ĸ��Ӽ�����һ��Ҫ��Ϊ���ĵĸ����б���
//                    //Debug.Log("nextcount" + nextList.Count);

//                    closeList.Add(item);
//                    if (!nextList.Contains(item))
//                    {
//                        nextList.Add(item);
//                    }
//                    //���ҵ��ĸ��Ӽ������б��ҵ��ĸ����б���
//                }
//            }
//        }
//        /// <summary>
//        /// �����ж϶��������������ж�
//        /// </summary>
//        /// <param name="startGrid"></param>
//        /// <param name="center"></param>
//        /// <param name="gridMap"></param>
//        /// <param name="gridCalculateType"></param>
//        /// <param name="moveType"></param>
//        /// <param name="closeList"></param>
//        /// <param name="nextList"></param>
//        private static void GridGroundScreen(Node startGrid, Node center, AStarPathfinding2D gridMap, GridCalculateType gridCalculateType, GridType moveType, List<Node> closeList, List<Node> nextList, List<Node> standedList)
//        {
//            for (int i = -1; i <= 1; i++)
//            {
//                for (int j = -1; j <= 1; j++)
//                {
//                    #region            �ж��˸����Ƿ��Թ�
//                    //if (!IsEffectiveGrid(i, j, center,out GridItem item, gridCalculateType, moveType, closeList, gridMap)) continue;
//                    Node item = IsEffectiveCoord(i, j, center);
//                    if (item == null || closeList.Contains(item) || (standedList != null && standedList.Contains(item))) continue;
//                    if (!IsEffectiveGrid(item, moveType, closeList)) continue;
//                    //if (standedList != null && standedList.Contains(item)) continue;
//                    //����������ӵ�ежԵ�λ������center���Ѿ��е�λ��
//                    //�жϵж�
//                    //if (ChessUtility.RelationOfTwoGrids(item, startGrid) == CampRelation.hostile) continue;
//                    #endregion
//                    //���ҵ��ĸ��Ӽ�����һ��Ҫ��Ϊ���ĵĸ����б���
//                    //Debug.Log("nextcount" + nextList.Count);

//                    if (standedList != null && item.chessStandLimit == true)
//                        standedList.Add(item);
//                    else
//                        closeList.Add(item);
//                    if (!nextList.Contains(item))
//                    {
//                        nextList.Add(item);
//                    }
//                    //���ҵ��ĸ��Ӽ������б��ҵ��ĸ����б���
//                }
//            }
//        }
//        private static void GridGroundScreen(Node center, GridType moveType,List<Node> closeList, List<Node> nextList)
//        {
//            for (int i = -1; i <= 1; i++)
//            {
//                for (int j = -1; j <= 1; j++)
//                {
//                    #region            �ж��˸����Ƿ��Թ�
//                    //if (!IsEffectiveGrid(i, j, center,out GridItem item, gridCalculateType, moveType, closeList, gridMap)) continue;
//                    Node item = IsEffectiveCoord(i, j, center);
//                    if (item == null || closeList.Contains(item) || (item.chessStandLimit==true)) continue;
//                    if (!IsEffectiveGrid(item, moveType, closeList)) continue;
//                    //if (standedList != null && standedList.Contains(item)) continue;
//                    //����������ӵ�ежԵ�λ������center���Ѿ��е�λ��
//                    //�жϵж�
//                    //if (ChessUtility.RelationOfTwoGrids(item, startGrid) == CampRelation.hostile) continue;
//                    #endregion
//                    //���ҵ��ĸ��Ӽ�����һ��Ҫ��Ϊ���ĵĸ����б���
//                    //Debug.Log("nextcount" + nextList.Count);
//                    if(!closeList.Contains(item))
//                        closeList.Add(item);
//                    if (!nextList.Contains(item))
//                    {
//                        nextList.Add(item);
//                    }
//                    //���ҵ��ĸ��Ӽ������б��ҵ��ĸ����б���
//                }
//            }
//        }
//        public static Node FindNearestGridItem(Node center, Node start, GridType gridType)
//        {
//            Vector2Int centerPos = AStarPathfinding2D.GetCoordByGrid(center);
//            Vector2Int startPos = AStarPathfinding2D.GetCoordByGrid(start);
//            Vector2Int dir = startPos - centerPos;
//            if (dir.x > 0) dir.x = 1;
//            else if (dir.x < 0) dir.x = -1;
//            if (dir.y > 0) dir.y = 1;
//            else if (dir.y < 0) dir.y = -1;
//            Node res = AStarPathfinding2D.GetGridByCoord(centerPos.x + dir.x, centerPos.y + dir.y, center.gridMap);
//            Node grid = null;
//            Queue<Node> queue = new Queue<Node>();
//            queue.Enqueue(res);
//            while (queue.Count > 0)
//            {
//                res = queue.Dequeue();
//                if (res == null) continue;
//                //if (!IsObstacleToThisGrid(gridType, res) && res.chessStandLimit == false) return res;
//                if (IsCanMoveGrid(center, gridType)) return res;

//                Vector2Int tempPos = AStarPathfinding2D.GetCoordByGrid(res);
//                Vector2Int tempOppositeAngles = new Vector2Int(centerPos.x * 2 - tempPos.x, centerPos.y * 2 - tempPos.y);
//                int num = 1;

//                int disX = (int)MathF.Abs(tempPos.x - centerPos.x);
//                int disY = (int)MathF.Abs(tempPos.y - centerPos.y);
//                while (disX >= 0 && disY >= 0)
//                {
//                    if (disX >= 0)
//                    {
//                        grid = AStarPathfinding2D.GetGridByCoord(tempPos.x - num * dir.x, tempPos.y, center.gridMap);
//                        if (IsCanMoveGrid(grid, gridType)) return grid;
//                    }
//                    if (disY >= 0)
//                    {
//                        grid = AStarPathfinding2D.GetGridByCoord(tempPos.x, tempPos.y - num * dir.y, center.gridMap);
//                        if (IsCanMoveGrid(grid, gridType)) return grid;
//                    }
//                    if (disX >= 0)
//                    {
//                        grid = AStarPathfinding2D.GetGridByCoord(tempOppositeAngles.x + num * dir.x, tempOppositeAngles.y, center.gridMap);
//                        if (IsCanMoveGrid(grid, gridType)) return grid;
//                    }
//                    if (disY >= 0)
//                    {
//                        grid = AStarPathfinding2D.GetGridByCoord(tempOppositeAngles.x, tempOppositeAngles.y + num * dir.y, center.gridMap);
//                        if (IsCanMoveGrid(grid, gridType)) return grid;
//                    }
//                    num++;
//                    disX--;
//                    disY--;
//                }
//                //����Ͳ������
//                //if (Vector2.Dot(dir, startPos - tempPos) < 0) return null;
//                //if (num >= 5) return null;
//                queue.Enqueue(AStarPathfinding2D.GetGridByCoord(tempPos.x + dir.x, tempPos.y + dir.y, center.gridMap));

//            }
//            return null;

//        }

//        public static Node FindNearestGridItem2(Node center, Node start, GridType gridType)
//        {
//            Vector2Int centerPos = AStarPathfinding2D.GetCoordByGrid(center);
//            Vector2Int startPos = AStarPathfinding2D.GetCoordByGrid(start);
//            Vector2Int dir = startPos - centerPos;
//            if (dir.x > 0) dir.x = 1;
//            else if (dir.x < 0) dir.x = -1;
//            if (dir.y > 0) dir.y = 1;
//            else if (dir.y < 0) dir.y = -1;
//            return FindNearestGridItem3(center, centerPos, startPos, dir, gridType);

//        }
//        public static Node FindNearestGridItem(Node center, GridType gridType)
//        {
//            if(center==null)return null;
//            List<Node>openList= new List<Node>();
//            List<Node> closeList = new List<Node>();
//            List<Node> nextList=new List<Node>();
//            openList.Add(center);
//            closeList.Add(center);
//            Node temp = null;
//            Vector2Int coord;
//            int[] dirX = new int[] { 1, -1,0,  0, -1, 1, -1, 1};
//            int[] dirY = new int[] { 0, 0, 1, -1, -1, -1, 1, 1};
//            while (openList.Count > 0)
//            {
//                for (int n = 0; n < openList.Count; n++)
//                {
//                    temp = openList[n];
//                    coord = AStarPathfinding2D.GetCoordByGrid(temp);
//                    if (IsCanMoveGrid(temp, gridType)) return temp;
//                    closeList.Add(temp);
//                    int time = 0;
//                    while (time < dirX.Length)
//                    {
//                        temp = AStarPathfinding2D.GetGridByCoord(dirX[time] + coord.x, dirY[time] + coord.y, center.gridMap);
//                        time++;
//                        if (temp == null || closeList.Contains(temp)) continue;
//                        if (IsCanMoveGrid(temp, gridType)) return temp;
//                        nextList.Add(temp);
//                    }
//                }
//                openList.Clear();
//                for (int i = 0; i < nextList.Count; i++)
//                    openList.Add(nextList[i]);
//                nextList.Clear();
//            }
//            return null;
//        }
//        private static Node FindNearestGridItem3(Node center, Vector2Int centerPos, Vector2Int startPos, Vector2Int dir, GridType gridType)
//        {
//            Node res = AStarPathfinding2D.GetGridByCoord(centerPos.x + dir.x, centerPos.y + dir.y, center.gridMap);
//            Vector2Int newDir = new Vector2Int(dir.x != 0 ? dir.x : 1, dir.y != 0 ? dir.y : 1);
//            int[] grid1DirX;
//            int[] grid1DirY;
//            int[] roundByTime;
//            int[] ins = null;
//            int flag = 0;
//            if (dir.x == 0)
//            {
//                grid1DirX = new int[] { 1, 0, -1 };
//                grid1DirY = new int[] { 0, 1, 0 };
//                roundByTime = new int[] { 1, 2, 1 };
//                ins = new int[] { 1, 1, -1, 1 };
//                flag = 1;
//            } else if (dir.y == 0)
//            {
//                grid1DirX = new int[] { 0, 1, 0 };
//                grid1DirY = new int[] { 1, 0, -1 };
//                roundByTime = new int[] { 1, 2, 1 };
//                ins = new int[] { 1, 1, 1, -1 };
//                flag = 2;
//            }
//            else
//            {
//                grid1DirX = new int[] { 1, 0 };
//                grid1DirY = new int[] { 0, 1 };
//                roundByTime = new int[] { 2, 2 };
//            }
//            int takeTime = grid1DirX.Length;

//            int num = 1;
//            Node grid = null;
//            while (res != null)
//            {
//                //if (!IsObstacleToThisGrid(gridType, res) && res.chessStandLimit == false) return res;
//                if (IsCanMoveGrid(res, gridType)) return res;



//                Vector2Int tempPos = AStarPathfinding2D.GetCoordByGrid(res);
//                Vector2Int[] gridPos = new Vector2Int[2];
//                switch (flag)
//                {
//                    case 0:
//                        gridPos[0] = new Vector2Int(tempPos.x - dir.x * grid1DirX[0], tempPos.y - dir.y * grid1DirY[0]);
//                        gridPos[1] = new Vector2Int(tempPos.x - dir.x * grid1DirX[1], tempPos.y - dir.y * grid1DirY[1]);
//                        break;
//                    case 1:
//                        gridPos[0] = new Vector2Int(tempPos.x - 1, tempPos.y);
//                        gridPos[1] = new Vector2Int(tempPos.x + 1, tempPos.y);
//                        break;
//                    case 2:
//                        gridPos[0] = new Vector2Int(tempPos.x, tempPos.y - 1);
//                        gridPos[1] = new Vector2Int(tempPos.x, tempPos.y + 1);
//                        break;
//                }
//                grid = AStarPathfinding2D.GetGridByCoord(gridPos[0], center.gridMap);
//                if (IsCanMoveGrid(grid, gridType)) return grid;
//                grid = AStarPathfinding2D.GetGridByCoord(gridPos[1], center.gridMap);
//                if (IsCanMoveGrid(grid, gridType)) return grid;

//                int time = 0;
//                while (time < takeTime)
//                {
//                    int round = roundByTime[time];
//                    while (round > 0)
//                    {
//                        round--;
//                        switch (flag)
//                        {
//                            case 0:
//                                gridPos[0].x -= dir.x * grid1DirX[0];
//                                gridPos[0].y -= dir.y * grid1DirY[0];
//                                grid = AStarPathfinding2D.GetGridByCoord(gridPos[0], center.gridMap);
//                                if (IsCanMoveGrid(grid, gridType)) return grid;
//                                gridPos[1].x -= dir.x * grid1DirX[1];
//                                gridPos[1].y -= dir.y * grid1DirY[1];
//                                grid = AStarPathfinding2D.GetGridByCoord(gridPos[1], center.gridMap);
//                                if (IsCanMoveGrid(grid, gridType)) return grid;
//                                break;
//                            case 1:
//                            case 2:
//                                gridPos[0].x -= ins[0] * newDir.x * grid1DirX[time];
//                                gridPos[0].y -= ins[1] * newDir.y * grid1DirY[time];
//                                grid = AStarPathfinding2D.GetGridByCoord(gridPos[0], center.gridMap);
//                                if (IsCanMoveGrid(grid, gridType)) return grid;
//                                gridPos[1].x -= ins[2] * newDir.x * grid1DirX[time];
//                                gridPos[1].y -= ins[3] * newDir.y * grid1DirY[time];
//                                grid = AStarPathfinding2D.GetGridByCoord(gridPos[1], center.gridMap);
//                                if (IsCanMoveGrid(grid, gridType)) return grid;
//                                break;
//                        }

//                    }
//                    if (flag == 0)
//                    {
//                        int temp = grid1DirX[0];
//                        grid1DirX[0] = grid1DirX[1];
//                        grid1DirX[1] = temp;
//                        temp = grid1DirY[0];
//                        grid1DirY[0] = grid1DirY[1];
//                        grid1DirY[1] = temp;
//                    }
//                    time++;
//                }

//                //����Ͳ������
//                if (UnityEngine.Vector2.Dot(dir, startPos - tempPos) < 0) return null;
//                //if (num >= 5) return null;
//                res = AStarPathfinding2D.GetGridByCoord(tempPos.x + dir.x, tempPos.y + dir.y, center.gridMap);
//                num++;

//            }
//            return null;
//        }
//        static bool IsCanMoveGrid(Node center, GridType gridType)
//        {
//            if (center == null) return false;
//            return !IsObstacleToThisGrid(gridType, center) && !center.chessStandLimit;
//        }

//        /// <summary>
//        /// �ҵ������ĸ�������ĵ�λ��
//        /// ֧�� �����Ѿ��͵о�
//        /// </summary>
//        /// <param name="unitGridItem"></param>
//        /// <param name="startGrid"></param>
//        /// <param name="findType"></param>
//        /// <param name="moveType"></param>
//        /// <param name="gridCalculateType"></param>
//        public static Node AStarFindRecentlyGrid(Node startGrid,float maxFindRange ,Func<Node,Node,bool> func,CampRelation findType = CampRelation.hostile, GridType moveType = GridType.Ground)
//        {
//            //��ʼ����Ҫ�õ��ı��ر���
//            if (startGrid == null) return null;
//            List<Node> closeList = new List<Node>();
//            List<Node> openList = new List<Node>();
//            List<Node> nextList = new List<Node>();
//            AStarPathfinding2D gridMap = startGrid.gridMap;
//            //GridItem end = null;
//            Node center=null;
//            List<Node> res = new List<Node>();
//            ////������ʼ��
//            openList.Add(startGrid);
//            closeList.Add(startGrid);
//            //float findRange = 0;
//            while (openList.Count>0)
//            {
//                //Debug.Log("findcount" + findList.Count);
//                for (int n = 0; n < openList.Count; n++)
//                {
//                    center = openList[n];
//                    if (Mathf.Abs(center.position.x - startGrid.position.x) > maxFindRange) continue;

//                    //finallyList.Add(center);
//                    //�Ƿ����յ�
//                    if (center.chessStandLimit && center != startGrid)
//                    {
//                        if (func?.Invoke(startGrid,center) == true)
//                            return center;
//                    }
//                    GridGroundScreen(startGrid, center, gridMap, GridCalculateType.Eight, moveType, closeList, nextList);
//                }

//                //Debug.Log("nextcount" + nextList.Count);
//                if (nextList.Count == 0)
//                {
//                    //û�и���Χ�ĸ����ˣ�����Ѱ�ң���ʾ����
//                    //Debug.Log("�޸��ӿ�����");
//                    break;
//                }
//                openList.Clear();
//                //Array.Copy(nextList, closeList, nextList.Count);
//                //openList=nextList
//                for (int i = 0; i < nextList.Count; i++)
//                {
//                    openList.Add(nextList[i]);
//                }
//                //Debug.Log("findcount" + findList.Count);
//                nextList.Clear();
//            }
//            nextList.Clear();
//            openList.Clear();
//            closeList.Clear();
//            return null;
//        }
//        //public static GridItem FindNearestGridItem2(GridItem center, GridItem start, GridType gridType)
//        //{
//        //    Vector2Int centerPos = GridMap.GetCoordByGrid(center);
//        //    Vector2Int startPos = GridMap.GetCoordByGrid(start);
//        //    Vector2Int dir = startPos - centerPos;
//        //    if (dir.x > 0) dir.x = 1;
//        //    else if (dir.x < 0) dir.x = -1;
//        //    if (dir.y > 0) dir.y = 1;
//        //    else if (dir.y < 0) dir.y = -1;

//        //    int[] dirX= new int[] {dir.x,dir.x,0,dir.x,-dir.x,0,-dir.x,-dir.x};
//        //    int[] dirY = new int[] { dir.y,0, , dir.x, -dir.x, 0, -dir.x, -dir.x };

//        //}
//        //public static GridItem FindNearEffectGrid(GridItem gridItem,Func<GridItem,bool> func)
//        //{
//        //    int[] arr = new int[] { -1, 0, 1 };
//        //}

//        /// <summary>
//        /// ���ݼ������ͼ�����������֮��ľ���
//        /// </summary>
//        /// <param name="start"></param>
//        /// <param name="end"></param>
//        /// <param name="gridCalculateType"></param>
//        /// <returns></returns>
//        public static float CalculateTwoGirdDistance(Node start, Node end)
//    {
//            return CalculateTwoGirdDistance(AStarPathfinding2D.GetCoordByGrid(start), AStarPathfinding2D.GetCoordByGrid(end));

//    }
//        public static float CalculateTwoGirdDistance(Vector2Int v1,Vector2Int v2)
//        {
//            return Vector2Int.Distance(v1, v2);
//        }
//    public static float CalculateTwoGirdDistance(int x1, int y1, int x2, int y2)
//    {
//            float y = Mathf.Pow(y2 - y1, 2);
//            float x = Mathf.Pow(x2 - x1, 2);
//            return Mathf.Sqrt(x + y);

//    }
//    /// <summary>
//    /// A��Ѱ·�㷨
//    /// ������ʼ����ս��,�Լ�һЩ����
//    /// �������·��(����ɵִ�)
//    /// </summary>
//    public static void AStarPathFinding(out Stack<Node> path, Node start, Node end, float range = 0, GridType moveType = GridType.Ground, GridCalculateType gridCalculateType = GridCalculateType.Four)
//    {
//        if (start == end)
//        {
//            path = new Stack<Node>();
//            return;
//        }
//        if (range==0&&(IsObstacleToThisGrid(moveType, end) || end.chessStandLimit))
//        {
//            Vector2Int pos = AStarPathfinding2D.GetCoordByGrid(end);
//            Debug.Log("ԭ�յ���" + pos.x + "/" + pos.y);
//            Debug.Log(end.ToString());

//            end = FindNearestGridItem2(end, start, moveType);


//            if (end == null)
//            { path = new Stack<Node>();
//                Debug.Log("�޿��ø���");
//                return; }
//            pos = AStarPathfinding2D.GetCoordByGrid(end);
//            Debug.Log("���յ���" + pos.x + "/" + pos.y);
//            //�Զ�Ѱ������Ŀɵִ����
//        }
//        AStarPathfinding2D gridMap = start.gridMap;
//        //endGrid = end;
//        //������ʼ��
//        List<Node> allList = new List<Node>();
//        List<Node> openList = new List<Node>();
//        List<Node> closeList = new List<Node>();
//        openList.Add(start);
//        allList.Add(start);
//        Vector2Int centerV2;
//        int center_x, center_y, end_x, end_y;
//        centerV2 = AStarPathfinding2D.GetCoordByGrid(end);
//        end_x = (int)centerV2.x;
//        end_y = (int)centerV2.y;
//        Node center;
//        while (true)
//        {
//            //����
//            openList.Sort();
//            center = openList[0];
//            centerV2 = AStarPathfinding2D.GetCoordByGrid(center);
//            center_x = (int)centerV2.x;
//            center_y = (int)centerV2.y;
//            //�Ƿ����յ�
//            if ((range == 0 && center_x == end_x && center_y == end_y) || (range>0&&CalculateTwoGirdDistance(center_x,center_y,end_x,end_y)<=range))
//            {
//                //����յ��е�λ�򷵻ؾ����յ�����ĸ���
//                //if (center.chessStandLimit)
//                //    center = center.parent;
//                //����·��
//                path = GeneratePath(center, allList);
//                //Debug.Log(path.Count);
//                break;
//            }
//            //����ѭ��
//            for (int i = -1; i <= 1; i++)
//            {
//                for (int j = -1; j <= 1; j++)
//                {
//                    #region            �ж��˸����Ƿ��Թ�
//                    if (!IsEffectiveGrid(i, j, center, out Node item, moveType, closeList)) continue;
//                    int x = center_x + i;
//                    int y = center_y + j;
//                    //�Ƿ����յ�
//                    if (x == end_x && y == end_y)
//                    {
//                        //if (center.chessStandLimit && end.chessStandLimit)
//                        //    continue;
//                    }
//                    else if (item.chessStandLimit)
//                    {
//                        continue;
//                    }
//                    #endregion
//                    //����F = G + H
//                    //�ж�G��ԭ����G�Ĵ�С������Ҫ����
//                    int _G = center.G + Calculate_Offset_G(i, j);
//                    if (item.G == 0 || item.G > _G)
//                    {
//                        //Debug.Log("cao");
//                        //���� F G H
//                        item.H = Calculate_H(x, y, end_x, end_y);
//                        item.G = _G;
//                        item.F = item.H + item.G;
//                        item.parent = center;
//                    }

//                    if (!openList.Contains(item))
//                        openList.Add(item);
//                    if (!allList.Contains(item))
//                        allList.Add(item);
//                }
//            }
//            openList.Remove(center);
//            if (openList.Count == 0)
//            {
//                Debug.LogWarning("Ѱ·ʧ�ܣ������޿���·��������");
//                for (int i = 0; i < allList.Count; i++)
//                {
//                    allList[i].ResetSelf();
//                }
//                allList.Clear();
//                path = new Stack<Node>();
//                break;
//            }
//            closeList.Add(center);
//        }
//        allList.Clear();
//        openList.Clear();
//        closeList.Clear();
//    }

//    private static Stack<Node> GeneratePath(Node item, List<Node> allList)
//    {
//        //������β
//        Stack<Node> pathStack = new Stack<Node>();
//        pathStack.Push(item);
//        while (item.parent != null)
//        {
//            item = item.parent;
//            pathStack.Push(item);
//        }
//        for (int i = 0; i < allList.Count; i++)
//        {
//            allList[i].ResetSelf();
//        }
//        allList.Clear();
//        return pathStack;
//    }
//    private static int Calculate_H(int x, int y, int end_x, int end_y)
//    {
//        int X = x - end_x;
//        X = X > 0 ? X : -X;
//        int Y = y - end_y;
//        Y = Y > 0 ? Y : -Y;
//        return 10 * (X + Y);
//    }
//    private static int Calculate_Offset_G(int i, int j)
//    {
//        if (i == 0 || j == 0)
//            return 10;
//        else
//            return 14;
//    }
//}

}
