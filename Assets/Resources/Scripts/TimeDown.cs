using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeDown : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private Text Timeboard;
    void Start()
    {
        StartCoroutine(TimeFunc(60));
        Timeboard = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private IEnumerator TimeFunc(float TimeCount)
    {
        do
        {
            yield return new WaitForSeconds(1);//��ÿ֡update֮����еȴ�һ��
            Timeboard.text = "<b>Time:"  + TimeCount.ToString() + "</b>";
            TimeCount -= 1;
        } while (TimeCount > 0);
        LevelManager.Instance.Loss();
    }
}
