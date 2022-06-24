using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRecoil : MonoBehaviour
{
    [SerializeField] Vector3 recoil;
    [SerializeField] float snappiness;
    [SerializeField] float returnSpeed;

    private Vector3 currentRot;
    private Vector3 targetRot;

    private void Update()
    {
        targetRot = Vector3.Lerp(targetRot, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRot = Vector3.Slerp(currentRot, targetRot, snappiness * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(currentRot);
    }

    public void RecoilFire()
    {
        targetRot += new Vector3(recoil.x, Random.Range(-recoil.y, recoil.y), Random.Range(-recoil.z, recoil.z));
    }
}
