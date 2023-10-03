using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.XR;

static public class MonsterBodySignifier
{
    public const int DayVersion = 0;
    public const int NightVersion = 1;
}
public class EnemyBehaviour : MonoBehaviour
{
    [Header("Monster Build Settings")]
    [SerializeField]
    Vector3 m_basePosition = new Vector3(16, .5f, 0);

    [SerializeField]
    bool _randomizeType = true;
    [SerializeField]
    EnemyType m_enemyType;

    [SerializeField]
    public MonsterMode m_monsterMode;

    [SerializeField]
    GameObject[] _monsterModeBodies;

    [SerializeField]
    List<Transform> m_eyes = new List<Transform>();

    NavMeshAgent m_navMeshAgent;


    [Header("DAY MODE BEHAVIOUR SETTTINGS")]
    [SerializeField]
    [Range(1, 75)]
    float m_playerSensingDistanceDayMode;

    GameObject m_player;
    Vector3 _direction2Player;
 
    [SerializeField]
    Vector3 _dayEyeRotationOffset = Vector3.zero;

    [SerializeField]
    GameObject _giftToPlayerPrefab;

    [SerializeField]
    Transform _giftSpawnPoint;
    bool isGiftGiven = false;

    [Header("NIGHT MODE BEHAVIOUR SETTTINGS")]
    [SerializeField]
    Animator _nightMonsterAnimator;

    AudioSource _audioSource;
 
    [SerializeField]
    [Range(1, 75)]
    float m_playerSensingDistanceNightMode;

    [SerializeField]
    float _wanderingDistance;

    [SerializeField]
    Vector3 _nightEyeRotationOffset = Vector3.zero;

    [SerializeField]
    float _attackDistance = 5;

    public GameObject _basePart;
    public bool _isThereAnyBasePartNear = false;

    public bool _isPlayerHaveATarget = false;
    public bool isArriving = false;

    [Header("Wandering Values")]
    public Vector3 _startPoint;
    public Vector3 _destinationPoint;


    private void Awake()
    {
        //Build Materials according to Type of Monster
        Material[] materials;
        
        if(_randomizeType)
        {
            m_enemyType = (EnemyType)Random.Range(0, 3);
        }
        switch (m_enemyType)
        {
            case EnemyType.BLUE:

                _monsterModeBodies[MonsterBodySignifier.DayVersion].GetComponent<Renderer>().material =
                    Resources.Load<Material>("Materials/JellyMonster/DayVersion/JellyMonster_Blue_DayTime_Body_Mat");

                materials = _monsterModeBodies[MonsterBodySignifier.NightVersion].GetComponent<Renderer>().materials;
                materials[0] = Resources.Load<Material>("Materials/JellyMonster/NightVersion/JellyMonster_Blue_Night_Body_Mat");
                _monsterModeBodies[MonsterBodySignifier.NightVersion].GetComponent<Renderer>().materials = materials;

                break;

            case EnemyType.GREEN:

                _monsterModeBodies[MonsterBodySignifier.DayVersion].GetComponent<Renderer>().material =
                    Resources.Load<Material>("Materials/JellyMonster/DayVersion/JellyMonster_Green_DayTime_Body_Mat");

                materials = _monsterModeBodies[MonsterBodySignifier.NightVersion].GetComponent<Renderer>().materials;
                materials[0] = Resources.Load<Material>("Materials/JellyMonster/NightVersion/JellyMonster_Green_Night_Body_Mat");
                _monsterModeBodies[MonsterBodySignifier.NightVersion].GetComponent<Renderer>().materials = materials;

                break;

            case EnemyType.RED:

                _monsterModeBodies[MonsterBodySignifier.DayVersion].GetComponent<Renderer>().material =
                    Resources.Load<Material>("Materials/JellyMonster/DayVersion/JellyMonster_Red_DayTime_Body_Mat");

                materials = _monsterModeBodies[MonsterBodySignifier.NightVersion].GetComponent<Renderer>().materials;
                materials[0] = Resources.Load<Material>("Materials/JellyMonster/NightVersion/JellyMonster_Red_Night_Body_Mat");
                _monsterModeBodies[MonsterBodySignifier.NightVersion].GetComponent<Renderer>().materials = materials;

                break;

        }

        SetMonsterMode(m_monsterMode);

    }

    // Start is called before the first frame update
    void Start()
    {
        m_player = GameObject.FindWithTag("Player");

        m_eyes.Add(_monsterModeBodies[MonsterBodySignifier.DayVersion].transform.Find("JellyMonsterEye Left"));
        m_eyes.Add(_monsterModeBodies[MonsterBodySignifier.DayVersion].transform.Find("JellyMonsterEye Right"));
        m_eyes.Add(_monsterModeBodies[MonsterBodySignifier.NightVersion].transform.parent.Find("JellyMonsterEye Left"));
        m_eyes.Add(_monsterModeBodies[MonsterBodySignifier.NightVersion].transform.parent.Find("JellyMonsterEye Right"));

        _startPoint = transform.position;

        m_navMeshAgent = GetComponent<NavMeshAgent>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        if (m_monsterMode == MonsterMode.DayMode)
        {
            if(!_monsterModeBodies[MonsterBodySignifier.DayVersion].active)
                SetMonsterMode(MonsterMode.DayMode);
            DayModeEnemyBehaviours();
        }

        else if (m_monsterMode == MonsterMode.NightMode)
        {
            if (_monsterModeBodies[MonsterBodySignifier.DayVersion].active)
                SetMonsterMode(MonsterMode.NightMode);
            NightModeEnemyBehaviours();
        }

    }

    void DayModeEnemyBehaviours()
    {
        if(IsEnemySensingPlayer(MonsterMode.DayMode))
        {
            LookAtPlayer(MonsterMode.DayMode);
            if (Vector3.Distance(transform.position, m_player.transform.position) < 3)
                SpawnPlayerGift();
            if(m_navMeshAgent.speed > 0)
            {
                m_navMeshAgent.speed = 0;
            }
              
        }
    }
    void NightModeEnemyBehaviours()
    {
        if (IsEnemySensingPlayer(MonsterMode.NightMode))
        {
            LookAtPlayer(MonsterMode.NightMode);

            if(Vector3.Distance(transform.position, m_player.transform.position) > _attackDistance )
            {
                Move(m_player.transform.position);
            }
            else
            {
                Attack(m_player.transform.position);
            }
               
        }
        else
        {
            if(!_basePart)
            {
                MarchToBase();
            }
            else
            {
                AttackToBase();
            }
          
            
           // WanderingAround();
        }
    }


    void Move(Vector3 destination)
    {

        /*  Vector3 dir = Vector3.Normalize( transform.position - destination );

          float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
          transform.localEulerAngles = new Vector3(0,angle - 90,0);

          dir = Vector3.Normalize(transform.position - destination);
          dir.y = 0;

          transform.Translate(-dir * Time.deltaTime * 3,Space.World);*/
        if(_nightMonsterAnimator.GetInteger("State" ) != 2)
        {
            _nightMonsterAnimator.SetInteger("State", 2);
            m_navMeshAgent.speed = 3.5f;
        }
        if(destination == m_player.transform.position)
        {
            if (IsValidDestination(destination, 10, out Vector3 validPosition))
            {
                m_navMeshAgent.SetDestination(destination);
            }
        }
    }
    
    bool IsValidDestination(Vector3 position, float radius, out Vector3 validPosition)
    {
        NavMeshHit hit;
        bool isValid = NavMesh.SamplePosition(position, out hit, radius, NavMesh.AllAreas);

        validPosition = hit.position;
        return isValid;
    }

    void Attack(Vector3 target)
    {
        if (_nightMonsterAnimator.GetInteger("State") != 1)
        {
            //Play Attack Animation
            _nightMonsterAnimator.SetInteger("State", 1);
            m_navMeshAgent.speed = 0;
        }
    
        if(!_audioSource.isPlaying)
        {
            _audioSource.clip = Resources.Load<AudioClip>("Audio/SFX/Monster-Attack_SFX");
            _audioSource.Play();
        }
         
        transform.LookAt(target);
    }

    Vector3 GetDirectionToBase()
    {
        return Vector3.Normalize(new Vector3(16,transform.position.y,0) - transform.position);
      
    }

    void LookAtPlayer(MonsterMode mode)
    {
        if(mode == MonsterMode.DayMode)
        {
            m_eyes[0].transform.LookAt(m_player.transform);
            m_eyes[0].transform.localEulerAngles += _dayEyeRotationOffset;

            m_eyes[1].transform.LookAt(m_player.transform);
            m_eyes[1].transform.localEulerAngles += _dayEyeRotationOffset;
        }

        else if (mode == MonsterMode.NightMode)
        {
            m_eyes[2].transform.LookAt(m_player.transform);
            m_eyes[2].transform.localEulerAngles += _nightEyeRotationOffset;

            m_eyes[3].transform.LookAt(m_player.transform);
            m_eyes[3].transform.localEulerAngles += _nightEyeRotationOffset;
        }
   
        
    }

    bool IsEnemySensingPlayer(MonsterMode mode)
    {
        float sensingDistance;

        if (mode == MonsterMode.DayMode)
             sensingDistance = m_playerSensingDistanceDayMode;
        else
            sensingDistance = m_playerSensingDistanceNightMode;

        float currentDistance = Vector3.Distance(transform.position, m_player.transform.position);

        if (currentDistance <= sensingDistance)
            return true;
        else
            return false;
    }

    void MarchToBase()
    {
        if(_nightMonsterAnimator.GetInteger("State") != 0)
        {
            _nightMonsterAnimator.SetInteger("State", 0);
            /* m_navMeshAgent.speed = 1f;
             if (IsValidDestination(m_basePosition, 10, out Vector3 validPosition))
             {
                 m_navMeshAgent.SetDestination(m_basePosition);
             }*/
           
        }
        Vector3 dir = Vector3.Normalize(m_basePosition - transform.position);

        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        transform.localEulerAngles = new Vector3(0, angle , 0);
        transform.Translate(Vector3.Normalize( m_basePosition - transform.position) * Time.deltaTime * -1);



    }
    void AttackToBase()
    {
        m_navMeshAgent.speed = 1f;
        if (_basePart && Vector3.Distance(_basePart.transform.position, transform.position) > _attackDistance)
             Move(_basePart.transform.position);
        else
        {
            Attack(_basePart.transform.position);
        }

    }
    void WanderingAround()
    {
        if (_isPlayerHaveATarget)
        {
            if ( !isArriving)
            {
                Move(_destinationPoint);
                if (Vector3.Distance(transform.position, _destinationPoint) < 1)
                {
                  
                    StartCoroutine(ArrivingToDestinationRoutine());
                }
            }              
                
        }
            
        else
        {
            _destinationPoint = new Vector3(_startPoint.x + Random.Range(-_wanderingDistance, _wanderingDistance),
          _destinationPoint.y,
          _startPoint.z + Random.Range(-_wanderingDistance, _wanderingDistance));

            _isPlayerHaveATarget = true;

            _startPoint = _destinationPoint;
        }
      
    }


    IEnumerator ArrivingToDestinationRoutine()
    {
        isArriving = true;
        yield return new WaitForSeconds(3);
        _isPlayerHaveATarget = false;
        isArriving = false;

    }

    void SpawnPlayerGift()
    {
        if (!isGiftGiven)
        {
           GameObject gift = Instantiate(_giftToPlayerPrefab);
            gift.transform.position = _giftSpawnPoint.position;

        }
            

        isGiftGiven = true;
    }

   // private int i = 0;
    public void SetMonsterMode(MonsterMode mode)
    {
        //i++;
        //Debug.Log("Setting monster mode how many times : " + i);
        m_monsterMode = mode;
        // Show Body type according to the Current mode of monster (Day mode or Night mode)
        switch (m_monsterMode)
        {
            case MonsterMode.DayMode:
                //m_navMeshAgent.destination = transform.position ;
                _monsterModeBodies[MonsterBodySignifier.DayVersion].SetActive(true);
                _monsterModeBodies[MonsterBodySignifier.NightVersion].transform.parent.gameObject.SetActive(false);
                break;
            case MonsterMode.NightMode:
                _monsterModeBodies[MonsterBodySignifier.DayVersion].SetActive(false);
                _monsterModeBodies[MonsterBodySignifier.NightVersion].transform.parent.gameObject.SetActive(true);
                break;
        }
    }

    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Sensing Base Part");
        if(other.CompareTag("BasePart"))
        {
            if(!_basePart)
            {
                _basePart = other.gameObject;
                _isThereAnyBasePartNear = true;
            }
            else
            {
                if(Vector3.Distance(_basePart.transform.position, transform.position) > Vector3.Distance(other.transform.position,transform.position))
                {
                    _basePart = other.gameObject;
                    _isThereAnyBasePartNear = true;
                }
                else
                {
                    return;
                }
            }
        }
    }

    public IEnumerator TakeDamageRoutine()
    {
        Color color;
        if (m_monsterMode == MonsterMode.NightMode)
        {
            SkinnedMeshRenderer  renderer  = _monsterModeBodies[1].GetComponent<SkinnedMeshRenderer>();
             color = renderer.materials[0].color;
            renderer.materials[0].color = Color.white;
        }
        else
        {
            MeshRenderer renderer = _monsterModeBodies[0].GetComponent<MeshRenderer>();
             color = renderer.material.color;
            renderer.material.color = Color.white;
        }
      
        _audioSource.clip = Resources.Load<AudioClip>("Audio/SFX/Monster-Hurt_SFX");
        _audioSource.Play();

        yield return new WaitForSeconds(.3f);

        if (m_monsterMode == MonsterMode.NightMode)
        {
            _monsterModeBodies[1].GetComponent<SkinnedMeshRenderer>().materials[0].color = color;
        }
        else
        {
            _monsterModeBodies[0].GetComponent<MeshRenderer>().material.color =  color;
        }

    }
    public IEnumerator DeathRoutine()
    {
        if(m_monsterMode == MonsterMode.NightMode)
        {
            _nightMonsterAnimator.SetInteger("State", 3);
            yield return new WaitForSeconds(2);
        }
        
        gameObject.SetActive(false);

        //Destroy(gameObject);
    }

}
