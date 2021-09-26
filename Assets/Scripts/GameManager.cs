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

    private SaveData data;

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
        else
        {
            data = new SaveData();
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

    public int GetCurrentUserID()
    {
        return data.currentUserID;
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

    public bool SetNewResult(int res)
    {
        if (userName == "" || data.currentUserID < 0) return false;
        if (res > result)
        {
            result = res;
            data.results[data.currentUserID] = res;
            
            return true;
        }
        return false;
    }

    public List<string> GetTopResults()
    {
        List<string> top = new List<string>();
        List<string> allNames = new List<string>();
        List<int> allResults = new List<int>();

        foreach (string n in data.names)
        {
            allNames.Add(n);
        }
        foreach (int r in data.results)
        {
            allResults.Add(r);
        }

        while (top.Count < 3 && allResults.Count > 0)
        {
            int maxResult = 0;
            int id = -1;
            for(int i = 0; i < allResults.Count; i++)
            {
                if(allResults[i] > maxResult)
                {
                    id = i;
                    maxResult = allResults[id];
                }
                else if(allResults[i] == 0)
                {
                    allResults.RemoveAt(i);
                    allNames.RemoveAt(i);
                }
            }
            if(id > -1)
            {
                top.Add(allNames[id] + " : " + allResults[id]);
                allResults.RemoveAt(id);
                allNames.RemoveAt(id);
            }
        }

        return top;
    }

    public List<int> GetTopResultsInt()
    {
        List<int> top = new List<int>();
        List<int> all = new List<int>();
        foreach(int res in data.results)
        {
            all.Add(res);
        }

        while (top.Count < 3 && all.Count > 0)
        {
            int maxResult = 0;
            int id = -1;
            for (int i = 0; i < all.Count; i++)
            {
                if (all[i] > maxResult)
                {
                    id = i;
                    maxResult = all[id];
                }
                else if (all[i] == 0)
                {
                    all.RemoveAt(i);
                }
            }
            if (id > -1)
            {
                top.Add(all[id]);
                all.RemoveAt(id);
            }
        }
        return top;
    }
}
