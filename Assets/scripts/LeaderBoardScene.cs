using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LeaderBoardScene : MonoBehaviour
{
    public Button backButton;
    public Text leaderFirst;
    public Text leaderSecond;
    public Text leaderThird;


    string[] StoreLeadersNames = { "com.stickHero.leaders.first", "com.stickHero.leaders.second", "com.stickHero.leaders.third" };
    // Start is called before the first frame update
    void Start()
    {
        int first = PlayerPrefs.GetInt(StoreLeadersNames[0], 0);
        int second = PlayerPrefs.GetInt(StoreLeadersNames[1], 0);
        int third = PlayerPrefs.GetInt(StoreLeadersNames[2], 0);

        leaderFirst.text = first.ToString();
        leaderSecond.text = second.ToString();
        leaderThird.text = third.ToString();

        backButton.onClick.AddListener(delegate()
        {
            SceneManager.LoadScene("scenes/StartMenu");
        });

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
