using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonsterHunterFMono
{
    class BasicProrationStrategy : ProrationStrategy
    {

        private double DamageProrationValue { get; set; }
        private double HitStunProrationValue { get; set; }

        private int comboLength { get; set; }

        // Restart the combo
        //
        public void startCombo()
        {
            DamageProrationValue = 1;
            HitStunProrationValue = 1;

        }

        public int calculateProratedDamage(HitInfo hitInfo)
        {            
            return (int)(hitInfo.Damage * DamageProrationValue);
        }

        public int calculateProratedHitStun(HitInfo hitInfo)
        {
            return (int)(hitInfo.Hitstun * HitStunProrationValue);

        }

        // Add a hit to the combo. For this basic/naive strategy all we care about is combo length
        //
        public void registerHit(HitInfo hitInfo)
        {
            comboLength += 1;
            // Combo cannot go below 20% damage
            //
            if (DamageProrationValue > .3)
            {
                DamageProrationValue -= .1;
            }
            
        }
    }
}
