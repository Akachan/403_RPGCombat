using System;
using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0 )]
    public class Progression : ScriptableObject
    {
        [SerializeField] private ProgressionCharacterClass[] progressionCharacterClasses = null;

        private Dictionary<CharacterClass, Dictionary<Stat, float[]>> _lookupTable = null;
        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            BuildLookup();

            float[] levels = _lookupTable[characterClass][stat];
            if (level > levels.Length) return 0;
     
            return levels[level-1];
        }
        
        public int GetLevels (Stat stat, CharacterClass characterClass)
        {
            BuildLookup();
            return _lookupTable[characterClass][stat].Length;
        }

        private void BuildLookup()
        {
            if (_lookupTable != null) return;
            _lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();
            
            foreach (var characterClass in progressionCharacterClasses)
            {
                var stats = new Dictionary<Stat, float[]>();
                
                foreach (var stat in characterClass.stats)
                {
                    stats[stat.stat] = stat.levels;
                }
                
                _lookupTable[characterClass.characterClass] = stats;
            }
        }

        [Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass characterClass;
            public ProgressionStat[] stats;
        }

        [Serializable]
        class ProgressionStat
        {
            public Stat stat;
            public float[] levels;

        }
        
    }
}