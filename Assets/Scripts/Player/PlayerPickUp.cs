using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
[RequireComponent(typeof(CircleCollider2D))]
public class PlayerPickUp : MonoBehaviour
{
    
    public AudioClip hitSound; // Clip âm thanh va chạm
    private AudioSource audioSource; // Component AudioSource

    PlayerStats playerStats;
    CircleCollider2D playerCollector;
    public float pullSpeed;
    private void Start()
    {
        playerStats = GetComponentInParent<PlayerStats>();
    }
    public void SetMagnet(float r){
        if(!playerCollector) playerCollector = GetComponent<CircleCollider2D>();
        playerCollector.radius = r;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent(out Pickup p)){
            PlayCollisionSound();
            p.Collect(playerStats,pullSpeed);
        }
    }
        private void PlayCollisionSound()
{
    audioSource = GetComponent<AudioSource>();
    hitSound = Resources.Load<AudioClip>("Audio/hurt");
    // Play your collision sound here
    if (audioSource != null)
    {
        audioSource.PlayOneShot(hitSound); // Replace 'yourCollisionClip' with your audio clip
    }
}
    // private void OnTriggerEnter2D(Collider2D other) {
    //     if(other.gameObject.TryGetComponent(out ICollect Collectible)){
    //         Collectible.Collect();
    //     }
    // }
}
