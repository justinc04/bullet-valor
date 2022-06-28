using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SingleShotGun : Gun
{
    public override void ShootBullets()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(.5f, .5f));
        ray.origin = cam.transform.position;
        ammo--;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag("Player"))
            {

                if (hit.collider == hitObject.GetComponent<PlayerController>().headCollider)
                {
                    hitObject.GetComponent<IDamageable>().TakeDamage(((GunInfo)itemInfo).headDamage);
                }
                else
                {
                    hitObject.GetComponent<IDamageable>().TakeDamage(((GunInfo)itemInfo).bodyDamage);
                }

                if (hitObject.GetComponent<PlayerController>().currentHealth <= 0)
                {
                    playerGameObject.GetComponent<PlayerController>().Kill();
                }

                return;
            }

            pv.RPC("RPC_Hit", RpcTarget.All, hit.point, hit.normal);
        }
    }

    [PunRPC]
    void RPC_Hit(Vector3 hitPosition, Vector3 hitNormal)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPosition, .3f);

        if (colliders.Length != 0)
        {
            GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * .001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
            Destroy(bulletImpactObj, 10);
            bulletImpactObj.transform.SetParent(colliders[0].transform);
        }
    }
}
