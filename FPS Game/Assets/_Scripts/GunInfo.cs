using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "FPS/New Gun")]
public class GunInfo : ItemInfo
{
    [Header("Properties")]
    public float headDamage;
    public float bodyDamage;
    public float fireRate;
    public float spread;
    public int ammoCapacity;
    public enum FireMode { Auto, SemiAuto, Semi };
    public FireMode fireMode;
    public enum GunClass { Primary, Secondary }
    public GunClass gunClass;

    [Header("Reloading")]
    public float reloadTime;
    public Vector3 reloadPosition;
    public float reloadAnimationSpeed;

    [Header("Camera Recoil")]
    public Vector3 hipFireRecoil;
    public Vector3 aimingRecoil;
    public enum RecoilType { Pattern, Random }
    public RecoilType recoilType;
    public Vector3 cameraKickback;
    public float snappiness;
    public float returnSpeed;

    [Header("Gun Recoil")]
    public Vector3 gunRotationRecoil;
    public Vector3 gunPositionRecoil;
    public float gunSnapSpeed;
    public float gunReturnSpeed;

    [Header("Aiming")]
    public bool canADS;
    public bool hasScopeOverlay;
    public Vector3 aimingPosition;
    public float aimingZoom;
    public float scopeInSpeed;
    public float aimingFireRate;
    public float aimingMoveSpeedAffector;

    [Header("Audio")]
    public string gunShotSound;
}
