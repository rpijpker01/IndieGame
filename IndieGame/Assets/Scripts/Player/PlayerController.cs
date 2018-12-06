using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool isAttacking { get; set; }

    //Player Stats
    private float _currentHealth;
    private float _healthRegenSpeed;
    private float _currentMana;
    private float _manaRegenSpeed;
    private float _armor;
    private float _damageResistance;
    private float _strength;
    private float _physicalDamage;
    private float _intelligence;
    private float _spellDamage;

    private bool _isPlayingDyingAnimation = true;

    private ItemDrop _droppedItem;

    public bool died = false;

    //Components
    private Collider _collider;
    public Animator animator;

    public enum AnimationState
    {
        Idle,
        Attacking,
        Running,
        Dying,
        Abilitying
    }
    public static AnimationState _animationState;

    // Use this for initialization
    private void Start()
    {
        _currentHealth = GameController.maxHealth;
        _currentMana = GameController.maxMana;

        _collider = GetComponent<Collider>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        Dying();
        UpdateManaAndHealth();
    }

    private void Dying()
    {
        if (_currentHealth <= 0)
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

    private void UpdateManaAndHealth()
    {
        if (_currentMana < GameController.maxMana)
        {
            _currentMana += _manaRegenSpeed * Time.deltaTime;
        }
        else if (_currentMana > GameController.maxMana)
        {
            _currentMana = GameController.maxMana;
        }

        if (_currentHealth < GameController.maxHealth)
        {
            _currentHealth += _healthRegenSpeed * Time.deltaTime;
        }
        else if (_currentHealth > GameController.maxHealth)
        {
            _currentHealth = GameController.maxHealth;
        }
    }

    private void PlayDyingAnimation()
    {
        if (died == false)
        {
            SetAnimationState(AnimationState.Dying);
            GameController.GoToHub();
            GameController.player.GetComponent<PlayerMovement>().enabled = false;
            GameController.player.GetComponent<PlayerAttacks>().enabled = false;
            died = true;
        }
        //_isPlayingDyingAnimation = false;
    }

    public void TakeDamage(float damage)
    {
        float dmgTaken = damage * (1 - (_damageResistance * 0.01f));
        //Subtract damage from current health
        _currentHealth -= dmgTaken;

        //Camera shake
        CameraFollowPlayer.cameraShake(1 + ((dmgTaken / 100) * 2), 750);

        //Display damage number
        GameController.damageNumbersCanvas.DisplayDamageNumber(true, dmgTaken, new Vector3(transform.position.x, transform.position.y + _collider.bounds.extents.y, transform.position.z));

        //Play sound
        GetComponent<SoundPlayer>().PlayRandomAudioClip(10, 14);
    }

    public float GetHealth(bool pToInt = false)
    {
        if (pToInt)
            return Mathf.Round(_currentHealth);
        else
            return _currentHealth;
    }
    public void SetHealth(float health)
    {
        if (died)
            _currentHealth = health;
        else
        {
            float hpPercent = _currentHealth / health;
            _currentHealth = health * hpPercent;
        }
    }
    public void Heal(float pHealthPercent, float pTime)
    {
        float healAmount = GameController.maxHealth * (pHealthPercent * 0.01f);
        _healthRegenSpeed += healAmount / pTime;
        StartCoroutine(NormalizeHealthRegen(pTime));
    }
    private IEnumerator NormalizeHealthRegen(float pTime)
    {
        yield return new WaitForSeconds(pTime);
        _healthRegenSpeed = 2.5f + (_strength * 0.1f);
    }

    public float GetMana(bool pToInt = false)
    {
        if (pToInt)
            return Mathf.Round(_currentMana);
        else
            return _currentMana;
    }
    public void SetMana(float mana)
    {
        if (isAttacking)
            _currentMana = mana;
        else
        {
            float manaPercent = _currentMana / mana;
            _currentMana = mana * manaPercent;
        }
    }
    public void Drink(float pManaPercent, float pTime)
    {
        float drinkAmount = GameController.maxMana * (pManaPercent * 0.01f);
        _manaRegenSpeed += drinkAmount / pTime;
        StartCoroutine(NormalizeManaRegen(pTime));
    }
    private IEnumerator NormalizeManaRegen(float pTime)
    {
        yield return new WaitForSeconds(pTime);
        _manaRegenSpeed = 2.5f + (_intelligence * 0.1f);
    }

    public void SetArmor(float pArmor)
    {
        _armor = pArmor;
        _damageResistance = _armor * 0.1f;
    }

    public void SetStrength(float pStrength)
    {
        _strength = pStrength;
        _healthRegenSpeed = 2.5f + (_strength * 0.1f);
        _physicalDamage = 10 + (_strength * 0.75f);
    }

    public void SetIntelligence(float pIntelligence)
    {
        _intelligence = pIntelligence;
        _manaRegenSpeed = 2.5f + (_intelligence * 0.1f);
        _spellDamage = 50 + (_intelligence * 0.5f);
    }

    public static void SetAnimationState(AnimationState animationState)
    {
        GameController.playerController.animator.SetBool("IsRunning", false);
        GameController.playerController.animator.SetBool("IsDeathing", false);
        GameController.playerController.animator.SetBool("IsIdling", false);
        GameController.playerController.animator.SetBool("IsSwording", false);
        GameController.playerController.animator.SetBool("IsAbilitying", false);
        switch (animationState)
        {
            case AnimationState.Idle:
                GameController.playerController.animator.SetBool("IsIdling", true);
                break;
            case AnimationState.Attacking:
                GameController.playerController.animator.SetBool("IsSwording", true);
                break;
            case AnimationState.Running:
                GameController.playerController.animator.SetBool("IsRunning", true);
                break;
            case AnimationState.Dying:
                GameController.playerController.animator.SetBool("IsDeathing", true);
                break;
            case AnimationState.Abilitying:
                GameController.playerController.animator.SetBool("IsAbilitying", true);
                break;
        }
    }

    public float PhysicalDamage { get { return _physicalDamage; } }
    public float SpellDamage { get { return _spellDamage; } }
}
