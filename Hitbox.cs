using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public interface IHitboxResponder
{
    void CollisionedWith(Collider2D collider);
}
public enum ColliderState
{
    Closed,
    Open,
    Colliding
}

public class Hitbox : MonoBehaviour
{

    [SerializeField] private LayerMask layermask;
    [SerializeField] private Color inactiveColor;
    [SerializeField] private Color collisionOpenColor;
    [SerializeField] private Color collidingColor;

    [SerializeField] private bool defaultOpen = false;
    [SerializeField] private bool isComboHitbox = false;

    public float knockBack = 0;

    public List<Bounds> hitboxes = new List<Bounds>();

    private List<Collider2D> damagedColliders = new List<Collider2D>();

    private ColliderState colliderState;

    private IHitboxResponder responder = null;

    public int direction => Mathf.Clamp((int)transform.root.localScale.x, -1, 1);

    private void OnEnable()
    {
        if (defaultOpen)
            colliderState = ColliderState.Open;
        else colliderState = ColliderState.Closed;
    }

    private void OnDisable()
    {
        damagedColliders.Clear();
        colliderState = ColliderState.Closed;
    }

    private void Update()
    {

        if (colliderState == ColliderState.Closed) return;

        Collider2D[] collidersFound = new Collider2D[0];
        foreach (Bounds box in hitboxes)
        {
            Vector3 position = new Vector2(transform.root.position.x + (box.center.x * direction), transform.root.position.y + box.center.y);
            if (collidersFound.Length == 0)
                collidersFound = Physics2D.OverlapBoxAll(transform.position + box.center * direction, box.size, 0, layermask);
            else
            {
                collidersFound = collidersFound.Concat(Physics2D.OverlapBoxAll(position + box.center, box.size, 0, layermask)).ToArray<Collider2D>();
            }
        }
        colliderState = collidersFound.Length > 0 ? ColliderState.Colliding : ColliderState.Open;
        foreach (Collider2D collider in collidersFound)
        {
            if (!damagedColliders.Contains(collider))
            {
                responder.CollisionedWith(collider);
                if (isComboHitbox && collider != null)
                    damagedColliders.Add(collider);
            }
        }
      
    }


    public void SetResponder(IHitboxResponder _responder)
    {
        responder = _responder;
    }
    public void OpenHitbox()
    {
        colliderState = ColliderState.Open;
    }

    public void CloseHitbox()
    {
        colliderState = ColliderState.Closed;
    }
    private void OnDrawGizmos()
    {
        CheckGizmoColor();
        foreach (Bounds box in hitboxes)
        {
            Vector3 position = new Vector2(transform.root.position.x + box.center.x * direction, transform.root.position.y + box.center.y);
            Gizmos.DrawWireCube(position, box.size);
        }
    }
    private void CheckGizmoColor()
    {
        switch (colliderState)
        {

            case ColliderState.Closed:
                Gizmos.color = inactiveColor;
                break;

            case ColliderState.Open:
                Gizmos.color = collisionOpenColor;
                break;

            case ColliderState.Colliding:
                Gizmos.color = collidingColor;
                break;
        }
    }
}
