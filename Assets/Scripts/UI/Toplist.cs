// (C) king.com Ltd 2018
using System.Collections.Generic;
using UnityEngine;

using Services;

public class Toplist : MonoBehaviour {

    public GameObject toplistEntryPrefab;
	private List<ToplistEntry> entryList = new List<ToplistEntry> ();

    public void Display(IToplistProvider provider, IToplistIdentifier identifier)
    {
        var transformCache = transform;

        provider.Get(identifier, (entries) => {
            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
				ToplistEntry[] currentList = GetComponents<ToplistEntry>();
				bool found = false;
				foreach(ToplistEntry item in entryList)
				{
					if(item.username.text == entry.Username)
					{
						item.score.text = entry.Score.ToString();
						found=true;
						break;
					}
				}
				if(!found)
				{
					ToplistEntry t = Instantiate(toplistEntryPrefab, transformCache).GetComponent<ToplistEntry>();//.Setup(entry.Username, entry.Score, i + 1);
					t.Setup(entry.Username, entry.Score, i + 1);
					entryList.Add(t);
				}
            }
        });
    }
}
