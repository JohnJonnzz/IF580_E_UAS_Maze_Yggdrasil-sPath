using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip collectSFX; // Sound effect for coin collection
    public float sfxVolume = 1f; // Volume of the sound effect (0.0 - 1.0)

    void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player
        if (other.CompareTag("Runner"))
        {
            // Play the collection sound effect
            if (collectSFX != null)
            {
                AudioSource.PlayClipAtPoint(collectSFX, transform.position, sfxVolume);
            }

            // Destroy the coin after collection
            Destroy(gameObject);
        }
    }
}
