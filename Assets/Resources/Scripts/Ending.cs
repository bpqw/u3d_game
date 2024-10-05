using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

public class Ending : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.Find("Button").GetComponent<Button>().onClick.AddListener(again);
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void again()
    {
        GameObject eventSystem = GameObject.Find("GameControl");
        if (eventSystem != null)
        {
            Destroy(eventSystem);
        }
        SceneManager.LoadScene("start");
    }
}
