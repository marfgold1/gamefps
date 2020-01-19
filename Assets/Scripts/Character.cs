using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class Character : MonoBehaviour {
    [Header("Camera")]
    public Transform camTrans;
    public float cameraSpeed = 10f;
    public float maxYAngle = 45f;
    [Header("Character & Attribute")]
    public float reloadSpeed = 2f;
    public float speed = 3f;
    public float runSpeed = 6f;
    public float jumpSpeed = 10f;
    public float gravity = 9.8f;
    public Weapon currentWeapon = null;
    public Transform weaponPivot;
    [Header("HP")]
    int _health;
    public int maxHealth;
    public int health
    {
        get
        {
            return _health;
        }
        set
        {
            if (value < 1)
            {
                _health = 0;
                Gameplay.instance.ShowLosePanel();
            }
            else
                _health = value;
            sliderHP.value = _health / (float)maxHealth;
            textHP.text = _health.ToString("0");
        }
    }
    public TextMeshProUGUI textHP;
    public Slider sliderHP;
    [Header("UI")]
    public TextMeshProUGUI infoPickup;
    public TextMeshProUGUI bulletText;
    public TextMeshProUGUI cantReload;
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip jumpClip;
    public AudioClip landClip;
    public AudioClip pickupClip;
    public AudioClip throwClip;
    public float stepDistance = 1f;
    float stepDiff;
    public AudioClip[] footstepClip;
    int currentStepClip;
    bool _isGround;
    bool isGrounded
    {
        get
        {
            return _isGround;
        }
        set
        {
            if(_isGround != value)
            {
                if (value)
                {
                    audioSource.PlayOneShot(landClip);
                }
                _isGround = value;
            }
        }
    }
    bool _isRunning = false;
    bool isRunning
    {
        get
        {
            return _isRunning;
        }
        set
        {
            if(_isRunning != value)
            {
                _isRunning = value;
                if (currentWeapon != null && currentWeapon.isReloading)
                    return;
                cam.DOKill();
                if (value)
                {
                    cam.DOFieldOfView(75f, 0.3f);
                } else
                {
                    cam.DOFieldOfView(60f, 0.3f);
                }
            }
        }
    }
    CharacterController _chara;
    Vector3 movement = Vector3.zero;
    Vector3 camRot = Vector3.zero;
    Transform _trans;
    Camera cam;

    // Singleton
    public static Character instance = null;
    private void Awake()
    {
        instance = this;
    }
    
    // Initializer
    void Start () {
        _chara = GetComponent<CharacterController>();
        _trans = transform;
        cameraSpeed = GameManager.instance.data.sensivity;
        isRunning = false;
        cam = camTrans.GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        maxHealth = 100;
        health = 100;
	}
	
    // Utility
    float ClampAngle(float angle, float from, float to)
    {
        angle = angle > 180 ? angle - 360 : angle;
        angle = Mathf.Clamp(angle, from, to);
        return angle;
    }
    public void SetFOVReload()
    {
        cam.DOKill();
        cam.DOFieldOfView(50f, 0.3f);
    }
    public void SetFOVNormal()
    {
        cam.DOKill();
        if (isRunning)
        {
            cam.DOFieldOfView(75f, 0.3f);
        }
        else
        {
            cam.DOFieldOfView(60f, 0.3f);
        }
    }

    RaycastHit hit;
    Vector3 mov;
    private void Update()
    {
        if (Gameplay.instance.isLose) return;
        // Throw and Unequip Weapon
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (currentWeapon != null)
            {
                audioSource.PlayOneShot(throwClip);
                bulletText.text = "";
                currentWeapon.enabled = false;
                currentWeapon.transform.parent = null;
                currentWeapon = null;
            }
        }

        // Pick-up Item System
        if (Physics.Raycast(camTrans.position, camTrans.forward, out hit, 3f, 512))
        {
            if (Physics.Raycast(camTrans.position, camTrans.forward, hit.distance, 1024)) { infoPickup.text = ""; return; }
            GameObject go = hit.transform.gameObject;
            if (go.tag == "Weapon")
            {
                if (currentWeapon == null)
                {
                    infoPickup.text = "(F) Pick-Up Weapon";
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        audioSource.PlayOneShot(pickupClip);
                        currentWeapon = go.GetComponent<Weapon>();
                        currentWeapon.transform.parent = weaponPivot;
                        currentWeapon.enabled = true;
                        currentWeapon.transform.DOLocalRotate(Vector3.zero, 0.2f);
                        currentWeapon.transform.DOLocalMove(Vector3.zero, 0.2f);
                    }
                }
                else
                {
                    infoPickup.text = "(F) Change Weapon";
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        audioSource.PlayOneShot(pickupClip);
                        currentWeapon.enabled = false;
                        currentWeapon.transform.parent = null;
                        currentWeapon = go.GetComponent<Weapon>();
                        currentWeapon.transform.parent = weaponPivot;
                        currentWeapon.enabled = true;
                        currentWeapon.transform.DOLocalRotate(Vector3.zero, 0.2f);
                        currentWeapon.transform.DOLocalMove(Vector3.zero, 0.2f);
                    }
                }
            }
            else
            {
                infoPickup.text = "";
            }
        }
        else
        {
            infoPickup.text = "";
        }
    }

    void FixedUpdate ()
    {
        if (Gameplay.instance.isLose) return;
        // MouseLook
        camRot = camTrans.localEulerAngles + (new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X")) * cameraSpeed * Time.fixedDeltaTime);
        camRot.x = ClampAngle(camRot.x, -maxYAngle, maxYAngle);
        camTrans.localEulerAngles = camRot;

        // Character Movement
        isGrounded = _chara.isGrounded;
        if (isGrounded)
        {
            movement = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            movement = camTrans.TransformDirection(movement);
            movement.y = 0f;
            if (currentWeapon != null && currentWeapon.isReloading)
                movement *= reloadSpeed;
            else
                movement *= (isRunning ? runSpeed : speed);
            isRunning = Input.GetKey(KeyCode.LeftShift);
            if (Input.GetKey(KeyCode.Space))
            {
                movement.y = jumpSpeed;
                audioSource.PlayOneShot(jumpClip);
            }
        }
        movement.y -= gravity * Time.fixedDeltaTime;
        _chara.Move(movement * Time.fixedDeltaTime);

        // Calculate audio step
        mov = movement;
        mov.y = 0;
        stepDiff += Mathf.Abs((mov*Time.fixedDeltaTime).magnitude);
        if(stepDiff > stepDistance)
        {
            // Make sure footstep clip is different
            int rnd = Random.Range(0, footstepClip.Length);
            if(rnd == currentStepClip)
            {
                rnd = (rnd + 1) % footstepClip.Length;
            }
            currentStepClip = rnd;
            audioSource.PlayOneShot(footstepClip[rnd]);
            stepDiff = 0;
        }
    }
}
