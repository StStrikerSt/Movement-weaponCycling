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
    [SerializeField] private AudioClip firingSound;
    [SerializeField] private AudioClip chamberSound;
    [SerializeField] private ParticleSystem muzzleEffectPrefab;
    [SerializeField] private GameObject projectilePrefab;


    private bool isEquipped;
    private float nextFireTime;
    private bool isOutOfAmmo;

    private void Awake()
    {
        //audioSource = transform.parent.GetComponent<AudioSource>();

    }

    private void Update()
    {
        if (!isEquipped)
            return;
    }


    private void HandleFiring()
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
            Quaternion spreadRotation = GetSpreadRotation();
            GameObject projectile = Instantiate(projectilePrefab, firingPoint.position, spreadRotation);

            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

            if (projectileRb != null)
            {
                projectileRb.AddForce(projectile.transform.forward * projectileForce, ForceMode.Impulse);
            }
        }

        if (muzzleEffectPrefab != null)
        {
            ParticleSystem muzzleInstance = Instantiate(
                muzzleEffectPrefab,
                firingPoint.position,
                firingPoint.rotation,
                firingPoint
            );

            muzzleInstance.Play();
            Destroy(muzzleInstance.gameObject, 2f);
        }

        if (audioSource != null && firingSound != null)
        {
            audioSource.PlayOneShot(firingSound);
        }
    }

    private Quaternion GetSpreadRotation()
    {
        Vector3 baseDirection = firingPoint.forward;

        float yaw = Random.Range(-projectileSpread, projectileSpread);
        float pitch = Random.Range(-projectileSpread, projectileSpread);

        Quaternion spreadRotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 finalDirection = spreadRotation * baseDirection;

        return Quaternion.LookRotation(finalDirection);
    }

    public void AttachToPlayer(Transform gunSlot)
    {
        if (gunSlot == null)
            return;

        transform.SetParent(gunSlot);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        isEquipped = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (colliders != null)
        {
            foreach (Collider col in colliders)
            {
                col.isTrigger = true;
            }
        }
    }

    public void InjectDefinition(GunDefination def)
    {
        Defination = def;

        fireRate = def.fireRate;
        projectileForce = def.projectileForce;
        projectileSpread = def.projectileSpread;
        projectileCount = def.projectileCount;
        ammo = def.ammoCount;

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
        if (!isEquipped)
            return;

        HandleFiring();
    }

    public void Fire(Vector3 FiringPoint, Quaternion rotation, GameObject prefab)
    {
        if (!isEquipped)
            return;

        HandleFiring(FiringPoint, rotation, prefab);
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
            Quaternion spreadRotation = GetSpreadRotation();
            GameObject projectile = Instantiate(projectilePrefab, firingPoint.position, spreadRotation);

            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

            if (projectileRb != null)
            {
                projectileRb.AddForce(projectile.transform.forward * projectileForce, ForceMode.Impulse);
            }
        }
    }
}
