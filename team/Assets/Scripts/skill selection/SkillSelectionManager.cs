using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class SkillSelectionManager : MonoBehaviour
{
    public static SkillSelectionManager Instance { get; private set; }

    public const int MaxSkillCount = 5;

    private List<SkillData> selectedSkills = new List<SkillData>();

    public IReadOnlyList<SkillData> SelectedSkills => selectedSkills;


    // 現在のシーンにある決定ボタン
    [SerializeField] private GameObject buttun;


    private void Awake()
    {
      
            if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // シーンを移動してもスキル情報を保持
        DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {
        // 現在のシーンの決定ボタンを探す
        buttun.SetActive(false);


        FindButton();
      

    }




    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        buttun = GameObject.Find("SkillDecisionButton");

        if (buttun != null)
        {
            // スキルが5個なら表示、それ以外なら非表示
            buttun.SetActive(selectedSkills.Count >= MaxSkillCount);
        }
    }





    /// <summary>
    /// 現在のシーンにある決定ボタンを探す
    /// </summary>
    private void FindButton()
    {
        // すでに存在していたら何もしない
        if (buttun != null)
        {
            return;
        }

        // 「SkillDecisionButton」という名前のGameObjectを探す
        buttun = GameObject.Find("SkillDecisionButton");

        if (buttun != null)
        {
            // まだ5個選んでいないなら非表示
            buttun.SetActive(selectedSkills.Count >= MaxSkillCount);

            Debug.Log("決定ボタンを取得しました。");
        }
        else
        {
            Debug.LogWarning(
                "SkillDecisionButton が見つかりません。"
            );
        }
    }


    /// <summary>
    /// スキルを選択する
    /// </summary>
    public bool SelectSkill(SkillData skill)
    {
        if (skill == null)
        {
            return false;
        }


        // すでに5個選択している
        if (selectedSkills.Count >= MaxSkillCount)
        {
            Debug.Log("スキルは最大5個まで選択できます。");
            return false;
        }


        // 同じスキルを2回選択できない
        if (selectedSkills.Contains(skill))
        {
            Debug.Log("このスキルはすでに選択されています。");
            return false;
        }


        // スキルを追加
        selectedSkills.Add(skill);


        Debug.Log(
            "スキル選択: " +
            skill.SkillName +
            " (" +
            selectedSkills.Count +
            "/" +
            MaxSkillCount +
            ")"
        );


        // 5個選択したら決定ボタンを表示
        if (selectedSkills.Count == MaxSkillCount)
        {
            ShowDecisionButton();
        }


        return true;
    }


    /// <summary>
    /// 決定ボタンを表示する
    /// </summary>
    private void ShowDecisionButton()
    {
        // ボタンがまだ取得できていなければ探す
        if (buttun == null)
        {
            FindButton();
        }


        if (buttun != null)
        {
            buttun.SetActive(true);

            Debug.Log("スキルを5個選択したので決定ボタンを表示しました。");
        }
        else
        {
            Debug.LogWarning(
                "決定ボタンが見つからないため表示できません。"
            );
        }
    }


    /// <summary>
    /// スキル選択をすべて消す
    /// </summary>
    /// 

    public void ClearSkills()
    {
        selectedSkills.Clear();

        if (buttun != null)
        {
            buttun.SetActive(false);
        }

        Debug.Log("選択したスキルをクリアしました。");
    }


    //public void ClearSkills()
    //{
    //    selectedSkills.Clear();


    //    // ボタンがなければ現在のシーンから探す
    //    if (buttun == null)
    //    {
    //        FindButton();
    //    }


    //    // 決定ボタンを非表示
    //    if (buttun != null)
    //    {
    //        buttun.SetActive(false);
    //    }


    //    Debug.Log("選択したスキルをクリアしました。");
    //}


    /// <summary>
    /// スキルを5個選択済みか
    /// </summary>
    public bool IsFull()
    {
        return selectedSkills.Count >= MaxSkillCount;
    }
}

