// CameraControl.cs

using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraControl : MonoBehaviour
{

    private Camera m_cam;
    private Transform m_selfTrans;
    private bool m_fingerDown = false;

    /// <summary>
    /// ��ָ��������ָλ��
    /// </summary>
    private Vector3 m_oneFingerDragStartPos;
    /// <summary>
    /// ˫ָ���ŵ���һ֡��˫ָ����
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
            // ˫ָ����
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
                // ��ָ����
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
    /// ��ָ����
    /// </summary>
    private void HandleFingerDragMove(Vector2 fingerPos)
    {
        //������
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
    /// ˫ָ����
    /// </summary>
    private void HandleTwoFingerScale()
    {
        float distance = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
        if (-1 == m_twoFingerLastDistance) m_twoFingerLastDistance = distance;
        // ����һ֡�Ƚϱ仯
        float scale = 0.1f * (distance - m_twoFingerLastDistance);
        ScaleCamere(scale);
        m_twoFingerLastDistance = distance;
    }

    /// <summary>
    /// ����������
    /// </summary>
    /// <param name="distance"></param>
    private void HandleMouseScrollWheel(float distance)
    {
        if (0 == distance) return;
        ScaleCamere(distance);
    }

    /// <summary>
    /// ������������ӿ�
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
    /// ��Ļ���껻���3D����
    /// </summary>
    /// <param name="screenPos">��Ļ����</param>
    /// <returns></returns>
    Vector3 GetWorldPos(Vector2 screenPos)
    {
        return m_cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0));
    }
}
