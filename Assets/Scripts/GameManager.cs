using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class Data
{
    public float volume = 1f;
    public float sensivity = 180f;
}

public class GameManager : MonoBehaviour {
    int _sceneIdx;
    public int loadSceneIndex
    {
        get
        {
            return _sceneIdx;
        }
        set
        {
            _sceneIdx = value;
        }
    }
    public static GameManager instance = null;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
        DontDestroyOnLoad(this);

        string s = PlayerPrefs.GetString("playerdata", "-");
        if (s != "-")
            data = JsonUtility.FromJson<Data>(s);
        else
            data = new Data();
    }

    /// <summary>
    /// Load another scene with loading screen
    /// </summary>
    /// <param name="scene">Scene index.</param>
    public void LoadScene(int scene) {
        loadSceneIndex = scene;
        SceneManager.LoadSceneAsync(1);
    }

    public Data data;
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString("playerdata", JsonUtility.ToJson(data));
    }
}
