using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIController : MonoBehaviour
{
    //½Ì±ÛÅæ
    public static UIController Instance;

    [SerializeField]
    Button jumpButton;
    [SerializeField]
    Button attackButton;
    [SerializeField]
    Button settingButton;
    [SerializeField]
    Button roundStartButton;
    [SerializeField]
    GameObject settingPanel;
    [SerializeField]
    GameObject roundPanel;
    [SerializeField]
    GameObject restartPanel;
    [SerializeField]
    Button exitButton;
    [SerializeField]
    Button resumeButton;
    [SerializeField]
    TMP_Text roundText;
    [SerializeField]
    TMP_Text scoreText;


    public Slider attackSlider;

    public Slider jumpSlider;


    public Action jumpAction;
    public Action attackAction;

    public Action jumpSkillAction;
    public Action attackSkillAction;

    public Action initAction;

    public Image jumpSkillImage;
    
    public Image attackSkillImage;
    [SerializeField]
    Image shieldImage;

  
    private void Start()
    {
        if(Instance == null)
            Instance = this;
      
        OnRoundPanel();
        InitSetting();
    }

    public void InitValue()
    {
        shieldImage.fillAmount = 1f;
        attackSkillImage.fillAmount = 0;
        jumpSkillImage.fillAmount = 0;
        jumpSlider.gameObject.SetActive(false);
        attackSlider.gameObject.SetActive(false);

    }

    public void OnRestartPanel()
    {
        scoreText.text = string.Format("{0:0.00}" ,GameManager.Instance.time);
        initAction.Invoke();
        InitValue();
        restartPanel.SetActive(true);

    }

    public void OnClickRestartGame()
    {
        GameManager.Instance.InitValue();
        Time.timeScale = 1f;
        restartPanel.SetActive(false);
        GameManager.Instance.CreateMonster();
    }


    public void OnRoundPanel()
    {
        shieldImage.fillAmount = 1;
        Time.timeScale = 0;
    
        roundText.text = GameManager.Instance.round.ToString();
        roundPanel.SetActive(true);
    }

    public void OnClickSetting()
    {
        Time.timeScale = 0f;
        settingPanel.SetActive(true);

    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        settingPanel.SetActive(false);

    }

    public void ExitGame()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

#else
         Application.Quit();
#endif

    }


    //ÃÊ±â ¼³Á¤
    void InitSetting()
    {
        jumpButton.onClick.AddListener(() => {

            jumpAction.Invoke();
        });

        attackButton.onClick.AddListener(() => {

            attackAction.Invoke();


        });


        settingButton.onClick.AddListener(() => {
            OnClickSetting();
        });


        resumeButton.onClick.AddListener(() => {
            ResumeGame();
        });

        exitButton.onClick.AddListener(() => {
            ExitGame();
        });
        roundStartButton.onClick.AddListener(() => {

            Time.timeScale = 1;
            roundPanel.SetActive(false);
            GameManager.Instance.CreateMonster();
        });

        jumpSlider.onValueChanged.AddListener((value)=> {

            if (value > 0.8f)
            {
                jumpSkillImage.fillAmount = 0f;
                jumpSlider.value = 0f;
                jumpSlider.gameObject.SetActive(false);
                jumpSkillAction.Invoke();
            }
            else
            {
                jumpSlider.value = 0f;
            }
        
        });

        
        attackSlider.onValueChanged.AddListener((value) => {

            if (value > 0.8f)
            {
                attackSkillImage.fillAmount = 0f;
                attackSlider.value = 0f;
                attackSlider.gameObject.SetActive(false);
                attackSkillAction.Invoke();
            }
            else
            {
                attackSlider.value = 0f;
            }

        });

    }

   

    public IEnumerator ShieldCoolTime()
    {
        shieldImage.fillAmount = 0;

        while (shieldImage.fillAmount < 1)
        {
            yield return null;
            shieldImage.fillAmount += Time.deltaTime*0.5f;
            
        }

        shieldImage.fillAmount = 1;
    }




    
}
