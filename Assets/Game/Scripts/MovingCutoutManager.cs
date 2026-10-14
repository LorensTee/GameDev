
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCutoutManager : MonoBehaviour
{
    [SerializeField] private float resetDelay = 2f;

    private MovingCutoutTarget[] targets;
    private TargetCarrierMovement[] carriers;

    private readonly HashSet<MovingCutoutTarget> hitTargets = new HashSet<MovingCutoutTarget>();

    private bool resetting;

    private void Start()
    {
        targets = GetComponentsInChildren<MovingCutoutTarget>(true);
        carriers = GetComponentsInChildren<TargetCarrierMovement>(true);
    }

    public void RegisterHit(MovingCutoutTarget target)
    {
        if (resetting || !hitTargets.Add(target))
        {
            return;
        }

        Debug.Log($"Cutouts hit: {hitTargets.Count}/{targets.Length}");

        if (targets.Length > 0 &&
            hitTargets.Count >= targets.Length)
        {
            StartCoroutine(ResetRound());
        }
    }

    private IEnumerator ResetRound()
    {
        resetting = true;

        foreach (TargetCarrierMovement carrier in carriers)
        {
            carrier.enabled = false;
        }

        yield return new WaitForSeconds(resetDelay);
        
        foreach (TargetCarrierMovement carrier in carriers)
        {
            carrier.ResetMovement();
        }

        Physics.SyncTransforms();
        
        foreach (MovingCutoutTarget target in targets)
        {
            target.PrepareForReset();
        }

        Physics.SyncTransforms();

        yield return new WaitForFixedUpdate();

        foreach (MovingCutoutTarget target in targets)
        {
            target.FinishReset();
        }
        
        foreach (TargetCarrierMovement carrier in carriers)
        {
            carrier.enabled = true;
        }

        hitTargets.Clear();
        resetting = false;

        Debug.Log("Moving-target round reset.");
    }
}
