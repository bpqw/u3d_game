using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class ModeUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
        transform.Find("msg/a").GetComponent<Button>().onClick.AddListener(OnCreatePeo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnCreatePeo()
    {
        GameManager.uimanger.CloseUI("ModeUI");
        GameObject eventSystem = GameObject.Find("EventSystem");
        if (eventSystem != null)
        {
            Destroy(eventSystem);
        }
        SceneManager.LoadScene("Level-1");
    }
    //public void OnCreatePeog()
    //{
    //    GameManager.uimanger.CloseUI("ModeUI");
    //}
    
}
