using System;

namespace UnityEngine
{
    [Serializable]
    public class Vector3Animation : SimpleAnimation
    {
        [NonSerialized] public Vector3 vector3ToAnimate;
        [SerializeField] public Vector3 originVector3;
        [SerializeField] public Vector3 destinationVector3;

        // It is mandatory to have a parameterless constructor to properly work with the SimpleAnimationsManager component in the inspector.
        public Vector3Animation() : this(Vector3.zero, Vector3.zero, Vector3.zero) { } 
        
        public Vector3Animation(Vector3 vector3ToAnimate, Vector3 destination, Vector3 origin, float duration = 1f, Curve curve = Curve.EaseInOut, WrapMode wrapMode = WrapMode.Once)
        {
            this.vector3ToAnimate = vector3ToAnimate;
            this.originVector3 = origin;
            this.destinationVector3 = destination;
            

            this.duration = duration;
            this.curve = SimpleAnimation.GetCurve(curve);
            this.wrapMode = wrapMode;
        }

        public override bool Step(float deltaTime, bool inverseIfMirror = true)
        {
            bool endOfAnimation = base.Step(deltaTime, inverseIfMirror);
            
            vector3ToAnimate = Vector3.Lerp(originVector3, destinationVector3, currentAnimationCurveValue);
            
            return endOfAnimation;
        }

        public override Object GetAnimatedObject(bool displayWarningIfNotApplicable)
        {
            if (displayWarningIfNotApplicable)
                Debug.LogWarning("Trying to obtain the animated object from a Vector3Animation. This action is not supported. Access the 'vector3ToAnimate' instead.");
            return null;
        }
    }
}
