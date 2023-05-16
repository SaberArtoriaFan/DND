// CameraControl.cs

using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraControl : MonoBehaviour
{

    private Camera m_cam;
    private Transform m_selfTrans;
    private bool m_fingerDown = false;

    /// <summary>
    /// 单指滑动的手指位置
    /// </summary>
    private Vector3 m_oneFingerDragStartPos;
    /// <summary>
    /// 双指缩放的上一帧的双指距离
    /// </summary>
    private float m_twoFingerLastDistance = -1;


    private void Awake()
    {
        m_cam = GetComponent<Camera>();
        m_selfTrans = transform;
    }

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            m_fingerDown = true;
            m_oneFingerDragStartPos = GetWorldPos(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            m_fingerDown = false;
            m_twoFingerLastDistance = -1;
        }

        if (m_fingerDown)
        {
            HandleFingerDragMove(Input.mousePosition);
        }
        var distance = Input.GetAxis("Mouse ScrollWheel");
        HandleMouseScrollWheel(distance * 10);
#else
        if (2 == Input.touchCount)
        {
            // 双指缩放
            HandleTwoFingerScale();
        }
        else if (1 == Input.touchCount)
        {
            if (TouchPhase.Began == Input.touches[0].phase)
            {
                m_fingerDown = true;
                m_oneFingerDragStartPos = GetWorldPos(Input.mousePosition);
            }
            else if (TouchPhase.Moved == Input.touches[0].phase)
            {
                // 单指滑动
                HandleFingerDragMove(Input.touches[0].position);
            }
            m_twoFingerLastDistance = -1;
        }
        else
        {
            m_fingerDown = false;
            m_twoFingerLastDistance = -1;
        }
#endif
    }


    /// <summary>
    /// 单指滑动
    /// </summary>
    private void HandleFingerDragMove(Vector2 fingerPos)
    {
        //滑动差
        Vector3 cha = m_oneFingerDragStartPos - GetWorldPos(fingerPos);
        Vector3 newP = m_cam.transform.position;
        newP.x = newP.x + cha.x;
        if (newP.x > 17) { newP.x = 17; }
        if (newP.x < -10) { newP.x = -10; }
        newP.y = newP.y + cha.y;
        if (newP.y > 1.4f) { newP.y = 1.4f; }
        if (newP.y < 0) { newP.y = 0; }

        m_selfTrans.position = newP;
    }

    /// <summary>
    /// 双指缩放
    /// </summary>
    private void HandleTwoFingerScale()
    {
        float distance = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
        if (-1 == m_twoFingerLastDistance) m_twoFingerLastDistance = distance;
        // 与上一帧比较变化
        float scale = 0.1f * (distance - m_twoFingerLastDistance);
        ScaleCamere(scale);
        m_twoFingerLastDistance = distance;
    }

    /// <summary>
    /// 鼠标滚轮缩放
    /// </summary>
    /// <param name="distance"></param>
    private void HandleMouseScrollWheel(float distance)
    {
        if (0 == distance) return;
        ScaleCamere(distance);
    }

    /// <summary>
    /// 缩放摄像机的视口
    /// </summary>
    /// <param name="scale"></param>
    private void ScaleCamere(float scale)
    {
        m_cam.orthographicSize -= scale * 0.1f;
        if (m_cam.orthographicSize < 4)
        {
            m_cam.orthographicSize = 4;
        }
        if (m_cam.orthographicSize > 6)
        {
            m_cam.orthographicSize = 6;
        }
    }

    /// <summary>
    /// 屏幕坐标换算成3D坐标
    /// </summary>
    /// <param name="screenPos">屏幕坐标</param>
    /// <returns></returns>
    Vector3 GetWorldPos(Vector2 screenPos)
    {
        return m_cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0));
    }
}
