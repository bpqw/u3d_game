using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Settings")]
    public GameObject weaponPrefab;
    public Transform weaponSocket;
    
    [Header("Weapon Position")]
    public Vector3 weaponPosition = new Vector3(0.2f, 0f, 0.1f); // Position offset relative to socket
    public Vector3 weaponRotation = new Vector3(0f, -90f, 0f);
    
    private GameObject currentWeapon;
    private Animator characterAnimator;

    void Start()
    {
        characterAnimator = GetComponent<Animator>();
        EquipWeapon();
    }

    void EquipWeapon()
    {
        if (weaponPrefab != null && weaponSocket != null)
        {
            currentWeapon = Instantiate(weaponPrefab, weaponSocket);
            currentWeapon.transform.localPosition = weaponPosition;
            // 添加额外的180度旋转
            Vector3 correctedRotation = weaponRotation + new Vector3(-90f, 0f, 0f);
            currentWeapon.transform.localRotation = Quaternion.Euler(correctedRotation);
            
            if (characterAnimator != null)
            {
                // If needed
                characterAnimator.SetBool("IsArmed", true);
            }
        }
    }

    public void ToggleWeaponVisibility(bool visible)
    {
        if (currentWeapon != null)
        {
            currentWeapon.SetActive(visible);
        }
    }

    public void ChangeWeapon(GameObject newWeaponPrefab)
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }

        weaponPrefab = newWeaponPrefab;
        EquipWeapon();
    }
}

public class WeaponAnimationEvents : MonoBehaviour
{
    private WeaponController weaponController;

    void Start()
    {
        weaponController = GetComponentInParent<WeaponController>();
    }

    public void OnWeaponDraw()
    {
        weaponController.ToggleWeaponVisibility(true);
    }

    public void OnWeaponHolster()
    {
        weaponController.ToggleWeaponVisibility(false);
    }
}