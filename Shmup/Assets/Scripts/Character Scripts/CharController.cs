using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharController : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject deathRecap;

    public bool isPaused = false;
    public bool isReloading = false;

    private CharStats stats;

    private Vector2 lastMoveDir = Vector2.zero;
    private bool isRolling = false;
    private float rollTime;
    private float maxRollTime = .1f;
    private float rollAmount = 10f;
    private bool rollOnCooldown = false;
    private float rollCooldown = 3f; // Time in seconds
    private float rollCooldownTimer;

    // Shooting variables
    private bool isFiring = false;
    private Vector2 fireDir = Vector2.up;
    private Vector2 lastFireDir;
    private Slider shootingSlider;

    // References to character components
    private Rigidbody2D rb;
    private Transform trans;
    private PlayerInputActions playerInputActions; // the script that contains the input information
    private PlayerInput playerInput;
    private Slider healthBar;

    // Refs of laser
    [SerializeField] private GameObject laser;
    private LineRenderer laserLineRenderer;
    private Vector3[] laserPos = new Vector3[2];


    // Weapon Selection HUD
    private SpriteRenderer weaponHUD;


    //Roll HUD
    [SerializeField] private SpriteRenderer rollHUDIcon;

    


    private void OnEnable()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }


    private void OnDisable()
    {
        playerInputActions.Player.Disable();
    }


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        trans = GetComponent<Transform>();
        playerInput = GetComponent<PlayerInput>();
        healthBar = GameObject.Find("HUD/Canvas/HealthBar").GetComponent<Slider>();
        laserLineRenderer = laser.GetComponent<LineRenderer>();
        shootingSlider = GameObject.Find("AmmoAndShooting_Canvas/ShootingSlider").GetComponent<Slider>();

    }

    private void Start()
    {
        if(!Singleton.Instance.GetFirstLoad())
            Singleton.Instance.LoadPlayerStats();

        stats = GetComponent<CharStats>();
        stats.healthTotal = stats.healthBase + stats.healthAdditive;

        StartCoroutine(stats.HealthRegen()); // Starts the health regen loop that continues until player dies

        healthBar.maxValue = stats.healthTotal;
        healthBar.value = stats.healthCurrent;

        weaponHUD = GameObject.Find("Weapon_Selection_HUD/Selected_Weapon").GetComponent<SpriteRenderer>();
        weaponHUD.sprite = stats.weaponEquipped.weaponSprite;
    }


    private void Update()
    {
        
        if(stats.healthCurrent <= 0) // Checks to see if player is supposed to be dead
        {
            Death();
        }

        if(rollOnCooldown)
        {
            StartCoroutine(RollHUDFade());
            //rollHUDIcon.color = new Color(rollHUDIcon.color.r, rollHUDIcon.color.g, rollHUDIcon.color.b, Mathf.Lerp(rollHUDIcon.color.a, 255f, rollCooldownTimer / rollCooldown * Time.deltaTime));
            rollCooldownTimer += Time.deltaTime;
            if(rollCooldownTimer >= rollCooldown)
            {
                rollOnCooldown = false;
                rollCooldownTimer = 0f;
            }
        }

        if(Keyboard.current.f1Key.wasPressedThisFrame) // Current key to switch between keyboard and controller input
        {
            Singleton.Instance.SwitchControlScheme();
        }

        if(Singleton.Instance.isUsingController)
        {
            fireDir = playerInputActions.Player.FireDir.ReadValue<Vector2>(); // Read joystick on controller
        }
        else // When using mouse + keyboard
        {
            CheckMouseInput();

            // Gets normalized mouse position on screen relative to the position of the middle
            // bottom left = (-1, -1)
            // top right = (1, 1)
            fireDir = Input.mousePosition - new Vector3(Camera.main.scaledPixelWidth/2, Camera.main.scaledPixelHeight/2, 0); 
            fireDir.Normalize();
        }

        for(int i=0; i<stats.weaponLastFired.Length; i++)
        {
            stats.weaponLastFired[i] += Time.deltaTime;
        }

        if(isFiring && !isPaused)
        {
            if (stats.weaponLastFired[stats.currWeaponIndex] >= 60f / (stats.weaponEquipped.fireRate + stats.weaponEquipped.fireRate*stats.fireRatePercentage) && !isRolling && stats.ammoInMag[stats.currWeaponIndex] > 0 && isReloading == false)
            {
                if (fireDir == Vector2.zero)
                {
                    stats.weaponLastFired[stats.currWeaponIndex] = stats.weaponEquipped.Fire(lastFireDir, trans);
                }
                else
                {
                    stats.weaponLastFired[stats.currWeaponIndex] = stats.weaponEquipped.Fire(fireDir, trans);
                }

                stats.UseAmmo(1);
                StopCoroutine(ShootingSliderLerp());
                StartCoroutine(ShootingSliderLerp());
            }
            else if(stats.ammoInMag[stats.currWeaponIndex] == 0)
            {
                Reload();
            }
        }

        // Laser only working for controller. Really only needed for controller aiming
        if (Singleton.Instance.isUsingController)
        {
            Vector3 laserEnd = new Vector3((playerInputActions.Player.FireDir.ReadValue<Vector2>().x*20f) + trans.position.x, (playerInputActions.Player.FireDir.ReadValue<Vector2>().y*20f) + trans.position.y);
            laserLineRenderer.SetPositions(new Vector3[] { trans.position, laserEnd });
        }
    }


    private void FixedUpdate() // Used for movement
    {
        if(stats.isMoveable && !isRolling)
        {
            Movement();
        }
        else if(isRolling && !isPaused)
        {
            rollTime += Time.deltaTime;
            if(rollTime >= maxRollTime)
            {
                isRolling = false;
                rollTime = 0;
            }
        }
    }

    
    public void CheckMouseInput() // Checks for left mouse button press and scroll wheel activity
    {
        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            isFiring = true;
        }
        else if(Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isFiring = false;
        }

        if(Mouse.current.scroll.ReadValue() != Vector2.zero)
        {
            SwitchWeaponMouse(Mouse.current.scroll.ReadValue());
        }
    }


    public void Movement()
    { 
        Vector2 inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();
        lastMoveDir = inputVector;
        if(isFiring) // Move at half speed when shooting
        {
            rb.velocity = new Vector2(inputVector.x, inputVector.y) * (stats.speedBase + stats.speedBase*(stats.speedAdditive/100)) * 5f * Time.deltaTime;
        }
        else
            rb.velocity = new Vector2(inputVector.x, inputVector.y) * (stats.speedBase + stats.speedBase*(stats.speedAdditive / 100)) * 10f * Time.deltaTime;
    }


    public void OnFire(InputAction.CallbackContext context) // Fire input for controller, mouse input not used by PlayerInput Component
    {
        if (context.started == true) // on button down
        {
            isFiring = true;
            print("Start Firing");
        }
        else if (context.canceled == true) // on button release
        {
            isFiring = false;

            if (playerInputActions.Player.FireDir.ReadValue<Vector2>() != Vector2.zero) // Save the last dir if not zero
            {
                lastFireDir = playerInputActions.Player.FireDir.ReadValue<Vector2>();
            }
            print("Stop Firing");
        }
    }


    private IEnumerator ShootingSliderLerp()
    {
        shootingSlider.value = 0;
        float elapsedTime = 0;

        var fireCycleDuration = 60f / (stats.weaponEquipped.fireRate + stats.weaponEquipped.fireRate * stats.fireRatePercentage);
        shootingSlider.maxValue = 60f / (stats.weaponEquipped.fireRate + stats.weaponEquipped.fireRate * stats.fireRatePercentage);

        while(elapsedTime < fireCycleDuration)
        {
            elapsedTime += Time.deltaTime;
            shootingSlider.value = Mathf.Lerp(0, shootingSlider.maxValue, elapsedTime / (60f / (stats.weaponEquipped.fireRate + stats.weaponEquipped.fireRate * stats.fireRatePercentage)));
            yield return null;
        }
        yield break;
    }

    public void Action(InputAction.CallbackContext context)
    {
        print(context);
    }


    public void ResetLevel(InputAction.CallbackContext context)
    {
        if(context.performed == true)
        {
            Singleton.Instance.ResetLevel();
        }
    }

    GameObject menuInst = null;
    public void PauseMenu(InputAction.CallbackContext context)
    {
        if(context.started == true)
        {
            if(isPaused)
            {
                isPaused = false;
                Time.timeScale = 1;

                if (menuInst != null)
                    Destroy(menuInst);
            }
            else
            {
                isPaused = true;
                menuInst = Instantiate(pauseMenu);
                menuInst.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -5);

                Time.timeScale = 0;
            }
        }
    }


    public void Roll(InputAction.CallbackContext context)
    {
        if(context.started == true && !isRolling && !rollOnCooldown && !isPaused)
        {
            isRolling = true;
            rollOnCooldown = true;
            
            rb.AddForce(lastMoveDir * rollAmount, ForceMode2D.Impulse);
        }
    }


    IEnumerator RollHUDFade() // Coroutine Function for fading in the rollHUDIcon based on its cooldown
    {
        rollHUDIcon.color = new Color(rollHUDIcon.color.r, rollHUDIcon.color.g, rollHUDIcon.color.b, 0f);

        Color c = rollHUDIcon.color;

        while (rollOnCooldown)
        {
            c.a = 1f * (rollCooldownTimer / rollCooldown); // Transitions between 0f and 1f
            rollHUDIcon.color = c;
            yield return null;
        }

        c.a = 1f; // Since the transition doesn't fully get to 1f make it equal 1f
        rollHUDIcon.color = c;
    }


    private void SwitchWeapon(int wepIndex) // Changes the weapon variable in CharStats and changes the weapon icon
    {
        stats.currWeaponIndex = wepIndex;
        stats.weaponEquipped = stats.weapons[stats.currWeaponIndex];
        weaponHUD.sprite = stats.weaponEquipped.weaponSprite;
        isReloading = false;
        stats.UpdateAmmoHUD();
    }


    public void SwitchWeaponDpad(InputAction.CallbackContext context) // Controller D-pad weapon switching
    {

    }


    public void SwitchWeaponMouse(Vector2 scrollDir) // Mouse wheel input for switching weapons
    {
        var wepIndex = stats.currWeaponIndex;

        if(scrollDir.y > 0) //clockwise
        {
            if(stats.currWeaponIndex < stats.weapons.Count-1)
            {
                wepIndex++;
            }
            else
            {
                wepIndex = 0;
            }
        }
        else
        {
            if (stats.currWeaponIndex > 0)
            {
                wepIndex--;
            }
            else
            {
                wepIndex = stats.weapons.Count-1;
            }
        }

        SwitchWeapon(wepIndex);
    }


    public void SwitchWeaponsKeyboard(InputAction.CallbackContext context) // Switching weapons with num keys
    {
        if(context.performed)
        {
            switch(context.control.name)
            {
                case "1":
                    SwitchWeapon(0);
                    break;

                case "2":
                    SwitchWeapon(1);
                    break;

                case "3":
                    SwitchWeapon(2);
                    break;

                case "4":
                    SwitchWeapon(3);
                    break;
            }
        }
    }


    public void Reload(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            if (!isReloading) // Don't wanna double down on reloading
            {
                isReloading = true;
                StartCoroutine(stats.Reload(stats.weaponEquipped.reloadTime));
            }
        }
    }

    private void Reload()
    {
        if (!isReloading)
        {
            isReloading = true;
            StartCoroutine(stats.Reload(stats.weaponEquipped.reloadTime));
        }
    }


    public void ReceiveDamage(float damage)
    {
        stats.healthCurrent -= damage - damage*(stats.damageReductionPercentage/100f);
        //print("Without: " + damage + " With defense: " + (damage - damage * (stats.damageReductionPercentage / 100f)));
        healthBar.value = stats.healthCurrent;
    }


    public void ReceiveHealth(float health)
    {
        stats.healthCurrent += health;
        healthBar.value = stats.healthCurrent;
    }


    private void Death() // Shows the death recap screen for specified amount of time after player dies (CURRENTLY NO DELAY WORKS)
    {
        rb.velocity = Vector2.zero;
        GetComponent<SpriteRenderer>().enabled = false;
        GameObject.Find("HUD").SetActive(false);
        enabled = false;
        

        print("Player has died");
        ShowDeathRecap();
    }


    private void ShowDeathRecap()
    {
        var d = Instantiate(deathRecap, transform);
        d.transform.localPosition = Vector2.zero;
    }
}