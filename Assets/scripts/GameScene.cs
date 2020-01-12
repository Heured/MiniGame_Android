using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameScene : MonoBehaviour
{

    public Text Score;
    public Text TopScore;
    public Text Tip;
    public Canvas canvas;
    public GameObject HeroPrefab;
    public GameObject gameOverLayerPrefab;

    GameObject hero;
    GameObject gameOverLayer;

    public AudioSource AudioPlayer;

    public Animator PerfectAct;

    float StackHeight = 400;     // 桥墩的高度
    float StackMaxWidth = 300;   // 桥墩的最大宽度
    float StackMinWidth = 100;   //桥墩的最小宽度
    float gravity = -10;    // 重力
    float StackGapMinWidth = 80;   // 桥墩间隙宽度
    float HeroSpeed = 760;     // 角色运动速度

    float DefinedScreenWidth;
    float DefinedScreenHeight;


    float XGAP = 20;    //角色与桥墩右边缘的距离
    float YGAP = 4;     //角色的腿长

    private int _score = 0;
    public int score
    {
        get
        {
            return _score;
        }
        set
        {
            if (value == 1)
            {
                Tip.color = new Color(255, 255, 255, 0);
            }
            _score = value;
            Score.text = _score.ToString();
        }
    }

    string StoreScoreName = "com.stickHero.score";
    string[] StoreLeadersNames = {"com.stickHero.leaders.first", "com.stickHero.leaders.second", "com.stickHero.leaders.third"};


    private bool _gameOver = false;
    public bool gameOver
    {
        get
        {
            return _gameOver;
        }
        set
        {
            if (value)
            {
                gameOverLayer = loadGameOverLayer();

                updateLeaders();
                checkHighScoreAndstore();
            }
            _gameOver = value;
        }
    }

    bool isBegin = false;
    bool isEnd = false;
    GameObject leftStack;  //左桥墩
    GameObject rightStack; //右桥墩

    float nextLeftStartX = 0;
    float stickHeight = 0;

    // Start is called before the first frame update
    void Start()
    {
        //  PlayerPrefs.DeleteAll();
        GameStart();
    }
    

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && !gameOver && !isBegin && !isEnd)
        {
            //Debug.Log("down");
            isBegin = true;
            GameObject stick = loadStick();

            StartCoroutine(StickResize(stick, DefinedScreenHeight - StackHeight, 1.5f));

            hero.GetComponent<Animator>().SetInteger("State", 2);
            playSoundFileNamed(StickHeroGameSceneEffectAudioName.StickGrowAudioName, true);
            
        }
        if(Input.GetMouseButtonUp(0) && isBegin && !isEnd)
        {
            //Debug.Log("up");
            isEnd = true;

            hero.GetComponent<Animator>().SetInteger("State", 0);
            StopAllCoroutines();
            AudioPlayer.loop = false;

            GameObject stick = GameObject.Find(StickHeroGameSceneChildName.StickName);
            stickHeight = stick.GetComponent<RectTransform>().rect.height;

            StartCoroutine(StickRotate(stick, -90, 0.4f, true));
            
        }
        
    }

    void GameStart()
    {
        TopScore.text = PlayerPrefs.GetInt(StoreScoreName, 0).ToString();

        leftStack =  LoadStacks(false, 0f);
        removeMidTouch(false, true);

        hero = loadHero();

        int maxGap = (int)(DefinedScreenWidth - StackMaxWidth - leftStack.GetComponent<RectTransform>().rect.width);
        float gap = (float)Mathf.Floor(Random.Range(StackGapMinWidth, maxGap));

        rightStack = LoadStacks(false, nextLeftStartX + gap);

        gameOver = false;
        
    }

    void GameRestart()
    {

    }

    void GameOver()
    {
        gameOver = true;
    }

    private void playSoundFileNamed(string name, bool loop)
    {
        AudioClip clip = Resources.Load<AudioClip>("AudioSources/" + name);
        AudioPlayer.clip = clip;
        AudioPlayer.loop = loop;
        AudioPlayer.Play();
    }

    private void showHighScore()
    {
        TopScore.text = score.ToString();

        playSoundFileNamed(StickHeroGameSceneEffectAudioName.HighScoreAudioName, false);

        GameObject highscore = gameOverLayer.transform.GetChild(3).gameObject;
        highscore.GetComponent<Animator>().SetBool("State",true);
        
    }

    private void updateLeaders()
    {
        int first = PlayerPrefs.GetInt(StoreLeadersNames[0], 0);
        int second = PlayerPrefs.GetInt(StoreLeadersNames[1], 0);
        int third = PlayerPrefs.GetInt(StoreLeadersNames[2], 0);
        if (score > first)
        {
            third = second; second = first; first = score;
        }
        else if (score > second)
        {
            third = second; second = score;
        }
        else if (score > third)
        {
            third = score;
        }

        PlayerPrefs.SetInt(StoreLeadersNames[0], first);
        PlayerPrefs.SetInt(StoreLeadersNames[1], second);
        PlayerPrefs.SetInt(StoreLeadersNames[2], third);
    }

    private void checkHighScoreAndstore()
    {
        int highScore = PlayerPrefs.GetInt(StoreScoreName, 0);
        
        if (score > highScore)
        {
            showHighScore();
            PlayerPrefs.SetInt(StoreScoreName, score);
        }
    }

    private bool checkPass()
    {
        GameObject stick = GameObject.Find(StickHeroGameSceneChildName.StickName);
        float rightPoint = DefinedScreenWidth / 2 + stick.GetComponent<RectTransform>().anchoredPosition.x + stickHeight;
        if (rightPoint <= nextLeftStartX && rightPoint >= nextLeftStartX - rightStack.GetComponent<RectTransform>().rect.width)
        {
            checkMidTouch();
            return true;
        }
        else
        {
            return false;
        }
    }

    private void checkMidTouch()
    {
        GameObject stick = GameObject.Find(StickHeroGameSceneChildName.StickName);
        float rightPoint = DefinedScreenWidth / 2 + stick.GetComponent<RectTransform>().anchoredPosition.x + stickHeight;
        if(rightPoint <= nextLeftStartX + 10 - rightStack.GetComponent<RectTransform>().rect.width / 2 && rightPoint >= nextLeftStartX - 10 - rightStack.GetComponent<RectTransform>().rect.width / 2)
        {
            score += 1;
            ShowPerfect();
            playSoundFileNamed(StickHeroGameSceneEffectAudioName.StickTouchMidAudioName, false);

        }
    }

    private void heroGo(bool pass)
    {
        if (!pass)
        {
            GameObject stick = GameObject.Find(StickHeroGameSceneChildName.StickName);
            float dis = stick.GetComponent<RectTransform>().anchoredPosition.x + stickHeight;

            hero.GetComponent<Animator>().SetInteger("State", 1);

            StartCoroutine(HeroMoveTo(hero, new Vector2(dis, hero.GetComponent<RectTransform>().anchoredPosition.y), HeroSpeed, pass));
        }
        else
        {
            float dis = nextLeftStartX - DefinedScreenWidth / 2 - hero.GetComponent<RectTransform>().rect.width / 2 - XGAP;

            hero.GetComponent<Animator>().SetInteger("State", 1);

            StartCoroutine(HeroMoveTo(hero, new Vector2(dis, hero.GetComponent<RectTransform>().anchoredPosition.y), HeroSpeed, pass));
        }
        
    }

    private void resetStickBuildState()
    {
        isBegin = false;
        isEnd = false;
    }

    private void moveStackAndCreateNew()
    {
        StartCoroutine(MoveToPosition(rightStack, new Vector2(rightStack.GetComponent<RectTransform>().rect.width / 2 - DefinedScreenWidth / 2, -DefinedScreenHeight / 2)));
        removeMidTouch(true, false);

        float x = rightStack.GetComponent<RectTransform>().rect.width - DefinedScreenWidth / 2 - hero.GetComponent<RectTransform>().rect.width / 2 - XGAP;
        float y = StackHeight + hero.GetComponent<RectTransform>().rect.height / 2 - DefinedScreenHeight / 2 - YGAP;
        StartCoroutine(MoveToPosition(hero, new Vector2(x,y)));

        GameObject stick = GameObject.Find(StickHeroGameSceneChildName.StickName);

        StartCoroutine(MoveAndDestroy(stick, new Vector2(- DefinedScreenWidth, stick.GetComponent<RectTransform>().anchoredPosition.y)));
        StartCoroutine(MoveAndDestroy(leftStack, new Vector2(- DefinedScreenWidth,  - DefinedScreenHeight / 2)));



    }

    private GameObject loadGameOverLayer()
    {
        GameObject gameOverLayer = Instantiate(gameOverLayerPrefab, canvas.GetComponent<RectTransform>());
        gameOverLayer.name = StickHeroGameSceneChildName.GameOverLayerName;
        gameOverLayer.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -1500);
        Button retry = gameOverLayer.GetComponent<RectTransform>().GetChild(1).gameObject.GetComponent<Button>();
        Button back = gameOverLayer.GetComponent<RectTransform>().GetChild(2).gameObject.GetComponent<Button>();
        retry.onClick.AddListener(delegate ()
        {
            SceneManager.LoadScene("scenes/GameScene");
        });
        back.onClick.AddListener(delegate ()
        {
            SceneManager.LoadScene("scenes/StartMenu");
        });
        StartCoroutine(MoveToPosition(gameOverLayer, new Vector2(0, 100f)));

        return gameOverLayer;
    }

    private GameObject loadStick()
    {
        GameObject stick = new GameObject();
        stick.name = StickHeroGameSceneChildName.StickName;
        stick.AddComponent<RectTransform>();
        stick.AddComponent<Image>().color = Color.black;
        stick.GetComponent<RectTransform>().sizeDelta = new Vector2(12, 1);
        stick.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
        stick.GetComponent<RectTransform>().SetParent(canvas.GetComponent<RectTransform>());
        stick.GetComponent<RectTransform>().localScale = new Vector2(1, 1);
        stick.GetComponent<RectTransform>().anchoredPosition = new Vector2(hero.GetComponent<RectTransform>().anchoredPosition.x + hero.GetComponent<RectTransform>().rect.width / 2 + 18, hero.GetComponent<RectTransform>().anchoredPosition.y - hero.GetComponent<RectTransform>().rect.height / 2);

        return stick;
    }

    private GameObject loadHero()
    {
        GameObject hero = Instantiate(HeroPrefab, canvas.GetComponent<RectTransform>());
        hero.name = StickHeroGameSceneChildName.HeroName;
        float x = nextLeftStartX - DefinedScreenWidth / 2 - hero.GetComponent<RectTransform>().rect.width / 2 - XGAP;
        float y = StackHeight + hero.GetComponent<RectTransform>().rect.height / 2 - DefinedScreenHeight / 2 - YGAP;
        hero.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);

        return hero;
    }

    private void removeMidTouch(bool animate, bool left)
    {
        GameObject stack = left ? leftStack : rightStack;
        GameObject mid = stack.GetComponent<RectTransform>().GetChild(0).gameObject;

        if (animate)
        {
            StartCoroutine(fadeAlpha(mid, 0f, 0.3f));
        }
        else
        {
            Destroy(mid);
        }
    }

    private GameObject LoadStacks(bool animate, float startLeftPoint)
    {
        int max = (int)(StackMaxWidth / 10);
        int min = (int)(StackMinWidth / 10);
        float width = (float)Mathf.Floor(Random.Range(min, max) * 10);
        float height = StackHeight;

        GameObject stack = new GameObject();
        stack.name = StickHeroGameSceneChildName.StackName;
        stack.AddComponent<RectTransform>();
        stack.AddComponent<Image>().color = Color.black;
        stack.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        stack.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
        stack.GetComponent<RectTransform>().SetParent(canvas.GetComponent<RectTransform>());
        stack.GetComponent<RectTransform>().localScale = new Vector2(1, 1);

        if (animate)
        {
            stack.GetComponent<RectTransform>().anchoredPosition = new Vector2(DefinedScreenWidth, - DefinedScreenHeight / 2);
            StartCoroutine(MoveToPosition(stack, new Vector2(startLeftPoint + width / 2 - DefinedScreenWidth / 2, - DefinedScreenHeight / 2)));
            Invoke("resetStickBuildState", 0.1f);
        }
        else
        {
            stack.GetComponent<RectTransform>().anchoredPosition = new Vector2(startLeftPoint + width / 2 - DefinedScreenWidth / 2, - DefinedScreenHeight / 2);
        }

        GameObject mid = new GameObject();
        mid.name = StickHeroGameSceneChildName.StackMidName;
        mid.AddComponent<RectTransform>();
        mid.AddComponent<Image>().color = Color.red;
        mid.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
        mid.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
        mid.GetComponent<RectTransform>().SetParent(stack.GetComponent<RectTransform>());
        mid.GetComponent<RectTransform>().localScale = new Vector2(1, 1);
        mid.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, height / 2);

        nextLeftStartX = width + startLeftPoint;

        return stack;
    }

    private void ShowPerfect()
    {
        PerfectAct.SetBool("State", true);
        Invoke("HidePerfect", 0.7f);
    }

    private void HidePerfect()
    {
        PerfectAct.SetBool("State", false);
    }



    private void OnEnable()
    {

        if (!SoundSwitch.SoundState)
        {
            AudioPlayer.mute = true;
        }
        else
        {
            AudioPlayer.mute = false;

        }
        DefinedScreenHeight = canvas.GetComponent<RectTransform>().rect.height;
        DefinedScreenWidth = DefinedScreenHeight * 9 / 16;


    }

    private void OnDisable()
    {

    }

    IEnumerator StickRotate(GameObject theThing, float to, float duration, bool falling)
    {
        yield return new WaitForSeconds(0.2f);
        Quaternion originRotation = theThing.GetComponent<RectTransform>().rotation;
        float gap = 0;

        while (theThing.GetComponent<RectTransform>().rotation != Quaternion.Euler(0, 0, to))
        {
            gap += Time.deltaTime / duration;
            theThing.GetComponent<RectTransform>().rotation = Quaternion.Slerp(originRotation, Quaternion.Euler(0,0,to), gap);
            yield return 0;
        }

        if (falling){

            playSoundFileNamed(StickHeroGameSceneEffectAudioName.StickFallAudioName, false);

            yield return new WaitForSeconds(0.2f);
            
            this.heroGo(this.checkPass());
        }
    }

    IEnumerator StickResize(GameObject stick, float toHeight, float duration)
    {
        float gap = (toHeight - stick.GetComponent<RectTransform>().rect.width) * Time.deltaTime / duration;
        while(stick.GetComponent<RectTransform>().rect.height < toHeight)
        {
            stick.GetComponent<RectTransform>().sizeDelta = new Vector2(stick.GetComponent<RectTransform>().rect.width, stick.GetComponent<RectTransform>().rect.height + gap);
            if(Mathf.Abs(toHeight - stick.GetComponent<RectTransform>().rect.height) < Mathf.Abs(gap))
            {
                stick.GetComponent<RectTransform>().sizeDelta = new Vector2(stick.GetComponent<RectTransform>().rect.width, toHeight);
            }
            yield return 0;
        }
    }

    IEnumerator fadeAlpha(GameObject theThing, float to, float duration)
    {
        if (theThing.GetComponent<Image>())
        {
            float gap = (to - theThing.GetComponent<Image>().color.a) * Time.deltaTime / duration;
            while (theThing.GetComponent<Image>().color.a != to)
            {
                Color newColor = theThing.GetComponent<Image>().color;
                newColor.a += gap;
                if(Mathf.Abs(newColor.a - to) < Mathf.Abs(gap))
                {
                    newColor.a = to;
                }
                theThing.GetComponent<Image>().color = newColor;
                yield return 0;
            }
        }
        else if(theThing.GetComponent<Text>())
        {
            float gap = (to - theThing.GetComponent<Text>().color.a) * Time.deltaTime / duration;
            while (theThing.GetComponent<Text>().color.a != to)
            {
                Color newColor = theThing.GetComponent<Text>().color;
                newColor.a += gap;
                if (Mathf.Abs(newColor.a - to) < Mathf.Abs(gap))
                {
                    newColor.a = to;
                }
                theThing.GetComponent<Text>().color = newColor;
                yield return 0;
            }
        }
    }

    IEnumerator HeroMoveTo(GameObject hero, Vector2 targetPos, float heroSpeed, bool pass)
    {
        while (hero.GetComponent<RectTransform>().anchoredPosition != targetPos)
        {
            hero.GetComponent<RectTransform>().anchoredPosition = Vector2.MoveTowards(hero.GetComponent<RectTransform>().anchoredPosition, targetPos, heroSpeed * Time.deltaTime);
            yield return 0;
        }
        if (!pass)
        {
            hero.GetComponent<Animator>().SetInteger("State", 0);
            StartCoroutine(StickRotate(GameObject.Find(StickHeroGameSceneChildName.StickName), -180, 0.4f, false));
            hero.GetComponent<Rigidbody2D>().velocity = new Vector2(0, gravity);

            playSoundFileNamed(StickHeroGameSceneEffectAudioName.DeadAudioName, false);

            Invoke("GameOver", 0.5f);

        }
        else
        {
            hero.GetComponent<Animator>().SetInteger("State", 0);
            score += 1;
            playSoundFileNamed(StickHeroGameSceneEffectAudioName.VictoryAudioName, false);

            moveStackAndCreateNew();
        }
    }

    IEnumerator MoveAndDestroy(GameObject theThing, Vector2 targetPos)
    {
        while (theThing.GetComponent<RectTransform>().anchoredPosition != targetPos)
        {
            theThing.GetComponent<RectTransform>().anchoredPosition = Vector2.MoveTowards(theThing.GetComponent<RectTransform>().anchoredPosition, targetPos, 3000 * Time.deltaTime);
            yield return 0;
        }

        if(theThing.name == StickHeroGameSceneChildName.StickName)
        {
            Destroy(theThing);
        }
        else if(theThing.name == StickHeroGameSceneChildName.StackName)
        {
            Destroy(theThing);
            int maxGap = (int)(DefinedScreenWidth - StackMaxWidth - rightStack.GetComponent<RectTransform>().rect.width);
            float gap = (float)Mathf.Floor(Random.Range(StackGapMinWidth, maxGap));

            leftStack = rightStack;
            rightStack = LoadStacks(true, leftStack.GetComponent<RectTransform>().rect.width + gap);
        }
    }

    IEnumerator MoveToPosition(GameObject theThing, Vector2 targetPos)
    {
        while (theThing.GetComponent<RectTransform>().anchoredPosition != targetPos)
        {
            theThing.GetComponent<RectTransform>().anchoredPosition = Vector2.MoveTowards(theThing.GetComponent<RectTransform>().anchoredPosition, targetPos, 3000 * Time.deltaTime);
            yield return 0;
        }
    }

    
}
