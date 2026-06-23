using _Project.Scripts.Core;
using _Project.Scripts.ScriptableObjects;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using System;
using _Project.Scripts.Runtime.Environment;
using _Project.Scripts.Runtime.RuntimeData;
using _Project.Scripts.Runtime.Systems;

namespace _Project.Scripts.Runtime.Characters
{
    public enum TeamType
    {
        Ally,
        Enemy
    }
    
    public class UnitController : MonoBehaviour
    {
        [SerializeField] private UnitsData _myUnitData;
        [SerializeField] private TeamType _team;
        
        public UnitsData MyUnitData => _myUnitData;
        public event Action<int, float> OnSkillCooldownUpdatedForUI;
        public event Action<int, int> OnHealthChanged;
        public event Action<UnitController> OnUnitDied;

        private SkillRuntimeData[] _skills;
        private SkillSystem _skillSystem;
        
        private HealthRuntimeData _healthData;
        private HealthSystem _healthSystem;
        
        private UnitSlot _currentSlot;
        private UnitStatsSystem _statsSystem;

        private GameEventBus _eventBus;
        
        public int CurrentHealth => _healthData.currentHealth;
        public int MaxHealth => _healthData.maxHealth;
        public UnitSlot CurrentSlot => _currentSlot;
        public TeamType Team => _team;

        [Inject]
        public void Construct(GameEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        private void Awake()
        {
            InitializeFields();
        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                Debug.Log($"{name} HP = {CurrentHealth}/{MaxHealth}");
            }
        }

        public void ReduceCooldown(int index)
        {
            _skillSystem?.ReduceCooldown(index, _myUnitData.clickCooldownReductionPercent / 100f);
        }
        
        public SkillRuntimeData GetSkillRuntime(int index)
        {
            if (_skills == null || index < 0 || index >= _skills.Length)
                return null;
            return _skills[index];
        }

        public void TakeDamage(int damage) => _healthSystem.TakeDamage(damage);
        
        public void Heal(int amount) => _healthSystem.Heal(amount);
        
        public void BindToSlot(UnitSlot slot) => _currentSlot = slot;
        
        public void Unbind() => _currentSlot = null;
        
        public void ApplyUpgrade(UpgradeData data)
        {
            _statsSystem.ApplyUpgrade(data);
            Debug.Log($"[{name}] Applied {data.upgradeName}");
        }
        
        public void ResetSession() => _statsSystem.ResetSession();

        private void InitializeFields()
        {
            _statsSystem = new UnitStatsSystem(_myUnitData, new UnitStatsData());
            InitializeHealth();
            InitializeSkills();
        }

        private void Subscribe()
        {
            if (_skillSystem == null) return;
            _skillSystem.OnCooldownUpdated -= OnCooldownProxy;
            _skillSystem.OnCooldownUpdated += OnCooldownProxy;
            
            if(_healthSystem == null) return;
            _healthSystem.OnHealthChanged -= OnHealthProxy;
            _healthSystem.OnHealthChanged += OnHealthProxy;
            _healthSystem.OnDeath -= OnDeathProxy;
            _healthSystem.OnDeath += OnDeathProxy;

            _eventBus.OnBattlePause -= OnPaused;
            _eventBus.OnBattlePause += OnPaused;
            _eventBus.OnBattleResume -= OnResumed;
            _eventBus.OnBattleResume += OnResumed;
        }

        private void Unsubscribe()
        {
            if (_skillSystem == null) return;
            _skillSystem.OnCooldownUpdated -= OnCooldownProxy;
            
            if(_healthSystem == null) return;
            _healthSystem.OnHealthChanged -= OnHealthProxy;
            _healthSystem.OnDeath -= OnDeathProxy;
            
            _eventBus.OnBattlePause -= OnPaused;
            _eventBus.OnBattleResume -= OnResumed;
        }
        
        private void OnCooldownProxy(int index, float fill)
        {
            OnSkillCooldownUpdatedForUI?.Invoke(index, fill);
        }

        private void OnHealthProxy(int current, int max)
        {
            OnHealthChanged?.Invoke(current, max);
        }

        private void OnDeathProxy()
        {
            Debug.Log($"{name} died", this);
            OnUnitDied?.Invoke(this);
            CurrentSlot.ClearUnit();
            gameObject.SetActive(false);
        }

        private void InitializeHealth()
        {
            _healthData = new HealthRuntimeData();
            _healthData.maxHealth = _statsSystem.GetMaxHealth();
            _healthData.currentHealth = _healthData.maxHealth;
            _healthData.armor = _statsSystem.GetArmor();
            _healthSystem = new HealthSystem(_healthData);
            
            OnHealthChanged?.Invoke(_healthData.currentHealth, _healthData.maxHealth);
        }

        private void InitializeSkills()
        {
            _skills = new SkillRuntimeData[_myUnitData.skillData.Length];

            for (int i = 0; i < _myUnitData.skillData.Length; i++)
            {
                var data = _myUnitData.skillData[i];
                _skills[i] = new SkillRuntimeData
                {
                    skillIndex = i,
                    skillName = data.skillName,
                    value = _statsSystem.GetSkillValue(i),
                    rawCooldown = _statsSystem.GetSkillCooldown(i),
                    cooldownRemaining = Mathf.Max(data.cooldown, 0f),
                    skillType = data.skillType
                };
            }
            _skillSystem = new SkillSystem(_skills, _eventBus, _team, this.GetCancellationTokenOnDestroy());
            _skillSystem.SetPause(_eventBus.IsBattlePause);
            _skillSystem.StartSkillLoops();
        }

        private void OnPaused() => _skillSystem?.SetPause(true);
        private void OnResumed() => _skillSystem?.SetPause(false);
    }
}