#region

using UnityEngine;

#endregion

namespace LlockhamIndustries.Decals
{
    /**
    * This component allows you to animate your projections. Attach to a projection renderer with a projection set up as a sprite sheet.
    * Designed to be printed with your projections. Attach to your prefab and enable print behaviours on your printer.
    */
    [RequireComponent(typeof(ProjectionRenderer))]
    public class SheetAnimator : MonoBehaviour
    {
        [Header("Basics")]
        /**
        * The number of collumns in the sprite sheet being sampled.
        */
        [Tooltip("The number of collumns in the sprite sheet being sampled.")]
        public int collumns;

        /**
        * Destroy the projection when the animator has finished its first loop.
        */
        [Tooltip("Destroy the projection when the animator has finished its first loop.")]
        public bool destroyOnComplete;

        /**
        * Sample frames from the bottom instead of the top.
        */
        [Tooltip("Sample frames from the bottom instead of the top.")]
        public bool invertY;

        private bool paused;

        //Backing fields
        private ProjectionRenderer projection;

        /**
        * The number of rows in the sprite sheet being sampled.
        */
        [Tooltip("The number of rows in the sprite sheet being sampled.")]
        public int rows;

        [Header("Advanced")]
        /**
        * Skip the first x frames of the animation.
        */
        [Tooltip("Skip the first x frames of the animation.")]
        public int skipFirst;

        /**
        * Skip the last x frames of the animation.
        */
        [Tooltip("Skip the last x frames of the animation.")]
        public int skipLast;

        /**
        * The playback speed, in frames per second.
        */
        [Tooltip("The playback speed, in frames per second.")]
        public float speed;

        private float time;

        private void Awake()
        {
            //Grab our projection
            projection = GetComponent<ProjectionRenderer>();
        }

        private void Update()
        {
            //Calculate count
            var count = collumns * rows - (skipFirst + skipLast);

            //Increment time
            if (!paused) time += Time.deltaTime * speed;
            if (time > count)
                if (destroyOnComplete)
                {
                    projection.Destroy();
                    return;
                }
                else
                {
                    time -= count;
                }

            //Calculate current frame
            var frame = skipFirst + Mathf.FloorToInt(time);

            //Calculate frame size
            var size = new Vector2(1 / collumns, 1 / rows);

            //Calculate current row & collumn
            var row = frame / collumns;
            var collumn = frame % collumns;

            //Calculate offset
            var x = size.x * collumn;
            var y = size.y * row;
            if (!invertY) y = 1 - size.y - y;

            //Set tiling
            projection.Tiling = new Vector2(size.x, size.y);

            //Set offset
            projection.Offset = new Vector2(x, y);

            //Update projection with new properties
            projection.UpdateProperties();
        }

        //Access methods
        /**
        * Plays the sprite animation.
        */
        public void Play()
        {
            paused = false;
        }

        /**
        * Pauses the sprite animation. Calling Play() will begin the animation again from the current position.
        */
        public void Pause()
        {
            paused = true;
        }

        /**
        * Stops the sprite animation. Calling Play() will begin the animation from the begining.
        */
        public void Stop()
        {
            paused = true;
            time = 0;
        }
    }
}