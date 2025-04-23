using UnityEngine;

public class coinDestroy : MonoBehaviour
{
    public GameObject coinCollectEffectPrefab;
    public AudioClip coinCollectClip;

    private bool collected = false; 

    void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;

        if (other.CompareTag("Player"))
        {
            collected = true;

            
            if (coinCollectClip != null)
            {
                AudioSource.PlayClipAtPoint(coinCollectClip, transform.position);
            }

            // Visual effect
            if (coinCollectEffectPrefab != null)
            {
                GameObject effect = Instantiate(coinCollectEffectPrefab, transform.position, Quaternion.identity);
                ParticleSystem ps = effect.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Play();
                    Destroy(effect, 0.8f);
                }
            }

            // Tell the player to update coin count
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.AddCoin(); 
            }

            Destroy(gameObject); 
        }
    }
}