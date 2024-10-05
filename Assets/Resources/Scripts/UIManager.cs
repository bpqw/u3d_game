using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private Transform canvasTf;//�����ı任���

    private List<GameObject> uiList;//�洢���ع��Ľ���ļ���

    public void Init()
    {
        //�������еĻ���
        canvasTf = GameObject.Find("Canvas").transform;
        //��ʼ������
        uiList = new List<GameObject>();
    }

    //��ʾ
    public T ShowUI<T>(string uiName) where T : Component
    {
        T ui = Find<T>(uiName);
        if (ui == null)
        {
            //������û�� ��Ҫ��Resources/UI�ļ����м���
            GameObject obj = Object.Instantiate(Resources.Load("UI/" + uiName), canvasTf) as GameObject;

            //������
            obj.name = uiName;

            //�����Ҫ�Ľű�
            ui = obj.AddComponent<T>();

            //��ӵ����Ͻ��д洢
            uiList.Add(obj);
        }
        else
        {
            //��ʾ
            ui.gameObject.SetActive(true);
        }

        return ui;
    }

    //����
    public void HideUI(string uiName)
    {
        GameObject ui = Find(uiName);
        if (ui != null)
        {
            ui.SetActive(true);
        }
    }


    public void CloseGunUI()
    {
        for (int i = uiList.Count - 1; i >= 0; i--)
        {
            if (uiList[i].name == "AKUI" || uiList[i].name == "��ǹUI" || uiList[i].name == "SCALEUI" || uiList[i].name == "G3UI" || uiList[i].name == "P90UI" || uiList[i].name == "M4UI" || uiList[i].name == "UZI")
            {
                GameObject ui = Find(uiList[i].name);
                if (ui != null)
                {
                    uiList.Remove(ui);
                    Object.Destroy(ui.gameObject);
                }
            }
        }
    }
    //�ر����н���
    public void CloseAllUI()
    {
        for (int i = uiList.Count - 1; i >= 0; i--)
        {
            Object.Destroy(uiList[i].gameObject);
        }

        uiList.Clear();//��ռ���
    }

    //�ر�ĳ������
    public void CloseUI(string uiName)
    {
        GameObject ui = Find(uiName);
        if (ui != null)
        {
            uiList.Remove(ui);
            Object.Destroy(ui.gameObject);
        }
    }

    //�Ӽ������ҵ����ֶ�Ӧ�Ľ���ű�
    public T Find<T>(string uiName) where T : Component
    {
        for (int i = 0; i < uiList.Count; i++)
        {
            if (uiList[i].name == uiName)
            {
                return uiList[i].GetComponent<T>();
            }
        }
        return null;
    }

    public GameObject Find(string uiName)
    {
        for (int i = 0; i < uiList.Count; i++)
        {
            if (uiList[i].name == uiName)
            {
                return uiList[i];
            }
        }
        return null;
    }

    //���ĳ������Ľű�
    public T GetUI<T>(string uiName) where T : Component
    {
        GameObject ui = Find(uiName);
        if (ui != null)
        {
            return ui.GetComponent<T>();
        }
        return null;
    }
}
