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

    private void OnEnable()
    {
        _target = transform.parent.transform.parent.transform;
        _bounds = _target.GetComponent<Collider>().bounds.extents;
        transform.position = Camera.main.WorldToScreenPoint(_target.position + Vector3.up * _bounds.y);
    }

    private void Update()
    {
        if (_target == null) return;

        if (_playerTransform == null)
            _playerTransform = GameController.playerController.gameObject.transform;

        Vector3 pos = _playerTransform.position - _target.position;
        transform.position = Camera.main.WorldToScreenPoint(_playerTransform.position - new Vector3(pos.x, -pos.y, pos.z) + Vector3.up * _bounds.y);
    }
}
