using UnityEngine;

public class CameraMovement : MonoBehaviour
{
  private const float sensititity = 1000f;
  private Transform _tr;
  private Rigidbody _rb;

  private void Awake()
  {
    this._rb = this.GetComponent<Rigidbody>();
    this._tr = this.GetComponent<Transform>();
  }

  private void FixedUpdate()
  {
    if (!BoardBuilder.Instance().playing)
      return;
    float axis1 = Input.GetAxis("Horizontal");
    float axis2 = Input.GetAxis("Vertical");
    float axis3 = Input.GetAxis("Jump");
    this._rb.AddRelativeForce(new Vector3(axis1, axis3, axis2) * 1000f * Time.fixedDeltaTime);
    this._rb.AddRelativeTorque(new Vector3((float) (-(double) Input.GetAxis("Mouse Y") * 0.333000004291534), Input.GetAxis("Mouse X"), 0.0f) * 1000f * Time.deltaTime);
    this._tr.rotation = Quaternion.Euler(this._tr.rotation.eulerAngles.x, this._tr.eulerAngles.y, 0.0f);
  }
}
