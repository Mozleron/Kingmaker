﻿// (C) king.com Ltd 2018
/**
This file is probably a mess for a seasoned Unity dev, but for what it is, it works.
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
//Brought in the Newtonsoft Json implementation because the Unity version is limited for what I was trying to do.
using Newtonsoft.Json;
using UnityEngine;

namespace Services
{
    public interface IToplistEntry
    {
        int Score { get; }
        string Username { get; }
    }

    public interface IToplistIdentifier
    {
        int LevelIndex { get; }
    }

    public interface IToplistProvider
    {
        bool Get(IToplistIdentifier identifier, Action<IList<IToplistEntry>> callback, int maxEntries = 10);

        bool ReportResult(IToplistIdentifier identifier, int score);
    }

    [Serializable]
    public struct ToplistEntry : IToplistEntry
    {
        public ToplistEntry(string username, int score)
        {
            Username = username;
            Score = score;
        }

        public int Score
        {
            get;
            private set;
        }

        public string Username
        {
            get;
            private set;
        }
    }

//I made this class so the TopList can be iterated through.
    public class ToplistEnum : IEnumerator
    {
        public List<IToplistEntry> _entries;
        int position = -1;

        public ToplistEnum(List<IToplistEntry> list)
        {
            _entries = list;
        }

        public bool MoveNext()
        {
            position++;
            return (position < _entries.Count);
        }

        public void Reset()
        {
            position = -1;
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public IToplistEntry Current
        {
            get
            {
                try
                {
                    return _entries[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }

//I made this implement IEnumerable so multiples of it could be used in the MultiTopList class.
//This was the core of my approach for handling high scores across multiple levels.
    public class LocalToplist : IToplistProvider, IEnumerable
    {
        List<IToplistEntry> entries;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public ToplistEnum GetEnumerator()
        {
            return new ToplistEnum(entries);
        }
        public LocalToplist()
        {
            entries = new List<IToplistEntry>();
        }

        string localUsername;
        virtual public void SetLocalUsername(string username)
        {
            localUsername = username;
        }

        virtual public bool Get(IToplistIdentifier identifier, Action<IList<IToplistEntry>> callback, int maxEntries = 10)
        {
            //Here is where maxEntries does its magic.
            callback(entries.GetRange(0,(entries.Count >= maxEntries ?maxEntries:entries.Count )));
            return true;
        }

        /**
         * Publishes a result to the toplist. If the user already has an equal or better score, this will be a no-op.
         *
         * @return False if an error occured.
         */
        virtual public bool ReportResult(IToplistIdentifier identifier, int score)
        {
            try
            {
                if (entries.Exists(e => e.Username == localUsername))
                {
                    ToplistEntry existingEntry = (ToplistEntry)entries.Find(e => e.Username == localUsername);
                    if (existingEntry.Score >= score)
                    {
                        return true;
                    }
                    entries.Remove(existingEntry);
                }
                entries.Add(new ToplistEntry(localUsername, score));
                return true;
            }
            catch(Exception e)
            {
                Console.Write("Error in LocalTopList.ReportResult" + e);
                return false;
            }
        }
    }

    public class MultiTopList : LocalToplist
    {
        private string localUserName{ get; set; }

        private string dataFile = "data.json";
        public Dictionary<int,LocalToplist> localLists;

        public MultiTopList(){
            localLists = new Dictionary<int, LocalToplist>();
        }

        override public bool ReportResult(IToplistIdentifier identifier, int score)
        {
            try
            {
                if (!localLists.ContainsKey(identifier.LevelIndex))
                {
                    localLists[identifier.LevelIndex] = new LocalToplist();
                }
                localLists[identifier.LevelIndex].SetLocalUsername(localUserName);
                localLists[identifier.LevelIndex].ReportResult(identifier, score);
                return true;
            }
            catch(Exception e)
            {
                Console.Write("Error in MultiTopList.RportResult:" + e);
                return false;
            }
        }

        override public bool Get(IToplistIdentifier identifier, Action<IList<IToplistEntry>> callback, int maxEntries = 10)
        {
            if(localLists.ContainsKey(identifier.LevelIndex))
            {
                localLists[identifier.LevelIndex].Get(identifier, callback, maxEntries);
            }
            else
            { return false; }
            return true;
        }

        override public void SetLocalUsername(string username)
        {
            localUserName = username;
        }

//This method writes out a very nice JSON representation of all the data entered in via the UI.
        public void SaveData()
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, dataFile);
            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }

            using (StreamWriter sw = File.CreateText(filePath))
            {
                sw.Write(JsonConvert.SerializeObject(localLists)); 
            }
        }

//Unfortunately, it does not come back in as neatly as I had hoped.
//I think the next step to make this approach work is a custom deserializer.
//This may highlight a design flaw of this approach, but that's where collaboration is so valuable.
        public void LoadData()
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, dataFile);
            if (File.Exists(filePath))
            {
                using(StreamReader sr = new StreamReader(filePath))
                {
                    string jsonString = sr.ReadToEnd();
                    localLists = JsonConvert.DeserializeObject<Dictionary<int, LocalToplist>>(jsonString);
                }
            }
        }
    }
}

