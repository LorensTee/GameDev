using System.Collections;
using TMPro;
using UnityEngine;
using StarterAssets;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player")]
    public Transform player;
    public Transform spawnPoint;
    public HealthManager healthManager;
    public ThirdPersonController playerController;
    public CharacterController characterController;

    [Header("Laser System")]
    public LaserSpawner laserSpawner;

    [Header("Timer")]
    public float timeLimit = 67f;

    [Header("UI")]
    public TMP_Text healthText;
    public TMP_Text timerText;
    public TMP_Text messageText;

    private float timeLeft;
    private bool timerRunning;
    private bool gameOver;

    private Vector3 spawnPosition;
    private Quaternion spawnRotation;

    private float baseMoveSpeed;
    private float baseSprintSpeed;
    private Coroutine speedCoroutine;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        if (healthManager == null)
            healthManager = player.GetComponent<HealthManager>();
        if (healthManager == null)
            healthManager = player.GetComponentInParent<HealthManager>();
        if (healthManager == null)
            healthManager = player.GetComponentInChildren<HealthManager>();

        if (playerController == null)
            playerController = player.GetComponent<ThirdPersonController>();
        if (playerController == null)
            playerController = player.GetComponentInParent<ThirdPersonController>();

        if (characterController == null && player != null)
            characterController = player.GetComponent<CharacterController>();
        if (characterController == null && player != null)
            characterController = player.GetComponentInParent<CharacterController>();

        if (spawnPoint != null)
        {
            spawnPosition = spawnPoint.position;
            spawnRotation = spawnPoint.rotation;
        }
        else
        {
            spawnPosition = player.position;
            spawnRotation = player.rotation;
        }

        if (playerController != null)
        {
            baseMoveSpeed = playerController.MoveSpeed;
            baseSprintSpeed = playerController.SprintSpeed;
        }

        healthManager.ResetHealth();
        UpdateHealthUI(healthManager.CurrentHealth);
        timeLeft = timeLimit;
        UpdateTimerUI();
        
        timerRunning = false;
        gameOver = false;
        if (messageText != null)
            messageText.text = "Step out to begin!";

        if (laserSpawner != null)
            laserSpawner.StartSpawning();
    }

    private void Update()
    {
        if (!timerRunning || gameOver)
            return;

        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            ResetRun();
            return;
        }

        UpdateTimerUI();
    }

    public bool IsRunActive => timerRunning;
    
    public void StartTimer()
    {
        if (timerRunning || gameOver)
            return;

        timerRunning = true;
        timeLeft = timeLimit;
        UpdateTimerUI();

        if (messageText != null)
            messageText.text = "";
    }
    
    public void StartRun()
    {
        StartTimer();
    }

    public void PlayerDied()
    {
        if (gameOver)
            return;

        ResetRun();
    }

    private void ResetRun()
    {
        bool wasTimerRunning = timerRunning;
        timerRunning = false;

        if (laserSpawner != null)
            laserSpawner.StopSpawning();
        DestroyAllLasers();
        ResetPlayerSpeed();
        ResetAllPanels();
        if (healthManager != null)
            healthManager.ResetHealth();
        
        if (playerController != null)
            playerController.enabled = false;
        if (characterController != null)
            characterController.enabled = false;

        player.position = spawnPosition;
        player.rotation = spawnRotation;
        Physics.SyncTransforms();

        if (characterController != null)
            characterController.enabled = true;
        if (playerController != null)
            playerController.enabled = true;
        
        if (laserSpawner != null)
            laserSpawner.StartSpawning();

        if (wasTimerRunning)
        {
            timerRunning = true;
            timeLeft = timeLimit;
            UpdateTimerUI();
            if (messageText != null)
                messageText.text = "";
        }
        else if (messageText != null)
        {
            messageText.text = "Step out to begin!";
        }
    }

    public void WinRun()
    {
        if (gameOver)
            return;

        gameOver = true;
        timerRunning = false;

        if (laserSpawner != null)
            laserSpawner.StopSpawning();
        DestroyAllLasers();
        ResetPlayerSpeed();

        if (messageText != null)
            messageText.text = "YOU WIN!";
    }

    public void UpdateHealthUI(int health)
    {
        if (healthText != null)
            healthText.text = "Health: " + health;
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
            timerText.text = "TIME: " + Mathf.CeilToInt(timeLeft);
    }

    public void GiveSpeedBoost(ThirdPersonController controller, float multiplier, float duration)
    {
        if (controller == null)
            return;

        if (speedCoroutine != null)
            StopCoroutine(speedCoroutine);

        controller.MoveSpeed = baseMoveSpeed * multiplier;
        controller.SprintSpeed = baseSprintSpeed * multiplier;

        speedCoroutine = StartCoroutine(RestoreSpeedAfter(duration));
    }

    private IEnumerator RestoreSpeedAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        ResetPlayerSpeed();
    }

    private void ResetPlayerSpeed()
    {
        if (speedCoroutine != null)
        {
            StopCoroutine(speedCoroutine);
            speedCoroutine = null;
        }

        if (playerController != null)
        {
            playerController.MoveSpeed = baseMoveSpeed;
            playerController.SprintSpeed = baseSprintSpeed;
        }
    }

    private void ResetAllPanels()
    {
        BonusPanel[] panels = FindObjectsByType<BonusPanel>(FindObjectsInactive.Exclude);
        foreach (BonusPanel panel in panels)
            panel.ResetPanel();
    }

    private void DestroyAllLasers()
    {
        MoveToStart[] movers = FindObjectsByType<MoveToStart>(FindObjectsInactive.Exclude);
        foreach (MoveToStart mover in movers)
        {
            if (mover != null)
                Destroy(mover.gameObject);
        }
        
        Laser[] lasers = FindObjectsByType<Laser>(FindObjectsInactive.Exclude);
        foreach (Laser laser in lasers)
        {
            if (laser != null && laser.GetComponentInParent<MoveToStart>() == null)
                Destroy(laser.gameObject);
        }
    }
}
