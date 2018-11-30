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

    private float _intensity = 0.5f;

    public delegate void CameraShake(float intensity, int durationInMs);
    public static CameraShake cameraShake;

    private static CameraFollowPlayer cam;

    private void Awake()
    {
        cameraShake = ShakeCamera;
        cam = this;
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
        if (_paused || _shaking || GameController.player == null) return;

        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(_playerTransform.position.x, _playerTransform.position.y * ySmoothing, _playerTransform.position.z) + _offset, ref _vel, cameraSpeed);
    }

    private void ShakeCamera(float intensity, int durationInMs = 1500)
    {
        _shaking = true;
        _intensity = intensity;
        _stopShakeTime = DateTime.Now.AddMilliseconds(durationInMs);
    }

    private void Shaking()
    {
        if (!_shaking) { return; }

        transform.position = new Vector3(transform.position.x + UnityEngine.Random.Range(-_intensity, _intensity) * _shakeMagnitude, transform.position.y + UnityEngine.Random.Range(-_intensity, _intensity) * _shakeMagnitude, transform.position.z);
        _shakeMagnitude /= 1.2f;

        if (_shakeMagnitude < 0.05f) { _shakeMagnitude = 0.3f; _shaking = false; }
    }

    public static void InvertCamera()
    {
        cam.transform.rotation = Quaternion.Euler(cam.transform.rotation.eulerAngles.x, cam.transform.rotation.eulerAngles.y + 180, cam.transform.rotation.eulerAngles.z);
        cam._offset = new Vector3(-cam._offset.x, cam._offset.y, -cam._offset.z);
    }

    public bool Paused { get { return _paused; } set { _paused = value; } }
}
