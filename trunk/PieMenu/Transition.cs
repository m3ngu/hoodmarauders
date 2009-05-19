using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

// Adapted to be compatible with GoblinXNA from an XNA pie menu implementation
// by Catalin Zima  http://www.catalinzima.com/?page_id=15

namespace Manhattanville.PieMenu
{
    /// <summary>
    /// The curve of the transition
    /// </summary>
    public enum TransitionCurve
    {
        Linear,
        SmoothStep,
        Exponential,
        Sqrt
    }
    public enum Direction
    {
        Ascending,
        Descending
    }

    /// <summary>
    /// Defines a transition
    /// </summary>
    public class Transition
    {
        #region Constructors
        public Transition(Direction direction, TransitionCurve transitionCurve, float transitionDuration)
        {
            currentPosition = 0.0f;
            interpolationCurve = transitionCurve;
            Reset(direction, transitionDuration);
        }

        #endregion


        public void Reset(Direction direction, float transitionLength)
        {
            this.direction = direction;
            speed = 1 / transitionLength;
            Reset();
        }
        public void Reset(Direction direction)
        {
            this.direction = direction;
            Reset();
        }
        public void Reset()
        {
            if (direction == Direction.Ascending)
                currentPosition = 0.0f;
            else
                currentPosition = 1.0f;
        }


        #region Timing properties

        private float currentPosition;

        public float CurrentPosition
        {
            get { return MathHelper.Lerp(0.0f, 1.0f, timeLerp()); }
        }

        private Direction direction;


        private float speed;

        /// <summary>
        /// The curve to use when interpolating (default linear)
        /// </summary>
        private TransitionCurve interpolationCurve;

        public TransitionCurve InterpolationCurve
        {
            get { return interpolationCurve; }
            set { interpolationCurve = value; }
        }

        /// <summary>
        /// Updates the current time
        /// </summary>
        /// <param name="time">The current time</param>
        public void Update(double elapsedTime)
        {
            if (direction == Direction.Ascending)
                currentPosition += speed * (float)elapsedTime;
            else
                currentPosition -= speed * (float)elapsedTime;

            if (Finished)
                if (OnTransitionEnd != null)
                    OnTransitionEnd(this);
        }


        /// <summary>
        /// Whether or not the transition has finished
        /// </summary>
        public bool Finished
        {
            get
            {
                if (direction == Direction.Ascending)
                    return (currentPosition >= 1.0f);
                else
                    return (currentPosition <= 0.0f);
            }
        }

        /// <summary>
        /// Gets a value between 0 and 1 indicating the current transition point for a given time
        /// </summary>
        /// <returns>A value between 0 and 1 indicating the current transition point</returns>
        private float timeLerp()
        {
            if (direction == Direction.Ascending)
            {
                if (currentPosition < 0.0f)
                    return 0;
                if (currentPosition > 1.0f)
                    return 1;
            }
            else
            {
                if (currentPosition < 0.0f)
                    return 1;
                if (currentPosition > 1.0f)
                    return 0;
            }

            double timelerp = currentPosition;

            switch (interpolationCurve)
            {
                case TransitionCurve.SmoothStep:
                    timelerp = (float)MathHelper.SmoothStep(0f, 1f, (float)timelerp);
                    break;
                case TransitionCurve.Exponential:
                    timelerp = Math.Pow(timelerp, 2);
                    break;
                case TransitionCurve.Sqrt:
                    timelerp = Math.Sqrt(timelerp);
                    break;
                case TransitionCurve.Linear:
                default:
                    break;
            }

            return (float)timelerp;

        }

        #endregion

        public SimpleDelegate OnTransitionEnd;
    }
}
