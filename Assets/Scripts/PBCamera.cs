using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PBCamera: MonoBehaviour {
  Transform _backupMount;
  Transform _backupTarget;

  Transform _mount;
  Transform _target;

  void Awake() {
    _backupMount = (new GameObject {
      name = name + "BackupMount"
    }).transform;

    _backupTarget = (new GameObject {
      name = name + "BackupTarget"
    }).transform;

    UpdateBackups();

    _mount = _backupMount;
    _target = _backupTarget;
  }

  void Update() {
    transform.position = _mount.position;
    transform.LookAt(_target);
  }

  public void SetMount(Transform mount) {
    if (mount == null) {
      UpdateBackups();
      _mount = _backupMount;
    }
    else {
      _mount = mount;
    }
  }

  public void SetTarget(Transform target) {
    if (target == null) {
      UpdateBackups();
      _target = _backupTarget;
    }
    else {
      _target = target;
    }
  }

  void UpdateBackups() {
    _backupMount.position = transform.position;
    _backupTarget.position = _backupMount.position + transform.rotation * Vector3.forward;
  }
}
