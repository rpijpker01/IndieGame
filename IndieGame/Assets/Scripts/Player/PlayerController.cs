using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool isAttacking { get; set; }

    [SerializeField]
    private float _startingHealth = 100;
    [SerializeField]
    private float _startingMana = 100;
    [SerializeField]
    [Range(0, 1)]
    private float _manaRegenSpeed = 0.05f;
    private float _health;
    private float _mana;
    private bool _isPlayingDyingAnimation = true;

    //Components
    private Collider _collider;

    // Use this for initialization
    private void Start()
    {
        _health = _startingHealth;
        _mana = _startingMana;

        _collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    private void Update()
    {
        Dying();
        UpdateMana();
    }

    private void Dying()
    {
        if (_health <= 0)
        {
            if (_isPlayingDyingAnimation)
            {
                PlayDyingAnimation();
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void UpdateMana()
    {
        if (_mana < _startingMana)
        {
            _mana += _manaRegenSpeed;
        }
        else if (_mana > _startingMana)
        {
            _mana = _startingMana;
        }
    }

    private void PlayDyingAnimation()
    {
        _isPlayingDyingAnimation = false;
    }

    public void TakeDamage(float damage)
    {
        //Subtract damage from current health
        _health -= damage;

        //Camera shake
        CameraFollowPlayer.cameraShake(1 + ((damage / 100) * 2), 750);

        //Display damage number
        GameController.damageNumbersCanvas.DisplayDamageNumber(true, damage, new Vector3(transform.position.x, transform.position.y + _collider.bounds.extents.y, transform.position.z));
    }

    public float GetHealth()
    {
        return _health;
    }

    public float GetMana()
    {
        return _mana;
    }
    public void SetMana(float mana)
    {
        _mana = mana;
    }
}
