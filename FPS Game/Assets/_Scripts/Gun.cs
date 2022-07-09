using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Gun : Item
{
    [SerializeField] Camera cam;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] ParticleSystem muzzleSmoke;
    [SerializeField] TrailRenderer bulletTrailPrefab;
    [SerializeField] GameObject bulletImpactPrefab;
    [SerializeField] GameObject bloodEffectPrefab;
    [SerializeField] GameObject scopeOverlay;
    [SerializeField] GameObject cameraRecoilObject;
    [SerializeField] GameObject reloadingObject;
    [SerializeField] GameObject ADSObject;
    [SerializeField] GameObject gunRecoilObject;

    public int ammo;
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
    private PlayerManager playerManager;

    private void Start()
    {
        ammo = ((GunInfo)itemInfo).ammoCapacity;
        cameraFOV = cam.fieldOfView;
        playerMovement = playerGameObject.GetComponent<PlayerMovement>();
        playerManager = GameManager.Instance.playerManager;
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
        playerAudio.Play(((GunInfo)itemInfo).gunShotSound);
        ammo--;

        for (int i = 0; i < ((GunInfo)itemInfo).bulletsPerShot; i++)
        {
            ShootBullets();
        }      
    }

    void ShootBullets()
    {
        Vector3 spreadX = cam.transform.up * Random.Range(-1f, 1f);
        Vector3 spreadY = cam.transform.right * Random.Range(-1f, 1f);
        float aimReduction = aiming ? .75f : 1;
        float jumpingInaccuracy = playerMovement.isGrounded ? 0 : Mathf.Abs(playerMovement.velocity.y);
        float movementInaccuracy = ((GunInfo)itemInfo).movementInaccuracy * (playerMovement.speed * playerMovement.direction.magnitude + jumpingInaccuracy) / playerMovement.runSpeed;
        float spreadAmount = Random.Range(-((GunInfo)itemInfo).spread, ((GunInfo)itemInfo).spread) * .001f * aimReduction;
        spreadAmount += movementInaccuracy * Mathf.Sign(spreadAmount) * .001f;
        Vector3 spread = (spreadX + spreadY).normalized * spreadAmount;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward + spread);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.distance > ((GunInfo)itemInfo).range)
            {
                pv.RPC("RPC_ShootBullets", RpcTarget.All, ray.GetPoint(((GunInfo)itemInfo).range), Vector3.zero, false, false);
                return;
            }

            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag("Player") && hitObject != playerGameObject)
            {

                if (hit.collider == hitObject.GetComponent<PlayerController>().headCollider)
                {
                    hitObject.GetComponent<IDamageable>().TakeDamage(((GunInfo)itemInfo).headDamage * playerManager.damageMultiplier);
                    playerManager.damageDealt += ((GunInfo)itemInfo).headDamage * playerManager.damageMultiplier;
                    playerManager.headShots++;
                }
                else
                {
                    hitObject.GetComponent<IDamageable>().TakeDamage(((GunInfo)itemInfo).bodyDamage * playerManager.damageMultiplier);
                    playerManager.damageDealt += ((GunInfo)itemInfo).bodyDamage * playerManager.damageMultiplier;
                    playerManager.bodyShots++;
                }

                if (hitObject.GetComponent<PlayerController>().currentHealth <= 0)
                {
                    playerGameObject.GetComponent<PlayerController>().Kill();
                }

                pv.RPC("RPC_ShootBullets", RpcTarget.All, hit.point, hit.normal, true, false);
            }
            else if (hitObject.layer == LayerMask.NameToLayer("Ground"))
            {
                pv.RPC("RPC_ShootBullets", RpcTarget.All, hit.point, hit.normal, false, true);
            }
        }
    }


    [PunRPC]
    void RPC_ShootBullets(Vector3 hitPosition, Vector3 hitNormal, bool hitPlayer, bool hitGround)
    {
        StartCoroutine(SpawnTrail(hitPosition, hitNormal, hitPlayer, hitGround));
    }

    IEnumerator SpawnTrail(Vector3 hitPosition, Vector3 hitNormal, bool hitPlayer, bool hitGround)
    {
        float time = 0;
        Vector3 startPos = muzzleFlash.transform.position;
        TrailRenderer trail = Instantiate(bulletTrailPrefab, startPos, Quaternion.identity);

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPos, hitPosition, time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }

        trail.transform.position = hitPosition;
        Destroy(trail.gameObject, 1);

        if (hitGround) 
        {
            SpawnImpact(hitPosition, hitNormal);
        }
        else if (hitPlayer && pv.IsMine)
        {
            SpawnBlood(hitPosition, hitNormal);
        }
    }

    void SpawnImpact(Vector3 hitPosition, Vector3 hitNormal)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPosition, .3f);

        if (colliders.Length != 0)
        {
            GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * .001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
            Destroy(bulletImpactObj, 10);
            bulletImpactObj.transform.SetParent(colliders[0].transform);
        }
    }

    void SpawnBlood(Vector3 hitPosition, Vector3 hitNormal)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPosition, .3f);

        if (colliders.Length != 0)
        {
            GameObject bloodEffectObj = Instantiate(bloodEffectPrefab, hitPosition + hitNormal * .001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bloodEffectPrefab.transform.rotation);
            Destroy(bloodEffectObj, 1);
        }
    }

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
        playerMovement.weaponSpeedAffector = 1;

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
        playerMovement.weaponSpeedAffector = ((GunInfo)itemInfo).aimingMoveSpeedAffector;

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
