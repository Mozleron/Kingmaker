// (C) king.com Ltd 2018

using System;
using System.Collections;
using System.Collections.Generic;

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

    public class LocalToplist : IToplistProvider
    {
        List<IToplistEntry> entries;
        public LocalToplist()
        {
            entries = new List<IToplistEntry>();
        }

        string localUsername;
        public void SetLocalUsername(string username)
        {
            localUsername = username;
        }

        public bool Get(IToplistIdentifier identifier, Action<IList<IToplistEntry>> callback, int maxEntries = 10)
        {
            callback(entries);
            return true;
        }

        /**
         * Publishes a result to the toplist. If the user already has an equal or better score, this will be a no-op.
         *
         * @return False if an error occured.
         */
        public bool ReportResult(IToplistIdentifier identifier, int score)
        {
            if (entries.Exists(e => e.Username == localUsername))
            {
                ToplistEntry existingEntry = (ToplistEntry) entries.Find(e => e.Username == localUsername);
                if (existingEntry.Score >= score)
                {
                    return true;
                }
                entries.Remove(existingEntry);
            }
		    entries.Add(new ToplistEntry(localUsername, score));
            return true;
        }
    }

    public class MultiTopList : LocalToplist
    {
        private string localUserName{ get; set; }
        Dictionary<int,LocalToplist> localLists;
        public MultiTopList(){
            localLists = new Dictionary<int, LocalToplist>();
        }

        new public bool ReportResult(IToplistIdentifier identifier, int score)
        {
            if(!localLists.ContainsKey(identifier.LevelIndex))
            {
                localLists[identifier.LevelIndex] = new LocalToplist();
            }
            localLists[identifier.LevelIndex].SetLocalUsername(localUserName);
            localLists[identifier.LevelIndex].ReportResult(identifier, score);
            return true;
        }

        new public bool Get(IToplistIdentifier identifier, Action<IList<IToplistEntry>> callback, int maxEntries = 10)
        {
            if(localLists.ContainsKey(identifier.LevelIndex))
            {
                localLists[identifier.LevelIndex].Get(identifier, callback, maxEntries);
            }
            else
            { return false; }
            return true;
        }

        new public void SetLocalUsername(string username)
        {
            localUserName = username;

        }
    }
}

