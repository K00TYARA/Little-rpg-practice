using UnityEngine;

public class Entity_AnimationEvents : MonoBehaviour {

    private Entity entity;
    private Enemy enemy;

    private void Awake() {
        entity = GetComponentInParent<Entity>();
        enemy = GetComponentInParent<Enemy>();
    }

    public void DamageTargets() => entity.DamageTargets();

    private void DisableMovementAndJump() => entity.EnableMovementAndJump(false);
    private void EnableMovementAndJump() => entity.EnableMovementAndJump(true);
    private void EntityDie() => entity.DisableAnimationAndDestroyEntity();
    private void PlayerDieStopMove() => entity.PlayerDieStopMove();
    private void EnableIsAttacking() => enemy.EnableIsAttacking(false);

}
