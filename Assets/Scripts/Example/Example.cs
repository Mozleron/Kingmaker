﻿// (C) king.com Ltd 2018

using System.Collections;
using System.Collections.Generic;
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

    Toplist toplist;

    Level current;
    LocalToplist provider;
	InputField levelIndex;
	InputField userName;
	InputField levelScore;
    void Awake()
	{
		levelIndex = GameObject.Find ("LevelIndex").GetComponent<InputField> ();
		levelScore = GameObject.Find("Score").GetComponent<InputField>();
		userName = GameObject.Find("Username").GetComponent<InputField>();
        provider = new LocalToplist();
        toplist = FindObjectOfType<Toplist>();
    }

    void Start()
    {
		levelIndex.onEndEdit.AddListener (SetLevel);
        SetLevel(1);
    }
		
    public void SetLevel(string level)
    {
		int iLevel = 0;
		if(int.TryParse(level, out iLevel))
			SetLevel(iLevel);
    }

	//initially assume just one level
	//think about expanding to more later
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
    }

    public void ShowToplist()
    {
        toplist.Display(provider, current);
    }
}