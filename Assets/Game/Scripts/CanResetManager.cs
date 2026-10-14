using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanResetManager : MonoBehaviour
{
    [SerializeField] private float resetDelay = 2f;

    private CanTarget[] cans;
    private readonly HashSet<CanTarget> fallenCans = new HashSet<CanTarget>();

    private bool resetInProgress;

    private void Start()
    {
        cans = GetComponentsInChildren<CanTarget>(true);
    }

    public void RegisterCanOnFloor(CanTarget can)
    {
        if (resetInProgress || !fallenCans.Add(can))
        {
            return;
        }

        Debug.Log($"Cans on floor: {fallenCans.Count}/{cans.Length}");

        if (fallenCans.Count == cans.Length)
        {
            StartCoroutine(ResetAfterDelay());
        }
    }

    private IEnumerator ResetAfterDelay()
    {
        resetInProgress = true;

        yield return new WaitForSeconds(resetDelay);

        foreach (CanTarget can in cans)
        {
            can.PrepareForReset();
        }

        Physics.SyncTransforms();

        yield return new WaitForFixedUpdate();

        foreach (CanTarget can in cans)
        {
            can.FinishReset();
        }

        fallenCans.Clear();
        resetInProgress = false;
    }
}