using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace PineFramework
{
    /// <summary>
    /// Provides a flexible and intuitive way for procedurally generating numeric values from user-defined expressions.
    /// </summary>
    public sealed class PineCog : PineObject
    {
        private int _period;
        private double _output;

        private MemoryStream msCode;
        private BinaryReader codeReader;
        private CogBytecode code;
        private readonly string _scriptName;

        private double[] _registers;

        private const int RegReset = (int)CogRegister.Reset;
        private const int RegOffset = (int)CogRegister.Offset;

        /// <summary>
        /// This event is called upon successful execution of the "fire" instruction in a cog.
        /// </summary>
        public event EventHandler<CogFireEventArgs> OnFire;

        /// <summary>
        /// Initializes a new script instance using cached resources.
        /// </summary>
        /// <param name="device">The device to associate the instance with.</param>
        /// <param name="period">The period, in ticks, for each cycle.</param>
        /// <param name="scriptName">The internal name of the script to load from cache.</param>
        public PineCog(PineDevice device, string scriptName, int period) : base(device)
        {
            if (period <= 0)
            {
                period = 1;
            }
            _period = period;
            _output = 0;
            _registers = new double[20];
            if (device.IsCached(scriptName))
            {
                code = device.Cache[scriptName];
                msCode = new MemoryStream(code.CompiledCode);
                codeReader = new BinaryReader(msCode);
                _scriptName = scriptName;
            }
            else
            {
                throw new PineException("Tried to load a script that didn't exist in the device cache ({0}). Please make sure to call PineDevice.CacheScript() before fetching it.", scriptName);
            }
        }

        /// <summary>
        /// Gets the most recently calculated output.
        /// </summary>
        public override double Output
        {
            get { return _output; }
        }    

        /// <summary>
        /// Gets the internal name of the script used by the instance.
        /// </summary>
        public string Name
        {
            get { return _scriptName; }
        }

        /// <summary>
        /// Gets the period for the instance.
        /// </summary>
        public int Period
        {
            get { return _period; }
        }
    
        /// <summary>
        /// Gets the value stored in the specified register.
        /// </summary>
        /// <param name="reg">The register to retrieve a value from.</param>
        /// <returns></returns>
        public double GetRegValue(CogRegister reg)
        {
            return _registers[(int)reg];
        }

        /// <summary>
        /// Stores a new value in the specified register.
        /// </summary>
        /// <param name="reg">The register to write to.</param>
        /// <param name="value">The value to store.</param>
        public void SetRegValue(CogRegister reg, double value)
        {
            _registers[(int)reg] = value;
        }

        public event EventHandler Tick;

        internal override void Iterate()
        {
            codeReader.BaseStream.Position = 0;

            double va, vb, vc;
            int reg0, reg1;
            double x = (double)(this.Device.TicksInternal % _period) / _period;

            double rmin = 0.0;
            double rmax = 1.0;
            Instruction ci;

            while(codeReader.BaseStream.Position < codeReader.BaseStream.Length)
            {
                byte bc = codeReader.ReadByte();
                ci = (Instruction)bc;
                bool ranged = x.InRange(rmin, rmax);

                switch(ci)
                {
                    case Instruction.Abs:
                        if (!ranged) break;
                        va = this.Device.Pop();
                        this.Device.Push(Math.Abs(va));
                        break;
                    case Instruction.Add:
                        if (!ranged) break;
                        va = this.Device.Pop();
                        vb = this.Device.Pop();
                        this.Device.Push(va + vb);
                        break;
                    case Instruction.Atan:
                        if (!ranged) break;
                        va = this.Device.Pop();
                        this.Device.Push(Math.Atan(va));
                        break;
                    case Instruction.Ceiling:
                        if (!ranged) break;
                        va = this.Device.Pop();
                        this.Device.Push(Math.Ceiling(va));
                        break;
                    case Instruction.Clamp:
                        if (!ranged) break;
                        vb = this.Device.Pop();
                        va = this.Device.Pop();
                        vc = this.Device.Pop();
                        this.Device.Push(vc < va ? va : vc > vb ? vb : vc);
                        break;
                    case Instruction.Copy:
                        if (!ranged) break;
                        va = this.Device.Pop();
                        this.Device.Push(va);
                        this.Device.Push(va);
                        break;
                    case Instruction.Cos:
                        if (!ranged) break;
                        va = this.Device.Pop();
                        this.Device.Push(Math.Cos(va));
                        break;
                    case Instruction.Divide:
                        if (!ranged) break;
                        vb = this.Device.Pop();
                        va = this.Device.Pop();
                        this.Device.Push(vb == 0 ? 0 : va / vb);
                        break;
                    case Instruction.Floor:
                        if (!ranged) break;
                        va = this.Device.Pop();
                        this.Device.Push(Math.Floor(va));
                        break;
                    case Instruction.Lerp:
                        if (!ranged) break;
                        vb = this.Device.Pop();
                        va = this.Device.Pop();
                        vc = this.Device.Pop();
                        this.Device.Push(va + (vb - va) * vc);
                        break;
                    case Instruction.Modulo:
                        if (!ranged) break;
                        vb = this.Device.Pop();
                        va = this.Device.Pop();
                        this.Device.Push(va % vb);
                        break;
                    case Instruction.Multiply:
                        if (!ranged) break;
                        va = this.Device.Pop();
                        vb = this.Device.Pop();
                        this.Device.Push(va * vb);
                        break;
                    case Instruction.Power:
                        if (!ranged) break;
                        vb = this.Device.Pop();
                        va = this.Device.Pop();
                        this.Device.Push(Math.Pow(va, vb));
                        break;
                    case Instruction.PushC:
                        va = codeReader.ReadDouble();
                        if (!ranged) break;
                        this.Device.Push(va);
                        break;
                    case Instruction.PushX:
                        if (!ranged) break;
                        this.Device.Push(x);
                        break;
                    case Instruction.Rand:
                        if (!ranged) break;
                        vb = this.Device.Pop();
                        va = this.Device.Pop();
                        this.Device.Push(GlobalRNG.NextDouble() * (vb - va) + va);
                        break;
                    case Instruction.RangeEnd:
                        rmin = 0.0;
                        rmax = 1.0;
                        break;
                    case Instruction.RangeStart:
                        va = codeReader.ReadDouble();
                        vb = codeReader.ReadDouble();
                        if (vb < va)
                        {
                            rmin = 0.0;
                            rmax = 1.0;
                        }
                        else
                        {
                            rmin = va;
                            rmax = vb;
                        }
                        break;
                    case Instruction.Sin:
                        if (!ranged) break;
                        va = this.Device.Pop();
                        this.Device.Push(Math.Sin(va));
                        break;
                    case Instruction.SquareRoot:
                        if (!ranged) break;
                        va = this.Device.Pop();
                        this.Device.Push(Math.Sqrt(va));
                        break;
                    case Instruction.Subtract:
                        if (!ranged) break;
                        vb = this.Device.Pop();
                        va = this.Device.Pop();
                        this.Device.Push(va - vb);
                        break;
                    case Instruction.Tan:
                        if (!ranged) break;
                        va = this.Device.Pop();
                        this.Device.Push(Math.Tan(va));
                        break;
                    case Instruction.LT:
                        if (!ranged) break;
                        vb = this.Device.Pop();
                        va = this.Device.Pop();
                        this.Device.Push(va < vb ? 1 : 0);
                        break;
                    case Instruction.GT:
                        if (!ranged) break;
                        vb = this.Device.Pop();
                        va = this.Device.Pop();
                        this.Device.Push(va > vb ? 1 : 0);
                        break;
                    case Instruction.LE:
                        if (!ranged) break;
                        vb = this.Device.Pop();
                        va = this.Device.Pop();
                        this.Device.Push(va <= vb ? 1 : 0);
                        break;
                    case Instruction.GE:
                        if (!ranged) break;
                        vb = this.Device.Pop();
                        va = this.Device.Pop();
                        this.Device.Push(va >= vb ? 1 : 0);
                        break;
                    case Instruction.PushReg:
                        reg0 = codeReader.ReadInt32();
                        if (!ranged) break;
                        if (reg0 < 0 || reg0 >= _registers.Length)
                        {
                            throw new PineException("Cog \"{0}\" tried to access an invalid register.", _scriptName);
                        }
                        this.Device.Push(_registers[reg0]);
                        break;
                    case Instruction.Pop:
                        reg0 = codeReader.ReadInt32();
                        if (!ranged) break;
                        if (reg0 < 0 || reg0 >= _registers.Length)
                        {
                            throw new PineException("Cog \"{0}\" tried to access an invalid register.", _scriptName);
                        }
                        _registers[reg0] = this.Device.Pop();
                        break;
                    case Instruction.Fire:
                        if (!ranged) break;
                        va = this.Device.Pop();
                        if (va != 0 && this.OnFire != null)
                        {
                            var e = new CogFireEventArgs(GetRegValue(CogRegister.EvA), GetRegValue(CogRegister.EvB), GetRegValue(CogRegister.EvC), GetRegValue(CogRegister.EvD), GetRegValue(CogRegister.EvE), GetRegValue(CogRegister.EvF));
                            this.OnFire(this, e);
                            SetRegValue(CogRegister.EvA, e.A);
                            SetRegValue(CogRegister.EvB, e.B);
                            SetRegValue(CogRegister.EvC, e.C);
                            SetRegValue(CogRegister.EvD, e.D);
                            SetRegValue(CogRegister.EvE, e.E);
                            SetRegValue(CogRegister.EvF, e.F);
                        }
                        break;
                    case Instruction.Zero:
                        reg0 = codeReader.ReadInt32();
                        if (!ranged) break;
                        if (reg0 < 0 || reg0 >= _registers.Length)
                        {
                            throw new PineException("Cog \"{0}\" tried to access an invalid register.", _scriptName);
                        }
                        _registers[reg0] = 0;
                        break;
                    case Instruction.JumpNotZero:
                        reg0 = codeReader.ReadInt32();
                        if (!ranged) break;
                        va = this.Device.Pop();
                        if (va != 0)
                        {
                            codeReader.BaseStream.Position = code.LabelEntryPoints[reg0];
                        }
                        break;
                    case Instruction.Jump:
                        reg0 = codeReader.ReadInt32();
                        if (!ranged) break;
                        codeReader.BaseStream.Position = code.LabelEntryPoints[reg0];
                        break;
                    case Instruction.Not:
                        if (!ranged) break;
                        va = this.Device.Pop();
                        if (va != 0)
                        {
                            this.Device.Push(0);
                        }
                        else
                        {
                            this.Device.Push(1);
                        }
                        break;
                    case Instruction.JumpAlternate:
                        reg1 = codeReader.ReadInt32();
                        reg0 = codeReader.ReadInt32();
                        if (!ranged) break;
                        va = this.Device.Pop();
                        if (va == 0)
                        {
                            codeReader.BaseStream.Position = code.LabelEntryPoints[reg0];
                        }
                        else
                        {
                            codeReader.BaseStream.Position = code.LabelEntryPoints[reg1];
                        }
                        break;
                    case Instruction.Call:
                        reg0 = codeReader.ReadInt32();
                        if (!ranged) break;
                        va = BitConverter.Int64BitsToDouble(codeReader.BaseStream.Position);
                        this.Device.Push(va);
                        codeReader.BaseStream.Position = code.LabelEntryPoints[reg0];
                        break;
                    case Instruction.Return:
                        if (!ranged) break;
                        va = this.Device.Pop();
                        codeReader.BaseStream.Position = BitConverter.DoubleToInt64Bits(va);
                        break;
                    case Instruction.Product:
                        if (!ranged) break;
                        reg0 = (int)this.Device.Pop(); // number of args
                        if (reg0 <= 0) break;
                        va = this.Device.Pop();
                        for (int i = 0; i < reg0 - 1; i++)
                        {
                            va *= this.Device.Pop();
                        }
                        this.Device.Push(va);
                        break;
                    case Instruction.Sum:
                        if (!ranged) break;
                        reg0 = (int)this.Device.Pop(); // number of args
                        if (reg0 <= 0) break;
                        va = 0;
                        for (int i = 0; i < reg0; i++)
                        {
                            va += this.Device.Pop();
                        }
                        this.Device.Push(va);
                        break;
                    case Instruction.NotEqual:
                        if (!ranged) break;
                        va = this.Device.Pop();
                        vb = this.Device.Pop();
                        this.Device.Push(va == vb ? 0 : 1);
                        break;
                    case Instruction.Equal:
                        if (!ranged) break;
                        va = this.Device.Pop();
                        vb = this.Device.Pop();
                        this.Device.Push(va == vb ? 1 : 0);
                        break;
                    case Instruction.InvLerp:
                        if (!ranged) break;
                        vc = this.Device.Pop();
                        vb = this.Device.Pop();
                        va = this.Device.Pop();
                        this.Device.Push((vc - va) / (vb - va));
                        break;
                    default:
                        throw new PineException("Script \"{0}\" tried to use an invalid operation ID (0x{1}).", _scriptName, string.Format("{0:X2}", bc).ToUpper());                        
                }
            }
            if (_registers[RegReset] > 0.0)
            {
                Array.Clear(_registers, 0, _registers.Length);
            }
            _output = this.Device.Pop() + _registers[RegOffset];
            if (this.Tick != null)
            {
                this.Tick(this, new EventArgs());
            }
        }

        /// <summary>
        /// Releases all resources specific to this instance.
        /// </summary>
        protected override void OnDispose()
        {
            codeReader.Close();
            msCode.Close();
        }
    }
}
