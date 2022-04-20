using ChessDotNet;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class BoardBuilder : MonoBehaviour
{
  public Color lightColor;
  public Color darkColor;
  public GameObject BoardPiece;
  public GameObject Pawn;
  public GameObject Bishop;
  public GameObject Rook;
  public GameObject Knight;
  public GameObject King;
  public GameObject Qween;
  public bool submitted;
  public bool playing = true;
  private static BoardBuilder board;
  private Queue<string> notifications = new Queue<string>(5);
  private int points;
  public ChessGame game;
  public Text testGUI;
  public Text pointsGUI;
  public Text logGUI;
  public GameObject HUDCanvas;
  public GameObject ScoreboardCanvas;
  private Transform pieceToMove;
  private Camera _cam;
  private char[,] boardSetup = new char[8, 8]
  {
    {
      'R',
      'H',
      'B',
      'Q',
      'K',
      'B',
      'H',
      'R'
    },
    {
      'P',
      'P',
      'P',
      'P',
      'P',
      'P',
      'P',
      'P'
    },
    {
      ' ',
      ' ',
      ' ',
      ' ',
      ' ',
      ' ',
      ' ',
      ' '
    },
    {
      ' ',
      ' ',
      ' ',
      ' ',
      ' ',
      ' ',
      ' ',
      ' '
    },
    {
      ' ',
      ' ',
      ' ',
      ' ',
      ' ',
      ' ',
      ' ',
      ' '
    },
    {
      ' ',
      ' ',
      ' ',
      ' ',
      ' ',
      ' ',
      ' ',
      ' '
    },
    {
      'P',
      'P',
      'P',
      'P',
      'P',
      'P',
      'P',
      'P'
    },
    {
      'R',
      'H',
      'B',
      'Q',
      'K',
      'B',
      'H',
      'R'
    }
  };

  public int Points
  {
    get => this.points;
    set
    {
      this.points = value;
      this.pointsGUI.text = "Points: " + (object) this.points;
      if (this.points >= 0)
        return;
      this.GameOver(ChessDotNet.Player.White, "Out of points", true);
    }
  }

  public static BoardBuilder Instance()
  {
    if ((Object) BoardBuilder.board == (Object) null)
      BoardBuilder.board = Object.FindObjectOfType<BoardBuilder>();
    return BoardBuilder.board;
  }

  public void ResetBoard()
  {
    this.StopCoroutine(this.HoldPiece());
    GameObject[] objectsOfType = Object.FindObjectsOfType<GameObject>();
    this.Points = 100;
    this.logGUI.text = string.Empty;
    for (int index = 0; index < objectsOfType.Length; ++index)
    {
      if (!(objectsOfType[index].tag == "Dont Destroy"))
        Object.Destroy((Object) objectsOfType[index]);
    }
    this.BuildBoard();
  }

  public void BuildBoard()
  {
    this.game = new ChessGame();
    for (int x = 0; x < 8; ++x)
    {
      for (int index = 0; index < 8; ++index)
      {
        GameObject gameObject1 = (GameObject) Object.Instantiate((Object) this.BoardPiece, new Vector3((float) x, 0.0f, (float) index), Quaternion.identity);
        if (index % 2 == 0 && x % 2 != 0 || index % 2 != 0 && x % 2 == 0)
        {
          gameObject1.name = string.Format(this.ChessFormat(x, index));
          gameObject1.GetComponentInChildren<MeshRenderer>().material.color = this.lightColor;
        }
        else
        {
          gameObject1.name = string.Format(this.ChessFormat(x, index));
          gameObject1.GetComponentInChildren<MeshRenderer>().material.color = this.darkColor;
        }
        char ch = this.boardSetup[index, x];
        switch (ch)
        {
          case 'H':
            GameObject gameObject2 = (GameObject) Object.Instantiate((Object) this.Knight, new Vector3((float) x, 0.6f, (float) index), Quaternion.identity);
            gameObject2.name = string.Format("Knight");
            gameObject2.GetComponent<UpdateMoves>().Setup(new Position(this.ChessFormat(x, index)));
            break;
          case 'K':
            GameObject gameObject3 = (GameObject) Object.Instantiate((Object) this.King, new Vector3((float) x, 0.6f, (float) index), Quaternion.identity);
            gameObject3.name = string.Format("King");
            gameObject3.GetComponent<UpdateMoves>().Setup(new Position(this.ChessFormat(x, index)));
            break;
          default:
            switch (ch)
            {
              case 'P':
                GameObject gameObject4 = (GameObject) Object.Instantiate((Object) this.Pawn, new Vector3((float) x, 0.6f, (float) index), Quaternion.identity);
                gameObject4.name = string.Format("Pawn");
                gameObject4.GetComponent<UpdateMoves>().Setup(new Position(this.ChessFormat(x, index)));
                continue;
              case 'Q':
                GameObject gameObject5 = (GameObject) Object.Instantiate((Object) this.Qween, new Vector3((float) x, 0.6f, (float) index), Quaternion.identity);
                gameObject5.name = string.Format("Qween");
                gameObject5.GetComponent<UpdateMoves>().Setup(new Position(this.ChessFormat(x, index)));
                continue;
              case 'R':
                GameObject gameObject6 = (GameObject) Object.Instantiate((Object) this.Rook, new Vector3((float) x, 0.6f, (float) index), Quaternion.identity);
                gameObject6.name = string.Format("Rook");
                gameObject6.GetComponent<UpdateMoves>().Setup(new Position(this.ChessFormat(x, index)));
                continue;
              default:
                if (ch != ' ' && ch == 'B')
                {
                  GameObject gameObject7 = (GameObject) Object.Instantiate((Object) this.Bishop, new Vector3((float) x, 0.6f, (float) index), Quaternion.identity);
                  gameObject7.name = string.Format("Bishop");
                  gameObject7.GetComponent<UpdateMoves>().Setup(new Position(this.ChessFormat(x, index)));
                  continue;
                }
                continue;
            }
        }
      }
    }
  }

  public void PickUpPiece(Rigidbody piece)
  {
    this.pieceToMove = piece.transform;
    this.StartCoroutine(this.HoldPiece());
  }

  [DebuggerHidden]
  private IEnumerator HoldPiece() => (IEnumerator) new BoardBuilder.\u003CHoldPiece\u003Ec__Iterator3()
  {
    \u003C\u003Ef__this = this
  };

  public void DropPiece() => this.StopCoroutine(this.HoldPiece());

  public void AddPoints(int amount, string reason)
  {
    this.Points += amount;
    this.DisplayNotification(reason);
  }

  public void Punishment(int penalty)
  {
    this.Points -= penalty;
    this.DisplayNotification("You did an illegal move! You lose " + (object) penalty + " points!");
  }

  public void Punishment(int penalty, ChessDotNet.Player owner)
  {
    if (owner == ChessDotNet.Player.Black)
      return;
    this.Points -= penalty;
    this.DisplayNotification(owner.ToString("D") + " did an illegal move! You lose " + (object) penalty + " points!");
  }

  public void DisplayNotification(string message) => this.notifications.Enqueue(message);

  public void AttackPieceCheck(Move move)
  {
    if (this.game.GetPieceAt(move.NewPosition) == (Piece) null || this.game.GetPieceAt(move.NewPosition).Owner != ChessUtilities.GetOpponentOf(move.Player))
      return;
    foreach (UpdateMoves updateMoves in Object.FindObjectsOfType<UpdateMoves>())
    {
      if (updateMoves.name.Split('_')[1] == move.NewPosition.ToString())
      {
        Object.Destroy((Object) updateMoves.gameObject);
        if (move.Player == ChessDotNet.Player.White)
          this.AddPoints(15, "Woah! You just attacked your opponent. You gain 15 points");
        else
          this.Punishment(15, ChessDotNet.Player.White);
      }
    }
  }

  public bool BotMovePiece()
  {
    Move bestMoveDetailed = this.GetBestMoveDetailed(ChessDotNet.Player.Black);
    if (!this.game.IsValidMove(bestMoveDetailed))
    {
      this.Punishment(10, bestMoveDetailed.Player);
      return false;
    }
    Vector3 position = this.GetTileObject(bestMoveDetailed.NewPosition).position;
    bestMoveDetailed.TransformPiece.position = new Vector3(position.x, 3f, position.z);
    return true;
  }

  public bool MovePiece(Move move)
  {
    ChessDotNet.Player opponentOf = ChessUtilities.GetOpponentOf(move.Player);
    if (!this.game.IsValidMove(move))
    {
      if (move.Player == ChessDotNet.Player.White)
        this.Punishment(5);
      return false;
    }
    this.AttackPieceCheck(move);
    int num = (int) this.game.ApplyMove(move, true);
    if (move.Player == ChessDotNet.Player.White)
      this.AddPoints(3, "You sucessfully moved a piece. You gain 3 points");
    this.logGUI.text = move.Player.ToString() + " : " + move.ToString();
    if (!this.game.HasAnyValidMoves(opponentOf))
      this.GameOver(opponentOf, "CheckMate", false);
    else if (this.game.WhoseTurn == ChessDotNet.Player.Black)
      this.BotMovePiece();
    return true;
  }

  private Move GetBestMoveDetailed(ChessDotNet.Player owner)
  {
    List<DetailedMove> detailedMoveList1 = new List<DetailedMove>();
    List<DetailedMove> detailedMoveList2 = new List<DetailedMove>();
    foreach (DetailedMove move in this.game.Moves)
    {
      if (move.Player == owner)
      {
        if (move.IsCapture)
        {
          detailedMoveList2.Add(move);
          break;
        }
        if (this.game.IsValidMove((Move) move))
          detailedMoveList1.Add(move);
      }
      else
        break;
    }
    if (detailedMoveList2.Count > 0)
    {
      detailedMoveList2.Sort((IComparer<DetailedMove>) new BoardBuilder.MyOrderingClass());
      return (Move) detailedMoveList2[0];
    }
    ReadOnlyCollection<Move> validMoves = this.game.GetValidMoves(ChessDotNet.Player.Black);
    Move bestMoveDetailed = validMoves[Random.Range(0, validMoves.Count - 1)];
    foreach (UpdateMoves updateMoves in Object.FindObjectsOfType<UpdateMoves>())
    {
      if (updateMoves.name.Split('_')[1] == bestMoveDetailed.OriginalPosition.ToString())
        bestMoveDetailed.TransformPiece = updateMoves.transform;
    }
    return bestMoveDetailed;
  }

  private void Update()
  {
    if (this.playing)
    {
      if (Input.GetMouseButtonDown(0))
      {
        RaycastHit hitInfo;
        if (Physics.Raycast(this._cam.ScreenPointToRay(new Vector3((float) (Screen.width / 2), (float) (Screen.height / 2))), out hitInfo))
        {
          if (hitInfo.collider.name.Length == 2)
            return;
          this.PickUpPiece(hitInfo.rigidbody);
        }
      }
      else
        this.DropPiece();
    }
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      this.ScoreboardCanvas.SetActive(false);
      this.HUDCanvas.SetActive(true);
      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = false;
      this.playing = true;
      this.GameOver(ChessDotNet.Player.White, "Forfeit", true);
    }
    if (!Input.GetKeyDown(KeyCode.Tab))
      return;
    if (this.ScoreboardCanvas.activeInHierarchy)
    {
      this.ScoreboardCanvas.SetActive(false);
      this.HUDCanvas.SetActive(true);
      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = false;
      this.playing = true;
      if (!this.submitted)
        return;
      this.ResetBoard();
    }
    else
    {
      this.ScoreboardCanvas.SetActive(true);
      this.HUDCanvas.SetActive(false);
      Cursor.lockState = CursorLockMode.None;
      Cursor.visible = true;
      this.playing = false;
    }
  }

  private void GameOver(ChessDotNet.Player player, string reason, bool forceRestart)
  {
    this.notifications.Clear();
    this.DisplayNotification("Player " + player.ToString("D") + " lost! Reason: " + reason);
    if (forceRestart)
    {
      this.ResetBoard();
    }
    else
    {
      this.ScoreboardCanvas.SetActive(true);
      this.HUDCanvas.SetActive(false);
      Cursor.lockState = CursorLockMode.None;
      Cursor.visible = true;
      this.playing = false;
      this.testGUI.text = "Press ESC to play again or TAB to view the highscores.";
    }
  }

  private Transform GetTileObject(Position pos) => GameObject.Find(this.ChessFormat((int) pos.File, pos.Rank - 1)).transform;

  private Transform GetPieceObject(Position pos, ChessDotNet.Player owner)
  {
    foreach (RaycastHit raycastHit in Physics.RaycastAll(this.GetTileObject(pos).position, Vector3.up))
    {
      if (raycastHit.collider.GetComponent<UpdateMoves>().owner == owner)
        return raycastHit.transform;
    }
    return (Transform) null;
  }

  private string ChessFormat(int x, int y)
  {
    string str = string.Empty;
    switch (x)
    {
      case 0:
        str = "A";
        break;
      case 1:
        str = "B";
        break;
      case 2:
        str = "C";
        break;
      case 3:
        str = "D";
        break;
      case 4:
        str = "E";
        break;
      case 5:
        str = "F";
        break;
      case 6:
        str = "G";
        break;
      case 7:
        str = "H";
        break;
    }
    return str + (y + 1).ToString();
  }

  [DebuggerHidden]
  private IEnumerator IDisplayNotificationCycle() => (IEnumerator) new BoardBuilder.\u003CIDisplayNotificationCycle\u003Ec__Iterator4()
  {
    \u003C\u003Ef__this = this
  };

  private void Awake() => this._cam = Object.FindObjectOfType<Camera>();

  private void Start()
  {
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
    this.BuildBoard();
    this.Points = 100;
    this.StartCoroutine(this.IDisplayNotificationCycle());
  }

  public class MyOrderingClass : IComparer<DetailedMove>
  {
    public int Compare(DetailedMove x, DetailedMove y) => x.Piece.GetImportance().CompareTo(y.Piece.GetImportance());
  }
}
