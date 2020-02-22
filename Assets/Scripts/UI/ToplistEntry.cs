// (C) king.com Ltd 2018

using UnityEngine;
using UnityEngine.UI;

public class ToplistEntry : MonoBehaviour
{
    public Text rank;
    public Text username;
    public Text score;

    public void Setup(string username, int score, int rank)
    {
        this.username.text = username;
        this.score.text = score.ToString();
        this.rank.text = rank.ToString();
    }
}
