using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class Menu : MonoBehaviour
{
  private const string singleplayerName = "Main";
  private const string multiplayerName = "ENTER NAME HERE";
  public GameObject loadingIcon;

  public void LoadSinglePlayer() => this.StartCoroutine(this.LoadAsync("Main"));

  [DebuggerHidden]
  private IEnumerator LoadAsync(string sceneName) => (IEnumerator) new Menu.\u003CLoadAsync\u003Ec__Iterator0()
  {
    sceneName = sceneName,
    \u003C\u0024\u003EsceneName = sceneName,
    \u003C\u003Ef__this = this
  };
}
