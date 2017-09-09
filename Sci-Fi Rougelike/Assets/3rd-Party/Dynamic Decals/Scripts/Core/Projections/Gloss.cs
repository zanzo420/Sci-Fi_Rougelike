#region

using System;
using UnityEngine;

#endregion

namespace LlockhamIndustries.Decals
{
    /**
    * Deferred Only gloss projection. Only affects the gloss channel of the deferred buffers. Useful for making things wetter or rougher.
    */
    [Serializable]
    public class Gloss : Deferred
    {
        //Materials
        protected Material[] deferredMaterials;

        /**
        * The primary color details of your projection.
        * The alpha channel of these properties is used to determine the projections transparency.
        */
        public GlossPropertyGroup gloss;

        //Static Properties
        [SerializeField]
        public GlossType glossType;

        /**
        * Defines how the gloss modifcation affects the surface.
        * Shine will have the decal shine the surface it's applied too. Great for making surfaces appear wet.
        * Dull will have the decal dull the surface it's applied too. Great for making surfaces appear worn or weathered.
        */
        public GlossType GlossType
        {
            get { return glossType; }
            set
            {
                if (glossType != value)
                {
                    glossType = value;
                    Mark();
                }
            }
        }

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
                //Initialize if required
                if (deferredMaterials == null || deferredMaterials.Length != 1)
                    deferredMaterials = new Material[1];

                //Find relevant shader
                Shader shader = null;
                switch (glossType)
                {
                    case GlossType.Shine:
                        shader = Shader.Find("Projection/Decal/Wet");
                        break;
                    case GlossType.Dull:
                        shader = Shader.Find("Projection/Decal/Dry");
                        break;
                }

                if (deferredMaterials[0] != null && deferredMaterials[0].shader != shader)
                    if (Application.isPlaying) Destroy(deferredMaterials[0]);
                    else DestroyImmediate(deferredMaterials[0], true);

                if (deferredMaterials[0] == null)
                {
                    //Create material
                    deferredMaterials[0] = new Material(shader);
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
            gloss.Apply(Material);
        }

        protected override void DestroyMaterials()
        {
            if (deferredMaterials != null)
                DestroyMaterialArray(deferredMaterials);
        }

        protected override void OnEnable()
        {
            //Initialize our property groups
            if (gloss == null) gloss = new GlossPropertyGroup(this);

            base.OnEnable();
        }

        protected override void GenerateIDs()
        {
            base.GenerateIDs();

            gloss.GenerateIDs();
        }

        //Instanced Properties
        public override void UpdateProperties()
        {
            //Initialize property array
            if (properties == null || properties.Length != 1) properties = new ProjectionProperty[1];

            //Normal Strength
            properties[0] = new ProjectionProperty("Glossiness", gloss._Glossiness, gloss.Glossiness);
        }
    }

    public enum GlossType { Shine, Dull }
}
