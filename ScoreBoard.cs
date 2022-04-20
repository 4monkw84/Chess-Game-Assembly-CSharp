using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
  private const string getScoreUrl = "http://isaakeriksson.t15.org/display.php";
  private const string postScoreUrl = "http://isaakeriksson.t15.org/addscore.php?";
  private const string secretKey = "youarenotsuposedtoseethis";
  private GameObject primaryScorePanel = new GameObject();
  public GameObject scorePanel;
  public GameObject loadingIcon;
  private ScoreBoard.Player primaryPlayer;

  public void OnEnable()
  {
    for (int index = 0; index < this.transform.childCount; ++index)
    {
      if (!(this.transform.GetChild(index).name == "Primary") || !BoardBuilder.Instance().submitted)
        Object.Destroy((Object) this.transform.GetChild(index).gameObject);
    }
    this.loadingIcon.SetActive(true);
    this.StartCoroutine(this.GetScores());
    if (!BoardBuilder.Instance().submitted)
    {
      this.primaryScorePanel = Object.Instantiate<GameObject>(this.scorePanel);
      this.primaryScorePanel.transform.SetParent(this.transform);
      this.primaryScorePanel.name = "Primary";
      this.primaryScorePanel.transform.Find("InputField").gameObject.SetActive(true);
      this.primaryScorePanel.transform.Find("Name").gameObject.SetActive(false);
      this.primaryScorePanel.GetComponentInChildren<InputField>().onEndEdit.AddListener((UnityAction<string>) (arg0 => this.SubmitScore()));
    }
    else
    {
      this.primaryScorePanel.transform.Find("InputField").gameObject.SetActive(false);
      this.primaryScorePanel.transform.Find("Name").gameObject.SetActive(true);
    }
    this.primaryScorePanel.transform.Find("Score").GetComponent<Text>().text = Object.FindObjectOfType<BoardBuilder>().Points.ToString();
  }

  [DebuggerHidden]
  public IEnumerator GetScores() => (IEnumerator) new ScoreBoard.\u003CGetScores\u003Ec__Iterator1()
  {
    \u003C\u003Ef__this = this
  };

  public void LateUpdate() => this.loadingIcon.transform.Rotate(0.0f, 0.0f, 200f * Time.deltaTime);

  public void SubmitScore()
  {
    this.primaryScorePanel.transform.Find("InputField").gameObject.SetActive(false);
    this.primaryScorePanel.transform.Find("Name").gameObject.SetActive(true);
    string text = this.primaryScorePanel.transform.Find("InputField").GetComponent<InputField>().text;
    string playerScore = Object.FindObjectOfType<BoardBuilder>().Points.ToString();
    this.primaryScorePanel.transform.Find("Name").GetComponent<Text>().text = text;
    if (!this.isActiveAndEnabled)
      return;
    this.StartCoroutine(this.ISubmitScore(text, playerScore));
  }

  [DebuggerHidden]
  private IEnumerator ISubmitScore(string playerName, string playerScore) => (IEnumerator) new ScoreBoard.\u003CISubmitScore\u003Ec__Iterator2()
  {
    playerName = playerName,
    playerScore = playerScore,
    \u003C\u0024\u003EplayerName = playerName,
    \u003C\u0024\u003EplayerScore = playerScore,
    \u003C\u003Ef__this = this
  };

  public void DisplayPlayers(List<ScoreBoard.Player> players)
  {
    players.Sort((IComparer<ScoreBoard.Player>) new ScoreBoard.MyOrderingClass());
    int count = players.Count;
    foreach (ScoreBoard.Player player in players)
    {
      GameObject gameObject = Object.Instantiate<GameObject>(this.scorePanel);
      foreach (Text componentsInChild in gameObject.GetComponentsInChildren<Text>())
      {
        if (componentsInChild.name == "Name")
          componentsInChild.text = string.Format("#{0}: {1}", (object) count, (object) player.name);
        else if (componentsInChild.name == "Score")
          componentsInChild.text = player.score.ToString();
      }
      gameObject.transform.SetParent(this.transform);
      gameObject.transform.SetAsFirstSibling();
      --count;
    }
    this.primaryScorePanel.transform.SetAsFirstSibling();
  }

  public struct Player
  {
    public string name;
    public int score;
  }

  public class MyOrderingClass : IComparer<ScoreBoard.Player>
  {
    public int Compare(ScoreBoard.Player x, ScoreBoard.Player y) => x.score.CompareTo(y.score);
  }
}
