using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.Find("startBtn").GetComponent<Button>().onClick.AddListener(onStartBtn);
        transform.Find("quitBtn").GetComponent<Button>().onClick.AddListener(onQUITBtn);
    }

    private void OnEnable()
    {
    }
    private void OnDisable()
    {
    }
    public void onStartBtn()
    {
        GameManager.uimanger.CloseAllUI();
        GameManager.uimanger.ShowUI<ModeUI>("ModeUI");
    }
    public void onQUITBtn()
    {
        Application.Quit();
    }

   
    // Update is called once per frame

}
