using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [Header("Injected Defination")]
    [SerializeField] private GunDefination Defination;

    [Header("Sockets / Components")]
    [SerializeField] private Transform artworkSocket;
    [SerializeField] private AudioSource audioSource;

    [Header("Runtime Data")]
    [SerializeField] private GameObject currentArtwork;
    [SerializeField] private float fireRate;
    [SerializeField] private Transform firingPoint;
    [SerializeField] private Collider[] colliders;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float projectileForce;
    [SerializeField] private float projectileSpread;
    [SerializeField] private int projectileCount;
    [SerializeField] private int ammo;
    [SerializeField] private float Damage;
    [SerializeField] private AudioClip firingSound;
    [SerializeField] private AudioClip chamberSound;
    [SerializeField] private ParticleSystem muzzleEffectPrefab;
    [SerializeField] private GameObject projectilePrefab;
    private GameObject holder;
    public bool IsEquipped => isEquipped;



    private bool isEquipped;
    private float nextFireTime;
    private bool isOutOfAmmo;

    private void HandleFiring(Vector3 position, Quaternion rotation)
    {
        if (Defination == null || projectilePrefab == null ||
            ammo <= 0 || fireRate <= 0 || projectileCount <= 0 ||
            Time.time < nextFireTime) return;
        nextFireTime = Time.time + (1f / fireRate);
        ammo--;
        isOutOfAmmo = ammo <= 0;
        for (int i = 0; i < projectileCount; i++)
        {
            Quaternion spreadRotation = GetSpreadRotation(rotation);
            GameObject projectile = Instantiate(projectilePrefab,
                position, spreadRotation, Defination);
            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
            if (projectileRb != null)
                projectileRb.AddForce(projectile.transform.forward *
                    projectileForce, ForceMode.Impulse);
        }
        if (muzzleEffectPrefab != null)
        {
            ParticleSystem effect = Instantiate(muzzleEffectPrefab,
                position, rotation);
            effect.Play();
            Destroy(effect.gameObject, 2f);
        }
        if (audioSource != null && firingSound != null)
            audioSource.PlayOneShot(firingSound);
    }


    private Quaternion GetSpreadRotation(Quaternion rotation)
    {
        float yaw = Random.Range(-projectileSpread, projectileSpread);
        float pitch = Random.Range(-projectileSpread, projectileSpread);
        return rotation * Quaternion.Euler(pitch, yaw, 0f);
    }


    public void AttachToHolder(Transform gunSlot, GameObject owner)
    {
        if (gunSlot == null || owner == null || isEquipped) return;
        holder = owner;
        transform.SetParent(gunSlot, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        isEquipped = true;
        Rigidbody body = GetComponent<Rigidbody>();
        if (body != null)
        {
            if (!body.isKinematic)
            {
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
            }
            body.useGravity = false;
            body.isKinematic = true;
        }
        foreach (Collider col in colliders)
            if (col != null) col.isTrigger = true;
    }

    public void DetachFromHolder(Vector3 dropImpulse)
    {
        if (!isEquipped) return;
        transform.SetParent(null, true);
        isEquipped = false;
        holder = null;
        foreach (Collider col in colliders)
            if (col != null) col.isTrigger = false;
        Rigidbody body = GetComponent<Rigidbody>();
        if (body != null)
        {
            body.isKinematic = false;
            body.useGravity = true;
            body.AddForce(dropImpulse, ForceMode.Impulse);
        }
    }


    public void InjectDefinition(GunDefination def)
    {
        if (def == null)
        {
            Debug.Log("No def was found - returning");
            return;
        }

        Defination = def;
        Debug.Log("def found - " + def);

        fireRate = def.fireRate;
        Debug.Log("fire rate = " + def.fireRate);
        projectileForce = def.projectileForce;
        Debug.Log("projectileForce = " + def.projectileForce);
        projectileSpread = def.projectileSpread;
        Debug.Log("projectileSpread = " + def.projectileSpread);
        projectileCount = def.projectileCount;
        Debug.Log("projectileCount = " + def.projectileCount);
        ammo = def.ammoCount;
        Debug.Log("ammoCount = " + def.ammoCount);
        Damage = def.Damage;
        Debug.Log("Damage = " + def.Damage);
        projectilePrefab = def.ProjectilePrefab;
        Debug.Log("ProjectilePrefab = " + def.ProjectilePrefab);
        isOutOfAmmo = ammo <= 0;
        nextFireTime = 0f;


        firingSound = def.firingSound;
        chamberSound = def.chamberSound;
        muzzleEffectPrefab = def.muzzleEffect;

        InjectArtwork();

        if (audioSource != null && chamberSound != null)
        {
            audioSource.PlayOneShot(chamberSound);
        }
    }

    private void InjectArtwork()
    {
        if (artworkSocket == null)
        {
            Debug.LogWarning("Gun has no artwork socket assigned");
            return;
        }

        if (currentArtwork != null)
        {
            Destroy(currentArtwork);
        }

        if (Defination == null || Defination.artworkPrefab == null)
        {
            Debug.LogWarning("No artwork prefab found on GunDefinition");
            return;
        }

        currentArtwork = Instantiate(Defination.artworkPrefab);

        currentArtwork.transform.SetParent(artworkSocket, false);

        currentArtwork.transform.localPosition = Vector3.zero;
        currentArtwork.transform.localRotation = Quaternion.identity;
        currentArtwork.transform.localScale = Vector3.one;
    }

    public void Fire()
    {
        if (firingPoint == null) return;
        Fire(firingPoint.position, firingPoint.rotation, projectilePrefab);
    }
    public void Fire(Vector3 position, Quaternion rotation, GameObject bulletVisualPrefab)
    {
        if (!isEquipped) return;
        HandleFiring(position, rotation, projectilePrefab);
    }


    private void HandleFiring(Vector3 FiringPoint, Quaternion rotation, GameObject prefab)
    {

        if (Defination == null)
            return;

        if (firingPoint == null)
            return;

        if (projectilePrefab == null)
            return;
        Debug.Log("Gun IS SHOOTING");
        if (ammo <= 0)
            return;

        if (Time.time < nextFireTime)
            return;

        nextFireTime = Time.time + (1f / fireRate);
        ammo--;
        if (ammo <= 0 && !isOutOfAmmo)
        {
            isOutOfAmmo = true;

        }

        for (int i = 0; i < projectileCount; i++)
        {
            Quaternion spreadRotation = GetSpreadRotation(rotation);
            GameObject projectile = Instantiate(projectilePrefab, firingPoint.position, spreadRotation);
            // Immediately after Instantiate(projectilePrefab, ...):
            Projectiles hit = projectile.GetComponent<Projectiles>();
            if (hit != null) hit.Initialise(holder);

            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

            if (projectileRb != null)
            {
                projectileRb.AddForce(projectile.transform.forward * projectileForce, ForceMode.Impulse);
            }
        }
    }
}
