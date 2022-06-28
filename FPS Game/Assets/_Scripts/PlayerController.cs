using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] GameObject cam;
    [SerializeField] GameObject UI;
    [SerializeField] GameObject playerModel;
    [SerializeField] TMP_Text healthText;
    [SerializeField] TMP_Text ammoText;
    [SerializeField] Collider[] hitColliders;
    [SerializeField] Collider movementCollider;
    public Collider headCollider;

    private const float maxHealth = 100;
    [HideInInspector] public float currentHealth = maxHealth;

    private PlayerManager playerManager;
    private PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        playerManager = PhotonView.Find((int)pv.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    private void Start()
    {
        if (pv.IsMine)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Destroy(playerModel);

            foreach (Collider collider in hitColliders)
            {
                Destroy(collider);
            }
        }
        else
        {
            Destroy(cam);
            Destroy(UI);
            Destroy(movementCollider);
        }
    }

    public void TakeDamage(float damage)
    {
        pv.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthText.text = currentHealth.ToString();

        if (currentHealth <= 0)
        {
            if (!pv.IsMine)
            {
                gameObject.SetActive(false);
                return;
            }

            Die();
        }
    }

    void Die()
    {
        playerManager.Die();
    }

    public void Kill()
    {
        playerManager.Kill();
    }

    public void UpdateAmmo(int ammo)
    {
        ammoText.text = ammo.ToString();
    }
}
