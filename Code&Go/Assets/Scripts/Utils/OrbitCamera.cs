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
    private float _pitch;
    private float _yaw;

    private float _iniPitch;
    private float _iniYaw;

    // this is where we want to go.
    private float _currentRotationPitch;
    private float _currentRotationYaw;
    private float _targetRotationPitch;
    private float _targetRotationYaw;

    public FocusPoint Target
    {
        get { return _target; }
        set { _target = value; }
    }

    public float Yaw
    {
        get { return _yaw; }
        private set { _yaw = value; }
    }

    public float Pitch
    {
        get { return _pitch; }
        private set { _pitch = value; }
    }

    public void Move(float yawDelta, float pitchDelta)
    {
        _yaw += yawDelta;
        _pitch += pitchDelta;

        ApplyConstraints();
    }

    public void Reset()
    {
        _yaw = _iniYaw;
        _pitch = _iniPitch;
    }

    private void ApplyConstraints()
    {
        float targetYaw = _target.transform.rotation.eulerAngles.y;
        float targetPitch = _target.transform.rotation.eulerAngles.x;

        float yawDifference = _yaw - targetYaw;
        float pitchDifference = _pitch - targetPitch;

        float yawOverflow = yawDifference - _target.YawLimit;
        float pitchOverflow = pitchDifference - _target.PitchLimit;

        // We'll simply use lerp to move a bit towards the focus target's orientation. Just enough to get back within the constraints.
        // This way we don't need to worry about wether we need to move left or right, up or down.
        //if (yawOverflow > 0) { _yaw = Mathf.Lerp(_yaw, targetYaw, yawOverflow / yawDifference); }
        //if (pitchOverflow > 0) { _pitch = Mathf.Lerp(_pitch, targetPitch, pitchOverflow / pitchDifference); }
    }

    void Awake()
    {
        // initialise our pitch and yaw settings to our current orientation.
        _pitch = transform.rotation.eulerAngles.x;
        _yaw = transform.rotation.eulerAngles.y;

        _iniYaw = _yaw;
        _iniPitch = _pitch;

        _currentRotationYaw = _yaw;
        _currentRotationPitch = _pitch;
    }

    void Update()
    {
        // calculate target positions
        _targetRotationPitch = _pitch;
        _targetRotationYaw = _yaw;

        //transform.rotation = Quaternion.Euler(_targetRotationPitch, _targetRotationYaw, 0);
        _currentRotationPitch = Mathf.Lerp(_currentRotationPitch, _targetRotationPitch, Mathf.Clamp01(Time.smoothDeltaTime * _damping));
        _currentRotationYaw = Mathf.Lerp(_currentRotationYaw, _targetRotationYaw, Mathf.Clamp01(Time.smoothDeltaTime * _damping));
        transform.rotation = Quaternion.Euler(_currentRotationPitch, _currentRotationYaw, 0);

        // offset the camera at distance from the target position.
        Vector3 offset = transform.rotation * (-Vector3.forward * _distance);
        transform.position = _target.transform.position + offset + _target.offset;
    }
    
    public void ResetInmediate()
    {
        Reset();
        _targetRotationPitch = _pitch;
        _targetRotationYaw = _yaw;

        _currentRotationYaw = _yaw;
        _currentRotationPitch = _pitch;

        transform.rotation = Quaternion.Euler(_targetRotationPitch, _targetRotationYaw, 0);

        // offset the camera at distance from the target position.
        Vector3 offset = transform.rotation * (-Vector3.forward * _distance);
        transform.position = _target.transform.position + offset + _target.offset;
    }
}