using UnityEngine;

public class GunSpawner : MonoBehaviour
{

    public Gun gunPrefab;
    public GunDefination[] definations;
    // Start is called before the first frame update
    void Start()
    {
        SpawnGun();
    }

    private void SpawnGun()
    {
        Gun newGun = Instantiate(gunPrefab, transform.position, Quaternion.identity);

        GunDefination randomdef = definations[Random.Range(0, definations.Length)];

        newGun.InjectDefinition(randomdef);
    }

    public void SpawnGunOnEnemy(GameObject targetGunSlot)
    {
        // Instantiate a gun as above but as a child of the target parsed into this argument
    }


}
