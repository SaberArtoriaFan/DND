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
    //    public float updateDistance = 1f;  // ����Ŀ����Զʱ����·��

    //    private List<Vector3> path;
    //    private int currentPathIndex = 0;

    //    private bool isCalculatingPath = false;  // �Ƿ����ڼ���·��
    //    private Thread calculateThread;          // ����·��������߳�

    //    private void Start()
    //    {
    //        // ��ʼ��·��
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
    //            // ����Ŀ��㣬�л�����һ��Ŀ���
    //            currentPathIndex++;
    //            if (currentPathIndex >= path.Count)
    //            {
    //                UpdatePath();  // ����·��
    //                return;
    //            }

    //            targetPos = path[currentPathIndex];
    //        }

    //        // ���Ƶ�λ�ƶ��ͳ���
    //        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    //        transform.LookAt(targetPos);

    //        // ����Ƿ���Ҫ����·��
    //        if (Vector3.Distance(transform.position, targetPos) > updateDistance || !IsPathValid())
    //            UpdatePath();
    //    }

    //    private bool IsPathValid()
    //    {
    //        for (int i = currentPathIndex + 1; i < path.Count; i++)
    //        {
    //            if (!IsWalkable(path[i])) // ·���м��в������ߵ�λ��
    //                return false;
    //        }

    //        return true;
    //    }

    //    private bool IsWalkable(Vector3 position)
    //    {
    //        foreach (GameObject obstacle in GameObject.FindGameObjectsWithTag("Obstacle"))
    //        {
    //            float distance = Vector3.Distance(obstacle.transform.position, position);

    //            if (distance < 1f) // ��������ϰ���Ĵ�СΪ1
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

    //        lock (path)  // ������·��֮ǰ������վ�·��
    //        {
    //            path.Clear();
    //            currentPathIndex = 0;
    //        }

    //        lock (path)  // ������·��
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
    //        // �����ϰ����б�
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
