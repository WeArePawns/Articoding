using UnityEngine;
using System.Collections;

/// <summary>
/// A basic orbital camera.
/// </summary>
public class OrbitCamera : MonoBehaviour
{
    // This is the target we'll orbit around
    [SerializeField]
    private FocusPoint _target;

    // Our desired distance from the target object.
    [SerializeField]
    private float _distance = 5;

    [SerializeField]
    private float _damping = 2;

    // These will store our currently desired angles
    private Quaternion _pitch;
    private Quaternion _yaw;

    private Quaternion _iniPitch;
    private Quaternion _iniYaw;

    // this is where we want to go.
    private Quaternion _targetRotation;

    private Camera cam;

    public float minFov, maxFov;
    public float sensitivity;
    private float fov = -1;
    private float initSize = -1;

    public FocusPoint Target
    {
        get { return _target; }
        set { _target = value; }
    }

    public float Yaw
    {
        get { return _yaw.eulerAngles.y; }
        private set { _yaw = Quaternion.Euler(0, value, 0); }
    }

    public float Pitch
    {
        get { return _pitch.eulerAngles.x; }
        private set { _pitch = Quaternion.Euler(value, 0, 0); }
    }

    public void Move(float yawDelta, float pitchDelta)
    {
        _yaw *= Quaternion.Euler(0, yawDelta, 0);
        _pitch *= Quaternion.Euler(pitchDelta, 0, 0);
        ApplyConstraints();
    }

    public void Reset()
    {
        _yaw = _iniYaw;
        _pitch = _iniPitch;
        fov = initSize;
    }

    private void ApplyConstraints()
    {

        Quaternion targetYaw = Quaternion.Euler(0, _target.transform.rotation.eulerAngles.y, 0);
        Quaternion targetPitch = Quaternion.Euler(_target.transform.rotation.eulerAngles.x, 0, 0);

        float yawDifference = Quaternion.Angle(_yaw, targetYaw);
        float pitchDifference = Quaternion.Angle(_pitch, targetPitch);

        float yawOverflow = yawDifference - _target.YawLimit;
        float pitchOverflow = pitchDifference - _target.PitchLimit;

        // We'll simply use lerp to move a bit towards the focus target's orientation. Just enough to get back within the constraints.
        // This way we don't need to worry about wether we need to move left or right, up or down.
        if (yawOverflow > 0) { _yaw = Quaternion.Slerp(_yaw, targetYaw, yawOverflow / yawDifference); }
        if (pitchOverflow > 0) { _pitch = Quaternion.Slerp(_pitch, targetPitch, pitchOverflow / pitchDifference); }

    }

    void Awake()
    {
        // initialise our pitch and yaw settings to our current orientation.
        _pitch = Quaternion.Euler(transform.rotation.eulerAngles.x, 0, 0);
        _yaw = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

        _iniYaw = _yaw;
        _iniPitch = _pitch;

        cam = GetComponent<Camera>();
    }

    void Update()
    {
        // calculate target positions
        _targetRotation = _yaw * _pitch;

        transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, Mathf.Clamp01(Time.smoothDeltaTime * _damping));

        // offset the camera at distance from the target position.
        Vector3 offset = transform.rotation * (-Vector3.forward * _distance);
        transform.position = _target.transform.position + offset + _target.offset;

        // Zoom con la rueda del ratón
        if (fov == -1) // Tiene que ir aqui porque el CameraFit tambien toca el ortographicSize
        {
            fov = cam.orthographicSize;
            initSize = cam.orthographicSize;
        }
        fov -= Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        fov = Mathf.Clamp(fov, minFov, maxFov);
    }

    private void LateUpdate()
    {
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, fov, Time.deltaTime * 3.0f);
    }
}