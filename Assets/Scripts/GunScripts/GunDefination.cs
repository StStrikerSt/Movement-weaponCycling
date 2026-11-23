using UnityEngine;

[CreateAssetMenu(menuName = "Guns/gun Defination")]
public class GunDefination : ScriptableObject
{
    public GameObject artworkPrefab;
    public GameObject ProjectilePrefab;
    public ParticleSystem muzzleEffect;

    public AudioClip firingSound;
    public AudioClip chamberSound;

    public float fireRate;

    public float projectileForce;
    public int projectileCount;
    public float projectileSpread;

    public int ammoCount;


}
