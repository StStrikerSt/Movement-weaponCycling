using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Image HealthBar;
    public Image ArmourBar;
    public float MaxHealth;
    public float MaxShield;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("HealthKit"))
        {
            Heal(10);
            Destroy(other.gameObject);
        }
        
        if(other.gameObject.CompareTag("ArmourKit"))
        {
            Repair(10);
            Destroy(other.gameObject);
        }
    }

    public void TakeDamage(float Damage)
    {
        if(MaxShield <= 0)
        {
            MaxHealth -= Damage;
        }
        else if(MaxShield >= 0)
        {
            MaxShield -= Damage;
        }
        
        HealthBar.fillAmount = MaxHealth / 100f;
        ArmourBar.fillAmount = MaxShield / 100f;
    }

    public void Heal(float HealingAmount)
    {
        MaxHealth += HealingAmount;
        MaxHealth = Mathf.Clamp(MaxHealth, 0, 100);
        HealthBar.fillAmount = MaxHealth / 100f;
    }

    public void Repair(float RepairAmount)
    {
        MaxShield += RepairAmount;
        MaxShield = Mathf.Clamp(MaxShield, 0, 100);
        ArmourBar.fillAmount = MaxShield / 100f;
    }

    public void OnProjectileHit()
    {
        TakeDamage(20);
    }
}
