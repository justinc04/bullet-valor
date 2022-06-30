using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class Gun : Item
{
    public GameObject bulletImpactPrefab;
    public Camera cam;
    public int ammo;

    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] ParticleSystem muzzleSmoke;
    [SerializeField] GameObject scopeOverlay;
    [SerializeField] GameObject cameraRecoilObject;
    [SerializeField] GameObject reloadingObject;
    [SerializeField] GameObject ADSObject;
    [SerializeField] GameObject gunRecoilObject;
    private float nextTimeToFire;
    private bool reloading;
    private bool earlyShootInput;

    private Vector3 recoil;
    private Vector3 camCurrentRot;
    private Vector3 camTargetRot;
    private Vector3 camCurrentPos;
    private Vector3 camTargetPos;


    public bool aiming;
    private float cameraFOV;

    private Vector3 gunCurrentRot;
    private Vector3 gunTargetRot;
    private Vector3 gunCurrentPos;
    private Vector3 gunTargetPos;

    private PlayerMovement playerMovement;

    private void Start()
    {
        ammo = ((GunInfo)itemInfo).ammoCapacity;
        cameraFOV = cam.fieldOfView;
        playerMovement = playerGameObject.GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (!pv.IsMine || !active)
        {
            return;
        }

        CameraRecoil();
        GunRecoil();

        if (reloading)
        {
            HipFire();
            return;
        }

        if (((GunInfo)itemInfo).fireMode == GunInfo.FireMode.Auto && Input.GetMouseButton(0) || Input.GetMouseButtonDown(0))
        {
            TryShoot();
        }
        
        if (((GunInfo)itemInfo).canADS && Input.GetMouseButton(1))
        {
            AimDownSight();
        }
        else
        {
            HipFire();
        }

        if (Input.GetKeyDown(KeyCode.R) && ammo < ((GunInfo)itemInfo).ammoCapacity)
        {
            StartCoroutine(Reload());
        }

        if (earlyShootInput && Time.time >= nextTimeToFire)
        {
            earlyShootInput = false;
            Shoot();
        }

        if (ammo == 0)
        {
            StartCoroutine(Reload());
        }
    }

    public override void Enable()
    {
        Vector3 cameraAngle = Vector3.zero;

        for (int i = 0; i < 3; i++)
        {
            cameraAngle[i] = cameraRecoilObject.transform.localEulerAngles[i] - (cameraRecoilObject.transform.localEulerAngles[i] > 180 ? 360 : 0);
        }

        camCurrentRot = cameraAngle;
    }

    public override void Disable()
    {
        camTargetRot = Vector3.zero;
        StopAllCoroutines();
        reloading = false;
        reloadingObject.transform.localPosition = Vector3.zero;
        ADSObject.transform.localPosition = Vector3.zero;
        gunRecoilObject.transform.localRotation = Quaternion.identity;

        if (pv.IsMine && ((GunInfo)itemInfo).hasScopeOverlay)
        {
            scopeOverlay.SetActive(false);
        }
    }

    void TryShoot()
    {
        if (ammo > 0)
        {
            if (!earlyShootInput && Time.time >= nextTimeToFire)
            {             
                Shoot();
            }
            else if (((GunInfo)itemInfo).fireMode == GunInfo.FireMode.SemiAuto)
            {
                earlyShootInput = true;
            }
        }
    }

    void Shoot()
    {
        nextTimeToFire = Time.time + 1 / (aiming ? ((GunInfo)itemInfo).aimingFireRate : ((GunInfo)itemInfo).fireRate);
        CameraRecoilFire();
        GunRecoilFire();
        pv.RPC("RPC_Shoot", RpcTarget.All);
        ShootBullets();
        playerAudio.Play(((GunInfo)itemInfo).gunShotSound);
    }

    public abstract void ShootBullets();

    [PunRPC]
    public void RPC_Shoot()
    {
        muzzleFlash.Play();

        if (muzzleSmoke != null)
        {
            muzzleSmoke.Play();
        }
    }

    public IEnumerator Reload()
    {
        reloading = true;

        float timer = ((GunInfo)itemInfo).reloadTime;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            reloadingObject.transform.localPosition = Vector3.Lerp(reloadingObject.transform.localPosition, ((GunInfo)itemInfo).reloadPosition, ((GunInfo)itemInfo).reloadAnimationSpeed * Time.deltaTime);
            yield return null;
        }
        
        ammo = ((GunInfo)itemInfo).ammoCapacity;
        reloading = false;

        while (!reloading)
        {
            reloadingObject.transform.localPosition = Vector3.Lerp(reloadingObject.transform.localPosition, Vector3.zero, ((GunInfo)itemInfo).reloadAnimationSpeed * Time.deltaTime);
            yield return null;
        }
    }

    void CameraRecoil()
    {
        camTargetRot = Vector3.Lerp(camTargetRot, Vector3.zero, ((GunInfo)itemInfo).returnSpeed * Time.deltaTime);
        camCurrentRot = Vector3.Slerp(camCurrentRot, camTargetRot, ((GunInfo)itemInfo).snappiness * Time.deltaTime);
        cameraRecoilObject.transform.localRotation = Quaternion.Euler(camCurrentRot);

        camTargetPos = Vector3.Lerp(camTargetPos, Vector3.zero, ((GunInfo)itemInfo).returnSpeed * Time.deltaTime);
        camCurrentPos = Vector3.Slerp(camCurrentPos, camTargetPos, ((GunInfo)itemInfo).snappiness * Time.deltaTime);
        cameraRecoilObject.transform.localPosition = camCurrentPos;
    }

    void CameraRecoilFire()
    {
        recoil = aiming ? ((GunInfo)itemInfo).aimingRecoil : ((GunInfo)itemInfo).hipFireRecoil;

        float recoilX = ((GunInfo)itemInfo).recoilType == GunInfo.RecoilType.Pattern ? recoil.x : Random.Range(0, recoil.x);
        float recoilY = Random.Range(-recoil.y, recoil.y);
        float recoilZ = Random.Range(-recoil.z, recoil.z);

        if (((GunInfo)itemInfo).recoilType == GunInfo.RecoilType.Pattern)
        {
            camTargetRot += new Vector3(recoilX, recoilY, recoilZ);
        }
        else
        {
            camTargetRot = new Vector3(recoilX, recoilY, recoilZ);
        }

        camTargetPos += new Vector3(((GunInfo)itemInfo).cameraKickback.x, ((GunInfo)itemInfo).cameraKickback.y, ((GunInfo)itemInfo).cameraKickback.z);
    }

    void HipFire()
    {
        aiming = false;
        ADSObject.transform.localPosition = Vector3.Lerp(ADSObject.transform.localPosition, Vector3.zero, ((GunInfo)itemInfo).scopeInSpeed * Time.deltaTime); 
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, cameraFOV, ((GunInfo)itemInfo).scopeInSpeed * Time.deltaTime);
        playerMovement.speedAffector = 1;

        if (((GunInfo)itemInfo).hasScopeOverlay && scopeOverlay.activeSelf)
        {
            scopeOverlay.SetActive(false);
            itemGameObject.SetActive(true);
        }
    }

    void AimDownSight()
    {
        aiming = true;
        ADSObject.transform.localPosition = Vector3.Lerp(ADSObject.transform.localPosition, ((GunInfo)itemInfo).aimingPosition, ((GunInfo)itemInfo).scopeInSpeed * Time.deltaTime);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, cameraFOV / ((GunInfo)itemInfo).aimingZoom, ((GunInfo)itemInfo).scopeInSpeed * Time.deltaTime);
        playerMovement.speedAffector = ((GunInfo)itemInfo).aimingMoveSpeedAffector;

        if (((GunInfo)itemInfo).hasScopeOverlay && !scopeOverlay.activeSelf && (Vector3.Distance(ADSObject.transform.localPosition, ((GunInfo)itemInfo).aimingPosition) < .01f))
        {
            scopeOverlay.SetActive(true);
            itemGameObject.SetActive(false);
        }
    }

    void GunRecoil()
    {
        gunTargetRot = Vector3.Lerp(gunTargetRot, Vector3.zero, ((GunInfo)itemInfo).gunReturnSpeed * Time.deltaTime);
        gunCurrentRot = Vector3.Slerp(gunCurrentRot, gunTargetRot, ((GunInfo)itemInfo).gunSnapSpeed * Time.deltaTime);
        gunRecoilObject.transform.localRotation = Quaternion.Euler(gunCurrentRot);

        gunTargetPos = Vector3.Lerp(gunTargetPos, Vector3.zero, ((GunInfo)itemInfo).gunReturnSpeed * Time.deltaTime);
        gunCurrentPos = Vector3.Slerp(gunCurrentPos, gunTargetPos, ((GunInfo)itemInfo).gunSnapSpeed * Time.deltaTime);
        gunRecoilObject.transform.localPosition = gunCurrentPos;
    }

    void GunRecoilFire()
    {
        float recoilX = ((GunInfo)itemInfo).gunRotationRecoil.x;
        float recoilY = Random.Range(-((GunInfo)itemInfo).gunRotationRecoil.y, ((GunInfo)itemInfo).gunRotationRecoil.y);
        float recoilZ = Random.Range(-((GunInfo)itemInfo).gunRotationRecoil.z, ((GunInfo)itemInfo).gunRotationRecoil.z);
        gunTargetRot += new Vector3(recoilX, recoilY, recoilZ);

        gunTargetPos += new Vector3(((GunInfo)itemInfo).gunPositionRecoil.x, ((GunInfo)itemInfo).gunPositionRecoil.y, ((GunInfo)itemInfo).gunPositionRecoil.z);
    }
}
