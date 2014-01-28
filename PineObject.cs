using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineFramework
{
    public abstract class PineObject : IDisposable
    {
        internal PineObject(PineDevice device)
        {            
            if (device == null)
            {
                this.Enabled = true;
                this.Device = null;
            }
            else if (device.Add(this))
            {
                this.Enabled = true;
                this.Device = device;
            }
            else
            {
                throw new PineException("Maximum objects were exceeded for the selected device (" + device.MaxObjects + ").");
            }
        }

        // Fields

        /// <summary>
        /// Determines whether or not the object should be updated by the associated device (does not affect PineVals).
        /// </summary>
        public bool Enabled;

        private bool _disposed;

        protected PineDevice Device;

        // Properties

        /// <summary>
        /// Gets the output value for the object.
        /// </summary>
        public abstract double Output
        {
            get;
        }

        public bool Disposed
        {
            get { return _disposed; }
        }

        // Methods
        
        internal virtual void Iterate()
        {
        }

        /// <summary>
        /// Unlinks a PINE object from its associated device.
        /// </summary>
        /// <returns></returns>
        public bool Remove()
        {
            if (this.Device != null)
            {
                return this.Device.Remove(this);
            }
            else
            {
                return false;
            }
        }

        // Operators
        public static implicit operator double(PineObject pineObj)
        {
            return pineObj.Output;
        }

        public static implicit operator float(PineObject pineObj)
        {
            return (float)pineObj.Output;
        }

        public static implicit operator PineObject(double value)
        {
            return new PineVal(value);
        }

        protected virtual void OnDispose()
        {

        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            GC.SuppressFinalize(this);
            this.Remove();
            this.OnDispose();
        }

        ~PineObject()
        {
            this.Dispose();
        }
    }
}
