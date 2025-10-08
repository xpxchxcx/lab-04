using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [Header("References")]
    private Camera _camera;
    public BoxCollider2D boundsAsset;
    [SerializeField] public IControllable mainControllable;

    [Header("Camera Settings")]
    public float followSmoothness = 5f;   // how quickly the camera catches up
    public float cameraHeight = -10f;     // z offset for top-down camera
    public Vector2 offset = Vector2.zero; // extra offset if needed
    private Vector3 _velocity = Vector3.zero;

    void Start()
    {
        GetCamera();
        tempPlayerControllableGrabber();
    }

    void tempPlayerControllableGrabber()
    {
        var playerObj = GameObject.FindWithTag("Player");
        mainControllable = playerObj?.GetComponent<IControllable>();
    }

    bool GetCamera()
    {
        _camera = GetComponent<Camera>();
        return _camera != null;
    }

    void LateUpdate()
    {
        if (!EnsureCameraAndPlayer()) return;

        Vector3 cameraPos = _camera.transform.position;
        Vector3 targetPos = mainControllable.WorldPosition();

        // Apply offset + fixed top-down height
        targetPos.x += offset.x;
        targetPos.y += offset.y;
        targetPos.z = cameraHeight;

        // Smooth follow
        _camera.transform.position = Vector3.SmoothDamp(
          cameraPos,
          targetPos,
          ref _velocity,
          1f / followSmoothness
      );

        ClampToCollider();
    }

    void ClampToCollider()
    {
        if (_camera == null || boundsAsset == null) return;

        float camHeight = _camera.orthographicSize * 2;
        float camWidth = camHeight * _camera.aspect;

        Bounds b = boundsAsset.bounds;
        Vector3 pos = _camera.transform.position;

        float minX = b.min.x + camWidth / 2;
        float maxX = b.max.x - camWidth / 2;
        float minY = b.min.y + camHeight / 2;
        float maxY = b.max.y - camHeight / 2;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        _camera.transform.position = pos;
    }

    private bool EnsureCameraAndPlayer()
    {
        if (mainControllable == null) return false;
        if (_camera == null && !GetCamera()) return false;
        return true;
    }
}
