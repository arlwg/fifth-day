using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NewPlayerWeaponSystem : MonoBehaviour
{
    // ReSharper disable Unity.PerformanceAnalysis
    public Item itemInHand;
    private Item itemInHandCheck;
    
    private float damage = 0;
    private float range = 0;
    private float attackSpeed;
    private PlayerController _player;
    private Transform weaponPoint;
    private Camera _camera;
    delegate void WeaponActionDelegate();
    private WeaponActionDelegate weaponAction;
    public List<WeaponType> rangeWeapons;
    public List<WeaponType> meleeWeapons;

    private Image handItemImage;
    private Sprite handItemDefaultSprite;
    
   


    private bool primaryMeleeAttack = true;
    private Animator playerAnimator;
    private bool readyToMeleeAttack = true;
    private bool readyToShoot = true;
    private float fireRate = 0.2f;
    public int count = 1;
    [Header("Bullet Properties")]
    private Image BulletsItemImage;
    [SerializeField] private Item bullet;
    [SerializeField] private int bulletCount;
    [SerializeField] private TMP_Text bulletCountText;
    private void Start()
    {
        handItemImage = GameObject.Find("HandItemImage").GetComponent<Image>();
        BulletsItemImage = GameObject.Find("BorderOfBullets").GetComponent<Image>();
        bulletCountText = GameObject.Find("BulletCountText").GetComponent<TMP_Text>();
        bulletCountText.text = bulletCount.ToString();
        handItemDefaultSprite = handItemImage.sprite;
        _player = FindObjectOfType<PlayerController>();
        _camera = FindCamera();
        weaponPoint = GameObject.Find("WeaponPoint").transform;
        itemInHandCheck = null;
        playerAnimator = GameObject.Find("FPS_Body").GetComponent<Animator>();
        setIconOfHandItem();
    }
    
    void Update()
    {
        if (BulletsItemImage.gameObject.activeSelf)
        {
            bulletCountText.text = bulletCount.ToString();
        }
        if (itemInHand != itemInHandCheck)
        {
            itemInHandCheck = itemInHand;
            setIconOfHandItem();
            //empty the weapon point
            Queue<GameObject> objtsToDelete = new Queue<GameObject>();
            foreach (Transform child in weaponPoint)
                objtsToDelete.Enqueue(child.gameObject);
            while (objtsToDelete.Count > 0)
                Destroy(objtsToDelete.Dequeue());
              
            damage = 4.0f;
            weaponAction = NotWeaponAction;
            range = 1.8f;
            if (itemInHand != null && itemInHand.itemType == ItemType.WEAPON)
            {
                var newWeaponObj = itemInHand.prefabOfAnObject;
                var newWeapon = newWeaponObj.GetComponent<Weapon>();
                damage = itemInHand.damage;
                range = itemInHand.range;
                
                //Instantiate the new object to the weapon point
                Instantiate(newWeaponObj, weaponPoint);
                
                if (rangeWeapons.Any(type => type == newWeapon.weaponType))
                {
                    weaponAction = RangeWeaponAction;
                    newWeapon.setupParticle();
                    fireRate = itemInHand.FireFrequency;
                }
                else if (meleeWeapons.Any(type => type == newWeapon.weaponType))
                {
                    weaponAction = MeleeWeaponAction;
                }
            }
        }
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            useWeaponAction();
        }
    }

    private void setIconOfHandItem()
    {
       
        handItemImage.sprite = (itemInHand) ? itemInHand.icon : handItemDefaultSprite;
        var temp = (itemInHand != null) ? itemInHand.prefabOfAnObject.GetComponent<Weapon>() : null;
       
        if(temp == null)
        {
            Debug.Log(temp);
            deactivateBullets();
            return;
        }
        if (temp != null)
        {
            if (rangeWeapons.Any(type => type == temp.weaponType))
            {
                BulletsItemImage.gameObject.SetActive(true);
                var inventoryManager = FindObjectOfType<InventoryManagerNew>();
                foreach (var slot in inventoryManager.listOfInventorySlots)
                {
                    if (slot.item == bullet)
                    {
                        bulletCount += slot.count;
                        slot.count = 0;
                    }
                }
                foreach (var slot in inventoryManager.listOfQuickSlots)
                {
                    if (slot.itemInSlot == bullet)
                    {
                        bulletCount += slot.itemCount;
                        slot.itemCount = 0;
                    }
                }
            }
            else
            {
                deactivateBullets();
            }
        }
        
    }

    void deactivateBullets()
    {
        BulletsItemImage.gameObject.SetActive(true);
        var inventoryManager = FindObjectOfType<InventoryManagerNew>();
        var slotWithBullets = inventoryManager.listOfInventorySlots.Find(slot => (slot.item == bullet));
        if (slotWithBullets != null)
        {
            slotWithBullets.item = bullet;
            slotWithBullets.count += bulletCount;
            bulletCount = 0;
        }
        var emptyInvetorySlot = inventoryManager.listOfInventorySlots.Find(slot => (slot.item == null));
        if (emptyInvetorySlot != null)
        {
            emptyInvetorySlot.item = bullet;
            emptyInvetorySlot.count = bulletCount;
            bulletCount = 0;
        }
        BulletsItemImage.gameObject.SetActive(false);
    }
    public void useWeaponAction()
    {
        weaponAction?.Invoke();
    }

   
    void RangeWeaponAction()
    {
        if (readyToShoot && bulletCount > 0)
        {
            bulletCount--;
            readyToShoot = false;
            AudioManager.Instance.Play("GunShot");
            StartCoroutine(ResetReadyForRangeAttack());
            itemInHand.prefabOfAnObject.GetComponent<Weapon>().PlayParticleSystem();
            simpleRayCastDamageToEnemy();
            Gather();
        }
    }
    IEnumerator ResetReadyForRangeAttack()
    {
        yield return new WaitForSeconds(fireRate);
        readyToShoot = true;
    }
   
    void MeleeWeaponAction()
    {
        //Todo: play animation of attack
        if (readyToMeleeAttack && _player.stamina > 12.0f )
        {
            readyToMeleeAttack = false;
            _player.stamina -= 12.0f;
            if (primaryMeleeAttack)
            {
                StopCoroutine(ResetMeleeAttackType());
                primaryMeleeAttack = false;
                playerAnimator.SetTrigger("MeleeAttack");
                StartCoroutine(ResetReadyForMeleeAttack("MeleeAttack"));
                StartCoroutine(ResetMeleeAttackType());
            }
            else
            {
                StopCoroutine(ResetMeleeAttackType());
                primaryMeleeAttack = true;
                playerAnimator.SetTrigger("MeleeAttack2");
                StartCoroutine(ResetReadyForMeleeAttack("MeleeAttack2"));
            }
            AudioManager.Instance.Play("AxeSound");
            StartCoroutine(MakeDamage());
        }
    }

    void NotWeaponAction()
    {
        if (readyToMeleeAttack && _player.stamina > 5.0f)
        {
            readyToMeleeAttack = false;
            _player.stamina -= 5.0f;
                StopCoroutine(ResetMeleeAttackType());
                playerAnimator.SetTrigger("MeleeAttack");
                StartCoroutine(ResetReadyForMeleeAttack("MeleeAttack"));
                StartCoroutine(MakeDamage());
        }
    }

    IEnumerator ResetMeleeAttackType()
    {
        yield return new WaitForSeconds(0.8f);
        primaryMeleeAttack = true;
    }
    IEnumerator ResetReadyForMeleeAttack(string attack)
    {
        yield return new WaitForSeconds(0.4f);
        playerAnimator.ResetTrigger(attack);
        readyToMeleeAttack = true;
    }
    IEnumerator MakeDamage()
    {
        yield return new WaitForSeconds(0.2f);
        simpleRayCastDamageToEnemy();
        Gather();
    }
    RaycastHit GetRayCastHit()
    {
        int playerLayer = LayerMask.NameToLayer("Player");
        int layerMask = ~(1 << playerLayer);
        
        
        RaycastHit hit;
        var cameraTransform = _camera.transform;
        Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, range, layerMask);
        return hit;
    }

    void Gather()
    {
        if (GetRayCastHit().collider != null)
        {
            Debug.Log("Testing : " + GetRayCastHit().transform.gameObject.name);
            GameObject hitObject = GetRayCastHit().transform.gameObject;
            if (hitObject.GetComponent<MapObject>() != null)
            {
                MapObject mObject = hitObject.GetComponent<MapObject>();
                Debug.Log("Gathering + " + mObject.name);
                mObject.GatherObject(FindObjectOfType<PlayerController>());
            }
        }
    }

    void simpleRayCastDamageToEnemy()
    {
        if (GetRayCastHit().collider != null)
        {
            GameObject hitObject = GetRayCastHit().transform.gameObject;
            EnemyHealth enemy = GetRayCastHit().transform.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                StartCoroutine(enemy.gameObject.GetComponent<EnemyBehaviour>().TakeDamageRoutine());
            }
                
        }
    }
    
    Camera FindCamera()
    {
        var result =(GameObject.Find("First_Person_Camera").GetComponent<Camera>() != null) ? GameObject.Find("First_Person_Camera").GetComponent<Camera>() : null;
        return result;
    }
    // ReSharper disable Unity.PerformanceAnalysis
}
