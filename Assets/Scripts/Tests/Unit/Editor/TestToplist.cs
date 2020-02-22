// (C) king.com Ltd 2018

using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

using Services;

public class TestToplist {

    struct Level : IToplistIdentifier
    {
        public int LevelIndex
        {
            get;
            set;
        }
    }

    LocalToplist toplist;

    [SetUp]
    public void Setup()
    {
        toplist = new LocalToplist();
    }

    [Test]
    public void SetUsername()
    {
        toplist.SetLocalUsername("Foo");
    }

    [Test]
    public void ReportResult()
    {
        Level level = new Level { LevelIndex = 1 };
        var score = 1000;

        var success = toplist.ReportResult(level, score);

        Assert.AreEqual(success, true);
    }

    [Test]
    public void ToplistContainsUsername()
    {
        const string username = "Foo";
        toplist.SetLocalUsername(username);

        Level level = new Level { LevelIndex = 1 };
        var score = 1000;
        toplist.ReportResult(level, score);

        toplist.Get(level, (entries) => {
            bool found = false;
            foreach(var entry in entries)
            {
                found |= entry.Username == username;
            }

            Assert.AreEqual(found, true);            
        });
    }

    [Test]
    public void ToplistContainsMaxEntries()
    {
        const string username = "Foo";
        toplist.SetLocalUsername(username);

        Level level = new Level { LevelIndex = 1 };
        for (int i = 0; i < 10; ++i)
        {
            var score = 1000 + i;
            toplist.ReportResult(level, score);
        }

        const int maxEntries = 3;
        toplist.Get(level, (entries) => {
            
            Assert.AreSame(entries.Count, maxEntries);

        }, maxEntries);
    }

    [Test]
    public void ToplistPersistance()
    {
        
    }

    [Test]
    public void ScoreIsUpdated()
    {

    }

    [Test]
    public void MultipleToplists()
    {

    }
}
