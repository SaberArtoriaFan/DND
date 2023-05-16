using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace XianXia.Terrain
{
    //public class FollowUnit : MonoBehaviour
    //{
    //    public GameObject target;
    //    public float speed = 5f;
    //    public float updateDistance = 1f;  // 距离目标点多远时更新路径

    //    private List<Vector3> path;
    //    private int currentPathIndex = 0;

    //    private bool isCalculatingPath = false;  // 是否正在计算路径
    //    private Thread calculateThread;          // 处理路径计算的线程

    //    private void Start()
    //    {
    //        // 初始化路径
    //        UpdatePath();
    //    }

    //    private void Update()
    //    {
    //        if (path.Count == 0)
    //            return;

    //        Vector3 targetPos = path[currentPathIndex];
    //        float distance = Vector3.Distance(transform.position, targetPos);
    //        E
    //        if (distance < 0.1f)
    //        {
    //            // 到达目标点，切换至下一个目标点
    //            currentPathIndex++;
    //            if (currentPathIndex >= path.Count)
    //            {
    //                UpdatePath();  // 更新路径
    //                return;
    //            }

    //            targetPos = path[currentPathIndex];
    //        }

    //        // 控制单位移动和朝向
    //        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    //        transform.LookAt(targetPos);

    //        // 检查是否需要更新路径
    //        if (Vector3.Distance(transform.position, targetPos) > updateDistance || !IsPathValid())
    //            UpdatePath();
    //    }

    //    private bool IsPathValid()
    //    {
    //        for (int i = currentPathIndex + 1; i < path.Count; i++)
    //        {
    //            if (!IsWalkable(path[i])) // 路径中间有不可行走的位置
    //                return false;
    //        }

    //        return true;
    //    }

    //    private bool IsWalkable(Vector3 position)
    //    {
    //        foreach (GameObject obstacle in GameObject.FindGameObjectsWithTag("Obstacle"))
    //        {
    //            float distance = Vector3.Distance(obstacle.transform.position, position);

    //            if (distance < 1f) // 这里假设障碍物的大小为1
    //            {
    //                return false;
    //            }
    //        }

    //        return true;
    //    }

    //    private void UpdatePath()
    //    {
    //        if (!isCalculatingPath)
    //        {
    //            isCalculatingPath = true;
    //            calculateThread = new Thread(CalculatePath);
    //            calculateThread.Start();
    //        }
    //    }

    //    private void CalculatePath()
    //    {
    //        List<Vector3> obstacles = new List<Vector3>();
    //        foreach (GameObject obstacle in GameObject.FindGameObjectsWithTag("Obstacle"))
    //        {
    //            obstacles.Add(obstacle.transform.position);
    //        }

    //        AStarPathfinding pathfinding = new AStarPathfinding();

    //        lock (path)  // 计算新路径之前，先清空旧路径
    //        {
    //            path.Clear();
    //            currentPathIndex = 0;
    //        }

    //        lock (path)  // 计算新路径
    //        {
    //            path.AddRange(pathfinding.FindPath(transform.position, target.transform.position, obstacles));
    //        }

    //        isCalculatingPath = false;
    //    }

    //    private void FixedUpdate()
    //    {
    //        UpdateObstacles();
    //    }

    //    private void UpdateObstacles()
    //    {
    //        // 更新障碍物列表
    //        List<Vector3> obstacles = new List<Vector3>();
    //        foreach (GameObject obstacle in GameObject.FindGameObjectsWithTag("Obstacle"))
    //        {
    //            obstacles.Add(obstacle.transform.position);
    //        }

    //        AStarPathfinding.SetObstacles(obstacles);
    //    }

    //    private void OnDrawGizmosSelected()
    //    {
    //        Gizmos.color = Color.yellow;
    //        Gizmos.DrawSphere(transform.position, 0.2f);

    //        Gizmos.color = Color.red;
    //        Gizmos.DrawWireSphere(transform.position, updateDistance);
    //    }
    //}
}
