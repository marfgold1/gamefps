using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class SceneHelper : MonoBehaviour {
    public Image background;
    public AudioSource audioSource;
    public static SceneHelper instance = null;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        background.DOFade(0f, 1f).OnComplete(() => { background.gameObject.SetActive(false); });
    }

    public void ChangeScene(int sceneIdx)
    {
        EventSystem.current.enabled = false;
        background.gameObject.SetActive(true);
        background.DOKill();
        background.DOFade(1f, 3f).OnComplete(() => { Time.timeScale = 1f; GameManager.instance.LoadScene(sceneIdx); });
    }
}
