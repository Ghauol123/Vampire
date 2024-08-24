using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BLBookWeapon : Weapon
{
    public int numberOfProjectiles = 5; // Number of projectiles to spawn around the player
    public float rotationSpeed = 50f; // Speed of rotation around the player
    public float radius = 1.5f; // Radius of the circle around the player
    public float currentCooldowns;
    private List<Projectile> activeProjectiles = new List<Projectile>();
    Projectile newProjectile;
    protected override void Start()
    {
        base.Start();
        SpawnProjectiles();
    }

    protected override void Update()
    {
        currentCooldowns = currentStats.cooldown;
        numberOfProjectiles = currentStats.number;
        rotationSpeed = currentStats.speed * 10;

        base.Update();

        // Update the position of the projectiles to rotate around the player
        if (activeProjectiles.Count > 0)
        {
            for (int i = 0; i < activeProjectiles.Count; i++)
            {
                if (activeProjectiles[i] != null)
                {
                    float angle = (360f / numberOfProjectiles) * i + Time.time * rotationSpeed;
                    Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * radius;
                    activeProjectiles[i].transform.position = owner.transform.position + offset;
                }
            }
        }
    }

    private void SpawnProjectiles()
    {
        // Spawn projectiles around the player at the start and keep them rotating
        for (int i = 0; i < numberOfProjectiles; i++)
        {
            float angle = (360f / numberOfProjectiles) * i;
            Vector3 spawnPosition = owner.transform.position + (Vector3)GetSpawnOffset(angle);

            newProjectile = Instantiate(
                currentStats.projectilePrefabs,
                spawnPosition,
                Quaternion.identity
            );

            newProjectile.weapon = this;
            newProjectile.owner = owner;
            activeProjectiles.Add(newProjectile);
        }
    }

    protected override bool Attack(int attackCount = 1)
    {
        if (!canAttack()) return false;
        SpawnProjectiles();
        if (currentCooldown <= 0) currentCooldown += currentStats.cooldown;
        return true;
    }

    protected virtual Vector2 GetSpawnOffset(float spawnAngle = 0)
    {
        // Calculate the spawn offset based on the angle to position the projectiles in a circle around the player
        return new Vector2(
            Mathf.Cos(spawnAngle * Mathf.Deg2Rad) * radius,
            Mathf.Sin(spawnAngle * Mathf.Deg2Rad) * radius
        );
    }
}
