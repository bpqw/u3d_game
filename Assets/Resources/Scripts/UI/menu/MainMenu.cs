using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Modular Dungeon"); // 载入游戏场景
    }

    public void ContinueGame()
    {
        // 这里添加读取存档逻辑
        Debug.Log("继续游戏");
    }

    public void OpenSettings()
    {
        // 打开设置面板
        Debug.Log("打开设置");
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}