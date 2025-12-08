using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMonster : MonoBehaviour
{
    [Header("오브젝트 체력")]
    public float maxHealth = 50f;
    private float currentHealth;

    [SerializeField] private AudioClip hitSound;  
    [SerializeField] private float hitFlashDuration = 0.15f; 


    private AudioSource audioSource;
    private SpriteRenderer[] spriteRenderers;
    private bool isFlashing = false;

    private Animator animator;

    public System.Action onDead;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    private void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponentInChildren<Animator>();
    }

    public void ApplyDamage(AttackInfo info)
    {
        currentHealth -= info.damage;

        PlayHitEffect();

        if (animator != null)
            animator.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            if (animator != null)
                animator.SetTrigger("Die");

            onDead?.Invoke();

            Destroy(gameObject, 2f);
        }
    }

    private void PlayHitEffect()
    {
        if (!isFlashing)
            StartCoroutine(HitFlashCoroutine());

        if (audioSource != null && hitSound != null)
            audioSource.PlayOneShot(hitSound);
    }

    IEnumerator HitFlashCoroutine()
    {
        isFlashing = true;

        List<Color> originalColors = new List<Color>();
        foreach (var sr in spriteRenderers)
            originalColors.Add(sr.color);

        foreach (var sr in spriteRenderers)
            sr.color = Color.red;

        yield return new WaitForSeconds(hitFlashDuration);

        for (int i = 0; i < spriteRenderers.Length; i++)
            spriteRenderers[i].color = originalColors[i];

        isFlashing = false;
    }

    public void PlayDieSound() { }
}
