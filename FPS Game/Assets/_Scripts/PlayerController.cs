using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using DG.Tweening;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] GameObject cams;
    [SerializeField] GameObject UI;
    public GameObject playerModel;
    public GameObject weaponModels;
    [SerializeField] GameObject equipableItems;
    [SerializeField] Vector3 itemsThirdPersonPos;
    [SerializeField] TMP_Text healthText;
    [SerializeField] TMP_Text ammoText;
    [SerializeField] Image crosshair;
    [SerializeField] Image damageOverlay;
    [HideInInspector] public ItemInfo currentAbility;
    public GameObject abilityGraphic;
    public Image abilityImage;
    public Color abilityCooldownColor;
    [SerializeField] Slider cooldownSlider;
    [SerializeField] Collider[] hitColliders;
    public Collider movementCollider;
    public Collider headCollider;

    [HideInInspector] public float maxHealth = 100;
    [HideInInspector] public float currentHealth;

    [HideInInspector] public bool canShoot = true;
    [HideInInspector] public float damageMultiplier = 1;
    [HideInInspector] public int damageMoney;

    [SerializeField] PlayerAudio playerAudio;
    private PlayerManager playerManager;
    private PlayerItems playerItems;
    private PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        playerManager = PhotonView.Find((int)pv.InstantiationData[0]).GetComponent<PlayerManager>();
        playerItems = GetComponent<PlayerItems>();
        currentHealth = maxHealth;
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

            int itemLayer = LayerMask.NameToLayer("Item");

            foreach (Item item in playerItems.itemReferences)
            {
                if (item.itemInfo.itemType == ItemInfo.ItemType.Equipable)
                {
                    item.transform.GetChild(0).gameObject.layer = itemLayer;
                }
            }
        }
        else
        {
            Destroy(cams);
            Destroy(UI);
            Destroy(movementCollider);
            equipableItems.transform.localPosition = itemsThirdPersonPos;
        }
    }

    public void TakeDamage(float damage)
    {
        playerAudio.Play("Body Impact");
        pv.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage)
    {
        currentHealth -= damage;
        UpdateHealth();

        if (pv.IsMine)
        {
            damageOverlay.DOPause();
            Color overlayColor = damageOverlay.color;
            overlayColor.a = 1;
            damageOverlay.color = overlayColor;
            damageOverlay.DOFade(0, .5f).SetEase(Ease.Linear);
        }

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
        crosshair.enabled = false;
        playerManager.Kill();
    }

    public void UpdateHealth()
    {
        int healthToDisplay = Mathf.RoundToInt(currentHealth);
        healthText.text = (healthToDisplay == 0 ? 1 : healthToDisplay).ToString();
    }

    public void UpdateAmmo(int ammo)
    {
        ammoText.text = ammo.ToString();
    }

    public void UpdateAbilityCooldown(float value)
    {
        cooldownSlider.value = value;
    }
}
