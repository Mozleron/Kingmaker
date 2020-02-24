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
    MultiTopList multiTopList;

    [SetUp]
    public void Setup()
    {
        toplist = new LocalToplist();
        multiTopList = new MultiTopList();
    }

    [Test]
    public void SetUsername()
    {
        Level level = new Level { LevelIndex = 1 };
        string username = "Foo";
        toplist.SetLocalUsername(username);
        toplist.Get(level, (entries) =>
        {
            bool found = false;
            foreach (var entry in entries)
            {
                found |= entry.Username == username;
            }

            Assert.AreEqual(found, true);
        });
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
        const string usernameBase = "Foo";

        Level level = new Level { LevelIndex = 1 };
        for (int i = 0; i < 10; ++i)
        {
            toplist.SetLocalUsername(usernameBase + i.ToString());
            var score = 1000;
            toplist.ReportResult(level, score);
        }

        const int maxEntries = 3;
        toplist.Get(level, (entries) => {
            Assert.AreEqual(entries.Count, maxEntries);
        }, maxEntries);
    }

    [Test]
    public void ToplistPersistance()
    {
        //Once I can get the read from JSON into a class working, here's how i'd approach this test:
        //Creat a new Multitoplist. 
        //Add values to it.
        //Destroy that list.
        //Create a new list.
        //As soon as the new list exists, check it for the values that were added above.
    }

    [Test]
    public void ScoreIsUpdated()
    {
        Level level = new Level { LevelIndex = 1 };
        toplist.SetLocalUsername("foo");
        var score = 1000;
        toplist.ReportResult(level, score);
        toplist.Get(level, (entries) =>
        {
            Assert.AreEqual(entries[0].Score, score);
        });
        score++;
        toplist.ReportResult(level, score);
        toplist.Get(level, (entries) =>
        {
            Assert.AreEqual(entries[0].Score, score);
        });
    }

    [Test]
    public void MultipleToplists()
    {
        Level level = new Level { LevelIndex = 1 };
        int maxLists = 10;
        var score = 1000;
        multiTopList.SetLocalUsername("foo");
        for (int i = 0; i < maxLists; ++i)
        {
            multiTopList.ReportResult(level, score);
            level.LevelIndex++;
        }
        Assert.AreEqual(multiTopList.localLists.Count, maxLists);
    }
}
