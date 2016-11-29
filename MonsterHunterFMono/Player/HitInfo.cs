using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonsterHunterFMono
{
    public class HitInfo
    {

        // How much damage the attack will do
        //
        private int damage = 0;

        // How many approximate frames of hit stun the move puts you in. This is different from untech time in the air i think
        //
        private int hitstun = 0;

        // How many approximate frames of blockstun the move puts you in
        //
        private int blockstun = 0;
        
        // By default, the hitzones of any animation is NONE because it might not be an attack
        //
        private Hitzone hitzone = Hitzone.NONE;

        // Air untech time is diff then ground hitstun. This is how long before they untech
        //
        private int airUntechTime = 0;

        private bool hardKnockDown = false;

        private bool freezeOpponent = false;

        private bool unblockable = false;

        private bool forceAirborne = false;

        private int groundXMovement = 0;
        private int groundYMovement = 0;

        private float airXVelocity = 0;
        private float airYVelocity = 0;
        private HitType hitType;
        public HitInfo(int hitstun, int blockstun, Hitzone hitzone)
        {
            this.hitstun = hitstun;
            this.blockstun = blockstun;
            this.hitzone = hitzone;
        }

        public HitType HitType
        {
            get { return hitType; }
            set { hitType = value; }
        }

        public int Damage
        {
            get { return damage; }
            set { damage = value; }
        }

        public int Hitstun
        {
            get { return hitstun; }
            set { hitstun = value; }
        }

        public int Blockstun
        {
            get { return blockstun; }
            set { blockstun = value; }
        }

        public int AirUntechTime
        {
            get { return airUntechTime; }
            set { airUntechTime = value; }
        }

        public Hitzone Hitzone
        {
            get { return hitzone; }
            set { hitzone = value; }
        }

        public bool IsHardKnockDown
        {
            get { return hardKnockDown; }
            set { hardKnockDown = value; } 
        }

        public int GroundXMovement
        {
            get { return groundXMovement; }
            set { groundXMovement = value; }
        }

        public int GroundYMovement
        {
            get { return groundYMovement; }
            set { groundYMovement = value; }
        }

        public float AirXVelocity
        {
            get { return airXVelocity; }
            set { airXVelocity = value; }
        }

        public float AirYVelocity
        {
            get { return airYVelocity; }
            set { airYVelocity = value; }
        }

        public bool FreezeOpponent
        {
            get { return freezeOpponent; }
            set { freezeOpponent = value; }
        }

        public bool Unblockable
        {
            get { return unblockable; }
            set { unblockable = value; }
        }
       
        public bool ForceAirborne
        {
            get { return forceAirborne; }
            set { forceAirborne = value; }
        }

    }
}
