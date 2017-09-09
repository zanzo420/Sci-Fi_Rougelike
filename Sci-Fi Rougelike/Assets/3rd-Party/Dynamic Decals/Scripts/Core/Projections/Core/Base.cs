#region

using System;
using UnityEngine;
#if UNITY_EDITOR

#endif

#endregion

namespace LlockhamIndustries.Decals
{
    /**
    * The base of all unlit forward projections (Unlit, Additive, Multiplicative)
    */
    [Serializable]
    public abstract class Base : Projection
    {
        //Static Properties
        /**
        * The primary color details of your projection.
        * The alpha channel of these properties is used to determine the projections transparency.
        */
        public AlbedoPropertyGroup albedo;

        //Materials
        protected Material[] forwardMaterials;

        //Instanced count
        public override int InstanceLimit
        {
            get { return 500; }
        }

        //Forward only
        public override bool ForwardOnly
        {
            get { return true; }
        }

        //Unsupported materials
        public override Material[] DeferredOpaque
        {
            get { return null; }
        }

        public override Material[] DeferredTransparent
        {
            get { return null; }
        }

        protected override void UpdateMaterials()
        {
            UpdateMaterialArray(forwardMaterials);
        }

        protected override void Apply(Material Material)
        {
            //Apply base
            base.Apply(Material);

            //Apply metallic
            albedo.Apply(Material);
        }

        protected override void DestroyMaterials()
        {
            if (forwardMaterials != null)
                DestroyMaterialArray(forwardMaterials);
        }

        protected override void OnEnable()
        {
            //Initialize our property groups
            if (albedo == null) albedo = new AlbedoPropertyGroup(this);

            base.OnEnable();
        }

        protected override void GenerateIDs()
        {
            base.GenerateIDs();

            albedo.GenerateIDs();
        }

        //Instanced Properties
        public override void UpdateProperties()
        {
            //Initialize property array
            if (properties == null || properties.Length != 1) properties = new ProjectionProperty[1];

            //Albedo Color
            properties[0] = new ProjectionProperty("Albedo", albedo._Color, albedo.Color);
        }
    }
}
