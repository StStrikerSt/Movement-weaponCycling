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
        Debug.Log("Attack Received");
        equippedGun.Fire();
    }

    private void OnTriggerEnter(Collider other)
    {
        nearbyGun = other.GetComponentInParent<Gun>();

        if (nearbyGun == null)
            return;
    }

    private void OnTriggerExit(Collider other)
    {
        nearbyGun = null;
    }

    public void CollectGun(Gun gun)
    {
        if (gun == null || gunSlot == null || gun.IsEquipped) return;
        if (equippedGun != null) DropGun(equippedGun);
        gun.AttachToHolder(gunSlot, gameObject);
        equippedGun = gun;
    }
    public void DropGun(Gun gun)
    {
        if (gun == null || gun != equippedGun) return;
        gun.DetachFromHolder(transform.forward * 2f);
        equippedGun = null;
        Destroy(gun.gameObject, 2f);
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
