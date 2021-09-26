using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI helloText;
    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] GameObject addNewUserObj;
    [SerializeField] List<GameObject> usersInfo;
    [SerializeField] List<GameObject> topInfo;
    

    private void Start()
    {
        onLoadMainScreen();
    }

    public void onLoadMainScreen()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.userName != "")
            {
                helloText.text = "Hello, " + GameManager.Instance.userName + "!";
            }
            else
            {
                helloText.text = "Hello, Gamer!";
            }

            if (GameManager.Instance.result > 0)
            {
                resultText.text = "Your top result is " + GameManager.Instance.result;
            }
            else
            {
                resultText.text = "Your top result is 0";
            }
        }
    }

    public void onLoadUsersScreen()
    {
        List<string> userNames = GameManager.Instance.GetUsersNames();
        List<int> userResults = GameManager.Instance.GetUsersResults();

        int count = userNames.Count;
        int maxCount = usersInfo.Count;
        if(count < maxCount)
        {
            addNewUserObj.SetActive(true);
        }
        else
        {
            addNewUserObj.SetActive(false);
        }

        foreach(GameObject userInfo in usersInfo)
        {
            userInfo.SetActive(false);
        }

        if(userNames.Count > 0 && userNames.Count == userResults.Count)
        {
            for (int i = 0; i < userNames.Count; i++)
            {
                usersInfo[i].SetActive(true);
                string userInfoName = "Name" + i;
                GameObject.Find(userInfoName).GetComponent<TextMeshProUGUI>().text = userNames[i];
            }
        }
    }

    public void onLoadResultsScreen()
    {
        List<string> top = GameManager.Instance.GetTopResults();
        int i = 0;
        foreach(GameObject topObj in topInfo)
        {
            if(i <= top.Count - 1)
            {
                topObj.GetComponent<TextMeshProUGUI>().text = top[i];
            }
            else
            {
                topObj.GetComponent<TextMeshProUGUI>().text = "";
            }
            i++;
        }
    }

    public void AddNewUser()
    {
        string userName = GameObject.Find("NewUserNameText").GetComponent<TextMeshProUGUI>().text;
        GameObject.Find("NewUserNameText").GetComponent<TextMeshProUGUI>().text = "";
        GameManager.Instance.AddNewUser(userName);
        onLoadUsersScreen();
    }

    public void DeleteUser(int id)
    {
        GameManager.Instance.DeleteUser(id);
        onLoadUsersScreen();
    }

    public void SelectUser(int id)
    {
        GameManager.Instance.SetActiveUser(id);
        onLoadUsersScreen();
    }

    public void StartGame()
    {
        GameManager.Instance.StartGame();
    }

    public void ExitGame()
    {
        GameManager.Instance.Exit();
    }
}
