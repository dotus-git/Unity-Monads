using System.Collections.Generic;
using UnityEngine;
using UnityUtils;

public class HealthSystem : Singleton<HealthSystem>
{
    public const int DEFAULT_MAX_HEALTH = 3;

    [Header("Health Settings")]
    public int MaxHealth = DEFAULT_MAX_HEALTH;

    public float CurrentHealth = DEFAULT_MAX_HEALTH;
    public float Spacing = 1.1f;

    [Header("Heart Sprites")]
    public GameObject HeartPrefab;

    public Color Color;
    
    public Sprite FullHeartSprite;
    public Sprite HalfHeartSprite;
    public Sprite EmptyHeartSprite;

    [Header("Heart Container")]
    public Transform HeartContainer; // A GameObject to hold all heart icons in the UI

    private readonly List<GameObject> _hearts = new();

    private void Start()
    {
        base.Awake();
        UpdateHearts();
    }

    [MediatorHandler]
    public void Handle(NewGame message)
    {
        MaxHealth = DEFAULT_MAX_HEALTH;
        CurrentHealth = DEFAULT_MAX_HEALTH;
        UpdateHearts();
    }

    public void TakeDamage(float amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - amount, 0, MaxHealth);
        UpdateHearts();

        if (CurrentHealth <= 0)
            DataMediator.Instance.Publish(new PlayerDead());
    }

    public void Heal(float amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
        UpdateHearts();
    }

    public void IncreaseMaxHealth(int amount)
    {
        MaxHealth += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
        UpdateHearts();
    }

    private void UpdateHearts()
    {
        if (!HeartPrefab || !HeartContainer)
        {
            Debug.LogError("HeartPrefab or HeartContainer is not assigned in the inspector!");

            if (FindObjectsByType<HealthSystem>(FindObjectsSortMode.None).Length > 1)
            {
                Debug.LogError("Multiple HealthSystem instances detected! Please ensure only one HealthSystem is present in the scene.");
            }
            
            return;
        }
        
        // Clear all current hearts
        while (_hearts.Count > 0)
        {
            Destroy(_hearts[0]);
            _hearts.RemoveAt(0);
        }
        
        // Draw hearts based on MaxHealth and CurrentHealth
        for (var i = 0; i < MaxHealth; i++)
        {
            var heart = Instantiate(HeartPrefab, HeartContainer, false);
            _hearts.Add(heart);

            var spriteRenderer = heart.GetComponent<SpriteRenderer>();
            if (!spriteRenderer) continue;

            spriteRenderer.color = Color;
            heart.transform.localPosition = new Vector3(i * Spacing, 0, 0);

            if (CurrentHealth >= i + 1)
                spriteRenderer.sprite = FullHeartSprite; // Full heart
            else if (CurrentHealth > i && CurrentHealth < i + 1)
                spriteRenderer.sprite = HalfHeartSprite; // Half heart
            else
                spriteRenderer.sprite = EmptyHeartSprite; // Empty heart
        }
    }
}