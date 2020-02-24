// (C) king.com Ltd 2018
using System.Collections.Generic;
using UnityEngine;

using Services;

public class Toplist : MonoBehaviour {

    public GameObject toplistEntryPrefab;
	private List<ToplistEntry> entryList = new List<ToplistEntry> ();
    private int lastLevel = 0;
    public void Display(IToplistProvider provider, IToplistIdentifier identifier)
    {
        var transformCache = transform;
        if (lastLevel != identifier.LevelIndex)
        {
			//A more efficient approach is probably to make a pool of ToplistEntry object and enable or disable them as needed.
			//But, for quick and dirty, just deleting and recreating them also works.
            foreach (ToplistEntry item in entryList)
			{
                item.Delete();
            }
            entryList.Clear();
        }
        string inType = provider.GetType().ToString();
        Debug.Log(inType);
        provider.Get(identifier, (entries) =>
		{
			for (int i = 0; i < entries.Count; i++)
			{
				var entry = entries[i];
				bool found = false;
				foreach (ToplistEntry item in entryList)
				{
					if (item.username.text == entry.Username)
					{
						item.score.text = entry.Score.ToString();
						found = true;
						break;
					}
				}
				if (!found)
				{
					ToplistEntry t = Instantiate(toplistEntryPrefab, transformCache).GetComponent<ToplistEntry>();
					t.Setup(entry.Username, entry.Score, i + 1);
					entryList.Add(t);
				}
			}
		});

        lastLevel = identifier.LevelIndex;
    }
}
