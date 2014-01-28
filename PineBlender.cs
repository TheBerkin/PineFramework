using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineFramework
{
    /// <summary>
    /// Provides logic for interpolating between numbers on a per-change basis of a single value.
    /// </summary>
    public class PineBlender : PineObject
    {
        private double _valueCalculated;
        private PineObject _valueActual;
        private double _factor;
        private PineBlendTechnique _type;

        public PineBlender(PineDevice device, PineObject value, double factor, PineBlendTechnique factorType) : base(device)
        {
            _valueCalculated = value;
            _valueActual = value;
            _factor = factor;
            _type = factorType;
        }

        /// <summary>
        /// The interpolated value of the blender.
        /// </summary>
        public override double Output
        {
            get { return _valueCalculated; }
        }

        /// <summary>
        /// The destination value of the blender, without interpolation.
        /// </summary>
        public double ActualValue
        {
            get { return _valueActual; }
            set { _valueActual = value; }
        }

        internal override void Iterate()
        {
            if (_valueCalculated == _valueActual) return;

            bool add = _valueActual > _valueCalculated;
            double diff = Math.Abs(_valueActual - _valueCalculated);

            switch (_type)
            {
                case PineBlendTechnique.Step:
                    if (add)
                    {
                        _valueCalculated += _factor;
                    }
                    else
                    {
                        _valueCalculated -= _factor;
                    }
                    break;
                case PineBlendTechnique.Taper:
                    if (add)
                    {
                        _valueCalculated += diff / _factor;
                    }
                    else
                    {
                        _valueCalculated -= diff / _factor;
                    }
                    break;
                case PineBlendTechnique.InverseTaper:
                    if (add)
                    {
                        _valueCalculated += _factor / diff;
                    }
                    else
                    {
                        _valueCalculated -= _factor / diff;
                    }
                    break;
            }

            // Clamp the resultant value for the appropriate direction if needed
            if ((add && _valueCalculated > _valueActual) || (!add && _valueCalculated < _valueActual))
            {
                _valueCalculated = _valueActual;
            }
        }
    }
}
