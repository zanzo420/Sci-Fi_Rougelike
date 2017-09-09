﻿#region

using System;
using UnityEngine;
#if UNITY_EDITOR

#endif

#endregion

namespace LlockhamIndustries.Decals
{
    [Serializable]
    public class ShapePropertyGroup
    {
        //Property Ids
        public int _MainTex;

        public int _Multiplier;

        [SerializeField]
        private float multiplier = 1;

        //Core
        protected Projection projection;

        //Backing Fields
        [SerializeField]
        private Texture2D texture;

        //Constructor
        public ShapePropertyGroup(Projection Projection)
        {
            projection = Projection;
        }

        //Properties
        public Texture2D Texture
        {
            get { return texture; }
            set
            {
                if (texture != value)
                {
                    texture = value;
                    Mark();
                }
            }
        }

        public float Multiplier
        {
            get { return multiplier; }
            set
            {
                if (multiplier != value)
                {
                    multiplier = value;
                    Mark();
                }
            }
        }

        //Runtime Methods
        protected void Mark()
        {
            projection.Mark();
        }

        public void GenerateIDs()
        {
            //Grab Ids
            _MainTex = Shader.PropertyToID("_MainTex");
            _Multiplier = Shader.PropertyToID("_Multiplier");
        }

        //Apply
        public void Apply(Material Material)
        {
            Material.SetTexture(_MainTex, texture);
            Material.SetFloat(_Multiplier, multiplier);
        }
    }
    [Serializable]
    public class AlbedoPropertyGroup
    {
        public int _Color;

        //Property Ids
        public int _MainTex;

        [SerializeField]
        private Color color = Color.grey;

        //Core
        protected Projection projection;

        //Backing Fields
        [SerializeField]
        private Texture2D texture;

        //Constructor
        public AlbedoPropertyGroup(Projection Projection)
        {
            projection = Projection;
        }

        //Properties
        /**
        * The primary color texture of your projection. Multiplied with the albedo color. 
        * The alpha channel of this texture is used to determine the projections transparency.
        */
        public Texture2D Texture
        {
            get { return texture; }
            set
            {
                if (texture != value)
                {
                    texture = value;
                    Mark();
                }
            }
        }

        /**
        * The primary color of your projection. Multiplied with the albedo map. 
        * The alpha channel is used to determine the projections transparency.
        */
        public Color Color
        {
            get { return color; }
            set
            {
                if (color != value)
                {
                    color = value;
                    Mark();
                }
            }
        }

        //Runtime Methods
        protected void Mark()
        {
            projection.Mark();
        }

        public void GenerateIDs()
        {
            //Grab Ids
            _MainTex = Shader.PropertyToID("_MainTex");
            _Color = Shader.PropertyToID("_Color");
        }

        //Apply
        public void Apply(Material Material)
        {
            Material.SetTexture(_MainTex, texture);
            Material.SetColor(_Color, color);
        }
    }
    [Serializable]
    public class GlossPropertyGroup
    {
        public int _Glossiness;

        //Property Ids
        public int _GlossMap;

        [SerializeField]
        private float glossiness = 1;

        //Core
        protected Projection projection;

        //Backing Fields
        [SerializeField]
        private Texture2D texture;

        //Constructor
        public GlossPropertyGroup(Projection Projection)
        {
            projection = Projection;
        }

        //Properties
        public Texture2D Texture
        {
            get { return texture; }
            set
            {
                if (texture != value)
                {
                    texture = value;
                    Mark();
                }
            }
        }

        public float Glossiness
        {
            get { return glossiness; }
            set
            {
                if (glossiness != value)
                {
                    glossiness = value;
                    Mark();
                }
            }
        }

        //Runtime Methods
        protected void Mark()
        {
            projection.Mark();
        }

        public void GenerateIDs()
        {
            //Grab Ids
            _GlossMap = Shader.PropertyToID("_GlossMap");
            _Glossiness = Shader.PropertyToID("_Glossiness");
        }

        //Apply
        public void Apply(Material Material)
        {
            Material.SetTexture(_GlossMap, texture);
            Material.SetFloat(_Glossiness, glossiness);
        }
    }
    [Serializable]
    public class MetallicPropertyGroup
    {
        public int _Glossiness;
        public int _Metallic;

        //Property Ids
        public int _MetallicGlossMap;

        [SerializeField]
        private float glossiness = 1;

        [SerializeField]
        private float metallicity = 0.5f;

        //Core
        protected Projection projection;

        //Backing Fields
        [SerializeField]
        private Texture2D texture;

        //Constructor
        public MetallicPropertyGroup(Projection Projection)
        {
            projection = Projection;
        }

        //Properties
        public Texture2D Texture
        {
            get { return texture; }
            set
            {
                if (texture != value)
                {
                    texture = value;
                    Mark();
                }
            }
        }

        public float Metallicity
        {
            get { return metallicity; }
            set
            {
                if (metallicity != value)
                {
                    metallicity = value;
                    Mark();
                }
            }
        }

        public float Glossiness
        {
            get { return glossiness; }
            set
            {
                if (glossiness != value)
                {
                    glossiness = value;
                    Mark();
                }
            }
        }

        //Runtime Methods
        protected void Mark()
        {
            projection.Mark();
        }

        public void GenerateIDs()
        {
            //Grab Ids
            _MetallicGlossMap = Shader.PropertyToID("_MetallicGlossMap");
            _Metallic = Shader.PropertyToID("_Metallic");
            _Glossiness = Shader.PropertyToID("_Glossiness");
        }

        //Apply
        public void Apply(Material Material)
        {
            Material.SetTexture(_MetallicGlossMap, texture);
            Material.SetFloat(_Metallic, metallicity);
            Material.SetFloat(_Glossiness, glossiness);
        }
    }
    [Serializable]
    public class SpecularPropertyGroup
    {
        public int _Glossiness;
        public int _SpecColor;

        //Property Ids
        public int _SpecGlossMap;

        [SerializeField]
        private Color color = new Color(0.2f, 0.2f, 0.2f, 1);

        [SerializeField]
        private float glossiness = 1;

        //Core
        protected Projection projection;

        //Backing Fields
        [SerializeField]
        private Texture2D texture;

        //Constructor
        public SpecularPropertyGroup(Projection Projection)
        {
            projection = Projection;
        }

        //Properties
        public Texture2D Texture
        {
            get { return texture; }
            set
            {
                if (texture != value)
                {
                    texture = value;
                    Mark();
                }
            }
        }

        public Color Color
        {
            get { return color; }
            set
            {
                if (color != value)
                {
                    color = value;
                    Mark();
                }
            }
        }

        public float Glossiness
        {
            get { return glossiness; }
            set
            {
                if (glossiness != value)
                {
                    glossiness = value;
                    Mark();
                }
            }
        }

        //Runtime Methods
        protected void Mark()
        {
            projection.Mark();
        }

        public void GenerateIDs()
        {
            //Grab Ids
            _SpecGlossMap = Shader.PropertyToID("_SpecGlossMap");
            _SpecColor = Shader.PropertyToID("_SpecColor");
            _Glossiness = Shader.PropertyToID("_Glossiness");
        }

        //Apply
        public void Apply(Material Material)
        {
            Material.SetTexture(_SpecGlossMap, texture);
            Material.SetColor(_SpecColor, color);
            Material.SetFloat(_Glossiness, glossiness);
        }
    }
    [Serializable]
    public class NormalPropertyGroup
    {
        //Property Ids
        public int _BumpMap;

        public int _BumpScale;

        //Core
        protected Projection projection;

        [SerializeField]
        private float strength = 1;

        //Backing Fields
        [SerializeField]
        private Texture2D texture;

        //Constructor
        public NormalPropertyGroup(Projection Projection)
        {
            projection = Projection;
        }

        //Properties
        public Texture2D Texture
        {
            get { return texture; }
            set
            {
                if (texture != value)
                {
                    texture = value;
                    Mark();
                }
            }
        }

        public float Strength
        {
            get { return strength; }
            set
            {
                if (strength != value)
                {
                    strength = value;
                    Mark();
                }
            }
        }

        //Runtime Methods
        protected void Mark()
        {
            projection.Mark();
        }

        public void GenerateIDs()
        {
            //Grab Ids
            _BumpMap = Shader.PropertyToID("_BumpMap");
            _BumpScale = Shader.PropertyToID("_BumpScale");
        }

        //Apply
        public void Apply(Material Material)
        {
            Material.SetTexture(_BumpMap, texture);
            Material.SetFloat(_BumpScale, strength);
        }
    }
    [Serializable]
    public class EmissivePropertyGroup
    {
        public int _EmissionColor;

        //Property Ids
        public int _EmissionMap;

        [SerializeField]
        private Color color = Color.black;

        [SerializeField]
        private float intensity = 1;

        //Core
        protected Projection projection;

        //Backing Fields
        [SerializeField]
        private Texture2D texture;

        //Constructor
        public EmissivePropertyGroup(Projection Projection)
        {
            projection = Projection;
        }

        //Properties
        public Texture2D Texture
        {
            get { return texture; }
            set
            {
                if (texture != value)
                {
                    texture = value;
                    Mark();
                }
            }
        }

        public float Intensity
        {
            get { return intensity; }
            set
            {
                if (intensity != value)
                {
                    intensity = value;
                    Mark();
                }
            }
        }

        public Color Color
        {
            get { return color; }
            set
            {
                if (color != value)
                {
                    color = value;
                    Mark();
                }
            }
        }

        //Runtime Methods
        protected void Mark()
        {
            projection.Mark();
        }

        public void GenerateIDs()
        {
            //Grab Ids
            _EmissionMap = Shader.PropertyToID("_EmissionMap");
            _EmissionColor = Shader.PropertyToID("_EmissionColor");
        }

        //Apply
        public void Apply(Material Material)
        {
            Material.SetTexture(_EmissionMap, texture);
            Material.SetColor(_EmissionColor, color * intensity);
        }
    }
}