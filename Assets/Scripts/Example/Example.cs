// (C) king.com Ltd 2018

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

using Services;

public class Example : MonoBehaviour {

    struct Level : IToplistIdentifier
    {
        public int LevelIndex
        {
            get;
            set;
        }
    }

    Toplist uiToplist;

    Level current;
    MultiTopList provider;
	InputField levelIndex;
	InputField userName;
	InputField levelScore;


    private string dataFile = "data.json";
    
    void Awake()
	{
		levelIndex = GameObject.Find ("LevelIndex").GetComponent<InputField> ();
		levelScore = GameObject.Find("Score").GetComponent<InputField>();
		userName = GameObject.Find("Username").GetComponent<InputField>();
        provider = new MultiTopList();
        uiToplist = FindObjectOfType<Toplist>();
        current = new Level();
    }

    void Start()
    {
        //provider.LoadData();
        levelIndex.onEndEdit.AddListener (SetLevel);
        SetLevel(1);
    }
		
    public void SetLevel(string level)
    {
		int iLevel = 0;
		if(int.TryParse(level, out iLevel))
			SetLevel(iLevel);
    }

	public void SetLevel(int level)
	{
		current.LevelIndex = level;
		levelIndex.text = level.ToString ();
	}

	public void SetUsername()
	{
		provider.SetLocalUsername (userName.text);
	}

    public void SetUsername(string username)
    {
        provider.SetLocalUsername(username);
    }

	public void ReportScore()
	{
		int score = 0;

		if(int.TryParse(levelScore.text, out score))
			ReportScore (score);
	}

    public void ReportScore(int score)
    {
        provider.ReportResult(current, score);
        provider.SaveData();
    }

    public void ShowToplist()
    {
        uiToplist.Display(provider, current);
    }

    public void LoadData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, dataFile);
        if(File.Exists(filePath))
        {
            provider = JsonUtility.FromJson<MultiTopList>(filePath);
        }
    }

    public void SaveData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, dataFile);
        if(!Directory.Exists(Application.streamingAssetsPath))
        {
            Directory.CreateDirectory(Application.streamingAssetsPath);
        }
        if(!File.Exists(filePath))
        {
            using(StreamWriter sw = File.CreateText(filePath))
            {
                sw.Write(JsonUtility.ToJson(provider));
            }
        }
    }
}
