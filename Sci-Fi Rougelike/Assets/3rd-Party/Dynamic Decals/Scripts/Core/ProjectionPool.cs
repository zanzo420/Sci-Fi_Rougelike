#region

using System.Collections.Generic;
using LlockhamIndustries.ExtensionMethods;
using UnityEngine;

#endregion

namespace LlockhamIndustries.Decals
{
    /**
    * In-built projection pooling class. Use ProjectionPool.GetPool() to get a reference to a pool instance or use ProjectionPool.Default to get a reference to the default pool.
    * You can the request projections from the pool as you see fit. Once you are done with them, instead of deleting them, use the Return method to return them back to the pool.
    */
    public class ProjectionPool
    {
        //Pool
        internal List<PoolItem> activePool;

        internal List<PoolItem> inactivePool;

        //Pool Details
        private readonly PoolInstance instance;

        private Transform parent;

        //Constructor
        public ProjectionPool(PoolInstance Instance)
        {
            instance = Instance;
        }

        public string Title
        {
            get { return instance.title; }
        }

        private int Limit
        {
            get
            {
                return instance.limits[QualitySettings.GetQualityLevel()];
            }
        }

        public int ID
        {
            get { return instance.id; }
        }

        //Parent
        internal Transform Parent
        {
            get
            {
                if (parent == null)
                {
                    //Generate multiscene gameObject
                    var gameObject = new GameObject(instance.title + " Pool");
                    Object.DontDestroyOnLoad(gameObject);

                    //Cache transform
                    parent = gameObject.transform;
                }
                return parent;
            }
        }

        //Get Pool
        /**
         * Returns a pool with the specified name, if it exists. If it doesn't, returns the default pool.
         * @param Title The title of the pool to be returned.
         */
        public static ProjectionPool GetPool(string Title)
        {
            return DynamicDecals.System.GetPool(Title);
        }

        /**
         * Returns a pool with the specified ID, if it exists. If it doesn't, returns the default pool.
         * @param ID The ID of the pool to be returned.
         */
        public static ProjectionPool GetPool(int ID)
        {
            return DynamicDecals.System.GetPool(ID);
        }

        //Check Intersecting
        /**
        * Checks to see if a point is intersecting with any of the projections in the pool.
        * Returns true if an intersecting projection is found, otherwise returns false.
        * @param Point The type of projection being requested.
        * @param intersectionStrength How far within the bounds of the projection the point must be before it's considered an intersection. 0 will consider a point anywhere within a projections bounds as an intersections. 1 will only a point as intersecting if it is perfectly at the center of a projections bounds.
        */
        public bool CheckIntersecting(Vector3 Point, float intersectionStrength)
        {
            if (activePool != null && activePool.Count > 0)
                for (var i = activePool.Count - 1; i >= 0; i--)
                    if (activePool[i].Renderer != null)
                    {
                        if (activePool[i].Renderer.CheckIntersecting(Point) > intersectionStrength) return true;
                    }
                    else
                    {
                        activePool.RemoveAt(i);
                    }
            return false;
        }

        //Request
        /**
        * Returns a projection of the specified type from the pool.
        * Projection will be enabled and ready to use. Use the return method once your done with it, do not delete it.
        * @param Renderer Optional - The renderer to copy from. In 90% of use cases this should be a prefab.
        * @param IncludeScripts Optional - Should the renderer being copied have it's scripts copied as well?
        */
        public ProjectionRenderer Request(ProjectionRenderer Renderer = null, bool IncludeBehaviours = false)
        {
            //Request Renderers until we get a valid
            ProjectionRenderer pr = null;
            while (pr == null) pr = RequestRenderer(Renderer, IncludeBehaviours);
            return pr;
        }

        private ProjectionRenderer RequestRenderer(ProjectionRenderer Renderer = null, bool IncludeBehaviours = false)
        {
            //Initialize active pool if required
            if (activePool == null) activePool = new List<PoolItem>();

            if (inactivePool != null && inactivePool.Count > 0)
            {
                //Grab first item in inactive pool
                var item = inactivePool[0];

                //Remove from inactive pool
                inactivePool.RemoveAt(0);

                //Add to active pool
                activePool.Add(item);

                //Initialize item
                item.Initialize(Renderer, IncludeBehaviours);

                return item.Renderer;
            }
            if (activePool.Count < Limit)
            {
                //Create item
                var item = new PoolItem(this);

                //Initialize item
                item.Initialize(Renderer, IncludeBehaviours);

                //Add to active pool
                activePool.Add(item);

                return item.Renderer;
            }
            else
            {
                //Grab oldest item in active pool
                var item = activePool[0];

                //Terminate item
                item.Terminate();

                //Move to end of pool
                activePool.RemoveAt(0);
                activePool.Add(item);

                //Initialize item
                item.Initialize(Renderer, IncludeBehaviours);

                return item.Renderer;
            }
        }
    }

    public class PoolItem
    {
        //Constructor
        public PoolItem(ProjectionPool Pool)
        {
            //Set Pool
            this.Pool = Pool;

            //Generate GameObject
            var go = new GameObject("Projection");
            go.transform.SetParent(this.Pool.Parent);

            //Disable
            go.SetActive(false);

            //Attach Renderer
            Renderer = go.AddComponent<ProjectionRenderer>();
            Renderer.PoolItem = this;
        }

        //Pool
        public ProjectionPool Pool { get; private set; }

        //Renderer
        public ProjectionRenderer Renderer { get; private set; }

        private bool Valid
        {
            get
            {
                //Check if the object still exists
                if (Renderer == null)
                {
                    if (Pool.activePool != null) Pool.activePool.Remove(this);
                    if (Pool.inactivePool != null) Pool.inactivePool.Remove(this);
                    return false;
                }

                return true;
            }
        }

        //Intialize / Terminate
        internal void Initialize(ProjectionRenderer Renderer = null, bool IncludeBehaviours = false)
        {
            if (Valid)
            {
                //Set parent
                this.Renderer.transform.SetParent(Pool.Parent);

                //Copy Renderer Properties
                if (Renderer != null)
                {
                    //Copy projection
                    this.Renderer.Projection = Renderer.Projection;

                    //Copy properties
                    this.Renderer.Tiling = Renderer.Tiling;
                    this.Renderer.Offset = Renderer.Offset;

                    this.Renderer.MaskMethod = Renderer.MaskMethod;
                    this.Renderer.MaskLayer1 = Renderer.MaskLayer1;
                    this.Renderer.MaskLayer2 = Renderer.MaskLayer2;
                    this.Renderer.MaskLayer3 = Renderer.MaskLayer3;
                    this.Renderer.MaskLayer4 = Renderer.MaskLayer4;

                    this.Renderer.Properties = Renderer.Properties;

                    if (IncludeBehaviours)
                        foreach (var component in Renderer.GetComponents<MonoBehaviour>())
                        {
                            //Don't copy transform and projection renderer components
                            if (component.GetType() == typeof(Transform)) continue;
                            if (component.GetType() == typeof(ProjectionRenderer)) continue;

                            this.Renderer.gameObject.AddComponent(component);
                        }

                    //Copy scale
                    this.Renderer.transform.localScale = Renderer.transform.localScale;
                }
                else
                {
                    //Reset scale
                    this.Renderer.transform.localScale = Vector3.one;
                }

                //Enable
                this.Renderer.gameObject.SetActive(true);
            }
        }

        internal void Terminate()
        {
            if (Valid)
            {
                //Disable
                Renderer.gameObject.SetActive(false);

                //Strip unnecessary components
                foreach (var component in Renderer.gameObject.GetComponents<Component>())
                {
                    //Don't remove transform and projection renderer components
                    if (component.GetType() == typeof(Transform)) continue;
                    if (component.GetType() == typeof(ProjectionRenderer)) continue;

                    Object.Destroy(component);
                }

                //Set parent
                Renderer.transform.SetParent(Pool.Parent);
            }
        }

        //Return
        public void Return()
        {
            //Remove from active pool
            Pool.activePool.Remove(this);

            //Terminate
            Terminate();

            //Return projection to inactive pool
            if (Pool.inactivePool == null) Pool.inactivePool = new List<PoolItem>();
            Pool.inactivePool.Add(this);
        }
    }
}