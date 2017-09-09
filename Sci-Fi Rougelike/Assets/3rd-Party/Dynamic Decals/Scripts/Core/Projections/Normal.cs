using UnityEngine;
using System.Collections;
using System;

namespace LlockhamIndustries.Decals
{
    /**
    * Deferred Only normal projection. Only affects the normal buffer. Useful for adding cracks or normal details to tiled surfaces.
    */
    [System.Serializable]
    public class Normal : Deferred
    {
        //Materials
        public override Material[] DeferredOpaque
        {
            get { return DeferredMaterials; }
        }
        public override Material[] DeferredTransparent
        {
            get { return DeferredMaterials; }
        }
        private Material[] DeferredMaterials
        {
            get
            {
                if (deferredMaterials == null || deferredMaterials.Length != 1)
                {
                    deferredMaterials = new Material[1];
                }
                if (deferredMaterials[0] == null)
                {
                    deferredMaterials[0] = new Material(Shader.Find("Projection/Decal/Normal"));
                    deferredMaterials[0].hideFlags = HideFlags.DontSave;
                    Apply(deferredMaterials[0]);
                }
                return deferredMaterials;
            }
        }

        //Instanced count
        public override int InstanceLimit
        {
            get { return 500; }
        }

        protected override void UpdateMaterials()
        {
            UpdateMaterialArray(deferredMaterials);
        }
        protected override void Apply(Material Material)
        {
            //Apply base
            base.Apply(Material);

            //Apply metallic
            normal.Apply(Material);
        }

        protected override void DestroyMaterials()
        {
            if (deferredMaterials != null)
            {
                DestroyMaterialArray(deferredMaterials);
            }
        }

        //Static Properties
        /**
        * The primary color details of your projection.
        * The alpha channel of these properties is used to determine the projections transparency.
        */
        public NormalPropertyGroup normal;

        protected override void OnEnable()
        {
            //Initialize our property groups
            if (normal == null) normal = new NormalPropertyGroup(this);

            base.OnEnable();
        }
        protected override void GenerateIDs()
        {
            base.GenerateIDs();

            normal.GenerateIDs();
        }

        //Instanced Properties
        public override void UpdateProperties()
        {
            //Initialize property array
            if (properties == null || properties.Length != 1) properties = new ProjectionProperty[1];

            //Normal Strength
            properties[0] = new ProjectionProperty("Normal", normal._BumpScale, normal.Strength);
        }

        //Materials
        protected Material[] deferredMaterials;
    }
}
