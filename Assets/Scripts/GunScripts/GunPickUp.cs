using UnityEngine;
using UnityEngine.InputSystem;

public class GunPickUp : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform gunSlot;

    [Header("Runtime")]
    [SerializeField] private Gun equippedGun;
    [SerializeField] private Gun nearbyGun;

    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private InputActionReference DropAction;

    public Transform GunSlot => gunSlot;
    public Gun EquippedGun => equippedGun;

    [SerializeField] private InputActionReference fireAction;

    

    private void OnEnable()
    {
        fireAction.action.performed += OnAttack;
        fireAction.action.Enable();
    }

    private void OnDisable()
    {
        fireAction.action.performed -= OnAttack;
        fireAction.action.Disable();
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        if (equippedGun == null)
            return;

        equippedGun.Fire();
    }

    private void OnTriggerEnter(Collider other)
    {
        nearbyGun = other.GetComponentInParent<Gun>();

        if (nearbyGun == null)
            return;

        if (equippedGun == null)
        {
            
        }
    }

    public void CollectGun(Gun gun)
    {
        if (gun == null || gunSlot == null)
            return;

        if (equippedGun != null)
        {
            DropGun(equippedGun);
        }

        equippedGun = gun;
        equippedGun.AttachToPlayer(gunSlot);
    }

    public void DropGun(Gun gun)
    {
        if (gun == null)
            return;

        gun.transform.SetParent(null);

        Rigidbody rb = gun.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;

            rb.AddForce(transform.forward * 2f, ForceMode.Impulse);
        }

        equippedGun = null;

        // Destroy dropped gun after 5 seconds
        Destroy(gun.gameObject, 2f);

        Debug.Log("Gun successfully dropped!");
    }

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed)
            return;

        Debug.Log("INTERACT RECEIVED");

        if (nearbyGun == null)
            return;

        if (equippedGun != null)
            return;

        CollectGun(nearbyGun);
    }

    public void OnDropWeapon(InputValue value)
    {
        if (!value.isPressed)
            return;

        Debug.Log("DROP RECEIVED");

        if (equippedGun == null)
            return;

        DropGun(equippedGun);
    }
}
