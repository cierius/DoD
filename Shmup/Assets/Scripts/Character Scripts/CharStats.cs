using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharStats : MonoBehaviour
{
    private CharController charController;
    private List<TextMesh> ammoHud = new List<TextMesh>(2);
    private Slider reloadingSlider;

    [Header("Movement")]
    public bool isMoveable = true;
    public float speedBase = 15f;
    public float speedAdditive = 0f; // Used for speed boosts / bonus speed upgrades

    [Header("Health & Defense")]
    public float healthBase = 100; // Max health without any additives
    public float healthAdditive = 0f;
    public float healthTotal; // Max health with additives
    public float healthCurrent = 100f;
    public float healthRegen = 0f; // Hp regen per sec (ticks 4 times per sec)
    public float damageReductionPercentage = 0f;

    [Header("Weapon Modifiers")]
    public float damageAdditive = 0f;
    public float fireRatePercentage = 0;
    public float reloadSpeedPercentage = 0f;
    public int critMultiplierAdditive = 0;
    public int critChanceAdditive = 0;

    [Header("Weapons & Ammo")]
    public List<WeaponBase> weapons = new List<WeaponBase>();
    public float[] weaponLastFired = new float[4] {0f, 0f, 0f, 0f};
    public int[] ammoInMag = new int[] { 0, 0, 0, 0 }; // 4 weapons - 4 mags 
    public int[] magsInInventory = new int[] { 0, 0, 0, 0 };
    public int[] magCarryMax = new int[] { 0, 0, 0, 0 };

    public WeaponBase weaponEquipped;
    public int currWeaponIndex = 0; // 0 is rifle



    private void Awake()
    {
        charController = GetComponent<CharController>();
        ammoHud.Add(GameObject.Find("Ammo_HUD/Current_Mag").GetComponent<TextMesh>());
        ammoHud.Add(GameObject.Find("Ammo_HUD/Total_Ammo").GetComponent<TextMesh>());

        reloadingSlider = GameObject.Find("AmmoAndShooting_Canvas/ReloadingSlider").GetComponent<Slider>();
    }


    private void Start()
    {
        if (Singleton.Instance.GetFirstLoad())
            for (int i = 0; i < weapons.Count; i++)
            {
                ammoInMag[i] = weapons[i].clipSize;
                magsInInventory[i] = magCarryMax[i];
            }

        UpdateAmmoHUD();
    }


    public void UseAmmo(int amount = 0) // Lowers the ammo when weapon is shot - Amount will be used at a later point with upgrades
    {
        ammoInMag[currWeaponIndex] -= amount;
        UpdateAmmoHUD();
    }


    public IEnumerator Reload(float duration)
    {
        if (magsInInventory[currWeaponIndex] > 0 && ammoInMag[currWeaponIndex] < weaponEquipped.clipSize) // Can reload when less than max mag size and has mags in inventory
        {
            StartCoroutine(LerpReloadSlider(duration));
            yield return new WaitForSeconds(duration);

            if (charController.isReloading) // Checks to make sure that the player hasn't switched weapons
            {
                ammoInMag[currWeaponIndex] = weaponEquipped.clipSize;
                magsInInventory[currWeaponIndex] -= 1;

                charController.isReloading = false;
            }

            UpdateAmmoHUD();
        }
        else
            yield return null;
    }


    private IEnumerator LerpReloadSlider(float duration)
    {
        float elapsedTime = 0;

        float startingAmmo = ammoInMag[currWeaponIndex];
        float finishAmmo = weaponEquipped.clipSize;
        float currentAmmo = ammoInMag[currWeaponIndex];

        while(currentAmmo < finishAmmo)
        {
            if (charController.isReloading)
            {
                elapsedTime += Time.deltaTime;
                currentAmmo = Mathf.Lerp(startingAmmo, finishAmmo, elapsedTime / duration);
                reloadingSlider.value = currentAmmo;
                yield return new WaitForFixedUpdate();
            }
            else
                yield break;
        }

        yield return null;
    }


    public void UpdateAmmoHUD()
    {
        ammoHud[0].text = ammoInMag[currWeaponIndex].ToString();
        ammoHud[1].text = magsInInventory[currWeaponIndex].ToString();

        reloadingSlider.maxValue = weaponEquipped.clipSize;
        reloadingSlider.value = ammoInMag[currWeaponIndex];

    }


    public IEnumerator HealthRegen() // Regenerates health every quarter second
    {
        while(healthCurrent > 0)
        {
            if (healthCurrent < healthTotal) // Only regen if missing health
                charController.ReceiveHealth(healthRegen / 4);

            if(healthCurrent > healthTotal) // Clamps the health to max health
                healthCurrent = healthTotal;

            yield return new WaitForSeconds(0.25f);
        }

        yield return null;
    }
}
