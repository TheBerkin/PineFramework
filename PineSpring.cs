using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineFramework
{
    /// <summary>
    /// Contains a constant number which may be manipulated using an offset that decays at a set rate.
    /// </summary>
    public class PineSpring : PineObject
    {
        private double _offsetActual;
        private double _offsetCalculated;

        private double _decayFactor;
        private double _stepFactor;

        private double _valueActual;
        private double _valueCalculated;

        private PineVal _inertia;

        public PineSpring(PineDevice device, double value, double offset, double decayFactor, double stepFactor, double inertiaFactor) : base(device)
        {
            _offsetActual = offset;
            _offsetCalculated = 0;
            _valueActual = value;
            _valueCalculated = value;
            _decayFactor = decayFactor;
            _stepFactor = stepFactor;
            _inertia = inertiaFactor;
        }

        /// <summary>
        /// Adds a specific amount to the existing offset.
        /// </summary>
        /// <param name="offset">The amount to add.</param>
        public void AddOffset(double offset)
        {
            _offsetActual += offset;
        }

        /// <summary>
        /// Gets the actual (base) value for the spring.
        /// </summary>
        public double ActualValue
        {
            get { return _valueActual; }
            set
            {
                _offsetActual += (value - _valueActual) * _inertia;
                _valueActual = value;
            }
        }

        /// <summary>
        /// Gets or sets the decay factor (offset * decay).
        /// </summary>
        public double DecayFactor
        {
            get { return _decayFactor; }
            set { _decayFactor = value; }
        }

        /// <summary>
        /// Gets or sets the stepping factor (offset / step).
        /// </summary>
        public double StepFactor
        {
            get { return _stepFactor; }
            set { _stepFactor = value; }
        }

        /// <summary>
        /// Gets or sets the inertia factor.
        /// </summary>
        public double InertiaFactor
        {
            get { return _inertia; }
            set { _inertia = value; }
        }

        /// <summary>
        /// Removes any existing offsets from the spring.
        /// </summary>
        public void Steady()
        {
            _offsetActual = 0;
            _offsetCalculated = 0;
        }

        internal override void Iterate()
        {
            _offsetCalculated += (_offsetActual - _offsetCalculated) * _stepFactor;
            _offsetActual *= _decayFactor;
            _valueCalculated = _valueActual + _offsetCalculated;
        }

        /// <summary>
        /// Gets the last calculated output for the spring.
        /// </summary>
        public override double Output
        {
            get { return _valueCalculated; }
        }
    }
}
