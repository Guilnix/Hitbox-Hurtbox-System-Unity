using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IHurtboxResponder
{
    void HittedBySomething(float _damage, int _hitDirection, float _knockBackForce);
}
public class Hurtbox : MonoBehaviour
{
    private ColliderState collidingState;
    private IHurtboxResponder responder;

    [SerializeField] private bool defaultOpen = true;
    private void Start()
    {
        if (defaultOpen)
            OpenHurtbox();
        else CloseHurtbox();
    }
    public void SetResponder(IHurtboxResponder _responder)
    {
        responder = _responder;
    }

    public void CloseHurtbox()
    {
        collidingState = ColliderState.Closed;
    }

    public void OpenHurtbox()
    {
        collidingState = ColliderState.Open;
    }
    public void HitResponder(float _damage, int _hitDirection, float _knockBackForce)
    {
        if (collidingState == ColliderState.Open)
            responder.HittedBySomething(_damage, _hitDirection, _knockBackForce);
    }
}
