using UnityEngine;

public class Entity_AnimationEvents : MonoBehaviour {

    private Entity entity;
    private Enemy enemy;

    private void Awake() {
        entity = GetComponentInParent<Entity>();
        enemy = GetComponentInParent<Enemy>();
    }

    public void DamageTargets() => entity.DamageTargets();

    private void DisableMovement() => entity.EnableMovement(false);
    private void EnableMovement() => entity.EnableMovement(true);
    private void EntityDie() => entity.EntityDie();
    private void PlayerDie() => entity.PlayerDie();
}
