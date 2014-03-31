﻿using System;
using System.Collections.Generic;
using Server.Engines.PartySystem;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Engines.ConPVP;

namespace Server.Spells.Bard
{
    public class TribulationChant : BardChant
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Tribulation", "In Jux Hur Rel",
            -1,
            9002);
        public TribulationChant(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
            
        }

        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds(1.75);
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 90.0;
            }
        }
        public override int RequiredMana
        {
            get
            {
                return 24;
            }
        }

        public override int UpkeepCost
        {
            get { return 8; }
        }

        public override bool BlocksMovement
        {
            get
            {
                return false;
            }
        }
        public override bool DelayedDamage
        {
            get
            {
                return false;
            }
        }
        public override void OnCast()
        {
            BaseInstrument.PickInstrument(Caster, OnPickedInstrument);
            this.FinishSequence();
        }

        public static void OnPickedInstrument(Mobile from, BaseInstrument instrument)
        {
            from.RevealingAction();
            from.Target = new TribulationTarget(from, instrument, new TribulationChant(from, null).UpkeepCost);
            from.NextSkillTime = Core.TickCount + 21600000;
        }

    }

    public class TribulationTarget : Target
    {
        private readonly BaseInstrument m_Instrument;
        private readonly bool m_SetSkillTime = true;
        private readonly int m_Upkeep;
        public TribulationTarget(Mobile from, BaseInstrument instrument, int upkeep)
            : base(BaseInstrument.GetBardRange(from, SkillName.Discordance), false, TargetFlags.None)
        {
            m_Instrument = instrument;
            m_Upkeep = upkeep;
        }

        protected override void OnTargetFinish(Mobile @from)
        {
			if (m_SetSkillTime)
				from.NextSkillTime = Core.TickCount;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            from.RevealingAction();

            if (!(targeted is Mobile))
            {
                //from.SendLocalizedMessage(1049528); // You cannot calm that!
            }
            else if (from.Region.IsPartOf(typeof (SafeZone)))
            {
                from.SendMessage("You may not peacemake in this area.");
            }
            else if (((Mobile) targeted).Region.IsPartOf(typeof (SafeZone)))
            {
                from.SendMessage("You may not peacemake there.");
            }
            else if (!m_Instrument.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1062488);
                // The instrument you are trying to play is no longer in your backpack!
            }
            else
            {
                TribulationUpkeepTimer song = new TribulationUpkeepTimer(m_Upkeep, from, null, (Mobile)targeted);

            }
        }
    }

    public class TribulationUpkeepTimer : BardTimer
    {
        private readonly Mobile m_Target;

        public TribulationUpkeepTimer(int upkeep, Mobile caster, BuffInfo buffInfo, Mobile target)
            : base(caster, false, BardEffect.Tribulation, 11)
        {
            m_Target = target;
        }

        protected override void OnTick()
        {
            if (++m_CurrentRound >= m_TotalRounds)
                EndEffect(m_Target);
            else
                Effect(m_Target);

        }

        protected override void Effect(Mobile target)
        {
            //Sanity Check
            if (!(target is Mobile)) return;

            m_Caster.SendMessage("Tribulation is not yet activated");


        }

        protected override void EndEffect(Mobile m)
        {
            if (!(m is Mobile)) return;
        }
    }
}