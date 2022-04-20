using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class UnityDub : MonoBehaviour
{
  private Texture2D texture;
  private byte[] buffer;
  public string virtualdubLocation;
  public string startRecording = "f1";
  public string stopRecording = "f2";
  private bool recording;
  private string location;
  private string prefix = "frame_";
  private List<string> frames;
  private int frameCount;
  public int FPS = 30;

  private void Start()
  {
    this.texture = new Texture2D(Screen.width, Screen.height);
    this.location = Application.dataPath + "/tmp/";
    if (!Directory.Exists(this.location))
      Directory.CreateDirectory(this.location);
    this.frames = new List<string>();
  }

  private void Update()
  {
    if (Input.GetKeyDown(this.startRecording))
    {
      Time.captureFramerate = this.FPS;
      this.recording = true;
    }
    if (Input.GetKeyDown(this.stopRecording))
    {
      Time.captureFramerate = 0;
      this.recording = false;
    }
    if (this.recording || this.frames.Count <= 0)
      return;
    Process process = new Process();
    process.StartInfo = new ProcessStartInfo()
    {
      FileName = this.virtualdubLocation + "virtualdub.exe",
      Arguments = "\"" + this.frames[0] + "\"",
      RedirectStandardOutput = true,
      UseShellExecute = false
    };
    process.Start();
    string end = process.StandardOutput.ReadToEnd();
    process.WaitForExit();
    MonoBehaviour.print((object) end);
    for (int index = 0; index < this.frames.Count; ++index)
      File.Delete(this.frames[index]);
    this.frames.Clear();
  }

  private void OnPostRender()
  {
    if (!this.recording)
      return;
    string path = this.location + this.prefix + this.frameCount.ToString("00000") + ".jpg";
    this.texture.ReadPixels(new Rect(0.0f, 0.0f, (float) Screen.width, (float) Screen.height), 0, 0);
    this.texture.Apply();
    this.buffer = this.texture.EncodeToJPG();
    File.WriteAllBytes(path, this.buffer);
    this.frames.Add(path);
    ++this.frameCount;
  }

  private void OnApplicationQuit()
  {
    if (!Directory.Exists(this.location))
      return;
    Directory.Delete(this.location);
  }
}
