using UnityEngine;

public class Entity_AnimationEvents : MonoBehaviour {

    private Entity entity;

    private void Awake() {
        entity = GetComponentInParent<Entity>();
    }

    private void DamageTargets() => entity.DamageTargets();
    private void EntityDie() => entity.EntityDie();
    private void PlayerDie() => entity.PlayerDie();
    private void UpdateState() => entity.UpdateState();
}
