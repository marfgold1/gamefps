using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Events;

public class Gameplay : MonoBehaviour {
    public int wave = 1;
    public Enemy enemyPrefab;
    public EnemyWeapon[] enemyWeaponPrefabs;
    int _enemyCount;
    public int enemyCount
    {
        get
        {
            return _enemyCount;
        }
        set
        {
            _enemyCount = value;
            waveStatus.text = "<b><size=35>Wave " + wave + "</size></b>\nEnemy: " + value + " / " + enemyStartSpawn;
            if (value < 1)
            {
                NextWave();
            }
        }
    }
    public int enemyStartSpawn = 1;
    public CanvasGroup pausedPanel;
    public TextMeshProUGUI infotextPanel;
    public Image countdownPanel;
    public CanvasGroup losePanel;
    public GameObject firstLoseSelectable;
    public TextMeshProUGUI waveStatus;

    public static Gameplay instance = null;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SpawnEnemy();
        ShowInfoMenu("Wave 1 Incoming..", true, 2f);
        isLose = false;
    }

    private void Update()
    {
        if (isLose) return;
        if (isPaused) return;
        // Pause Menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            InfoMenu("P A U S E D", false);
        }
    }

    public void NextWave()
    {
        enemyStartSpawn++;
        wave++;
        Weapon[] weapons = FindObjectsOfType<Weapon>();
        foreach (Weapon w in weapons)
        {
            w.bulletStock += w.maxBullet * wave;
            w.bullet = w.maxBullet;
        }
        Character.instance.maxHealth = 100 + (50 * wave);
        Character.instance.health = Character.instance.maxHealth;
        SpawnEnemy();
        ShowInfoMenu("<b>Wave " + (wave-1) + " Complete!</b>\n<size=30>Wave " + wave + " Incoming..</size>", true, 2f);
    }
    
    Vector3 randomPoint;
    Vector3 center = new Vector3(5.5f, 1f, 5.7f);
    Vector3 result;
    public void SpawnEnemy()
    {
        enemyCount = enemyStartSpawn;
        for(int i = 0; i < enemyStartSpawn; i++)
        {
            Enemy e = Instantiate(enemyPrefab);
            while (true) {
                randomPoint = center + Random.insideUnitSphere * 17f;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
                {
                    result = hit.position;
                    break;
                }
            }
            e.transform.position = result;
            e.AssignWeapon(enemyWeaponPrefabs[Random.Range(0, enemyWeaponPrefabs.Length)]);
        }
    }

    Tween timeTween;
    public void AnimateTime(float to, float duration = 0.5f)
    {
        if (timeTween != null)
            timeTween.Kill();
        timeTween = DOTween.To(() => Time.timeScale, (x) => Time.timeScale = x, to, duration);
    }
    
    bool isPaused = false;
    bool panelShow = false;
    public void InfoMenu(string info, bool withPauseFlag = true)
    {
        if (panelShow)
            HideInfoMenu(withPauseFlag);
        else
            ShowInfoMenu(info, withPauseFlag);
    }

    public void ShowInfoMenu(string info, bool withPauseFlag = true, float autoHide = 0f)
    {
        pausedPanel.gameObject.SetActive(true);
        pausedPanel.DOKill();
        if(withPauseFlag)
            isPaused = true;
        panelShow = true;
        pausedPanel.DOFade(1f, 0.5f);
        infotextPanel.text = info;
        countdownPanel.fillAmount = 0;
        AnimateTime(0f, 0.3f);
        if(autoHide > 0)
        {
            countdownPanel.DOFillAmount(1f, autoHide).OnComplete(() => HideInfoMenu()).SetEase(Ease.Linear);
        }
    }

    public void HideInfoMenu(bool withPauseFlag = true)
    {
        pausedPanel.DOKill();
        if(withPauseFlag)
            isPaused = false;
        panelShow = false;
        pausedPanel.DOFade(0f, 0.5f).OnComplete(() => {
            pausedPanel.gameObject.SetActive(false);
        });
        AnimateTime(1f, 0.3f);
    }

    public bool isLose = false;
    public void ShowLosePanel()
    {
        if (isLose) return;
        isLose = true;
        LosePanel();
        AnimateTime(0f, 0.5f);
    }
    void LosePanel()
    {
        Cursor.lockState = CursorLockMode.None;
        losePanel.gameObject.SetActive(true);
        losePanel.DOFade(1f, 0.5f);
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(firstLoseSelectable);
    }

    public void ExitGame()
    {
        MessageBox.instance.ShowMessageBox("Exit Game", "Are you sure want to exit the game?", true, "Yes", "No", () => Application.Quit());
    }

    public void ExitToMainMenu()
    {
        MessageBox.instance.ShowMessageBox("Exit to main menu", "Are you sure want to go to the main menu?", true, "Yes", "No", () => { SceneHelper.instance.ChangeScene(0); });
    }

    public void Restart()
    {
        SceneHelper.instance.ChangeScene(2);
    }
}
