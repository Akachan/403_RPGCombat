﻿using System.Collections;
using System.Collections.Generic;

namespace RPG.Stats
{
    public interface IModifierProvider : IEnumerable
    {
        IEnumerable<float> GetAdditiveModifiers(Stat stat);
        IEnumerable<float> GetPercentageModifiers(Stat stat);
    }
}