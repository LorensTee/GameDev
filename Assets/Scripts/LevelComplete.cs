using UnityEngine;

public class LevelComplete : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Congrats, you've reached the end!");
    }
}