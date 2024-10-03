using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Text scoreBoard;
    [SerializeField] private int totalFoodCnt;
    [SerializeField] private int collectedCnt;
    private AudioSource getting;
    [SerializeField] private Transform endUI;
    [SerializeField] private Transform LossUI;

    public static LevelManager Instance { get; private set; }

    private void Awake()
    {
        totalFoodCnt = 7;
        collectedCnt = 0;

        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("LevelManager Instance already initialized");
        }
    }

    private void Start()
    {
        scoreBoard = GetComponentInChildren<Text>();
        getting = GetComponent<AudioSource>();
    }

    private void Update()
    {
        scoreBoard.text = "<b>Score:" + collectedCnt.ToString() + "/" + totalFoodCnt.ToString() + "</b>";
        CheckLevelStatus();
    }

    public void CollectOne()
    {
        collectedCnt += 1;
        getting.PlayOneShot(getting.clip);
    }

    public void Loss()
    {
        GameManager.ShowMouse(true);
        Time.timeScale = 0;
        LossUI.gameObject.SetActive(true);
    }

    private void CheckLevelStatus()
    {
        if (collectedCnt == totalFoodCnt)
        {
            GameManager.ShowMouse(true);
            Time.timeScale = 0;
            endUI.gameObject.SetActive(true);
        }
    }
}
