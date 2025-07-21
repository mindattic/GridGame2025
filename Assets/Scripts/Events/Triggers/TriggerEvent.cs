using System.Collections;
using UnityEngine;
using game = GameManagerHelper;
public class TriggerEvent
{
    protected MonoBehaviour context;
    public bool HasExecuted { get; protected set; }

    private IEnumerator routine;

    public TriggerEvent() { }

    public TriggerEvent(IEnumerator routine)
    {
        this.routine = routine;
    }

    //Blocking version — must be used with `yield return`
    //e.g.:
    //yield return new ActorAttackEvent(attackResult, vfx.Attack).Execute(this);

    public virtual IEnumerator Execute(MonoBehaviour context)
    {
        if (context == null)
            yield break;

        this.context = context;
        yield return context.StartCoroutine(Run());
        HasExecuted = true;
    }

    //Fire-and-forget version — runs without yield
    //e.g.:
    //new ActorAttackEvent(attackResult, vfx.Attack).ExecuteAsync(this);
    public void ExecuteAsync(MonoBehaviour context)
    {
        if (context == null)
            return;

        this.context = context;
        context.StartCoroutine(Run());
        HasExecuted = true;
    }

    //Default behavior — either subclass overrides or coroutine runs directly
    public virtual IEnumerator Run()
    {
        if (routine != null)
            yield return routine;
    }
}
