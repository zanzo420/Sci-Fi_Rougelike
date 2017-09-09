#region

using System.Collections;
using LlockhamIndustries.Decals;
using UnityEngine;

#endregion

namespace LlockhamIndustries.Misc
{
    [RequireComponent(typeof(RayPositioner))]
    public class Selectable : MonoBehaviour
    {
        //Backing fields
        private ProjectionRenderer decal;

        private bool selected;
        public Color selectedColor = Color.white;
        public Color unselectedColor = Color.gray;

        //Properties
        public bool Selected
        {
            set
            {
                //Adjust decal color
                if (decal != null && selected != value)
                {
                    //Update value
                    selected = value;

                    //Adjust decal
                    if (selected)
                    {
                        decal.SetColor(0, selectedColor);
                        decal.UpdateProperties();
                    }
                    else
                    {
                        decal.SetColor(0, unselectedColor);
                        decal.UpdateProperties();
                    }
                }
            }
            get { return selected; }
        }

        private ProjectionRenderer Decal
        {
            set
            {
                if (decal != value)
                {
                    //Set decal
                    decal = value;

                    //Adjust decal
                    if (selected)
                    {
                        decal.SetColor(0, selectedColor);
                        decal.UpdateProperties();
                    }
                    else
                    {
                        decal.SetColor(0, unselectedColor);
                        decal.UpdateProperties();
                    }
                }
            }
        }

        //Generic methods
        private void OnEnable()
        {
            //Register to selector
            StartCoroutine(Register());

            //Grab projection
            StartCoroutine(GrabProjection());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            Deregister();
        }

        private IEnumerator GrabProjection()
        {
            //Grab ray positioner
            var positioner = GetComponent<RayPositioner>();

            //Grab projection
            while (decal == null)
            {
                Decal = positioner.Active;
                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator Register()
        {
            //Wait for selector
            while (!Selector.Initialized) yield return new WaitForEndOfFrame();

            //Register
            Selector.Register(this);
        }

        private void Deregister()
        {
            //Selector check
            if (!Selector.Initialized) return;

            //Deregister
            Selector.Deregister(this);
        }
    }
}