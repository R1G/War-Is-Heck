using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour {

	GameObject target;
	TroopClass tc;
	public float attackSpeed;
	public int attackDamage;
	public float attackRange;
	float attackTimer = 0f;
	public Animator anim;
	public Weapon weapon;

	void Start() {
		tc = GetComponent<TroopClass>();
		SetAnimLayer();
		attackSpeed=1f;
		attackRange=1f;
		attackDamage=10;
	}

	void Update() {
		// Can only attack when the timer is greater than attack speed, hence;
		// Greater attack speed value actually means more time between attacks
		attackTimer += 0.5f * Time.deltaTime;

		// If there is no target to attack, then find a target in range
		if(target==null) {
			anim.SetBool("InCombat", false);
			attackTimer=0f;
			if(tc.IsStopped()) {
				target = tc.AcquireTarget();
			}
		// Else if there is a target, attack if in range or move to it if not in range
		} else {
			if(ProgUtils.InRange(gameObject,target,attackRange)) {
				if(ProgUtils.InLineOfSight(gameObject,target)) {
					Attack();
				} else {
					tc.Move(target.transform.position);
				}
			} else {
				tc.Move(ProgUtils.GetNearestWithinRange(gameObject,target,attackRange-2f));
			}
		}
	}

	public void Attack() {
		if(attackTimer>=attackSpeed) {
			anim.SetBool("InCombat", true);
			transform.LookAt(target.transform.position);
			anim.SetTrigger ("Hit");
			// Damage dealt to the enemy is a lot less if that enemy is in cover. See Cover System
			if(MapClass.InCover(gameObject,target)) {
				Debug.Log("In Cover!");
				target.GetComponent<Health> ().TakeDamage (attackDamage/8, this.gameObject);
			} else {
				target.GetComponent<Health> ().TakeDamage (attackDamage, this.gameObject);
			}
			attackTimer = 0f;
		}
	}

	public void SetTarget(GameObject attackTarget) {
		target = attackTarget;
	}

	public void SetWeapon(Weapon w) {
		weapon = w;
		SetAnimLayer();
		if(w==null) {
			attackRange = 0f;
			attackSpeed = 0f;
			attackDamage = 0;
		} else {
			attackRange = w.attackRange;
			attackSpeed = w.attackSpeed;
			attackDamage = w.attackDamage;
		}
	}

	public void ClearTarget() {
		target = null;
		attackTimer=0f;
	}

	public bool InCombat() {
		if (target != null)
			return true;
		return false;
	}

	// Depending on what type of Weapon is equipped, a different animation layer is picked
	public void SetAnimLayer() {
		if(weapon==null) {
			anim.SetLayerWeight(1,1f);
			return;
		}
		string weapType = weapon.GetWeaponType();

		if(weapType == "SWORD") {
			anim.SetLayerWeight(4, 1f);
		} else if(weapType == "RIFLE") {
			anim.SetLayerWeight(3,1f);
		} else if(weapType == "PISTOL") {
			anim.SetLayerWeight(2,1f);
		} else {
			anim.SetLayerWeight(1,1f);
		}
	}

}
