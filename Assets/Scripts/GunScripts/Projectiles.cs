using UnityEngine;

public class Projectiles : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("Gun IS SHOOTING");
    }
    // Destroy self on trigger enter Unless the colision was with another projectile
    private void OnTriggerEnter(Collider other)
    {
        Projectiles projectile = other.GetComponent<Projectiles>();

        if (other.CompareTag("Enemy") == true)
        {
            Destroy(other.gameObject);
        }

        if (projectile != null) // Must have hit a projectile
            return;
       
    }
}
