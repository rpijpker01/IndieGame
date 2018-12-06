using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollowParentInWorldSpace : MonoBehaviour
{
    private Transform _playerTransform;
    private Transform _target;
    private Vector3 _position;
    private Vector3 _bounds;
    private float _upperColliderBound;

    public float offset;

    private void OnEnable()
    {
        _target = transform.parent.transform.parent.transform;
        _bounds = _target.GetComponent<Collider>().bounds.extents;
        transform.position = Camera.main.WorldToScreenPoint(_target.position - new Vector3(0, offset, 0) + Vector3.up * _bounds.y);
    }

    private void FixedUpdate()
    {
        if (_target == null) return;

        if (_playerTransform == null)
            _playerTransform = GameController.playerController.gameObject.transform;

        transform.position = Camera.main.WorldToScreenPoint(_target.position - new Vector3(0, offset, 0) + Vector3.up * _bounds.y);
    }
}
