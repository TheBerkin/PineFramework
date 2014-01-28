using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace PineFramework
{
    /// <summary>
    /// Devices are used to host and update lists of PINE objects.
    /// </summary>
    public class PineDevice
    {
        /// <summary>
        /// The maximum size of the stack.
        /// </summary>
        public const int MaxStackSize = 128;

        /// <summary>
        /// The maximum allowed objects for this device.
        /// </summary>
        public readonly int MaxObjects;

        private PineObject[] list;
        private int listSize;

        private double[] stack;
        private int stackSize;

        private bool _enabled;
        internal long TicksInternal;

        internal Dictionary<string, CogBytecode> Cache;

        public PineDevice(int maxObjects)
        {
            this.MaxObjects = maxObjects;

            list = new PineObject[MaxObjects];
            listSize = 0;

            stack = new double[MaxStackSize];
            stackSize = 0;

            _enabled = true;
            TicksInternal = 0;

            this.Cache = new Dictionary<string, CogBytecode>();
        }

        /// <summary>
        /// Compiles a script and caches it within the device.
        /// </summary>
        /// <param name="name">The internal name for the script.</param>
        /// <param name="path">The path to the script file.</param>
        /// <returns></returns>
        public bool LoadScript(string name, string path)
        {
            if (Cache.ContainsKey(name)) return false;
            if (path.EndsWith(".cog") || path.EndsWith(".txt"))
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    var code = PineCompiler.Compile(reader.ReadToEnd());
                    Cache.Add(name, code);
                    return true;
                }
            }
            else if (path.EndsWith(".pcbf"))
            {
                var code = CogBytecode.FromFile(path);
                Cache.Add(name, code);
                return true;
            }
            else
            {
                throw new NotSupportedException("The framework attempted to load a file with an unrecognized extension: " + path);
            }
        }

        /// <summary>
        /// Gets or sets the enabled status of the device.
        /// </summary>
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        /// <summary>
        /// Gets the number of ticks elapsed.
        /// </summary>
        public long Ticks
        {
            get { return TicksInternal; }
        }

        /// <summary>
        /// Determines if a script with a specific name is cached within the device.
        /// </summary>
        /// <param name="cogName">The internal name of the script.</param>
        /// <returns></returns>
        public bool IsCached(string cogName)
        {
            return Cache.ContainsKey(cogName);
        }

        private int NearestSlot()
        {
            for (int i = 0; i < this.MaxObjects; i++)
            {
                if (list[i] == null)
                {
                    return i;
                }
            }
            return -1;
        }

        internal bool Add(PineObject obj)
        {
            int slot = NearestSlot();
            if (slot + 1 >= listSize)
            {
                listSize = slot + 1;
            }
            else if (slot < 0)
            {
                return false;
            }
            list[slot] = obj;
            return true;
        }

        /// <summary>
        /// Finds the index of a PINE object.
        /// </summary>
        /// <param name="obj">The PINE object to locate.</param>
        /// <returns>Object index if found, -1 if not found.</returns>
        public int IndexOf(PineObject obj)
        {
            for(int i = 0; i < listSize; i++)
            {
                if (list[i] == obj)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns a PINE object located at a specific index.
        /// </summary>
        /// <param name="index">The index of the PINE object to be retrieved.</param>
        /// <returns></returns>
        public PineObject GetObjectAt(int index)
        {
            return list[index];
        }

        /// <summary>
        /// Removes a PINE object at a specific index from the device queue.
        /// </summary>
        /// <param name="id">The index of the object to remove.</param>
        /// <returns></returns>
        public bool RemoveAt(int id)
        {
            if (id >= listSize)
            {
                return false;
            }
            else if (list[id] == null)
            {
                return false;
            }
            else
            {
                list[id] = null;
                if (id + 1 == listSize)
                {                    
                    do
                    {
                        listSize--;
                        if (listSize == 0) break;
                    }
                    while (list[listSize - 1] == null);
                }                
                return true;
            }
        }

        /// <summary>
        /// Removes a specific PINE objects from the device queue.
        /// </summary>
        /// <param name="obj">The object to remove.</param>
        /// <returns></returns>
        public bool Remove(PineObject obj)
        {
            for(int i = 0; i < listSize; i++)
            {
                if (list[i] == obj)
                {
                    list[i] = null;
                    if (i + 1 == listSize)
                    {
                        do
                        {
                            listSize--;
                            if (listSize == 0) break;
                        }
                        while (list[listSize - 1] == null);
                    }                    
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes all associated PINE objects from the device's queue.
        /// </summary>
        public void ClearAll()
        {
            for(int i = 0; i < listSize; i++)
            {
                list[i] = null;
            }
            listSize = 0;
        }

        /// <summary>
        /// Updates all currently loaded PINE objects.
        /// </summary>
        /// <returns>The number of PINE objects successfully updated.</returns>
        public int Iterate()
        {
            if (!_enabled) return 0;
            int count = 0;
            PineObject currentObj;
            for(int i = 0; i < listSize; i++)
            {
                currentObj = list[i];
                if (currentObj != null)
                {
                    if (currentObj.Enabled && !currentObj.Disposed)
                    {
                        currentObj.Iterate();
                        count++;
                    }
                }
            }
            TicksInternal++;
            return count;
        }

        /// <summary>
        /// Gets the current stack size of the device.
        /// </summary>
        /// <returns></returns>
        public int GetStackSize()
        {
            return stackSize;
        }

        /// <summary>
        /// Gets the current number of PINE objects loaded to the device.
        /// </summary>
        /// <returns></returns>
        public int GetObjectCount()
        {
            return listSize;
        }

        #region Internals

        internal double Pop()
        {
            if (stackSize == 0)
            {
                throw new PineException("Tried to pop from an empty stack.");
            }
            int index = stackSize - 1;
            stackSize--;
            return stack[index];
        }

        internal void Push(double value)
        {
            if (stackSize >= MaxStackSize)
            {
                throw new PineException("Stack size exceeded ("+MaxStackSize+").");
            }
            stack[stackSize] = value;
            stackSize++;
        }

        #endregion
    }
}
