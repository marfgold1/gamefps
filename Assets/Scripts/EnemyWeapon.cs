using UnityEngine;
using DG.Tweening;

public class EnemyWeapon : MonoBehaviour {
    int _bullet = 5;
    public int bullet
    {
        get
        {
            return _bullet;
        }
        set
        {
            _bullet = value;
            if(value < 1)
            {
                Reload();
            }
        }
    }
    [Header("=== Bullet ===")]
    [Header("Prefabs and GOs")]
    public Transform bulletPrefab;
    public Transform bulletPivot;
    public GameObject muzzle;
    [Header("Bullet Configuration")]
    public int maxBullet = 5;
    public float bulletTime = 0.2f;
    public float reloadTime = 0.4f;
    [Header("=== Audio ===")]
    public AudioSource audioSource;
    public AudioClip shootClip;
    public AudioClip reloadClip;
    float currentBulletTime;
    bool canShoot = false;
    [HideInInspector]
    public bool isReloading = false;
    Transform trans;

    void Start () {
        bullet = maxBullet;
        canShoot = true;
        trans = transform;
        isReloading = false;
	}
	
    public void Reload()
    {
        if (isReloading) return;
        isReloading = true;
        canShoot = false;
        trans.DOKill();
        trans.DOLocalMoveY(-0.01f, 0.1f);
        audioSource.PlayOneShot(reloadClip);
        trans.DOLocalRotate(new Vector3(30f, 0f), 0.1f).OnComplete(()=> {
            trans.DOLocalMoveY(0, 0.1f).SetDelay(reloadTime);
            trans.DOLocalRotate(Vector3.zero, 0.1f).SetDelay(reloadTime).OnComplete(()=> {
                bullet = maxBullet;
                currentBulletTime = bulletTime;
                canShoot = true;
                isReloading = false;
            });
        });
    }

    public void Shoot()
    {
        if (!canShoot) return;
        if(currentBulletTime >= bulletTime)
        {
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
	}
}
