using System;
using System.Collections.Generic;
using GameDevTV.Utils;
using RPG.Attributes;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace RPG.Stats
{
  public class BaseStats : MonoBehaviour
  {
    [SerializeField] [Range(1,99)] private int startingLevel = 1;
    [SerializeField] private CharacterClass characterClass;
    [SerializeField] private Progression progression = null;
    [SerializeField] private GameObject levelUpEffectPrefab = null;
    
    public event Action OnLevelUp;
    Experience _experience;
    
    LazyValue<int> _currentLevel;
    

    private void Awake()
    {
      _experience = GetComponent<Experience>();
      _currentLevel = new LazyValue<int>(CalculateLevel);
    }

    private void Start()
    {
      _currentLevel.ForceInit();
    }
    

    private void OnEnable()
    {
      if (_experience != null)
      {
        _experience.OnExperienceGained += UpdateLevel;
      }
    }

    private void OnDisable()
    {
      if (_experience != null)
      {
        _experience.OnExperienceGained -= UpdateLevel;
      }
    }

    private void UpdateLevel()
    {
     int newLevel = CalculateLevel();
     if (newLevel > _currentLevel.value)
     {
        _currentLevel.value = newLevel;
        print("LevelUp");
        LevelUpEffect();
        OnLevelUp?.Invoke();

     }
    }

  

    private void LevelUpEffect()
    {
      Instantiate(levelUpEffectPrefab, transform);
    }

    public float GetStat(Stat stat)
    {
      return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * ( 1 + GetPercentageModifier(stat) / 100f);
    }

    private float GetBaseStat(Stat stat)
    {
      return progression.GetStat(stat, characterClass, GetLevel());
    }

    public int GetLevel()
    {
      return _currentLevel.value;
    }

    private float GetAdditiveModifier(Stat stat)
    {
      var components = GetComponents<IModifierProvider>();
      var totalModifier = 0f;
      foreach (var provider in components)
      {
        foreach (var modifier in provider.GetAdditiveModifiers(stat))
        {
          totalModifier += modifier;
        }
      }
      return totalModifier;
    }
    
    private float GetPercentageModifier(Stat stat)
    {
      var components = GetComponents<IModifierProvider>();
      var totalModifier = 0f;
      foreach (var provider in components)
      {
        foreach (var modifier in provider.GetPercentageModifiers(stat))
        {
          totalModifier += modifier;
        }
      }
      return totalModifier;
    }
    
    public int CalculateLevel()
    {
      
      if(_experience == null) return startingLevel;

      float currentXp = _experience.GetExperience();
      int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);

      for (int level = 1; level <= penultimateLevel; level++)
      {
        float xpToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
        if (xpToLevelUp > currentXp)
        {
          return level;
        }
      }
      return penultimateLevel + 1;
    }
  }
}
