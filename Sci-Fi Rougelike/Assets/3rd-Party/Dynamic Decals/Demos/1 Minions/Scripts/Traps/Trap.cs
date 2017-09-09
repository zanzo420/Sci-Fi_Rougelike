﻿#region

using System.Collections;
using UnityEngine;

#endregion

namespace LlockhamIndustries.Misc
{
    public abstract class Trap : MonoBehaviour
    {
        [Header("General")]
        public bool autoRearm;

        //Backing fields
        private TrapState state = TrapState.Idle;

        //Access
        public TrapState State
        {
            get { return state; }
        }

        public void Trigger()
        {
            //Stop rearming
            if (state == TrapState.Rearming) StopCoroutine("OnRearm");

            //Start triggering
            if (state != TrapState.Triggering)
            {
                state = TrapState.Triggering;
                StartCoroutine("OnTrigger");
            }
        }

        public void Rearm()
        {
            //Stop triggering
            if (state == TrapState.Triggering) StopCoroutine("OnTrigger");

            //Start rearming
            if (state != TrapState.Rearming)
            {
                state = TrapState.Rearming;
                StartCoroutine("OnRearm");
            }
        }

        //Innards
        protected abstract IEnumerator OnTrigger();

        protected abstract IEnumerator OnRearm();

        protected void TriggerComplete()
        {
            //No longer triggering
            state = TrapState.Idle;

            if (autoRearm) Rearm();
        }

        protected void RearmComplete()
        {
            //No longer rearming
            state = TrapState.Idle;
        }
    }

    public enum TrapState { Idle, Triggering, Rearming }
}