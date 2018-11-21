using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField]
    private Vector3 _offset;
    [SerializeField]
    [Range(0, 1)]
    private float ySmoothing;
    [SerializeField]
    [Range(0, 1)]
    private float cameraSpeed;

    private bool _paused;

    private Transform _playerTransform;
    private Vector3 _vel = new Vector3();

    //Camera shake vars
    [SerializeField]
    private float _shakeMagnitude = 1f;
    private bool _shaking;
    private DateTime _stopShakeTime = DateTime.MinValue;
    private float _defaultCameraSpeed;

    public delegate void CameraShake();
    public static CameraShake cameraShake;

    private void Awake()
    {
        cameraShake = ShakeCamera;
    }

    private void Start()
    {
        _playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        transform.position = _playerTransform.transform.position + _offset;

        _defaultCameraSpeed = cameraSpeed;
    }

    private void Update()
    {
        Shaking();
    }

    private void FixedUpdate()
    {
        if (_paused || _shaking) return;

        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(_playerTransform.position.x, _playerTransform.position.y * ySmoothing, _playerTransform.position.z) + _offset, ref _vel, cameraSpeed);
    }

    private void ShakeCamera()
    {
        _shaking = true;
        _stopShakeTime = DateTime.Now.AddMilliseconds(1500);
    }

    private void Shaking()
    {
        if (!_shaking) { return; }

        transform.position = new Vector3(transform.position.x + UnityEngine.Random.Range(-1f, 1f) * _shakeMagnitude, transform.position.y + UnityEngine.Random.Range(-1f, 1f) * _shakeMagnitude, transform.position.z);
        _shakeMagnitude /= 1.2f;

        if (_shakeMagnitude < 0.05f) { _shakeMagnitude = 1; _shaking = false; }
    }

    public bool Paused { get { return _paused; } set { _paused = value; } }
}
