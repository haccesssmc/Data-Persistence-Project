using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI TopScoreText;
    public GameObject GameOverScreen;
    public List<GameObject> GameOverTitleTexts;

    private bool m_Started = false;
    private int m_Points;
    
    private bool m_GameOver = false;

    
    // Start is called before the first frame update
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }

        SetTopScore(0);
    }

    bool SetTopScore(int score)
    {
        if (GameManager.Instance != null && GameManager.Instance.userName != "")
        {
            bool isTop = GameManager.Instance.SetNewResult(score);
            TopScoreText.text = "Top Score : " + GameManager.Instance.userName + " : " + GameManager.Instance.result;
            return isTop;
        }
        return false;
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(1);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        if (m_Points > GameManager.Instance.result)
        {
            List<int> top = GameManager.Instance.GetTopResultsInt();

            int id = top.Count;
            for(int i = top.Count; i > 0; i--)
            {
                if(m_Points > top[i - 1])
                {
                    id = i - 1;
                }
            }
            if (id < 0) id = 0;
            GameOverTitleTexts[id].SetActive(true);
        }
        else
        {
            GameOverTitleTexts[4].SetActive(true);
        }

        SetTopScore(m_Points);
        m_GameOver = true;
        GameOverScreen.SetActive(true);
    }
}
