#region

using System;
using UnityEngine;

#endregion

namespace LlockhamIndustries.Decals
{
    /**
    * The base of all deferred only projections (Gloss, Normal)
    */
    [Serializable]
    public abstract class Deferred : Projection
    {
        //Deferred only
        public override bool DeferredOnly
        {
            get { return true; }
        }

        //Unsupported materials
        public override Material[] Forward
        {
            get { return null; }
        }
    }
}