using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static UIManager uimanger;

    public static bool isload = false;
    // Start is called before the first frame update
    private void Awake()
    {
        
            uimanger = new UIManager();
            uimanger.Init();        
    }
    void Start()
    {
        uimanger.ShowUI<LoginUI>("LoginUI");
    }

    public static void ShowMouse(bool isShow)
    {
        Cursor.visible = isShow;

        if (isShow)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
