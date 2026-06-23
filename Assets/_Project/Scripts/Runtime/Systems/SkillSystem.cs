using System;
using System.Threading;
using _Project.Scripts.Core;
using _Project.Scripts.Runtime.Characters;
using _Project.Scripts.Runtime.RuntimeData;
using _Project.Scripts.ScriptableObjects;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Runtime.Systems
{
    public class SkillSystem
    {
        private SkillRuntimeData[]  _skills;
        private CancellationToken _token;
        private TeamType _team;
        private GameEventBus _eventBus;
        private bool _pause;

        public event Action<int, float> OnCooldownUpdated;

        public SkillSystem(SkillRuntimeData[] skills, GameEventBus eventBus, TeamType team, CancellationToken token, string ownerName = "Unknown")
        {
            _skills = skills;
            _eventBus = eventBus;
            _team = team;
            _token = token;
        }

        public void StartSkillLoops()
        {
            for (int i = 0; i < _skills.Length; i++)
            {
                RunSkillLoop(_skills[i]).Forget();
            }
        }

        public void ReduceCooldown(int skillIndex, float percent)
        {
            if (_pause) return;
            if (skillIndex < 0 || skillIndex >= _skills.Length)
                return;
            
            var skill = _skills[skillIndex];
            if (skill.cooldownRemaining <= 0f)
                return;
            
            float reduction = skill.cooldownRemaining * percent;
            reduction = Mathf.Clamp(reduction, 0.5f, 2f);
            
            skill.cooldownRemaining = Mathf.Max(skill.cooldownRemaining - reduction, 0f);
        }
        
        public void SetPause(bool pause) => _pause = pause;

        private async UniTaskVoid RunSkillLoop(SkillRuntimeData skill)
        {
            while (!_token.IsCancellationRequested)
            {
                skill.cooldownRemaining = skill.rawCooldown;
                while (skill.cooldownRemaining > 0f && !_token.IsCancellationRequested)
                {
                    if (!_pause)
                    {
                        skill.cooldownRemaining -= Time.deltaTime;
                        skill.cooldownRemaining = Mathf.Max(skill.cooldownRemaining, 0f);

                        float fill = 1f - (skill.cooldownRemaining / skill.rawCooldown);
                        OnCooldownUpdated?.Invoke(skill.skillIndex, fill);
                    }
                    await UniTask.Yield(PlayerLoopTiming.Update, _token);
                }

                if (!_token.IsCancellationRequested) 
                    ApplySkill(skill);
                
                OnCooldownUpdated?.Invoke(skill.skillIndex, 1f);
                await UniTask.Yield(_token);
            }
        }

        private void ApplySkill(SkillRuntimeData skill)
        {
            if (_team == TeamType.Enemy)
                _eventBus?.EnemyAttacked(skill.value);
            else
            {
                if (skill.skillType == SkillType.Heal)
                    _eventBus?.PlayerHeal(skill.value);
                else
                    _eventBus?.PlayerAttacked(skill.value);
            }
        }
    }
}