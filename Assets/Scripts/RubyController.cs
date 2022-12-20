using System.Security.Cryptography;
using UnityEngine;

public class RubyController : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    public float speed = 5.0f;
    private float _vertical;
    private float _horizontal;
    // obj property
    public int maxHealth = 10;
    public bool invincible;
    public float maxInvincibleTime = 0.01f;
    private float _invincibleTimer;
    private bool _launching = false;
    private Vector2 _lookDirection = new Vector2(0, 0);
    public float maxLaunchInterval = 1;
    private float _launchIntervalTimer = 0.0f;
    public GameObject projectilePrefab;
    private static readonly int Launch1 = Animator.StringToHash("Launch");
    private static readonly int Property = Animator.StringToHash("Look X");
    private static readonly int Property1 = Animator.StringToHash("Look Y");
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Hit = Animator.StringToHash("Hit");
    public int Health { get; private set; } // 只指定get方法 set 方法设置为私有 此时属性只可读
    
    // 音频源
    private AudioSource _audioSource;
    public AudioClip launchSound;
    public AudioClip hurtSound;
    public AudioClip runSound;
    public AudioClip strawBerrySound;
    public AudioClip deathSound;
    private void Start()
    {
         _rigidbody2D = GetComponent<Rigidbody2D>();  // 使用刚体的位置作为移动物体的基准 防止抖动
         _animator = GetComponent<Animator>();
         _audioSource = GetComponent<AudioSource>();
        // QualitySettings.vSyncCount = 0; 
        // Application.targetFrameRate = 10;  // 每秒update 跑10次  10帧
        
        // property
        Health = maxHealth;
        invincible = false;
    }

    // Update is called once per frame
    // 每帧执行一次
    // 画面每渲染一次，就是一帧，每帧的时间是不固定的， 帧是游戏运行的最小单位
    // 需要实时监控的代码写在此处
    private void Update()
    {
        // 无敌的处理
        if (invincible)
        {
            _invincibleTimer -= Time.deltaTime; //deltaTime 为上一帧到下一帧的间隔  故时间的变动写在Update 内
            // Debug.Log($"{_invincibleTimer}");
        }

        if (_invincibleTimer < 0)
        {
            invincible = false;
        }
        
        // 发射飞弹的处理
        if (_launchIntervalTimer > 0)
        {
            _launchIntervalTimer -= Time.deltaTime;
        }
        else if(Input.GetKeyDown(KeyCode.C) || Input.GetAxis("Fire1") != 0)
        {
            // 使用x激活射线投射  用于对话系统
            RaycastHit2D hit = Physics2D.Raycast(_rigidbody2D.position + Vector2.up * 0.2f,  // 射线发出的位置
                _lookDirection,   // 投射方向
                1.5f,   // 射线距离
                LayerMask.GetMask("NPC"));  // 生效的层
            if (hit.collider is not null)
            {
                NPC npc = hit.collider.GetComponent<NPC>();
                if (npc != null)
                {
                    npc.DisplayDialog();
                }
                // Debug.Log($"射线碰撞到了 {hit.collider.gameObject} ");
            }
            else
            {
                // 发射飞弹 间隔1s
                Debug.Log("Ruby 正在发射飞弹！");
                Launch();
                _launchIntervalTimer = maxLaunchInterval;
            }
        }
    }

    // 固定更新方法，和物理相关的操作代码，都要写在此方法中。
    // 固定更新的时间是0.02s，1秒执行50次，可在Edit--->Project Settings--->Time面板中的Fixed Timestep 查看。
    private void FixedUpdate()
    {
        // 获取输入的方向x,y
        _horizontal = Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Vertical");
        // if (_horizontal != 0 || _vertical != 0 && !_audioSource.isPlaying)  // todo 添加脚步声
        // {
            // _audioSource.PlayOneShot(runSound);
        // }
        // Debug.Log($"{_horizontal} {_vertical}");
        // 根据移动的方向发送数据给 animator 动画先出再移动
        var move = new Vector2(_horizontal, _vertical);  // 向量存储本次要移动的数据
        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f)) // 如移动的值x or y不为0 设置新的值
        {
            _lookDirection.Set(move.x, move.y);
            // Debug.Log($"UNormalize _lookDirection {_lookDirection.x} {_lookDirection.y}");
            _lookDirection.Normalize();  // 使长度为1
            // Debug.Log($"Normalized _lookDirection {_lookDirection.x} {_lookDirection.y}");
        }
        // 为0则应用之前的值
        _animator.SetFloat(Property, _lookDirection.x);
        _animator.SetFloat(Property1, _lookDirection.y);
        _animator.SetFloat(Speed, move.magnitude);


        // 获取对象位置 向量 进行移动
        var objPosition = _rigidbody2D.position;
        // objPosition.x -= 0.1f;
        objPosition.x += speed * _horizontal * Time.deltaTime; // Time.deltaTime 为一帧的运行时间  
        objPosition.y += speed * _vertical * Time.deltaTime;
        // 更新对象位置到新的位置
        // transform.position = objPosition;
        // _rigidbody2D.position = objPosition;
        _rigidbody2D.MovePosition(objPosition);
        
        // 死亡
        if (Health == 0)
        {
            _audioSource.PlayOneShot(deathSound);
            AudioSource.PlayClipAtPoint(deathSound,objPosition,1f);
            Destroy(gameObject);
        }
    }
    
    // 改变生命值
    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            _animator.SetTrigger(Hit);
            if (invincible) // 处在无敌状态且受到伤害则返回
                return;
            invincible = true;
            _invincibleTimer = maxInvincibleTime;
            _audioSource.PlayOneShot(hurtSound);
        }
        else
        {
            _audioSource.PlayOneShot(strawBerrySound);
        }
        Health = Mathf.Clamp(Health + amount, 0, maxHealth);  // 限定函数 （结果值， 最小值， 最大值） 小于最小值则取最小值  
        UiHealthBar.Instance.SetValue(Health/(float)maxHealth);
        // Debug.Log($"Health: {Health}/{maxHealth}");
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void Launch()
    {
        PlaySound(launchSound, 1);
        // Quaternion.identity 表示“无旋转”
        var projectileObject = Instantiate(projectilePrefab, 
            _rigidbody2D.position + Vector2.up * 0.5f,
            Quaternion.identity);  // 初始化对象
        var projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(director: _lookDirection, force: 500);
        _animator.SetTrigger(Launch1);
    }

    public void PlaySound(AudioClip audioClip, float volume)
    {
        // 播放指定音频
        _audioSource.PlayOneShot(audioClip, volume);
    } 
}