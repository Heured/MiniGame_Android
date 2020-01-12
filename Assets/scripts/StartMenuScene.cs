using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public static class SoundSwitch{
    public static bool SoundState = true;
}

public class StartMenuScene : MonoBehaviour
{
    /*
    float StackWidth = 300;
    float StackHeight = 400;
    */

    float maxAspectRatioWidth;
    float playableMargin;
    

    RectTransform canvasRect;
    public Button StartButton;
    public Button LeaderBoardButton;
    public Button SoundSwitchButton;

    public Sprite SoundOn;
    public Sprite SoundOff;

    public AudioSource ClickSound;
    
    AudioSource musicPlayer;
    public AudioSource musicPlayerPrefab;
    /*
    void playAbleRect()
    {
        canvasRect = this.gameObject.transform.GetChild(1).gameObject.GetComponent<RectTransform>();
        float maxAspectRatio = 16 / 9 ;
        maxAspectRatioWidth = canvasRect.rect.height / maxAspectRatio;
        playableMargin = (canvasRect.rect.width - maxAspectRatioWidth) / 2.0f;
    }*/

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        
        if (!GameObject.Find("MusicPlayer"))
        {
            musicPlayer = Instantiate(musicPlayerPrefab);
            musicPlayer.gameObject.name = "MusicPlayer";
            DontDestroyOnLoad(musicPlayer);

        }
        else
        {
            musicPlayer = GameObject.Find("MusicPlayer").GetComponent<AudioSource>();
        }

        if (SoundSwitch.SoundState)
        {
            SoundSwitchButton.GetComponent<Image>().sprite = this.SoundOn;
            ClickSound.mute = false;
            musicPlayer.mute = false;
        }
        else
        {
            SoundSwitchButton.GetComponent<Image>().sprite = this.SoundOff;
            ClickSound.mute = true;
            musicPlayer.mute = true;

        }

        SoundSwitchButton.onClick.AddListener(delegate ()
        {
            if (SoundSwitch.SoundState)
            {
                SoundSwitch.SoundState = false;
                SoundSwitchButton.GetComponent<Image>().sprite = this.SoundOff;

                ClickSound.mute = true;
                musicPlayer.mute = true;
            }
            else
            {
                SoundSwitch.SoundState = true;
                SoundSwitchButton.GetComponent<Image>().sprite = this.SoundOn;
                ClickSound.mute = false;
                musicPlayer.mute = false;

            }
        });

        StartButton.onClick.AddListener(delegate ()
        {
            SceneManager.LoadScene("scenes/GameScene");
        });

        LeaderBoardButton.onClick.AddListener(delegate ()
        {
            SceneManager.LoadScene("scenes/LeaderBoard");
        });
    }

    private void OnDisable()
    {
        
    }

    private void LoadBackGround()
    {
       // GameObject BackGround = Sprite()
    }
}
