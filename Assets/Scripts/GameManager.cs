using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
        using UnityEditor;
#endif


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public string userName;
    public int result;

    private SaveData data = new SaveData();

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Load();

        SetActiveUser(data.currentUserID);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("main");
    }

    public void Exit()
    {
        Save();
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    [Serializable]
    public class SaveData
    {
        public List<string> names = new List<string>();
        public List<int> results = new List<int>();
        public int currentUserID = -1;
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(data);
        string path = Application.dataPath + "/save.json";
        File.WriteAllText(path, json);
    }

    public void Load()
    {
        string path = Application.dataPath + "/save.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            data = JsonUtility.FromJson<SaveData>(json);
        }
    }

    public List<string> GetUsersNames()
    {
        return data.names;
    }

    public List<int> GetUsersResults()
    {
        return data.results;
    }

    public void AddNewUser(string name)
    {
        if (name.Length <= 0 || data.names.Exists(x => x.StartsWith(name))) return;

        data.names.Add(name);
        data.results.Add(0);
        SetActiveUser(data.names.Count - 1);
    }

    public void SetActiveUser(int id)
    {
        data.currentUserID = id;

        if(id > -1)
        {
            userName = data.names[id];
            result = data.results[id];
        }
        else
        {
            userName = "";
            result = 0;
        }
    }

    public void DeleteUser(int id)
    {
        if(data.currentUserID == id)
        {
            SetActiveUser(id - 1);
        }

        data.names.RemoveAt(id);
        data.results.RemoveAt(id);

    }

    public List<string> GetTopResults()
    {
        List<string> top = new List<string>();
        List<string> names = data.names;
        List<int> results = data.results;

        while(top.Count < 3 && results.Count > 0)
        {
            int maxResult = 0;
            int id = -1;
            for(int i = 0; i < results.Count; i++)
            {
                if(results[i] > maxResult)
                {
                    id = i;
                    maxResult = results[id];
                }
                else if(results[i] == 0)
                {
                    results.RemoveAt(i);
                    names.RemoveAt(i);
                }
            }
            if(id > -1)
            {
                top.Add(name[id] + " : " + results[id]);
                results.RemoveAt(id);
                names.RemoveAt(id);
            }
        }

        return top;
    }
}
