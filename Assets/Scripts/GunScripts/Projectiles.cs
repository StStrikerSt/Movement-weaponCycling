using UnityEngine;
public class Projectiles : MonoBehaviour
{
    private GameObject shooter;
    private string shooterTag;
    private bool hasHit;
    private float projectileDamage;
    private PlayerHealth health;
    public GunDefination firedFrom;
    private void Awake() 
    {
        Destroy(gameObject, 5f); 
    }
    private void OnTriggerEnter(Collider other)
    {
        if (hasHit || other.GetComponentInParent<Projectiles>() != null)
            return;
        if (shooter != null && other.transform.IsChildOf(shooter.transform))
            return;
        Transform target = other.transform;
        while (target != null && !target.CompareTag("Player") &&
               !target.CompareTag("Enemy")) target = target.parent;
        if (target != null)
        {
            if (target.tag == shooterTag) return;
            hasHit = true;
            health.TakeDamage(firedFrom.Damage);
        }
        else
        {
            if (other.isTrigger) return;
            hasHit = true;
        }
        Destroy(gameObject);
    }
    public void Initialise(GameObject owner)
    {
        shooter = owner;
        shooterTag = owner != null ? owner.tag : null;
        owner.GetComponentInChildren<Gun>();
        Debug.Log("Owners gun" + owner);
    }
}

