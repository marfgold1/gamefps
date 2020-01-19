using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Loading : MonoBehaviour {
    public Image loadingBar;
    float progress;
    AsyncOperation asyncLoad = null;
    bool isDone = false;

	// Use this for initialization
	void Start () {
        asyncLoad = SceneManager.LoadSceneAsync(GameManager.instance.loadSceneIndex);
        asyncLoad.allowSceneActivation = false;
        progress = 0f;
        isDone = false;
	}
	
	// Update is called once per frame
	void Update () {
        progress = Mathf.Lerp(progress, asyncLoad.progress / 0.9f, Time.deltaTime * 3f);
        loadingBar.fillAmount = progress;
        if(progress > 0.97f) {
            if (!isDone)
            {
                isDone = true;
                SceneHelper.instance.background.DOFade(0f, 1f).OnComplete(() => asyncLoad.allowSceneActivation = true);
            }
        }
	}
}
