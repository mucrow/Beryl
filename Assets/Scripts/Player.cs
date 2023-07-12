using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// rig.rot.x=-85, rig.pos.y=2, mount.pos.z=0
// rig.rot.x=0, rig.pos.y=2, mount.pos.z=0
// rig.rot.x=85, rig.pos.y=2, mount.pos.z=-10


public class Player: MonoBehaviour {
  [SerializeField] Transform _idealThirdPersonCameraMount;
  [SerializeField] Transform _idealThirdPersonCameraMountRig;

  [SerializeField] float _cameraHorizontalSpeed = 10f;
  [SerializeField] float _cameraVerticalSpeed = 10f;
  [SerializeField] float _moveSpeed = 6f;

  Transform _thirdPersonCameraMount;
  PBCamera _mainCamera;
  Rigidbody _rigidbody;

  void Awake() {
    _rigidbody = GetComponent<Rigidbody>();

    _thirdPersonCameraMount = (new GameObject() {
      name = name + "ThirdPersonCameraMount"
    }).transform;
  }

  void Start() {
    AddToCameraVertical(0f);

    _mainCamera = Camera.main.GetComponent<PBCamera>();
    _mainCamera.SetMount(_thirdPersonCameraMount);
    _mainCamera.SetTarget(_idealThirdPersonCameraMountRig);
  }

  void Update() {
    // if (_moveSpeed != 0f) {
    //   var newPosition = transform.position;
    //   newPosition.x = posOscPivot + 10f * Mathf.Sin(Time.timeSinceLevelLoad * 0.5f * _moveSpeed);
    //   _rigidbody.Move(newPosition, transform.rotation);
    // }

    var cameraHorizontalInput = 0f;
    if (Input.GetKey(KeyCode.RightArrow)) {
      cameraHorizontalInput -= 1f;
    }
    if (Input.GetKey(KeyCode.LeftArrow)) {
      cameraHorizontalInput += 1f;
    }
    if (cameraHorizontalInput != 0f) {
      AddToCameraHorizontal(cameraHorizontalInput * _cameraHorizontalSpeed * Time.deltaTime);
    }

    var cameraVerticalInput = 0f;
    if (Input.GetKey(KeyCode.UpArrow)) {
      cameraVerticalInput -= 1f;
    }
    if (Input.GetKey(KeyCode.DownArrow)) {
      cameraVerticalInput += 1f;
    }
    if (cameraVerticalInput != 0f) {
      AddToCameraVertical(cameraVerticalInput * _cameraVerticalSpeed * Time.deltaTime);
    }

    UpdateThirdPersonCameraMountPosition();
  }

  void FixedUpdate() {
    var moveInput = Vector3.zero;

    if (Input.GetKey(KeyCode.S)) {
      moveInput.z -= 1;
    }
    if (Input.GetKey(KeyCode.W)) {
      moveInput.z += 1;
    }

    if (Input.GetKey(KeyCode.A)) {
      moveInput.x -= 1;
    }
    if (Input.GetKey(KeyCode.D)) {
      moveInput.x += 1;
    }

    var cameraRotation = Quaternion.Euler(0f, _mainCamera.transform.rotation.y, 0f);
    _rigidbody.velocity = cameraRotation * transform.rotation * moveInput.normalized * _moveSpeed;
  }

  void UpdateThirdPersonCameraMountPosition() {
    var idealMountRigPosition = _idealThirdPersonCameraMountRig.position;
    var idealMountPosition = _idealThirdPersonCameraMount.position;
    var positionDelta = idealMountPosition - idealMountRigPosition;
    var mountDistance = positionDelta.magnitude;
    var hits = Physics.RaycastAll(idealMountRigPosition, positionDelta, mountDistance);
    foreach (var hit in hits) {
      mountDistance = Mathf.Min(mountDistance, hit.distance);
    }
    _thirdPersonCameraMount.position = idealMountRigPosition + (positionDelta.normalized * mountDistance);
  }

  void AddToCameraHorizontal(float amount) {
    var rigAngles = _idealThirdPersonCameraMountRig.eulerAngles;
    rigAngles.y += amount;
    _idealThirdPersonCameraMountRig.eulerAngles = rigAngles;
  }

  void AddToCameraVertical(float amount) {
    var rigAngles = _idealThirdPersonCameraMountRig.eulerAngles;

    var newAngle = rigAngles.x + amount;
    if (newAngle < 180f) {
      newAngle = Mathf.Clamp(newAngle, -85f, 85f);
    }
    else {
      newAngle = Mathf.Max(newAngle, 360f - 85f);
    }

    rigAngles.x = newAngle;
    _idealThirdPersonCameraMountRig.eulerAngles = rigAngles;

    var mountPosition = _idealThirdPersonCameraMount.localPosition;
    if (newAngle <= 0f || newAngle >= 180f) {
      mountPosition.z = -1 * (10f - 10f * ((((newAngle + 360f) % 360f) - 275f) / 85f));
    }
    // 0 < theta <= 10
    else if (newAngle <= 1f) {
      mountPosition.z = -10f * (1 - Mathf.Pow(1 - newAngle / 1f, 1));
    }
    // 10 < theta < 180
    else {
      mountPosition.z = -10f;
    }
    _idealThirdPersonCameraMount.localPosition = mountPosition;

    var rigPosition = _idealThirdPersonCameraMountRig.localPosition;
    if (newAngle <= 0f || newAngle >= 180f) {
      rigPosition.z = 10f - 10f * ((((newAngle + 360f) % 360f) - 275f) / 85f);
    }
    else {
      rigPosition.z = 0f;
    }
    _idealThirdPersonCameraMountRig.localPosition = rigPosition;
  }
}
