using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BotAnimations : MonoBehaviour {

    [SerializeField] Rigidbody2D playerRigidbody;
    [SerializeField] Animator animator;
    [SerializeField] PlayerBot player;
    [SerializeField] BotWeaponController weaponController;
    [SerializeField] GameObject pistolObject;
    [SerializeField] GameObject automatonObject;
    [SerializeField] GameObject rocketLauncherObject;
    [SerializeField] GameObject flameThrowerObject;
    [SerializeField] GameObject flameBag;
    [SerializeField] GameObject snipeObject;
    [SerializeField] GameObject mineObject;


    private int pistolLayerIndex;
    private int automatonLayerIndex;
    private int rocketLayerIndex;
    private int flameLyaerIndex;
    private int snipeLayrIndex;
    private int mineLayerIndex;
    private int health;
    private bool isGrounded = true;

    private void Start()
    {
        pistolLayerIndex = animator.GetLayerIndex("TopPistol");
        automatonLayerIndex = animator.GetLayerIndex("TopAutomaton");
        rocketLayerIndex = animator.GetLayerIndex("TopRocketLauncher");
        flameLyaerIndex = animator.GetLayerIndex("TopFlameThrower");
        snipeLayrIndex = animator.GetLayerIndex("TopSnipe");
        mineLayerIndex = animator.GetLayerIndex("TopMine");
    }

    private void OnEnable()
    {
        weaponController.Fire += OnFire;
        weaponController.WeaponChanged += OnWeaponChanged;
    }

    private void OnDisable()
    {
        weaponController.Fire -= OnFire;
        weaponController.WeaponChanged -= OnWeaponChanged;
    }

    private void Update()
    {
        UpdateAnimation();
    }


    private void UpdateAnimation()
    {
        if (health > 0 && player.Health == 0) {
            animator.SetTrigger("Death");
        }
        if (health == 0 && player.Health > 0)  {
            animator.SetTrigger("Live");
        }
        if (player.IsGrounded == false && player.IsGrounded != isGrounded)
        {
            animator.SetTrigger("Jump");
        }
        isGrounded = player.IsGrounded;
        animator.SetBool("Ground", player.IsGrounded);


        health = player.Health;
        animator.SetInteger("Health", health);
        animator.SetBool("Crouch", player.IsCrouch);       
        animator.SetFloat("Speed", Math.Abs(playerRigidbody.velocity.x));
    }

    private void OnFire(WeaponType weapon)
    {
        animator.SetTrigger("Fire");
    }

    private void OnWeaponChanged(WeaponType weapon)
    {
        switch (weapon)
        {
            case WeaponType.PISTOL:
                animator.SetLayerWeight(pistolLayerIndex, 1f);
                animator.SetLayerWeight(automatonLayerIndex, 0f);
                animator.SetLayerWeight(rocketLayerIndex, 0f);
                animator.SetLayerWeight(flameLyaerIndex, 0f);
                animator.SetLayerWeight(snipeLayrIndex, 0f);
                animator.SetLayerWeight(mineLayerIndex, 0f);
                pistolObject.SetActive(true);
                automatonObject.SetActive(false);
                rocketLauncherObject.SetActive(false);
                flameThrowerObject.SetActive(false);
                flameBag.SetActive(false);
                snipeObject.SetActive(false);
                mineObject.SetActive(false);
                break;
            case WeaponType.AUTOMATON:
                animator.SetLayerWeight(pistolLayerIndex, 0f);
                animator.SetLayerWeight(automatonLayerIndex, 1f);
                animator.SetLayerWeight(rocketLayerIndex, 0f);
                animator.SetLayerWeight(flameLyaerIndex, 0f);
                animator.SetLayerWeight(snipeLayrIndex, 0f);
                animator.SetLayerWeight(mineLayerIndex, 0f);
                pistolObject.SetActive(false);
                automatonObject.SetActive(true);
                rocketLauncherObject.SetActive(false);
                flameThrowerObject.SetActive(false);
                flameBag.SetActive(false);
                snipeObject.SetActive(false);
                mineObject.SetActive(false);
                break;
            case WeaponType.FLAMETHROWER:
                animator.SetLayerWeight(pistolLayerIndex, 0f);
                animator.SetLayerWeight(automatonLayerIndex, 0f);
                animator.SetLayerWeight(rocketLayerIndex, 0f);
                animator.SetLayerWeight(flameLyaerIndex, 1f);
                animator.SetLayerWeight(snipeLayrIndex, 0f);
                animator.SetLayerWeight(mineLayerIndex, 0f);
                pistolObject.SetActive(false);
                automatonObject.SetActive(false);
                rocketLauncherObject.SetActive(false);
                flameThrowerObject.SetActive(true);
                flameBag.SetActive(true);
                snipeObject.SetActive(false);
                mineObject.SetActive(false);
                break;
            case WeaponType.ROKET_LAUNCHER:
                animator.SetLayerWeight(pistolLayerIndex, 0f);
                animator.SetLayerWeight(automatonLayerIndex, 0f);
                animator.SetLayerWeight(rocketLayerIndex, 1f);
                animator.SetLayerWeight(flameLyaerIndex, 0f);
                animator.SetLayerWeight(snipeLayrIndex, 0f);
                animator.SetLayerWeight(mineLayerIndex, 0f);
                pistolObject.SetActive(false);
                automatonObject.SetActive(false);
                rocketLauncherObject.SetActive(true);
                flameThrowerObject.SetActive(false);
                flameBag.SetActive(false);
                snipeObject.SetActive(false);
                mineObject.SetActive(false);
                break;
            case WeaponType.LASER:
                animator.SetLayerWeight(pistolLayerIndex, 0f);
                animator.SetLayerWeight(automatonLayerIndex, 0f);
                animator.SetLayerWeight(rocketLayerIndex, 0f);
                animator.SetLayerWeight(flameLyaerIndex, 0f);
                animator.SetLayerWeight(snipeLayrIndex, 1f);
                animator.SetLayerWeight(mineLayerIndex, 0f);
                pistolObject.SetActive(false);
                automatonObject.SetActive(false);
                rocketLauncherObject.SetActive(false);
                flameThrowerObject.SetActive(false);
                flameBag.SetActive(false);
                snipeObject.SetActive(true);
                mineObject.SetActive(false);
                break;
            case WeaponType.MINE:
                animator.SetLayerWeight(pistolLayerIndex, 0f);
                animator.SetLayerWeight(automatonLayerIndex, 0f);
                animator.SetLayerWeight(rocketLayerIndex, 0f);
                animator.SetLayerWeight(flameLyaerIndex, 0f);
                animator.SetLayerWeight(snipeLayrIndex, 0f);
                animator.SetLayerWeight(mineLayerIndex, 1f);
                pistolObject.SetActive(false);
                automatonObject.SetActive(false);
                rocketLauncherObject.SetActive(false);
                flameThrowerObject.SetActive(false);
                flameBag.SetActive(false);
                snipeObject.SetActive(false);
                mineObject.SetActive(true);
                break;
        }
    }
}
