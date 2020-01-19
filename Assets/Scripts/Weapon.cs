using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Weapon : MonoBehaviour {
    int _bullet = 5;
    [Header("=== Bullet ===")]
    [Header("Prefabs and GOs")]
    public Transform bulletPrefab;
    public Transform bulletPivot;
    public GameObject muzzle;
    public int bullet
    {
        get
        {
            return _bullet;
        }
        set
        {
            _bullet = value;
            if(enabled)
                bulletText.text = "Bullet: " + value.ToString("0") + "/" + maxBullet.ToString("0")
                                  + "\n<size=23>In stock: " + bulletStock.ToString("0") + "</size>";
        }
    }
    [Header("Bullet Configuration")]
    public int maxBullet = 5;
    public int bulletStock = 20;
    public float bulletTime = 0.2f;
    public float reloadTime = 0.4f;
    [Header("=== Audio ===")]
    public AudioSource audioSource;
    public AudioClip shootClip;
    public AudioClip reloadClip;
    float currentBulletTime;
    bool canShoot = false;
    TextMeshProUGUI bulletText
    {
        get
        {
            return Character.instance.bulletText;
        }
    }
    TextMeshProUGUI cantReload
    {
        get
        {
            return Character.instance.cantReload;
        }
    }
    [HideInInspector]
    public bool isReloading = false;
    Transform trans;

    private void OnEnable()
    {
        bullet = bullet;
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    private void OnDisable()
    {
        GetComponent<BoxCollider>().enabled = true;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddForce(trans.forward * 300f);
    }

    void Start () {
        bullet = maxBullet;
        canShoot = true;
        trans = transform;
        isReloading = false;
	}
	
    public void Reload()
    {
        if (isReloading) return;
        Character.instance.SetFOVReload();
        if(bulletStock < 1)
        {
            cantReload.DOKill();
            cantReload.DOFade(1f, 0.5f).OnComplete(() => cantReload.DOFade(0f, 0.5f).SetDelay(3f));
            cantReload.text = "No enough bullet to reload";
            return;
        }
        isReloading = true;
        canShoot = false;
        trans.DOKill();
        trans.DOLocalMoveY(-0.01f, 0.1f);
        audioSource.PlayOneShot(reloadClip);
        trans.DOLocalRotate(new Vector3(30f, 0f), 0.1f).OnComplete(()=> {
            trans.DOLocalMoveY(0, 0.1f).SetDelay(reloadTime);
            trans.DOLocalRotate(Vector3.zero, 0.1f).SetDelay(reloadTime).OnComplete(()=> {
                if (bulletStock < maxBullet)
                {
                    bulletStock = 0;
                    bullet = bulletStock;
                }
                else
                {
                    bulletStock -= maxBullet;
                    bullet = maxBullet;
                }
                currentBulletTime = bulletTime;
                canShoot = true;
                isReloading = false;
                Character.instance.SetFOVNormal();
            });
        });
    }

    public void Shoot()
    {
        if (!canShoot) return;
        if(currentBulletTime >= bulletTime)
        {
            if(bullet < 1)
            {
                cantReload.DOKill();
                cantReload.DOFade(1f, 0.2f).OnComplete(() => cantReload.DOFade(0f, 0.2f).SetDelay(2f));
                cantReload.text = "Bullet is empty!";
                return;
            }
            Transform t = Instantiate(bulletPrefab);
            t.SetPositionAndRotation(bulletPivot.position, bulletPivot.rotation);
            audioSource.PlayOneShot(shootClip);
            t.localScale = bulletPivot.lossyScale;
            muzzle.SetActive(true);
            Invoke("DisableMuzzle", 0.1f);
            bullet--;
            currentBulletTime = 0;
        }
    }

    void DisableMuzzle()
    {
        muzzle.SetActive(false);
    }

	void Update () {
        if (!isReloading)
            currentBulletTime += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.R)) //reload
        {
            Reload();
        }
        if (Input.GetMouseButton(0))
        {
            Shoot();
        }
	}
}
