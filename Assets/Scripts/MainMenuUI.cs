using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenuUI : MonoBehaviour {
    public Selectable[] selectables;
    public GameObject settingsPanel;
    public Image tittle;
    public Image basePanel;
    public Shadow shadowPanel;
    public Slider volume;
    public Slider sensivity;

    private void Start()
    {
        volume.value = GameManager.instance.data.volume;
        sensivity.value = GameManager.instance.data.sensivity;
    }

    public void OnVolumeChange(float val)
    {
        GameManager.instance.data.volume = val;
        AudioListener.volume = val;
    }

    public void OnSensivityChange(float val)
    {
        GameManager.instance.data.sensivity = val;
    }

    bool isSettingShow = false;
    public void ShowSettingsPanel()
    {
        for(int i = 0; i < selectables.Length; i++)
        {
            selectables[i].interactable = false;
        }

        settingsPanel.SetActive(true);
        tittle.DOFillAmount(1f, 0.2f).OnComplete(()=> {
            DOTween.To(() => shadowPanel.effectDistance, (x) => shadowPanel.effectDistance = x, new Vector2(10f, -10f), 0.3f);
            basePanel.DOFillAmount(1f, 0.4f).OnComplete(()=> {
                isSettingShow = true;
            });
        });

    }

    public void HideSettingsPanel()
    {
        isSettingShow = false;
        DOTween.To(() => shadowPanel.effectDistance, (x) => shadowPanel.effectDistance = x, Vector2.zero, 0.3f);
        basePanel.DOFillAmount(0f, 0.4f).OnComplete(() => {
            tittle.DOFillAmount(0f, 0.2f).OnComplete(() => {
                for (int i = 0; i < selectables.Length; i++)
                {
                    selectables[i].interactable = true;
                }
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(selectables[1].gameObject);
                settingsPanel.SetActive(false);
            });
        });
    }

    private void Update()
    {
        if (isSettingShow)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HideSettingsPanel();
            }
        }
    }

    public void StartGame()
    {
        SceneHelper.instance.ChangeScene(2);
    }

    public void ExitGame()
    {
        MessageBox.instance.ShowMessageBox("Exit Game", "Are you sure want to exit the game?", true, "Yes", "No", () => Application.Quit());
    }

    public void HowToPlay()
    {
        MessageBox.instance.ShowInfoBox("How To Play", "rmb / Right mouse click: shoot\nr: reload\nq: throw weapon\nf: pick weapon\nspace: jump\nesc: pause game\nshift: run\nawsd: move", "OK THEN");
    }
}
