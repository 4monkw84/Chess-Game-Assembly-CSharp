using ChessDotNet;
using UnityEngine;

public class UpdateMoves : MonoBehaviour
{
  private Position originalPosition;
  private Position newPosition;
  private Transform _tr;
  private AudioSource _as;
  private Vector3 cachePosition;
  private Rigidbody _rb;
  private Quaternion _rot;
  public ChessDotNet.Player owner;

  private void Awake()
  {
    this._rb = this.GetComponent<Rigidbody>();
    this._tr = this.GetComponent<Transform>();
    this._as = this.GetComponent<AudioSource>();
    this._rot = Quaternion.Euler(Vector3.zero);
  }

  public void Setup(Position startPosition)
  {
    this.originalPosition = startPosition;
    this.UpdatePositionName(startPosition);
    if (this.ReadPiecePosition().Rank >= 7)
    {
      this.GetComponent<MeshRenderer>().material.color = Color.black;
      this.owner = ChessDotNet.Player.Black;
    }
    else
      this.owner = ChessDotNet.Player.White;
  }

  public void UpdatePositionName(Position position)
  {
    if (this.name.Contains("_"))
    {
      this.gameObject.name = this.gameObject.name.Split('_')[0] + "_" + position.ToString();
    }
    else
    {
      GameObject gameObject = this.gameObject;
      gameObject.name = gameObject.name + "_" + position.ToString();
    }
  }

  public void OnPieceDropped()
  {
    if (BoardBuilder.Instance().MovePiece(new Move(this.originalPosition, this.newPosition, this.owner)
    {
      TransformPiece = this._tr
    }))
    {
      this.originalPosition = this.newPosition;
      this.UpdatePositionName(this.originalPosition);
      foreach (UpdateMoves updateMoves in Object.FindObjectsOfType<UpdateMoves>())
      {
        if (updateMoves.name.Contains("_") && updateMoves.name != this.gameObject.name)
        {
          if (updateMoves.name.Split('_')[1] == this.newPosition.ToString())
            Object.Destroy((Object) updateMoves.gameObject);
        }
      }
    }
    else
      this.MoveBackPiece();
  }

  public void MoveBackPiece()
  {
    BoardBuilder.Instance().DropPiece();
    this._rb.velocity = Vector3.zero;
    this._rb.rotation = Quaternion.Euler(Vector3.zero);
    this.cachePosition = GameObject.Find(this.gameObject.name.Split('_')[1]).transform.position;
    this._rb.MovePosition(new Vector3(this.cachePosition.x, 3f, this.cachePosition.z));
  }

  private void OnCollisionEnter(Collision collision)
  {
    if (!((Object) this._as != (Object) null))
      return;
    this._as.Play();
  }

  private void OnCollisionStay(Collision collision)
  {
    if ((double) Mathf.Abs(this._tr.position.x - this.cachePosition.x) <= 0.00999999977648258 && (double) Mathf.Abs(this._tr.position.z - this.cachePosition.z) <= 0.00999999977648258 || Input.GetMouseButton(0) && this.owner == ChessDotNet.Player.White || !(this.GetPiecePosition() != this.originalPosition))
      return;
    this.newPosition = this.GetPiecePosition();
    if (this.newPosition == (Position) null)
      return;
    this.OnPieceDropped();
  }

  public Position ReadPiecePosition()
  {
    if (!this.gameObject.name.Contains("_"))
      return (Position) null;
    return new Position(this.gameObject.name.Split('_')[1]);
  }

  public Position GetPiecePosition()
  {
    foreach (RaycastHit raycastHit in Physics.RaycastAll(this._tr.position, Vector3.down))
    {
      if (raycastHit.collider.name.Length == 2)
        return new Position(raycastHit.collider.name);
    }
    return (Position) null;
  }

  private Vector3 RoundVector3(Vector3 input) => new Vector3(Mathf.Round(input.x), input.y, Mathf.Round(input.z));

  public void LateUpdate()
  {
    if ((double) this._tr.position.y < -10.0 || (double) this._tr.position.y > 10.0)
      this.MoveBackPiece();
    this._tr.rotation = this._rot;
  }
}
