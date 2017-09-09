#region

using System.Collections.Generic;
using LlockhamIndustries.Decals;
using UnityEngine;

#endregion

namespace LlockhamIndustries.Misc
{
    public class GooManager : MonoBehaviour
    {
        //Singleton
        private static GooManager singleton;

        private List<ProjectionRenderer> bounce;

        private List<ProjectionRenderer> slide;
        private List<ProjectionRenderer> stick;

        private void Awake()
        {
            //Initialize singleton
            if (singleton == null) singleton = this;
            else if (singleton != this) Destroy(gameObject);
        }

        private static List<ProjectionRenderer> GetGoo(GooType Type)
        {
            if (singleton != null)
                switch (Type)
                {
                    case GooType.Slide:
                        if (singleton.slide == null) singleton.slide = new List<ProjectionRenderer>();
                        return singleton.slide;
                    case GooType.Bounce:
                        if (singleton.bounce == null) singleton.bounce = new List<ProjectionRenderer>();
                        return singleton.bounce;
                    case GooType.Stick:
                        if (singleton.stick == null) singleton.stick = new List<ProjectionRenderer>();
                        return singleton.stick;
                }
            return null;
        }

        public static bool Register(ProjectionRenderer Projection, GooType Type)
        {
            //Grab goo
            var goo = GetGoo(Type);

            if (goo != null)
            {
                //Add projection to goo
                if (!goo.Contains(Projection)) goo.Add(Projection);

                //Successfully registered
                return true;
            }

            //Singleton not yet initialized, cannot be registered
            return false;
        }

        public static void Deregister(ProjectionRenderer Projection, GooType Type)
        {
            //Grab goo
            var goo = GetGoo(Type);

            //Remove projection from goo
            if (goo != null) goo.Remove(Projection);
        }

        public static bool WithinGoo(GooType Type, Vector3 Point, float Strictness)
        {
            //Grab goo
            var goo = GetGoo(Type);

            if (goo != null)
                for (var i = 0; i < goo.Count; i++)
                    if (goo[i].CheckIntersecting(Point) > Strictness) return true;

            return false;
        }
    }

    public enum GooType { Slide, Bounce, Stick }
}

