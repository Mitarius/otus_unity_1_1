using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public enum State
    {
        Idle,
        RunningToEnemy,
        RunningFromEnemy,
        BeginAttack,
        Attack,
        BeginShoot,
        Shoot,
        Dying,
        Dead,
    }

    public enum Weapon
    {
        Pistol,
        Bat,
        Fist,
    }

    public Weapon weapon;
    public float runSpeed;
    public float distanceFromEnemy;
    public Character target;
    State state;
    Animator animator;
    Vector3 originalPosition;
    Quaternion originalRotation;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        state = State.Idle;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    public void SetState(State newState)
    {
        state = newState;
    }

    [ContextMenu("Attack")]
    void AttackEnemy()
    {
        switch (weapon) {
            case Weapon.Bat:
                if (target.state != State.Dead & state != State.Dead)
                    state = State.RunningToEnemy;
                break;

            case Weapon.Fist:
                if (target.state != State.Dead & state != State.Dead)
                    state = State.RunningToEnemy;
                break;

            case Weapon.Pistol:
                if (target.state != State.Dead & state != State.Dead)
                    state = State.BeginShoot;
                break;
        }
    }

    bool RunTowards(Vector3 targetPosition, float distanceFromTarget)
    {
        Vector3 distance = targetPosition - transform.position;
        if (distance.magnitude < 0.00001f) {
            transform.position = targetPosition;
            return true;
        }

        Vector3 direction = distance.normalized;
        transform.rotation = Quaternion.LookRotation(direction);

        targetPosition -= direction * distanceFromTarget;
        distance = (targetPosition - transform.position);

        Vector3 step = direction * runSpeed;
        if (step.magnitude < distance.magnitude) {
            transform.position += step;
            return false;
        }

        transform.position = targetPosition;
        return true;
    }

    void FixedUpdate()
    {
        switch (state) {
            case State.Idle:
                animator.SetFloat("Speed", 0.0f);
                transform.rotation = originalRotation;
                break;

            case State.RunningToEnemy:
                animator.SetFloat("Speed", runSpeed);
                if (RunTowards(target.transform.position, distanceFromEnemy))
                    state = State.BeginAttack;
                break;

            case State.BeginAttack:
                switch (weapon){
                    case Weapon.Bat:
                        animator.SetTrigger("MeleeAttack");
                        state = State.Attack;
                        break;

                    case Weapon.Fist:
                        animator.SetTrigger("FistAttack");
                        state = State.Attack;
                        break;
                }
                break;

            case State.Attack:
                if (target.state != State.Dead)
                target.SetState(State.Dying);
                break;

            case State.BeginShoot:
                animator.SetTrigger("Shoot");
                state = State.Shoot;
                break;

            case State.Shoot:
                target.SetState(State.Dying);
                break;

            case State.RunningFromEnemy:
                animator.SetFloat("Speed", runSpeed);
                if (RunTowards(originalPosition, 0.0f))
                    state = State.Idle;
                break;

            case State.Dead:
                break;

            case State.Dying:
                animator.SetBool("Dying", true);
                state = State.Dead;
                break;
        }
    }
}
