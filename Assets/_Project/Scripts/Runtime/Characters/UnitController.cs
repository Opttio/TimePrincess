using _Project.Scripts.Core;
using _Project.Scripts.ScriptableObjects;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using System;
using System.Threading;

namespace _Project.Scripts.Runtime.Characters
{
    [System.Serializable]
    public class SkillRuntimeData
    {
        public string skillName;
        public int damage;
        public float rawCooldown;
        public float cooldownRemaining;
    }
    
    public class UnitController : MonoBehaviour
    {
        [SerializeField] private UnitsData _myUnitData;
        [SerializeField] private bool _isEnemy;
        
        public UnitsData MyUnitData => _myUnitData;

        private int _maxHealth;
        private int _currentHealth;
        private float _armor;
        private SkillRuntimeData[] _skills;
        private Action[] _reduceSkillLambdas;
        private CancellationTokenSource _cts;

        private GameEventBus _eventBus;

        [Inject]
        public void Construct(GameEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        private void Start()
        {
            InitializeFields();
            if (!_isEnemy)
            {
                _cts = new CancellationTokenSource();
                for (int i = 0; i < _skills.Length; i++)
                {
                    var skill = _skills[i];
                    RunSkillLoop(skill, _cts.Token).Forget();
                }
            }
        }
        private void OnEnable()
        {
            if (_isEnemy)
                _eventBus.OnPlayerAttack += TakeDamage;
            else
                _eventBus.OnEnemyAttack += TakeDamage;

            // Підписка на універсальну подію
            _eventBus.OnReduceSkillCooldown += HandleReduceSkillCooldown;
        }

        private void OnDisable()
        {
            if (_isEnemy)
                _eventBus.OnPlayerAttack -= TakeDamage;
            else
                _eventBus.OnEnemyAttack -= TakeDamage;

            _eventBus.OnReduceSkillCooldown -= HandleReduceSkillCooldown;
            
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }
        
        private void InitializeFields()
        {
            _maxHealth = _myUnitData.maxHealth;
            _currentHealth = _maxHealth;
            _armor = _myUnitData.armor;

            _skills = new SkillRuntimeData[_myUnitData.skillData.Length];

            for (int i = 0; i < _myUnitData.skillData.Length; i++)
            {
                var data = _myUnitData.skillData[i];
                _skills[i] = new SkillRuntimeData
                {
                    skillName = data.skillName,
                    damage = Mathf.Max(data.damage, 0),
                    rawCooldown = Mathf.Max(data.cooldown, 0f),
                    cooldownRemaining = Mathf.Max(data.cooldown, 0f)
                };
            }
        }
        
        private void TakeDamage(int damage)
        {
            if (_currentHealth <= 0)
                return;

            int finalDamage = ApplyArmor(damage, _armor);
            _currentHealth -= finalDamage;

            Debug.Log($"{_myUnitData.unitName} took {finalDamage} dmg (raw: {damage}, armor: {_armor}) -> HP = {_currentHealth}");

            if (_currentHealth <= 0)
            {
                Debug.Log($"{_myUnitData.unitName} died");
                // TODO: деактивуємо, відписуємось, ставимо труп і т.д.
            }
            
        }
        
        private int ApplyArmor(int damage, float armor)
        {
            int finalDmg = Mathf.FloorToInt(damage * (100f / (100f + armor)));
            return Mathf.Max(finalDmg, 1); // мінімум 1 dmg
        }
        
        private void HandleReduceSkillCooldown(int skillIndex)
        {
            if (skillIndex < 0 || skillIndex >= _skills.Length)
                return;
            var skill = _skills[skillIndex];
            if (skill.cooldownRemaining <= 0f)
                return;

            float reduction = skill.cooldownRemaining * (_myUnitData.clickCooldownReductionPercent / 100f);
            reduction = Mathf.Clamp(reduction, 0.5f, 2f);
            skill.cooldownRemaining = Mathf.Max(skill.cooldownRemaining - reduction, 0f);

            Debug.Log($"{skill.skillName} cooldown reduced by {reduction:F2} → {skill.cooldownRemaining:F2}");
        }

        private async UniTaskVoid RunSkillLoop(SkillRuntimeData skill, CancellationToken token)
        {
            // UniTaskVoid використовується для "fire-and-forget" з UniTask
            while (!token.IsCancellationRequested)
            {
                // атака
                Debug.Log($"{_myUnitData.unitName} attack for {skill.damage}");
                if (_isEnemy)
                    _eventBus.EnemyAttacked(skill.damage);
                else
                    _eventBus.PlayerAttacked(skill.damage);

                // встановлюємо початковий КД
                skill.cooldownRemaining = skill.rawCooldown;

                // цикл кулдауну
                while (skill.cooldownRemaining > 0f && !token.IsCancellationRequested)
                {
                    skill.cooldownRemaining -= Time.deltaTime;
                    skill.cooldownRemaining = Mathf.Max(skill.cooldownRemaining, 0f);

                    float fill = 1f - (skill.cooldownRemaining / skill.rawCooldown);
                    _eventBus.SkillCooldownUpdated(Array.IndexOf(_skills, skill), fill);

                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }

                // КД закінчився — UI на 1
                _eventBus.SkillCooldownUpdated(Array.IndexOf(_skills, skill), 1f);

                await UniTask.Yield(token);
            }
        }
    }
}