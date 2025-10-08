using UnityEngine;

public class Entity_AnimationEvents : MonoBehaviour {

    private Entity entity;

    private void Awake() {
        entity = GetComponentInParent<Entity>();
    }

    private void DamageTargets() => entity.DamageTargets();
    private void DisableAnimatorAfterEntityDie() => entity.DisableAnimatorAfterEntityDie();
    private void UpdateState() => entity.UpdateState();
}
