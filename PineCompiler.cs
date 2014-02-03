using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace PineFramework
{
    /// <summary>
    /// Contains the functionality necessary for compiling PINE cog bytecode.
    /// </summary>
    public static class PineCompiler
    {
        /// <summary>
        /// Compiles a cog from a given input.
        /// </summary>
        /// <param name="input">The uncompiled input.</param>
        /// <returns></returns>
        public static CogBytecode Compile(string input)
        {
            StringReader reader = new StringReader(input);
            MemoryStream ms = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(ms, Encoding.UTF8);

            Dictionary<string, int> labelDefs = new Dictionary<string,int>(); // labelname -> entryPoint
            List<string> labelCalls = new List<string>(); // labelIndex -> labelName

            while (reader.Peek() > -1)
            {
                string s = reader.ReadLine().Trim();
                if (s.StartsWith("'"))
                {
                    continue;
                }
                else
                {
                    int commentIndex = s.IndexOf("'");
                    if (commentIndex > -1)
                    {
                        s = s.Substring(0, commentIndex);
                    }
                }
                
                if (s.StartsWith("#"))
                {
                    string label = s.Substring(1);
                    if (label.Contains(' ') || label.Length == 0) continue;
                    if (labelDefs.ContainsKey(label))
                    {
                        throw new PineException("Pine compile error: Label \"{0}\" was defined more than once.", label);
                    }
                    else
                    {
                        labelDefs.Add(label, (int)writer.BaseStream.Length);
                    }
                    continue;
                }
                
                string[] parts = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0)
                {
                    continue;
                }
                double a;
                double b;
                string lbl1;
                string lbl2;
                int reg = -1;
                switch (parts[0].ToLower())
                {
                    case "push":
                        reg = GetRegisterIndex(parts[1]);
                        if (parts.Length != 2)
                        {
                            throw new PineException("Cog compile error: Invalid push syntax \"{0}\"", s);
                        }
                        if (parts[1] == "x")
                        {
                            writer.Write((byte)Instruction.PushX);
                        }
                        else if (parts[1] == "pi")
                        {
                            writer.Write((byte)Instruction.PushC);
                            writer.Write(Math.PI);
                        }
                        else if (reg > -1)
                        {
                            writer.Write((byte)Instruction.PushReg);
                            writer.Write(reg);
                        }
                        else if (double.TryParse(parts[1], out a))
                        {
                            writer.Write((byte)Instruction.PushC);
                            writer.Write(a);
                        }
                        else
                        {
                            throw new PineException("Cog compile error: Bad push value \"{0}\"", parts[1]);
                        }
                        break;
                    case "add":
                    case "+":
                        writer.Write((byte)Instruction.Add);
                        break;
                    case "subtract":
                    case "sub":
                    case "-":
                        writer.Write((byte)Instruction.Subtract);
                        break;
                    case "divide":
                    case "div":
                    case "/":
                        writer.Write((byte)Instruction.Divide);
                        break;
                    case "multiply":
                    case "mul":
                    case "*":
                        writer.Write((byte)Instruction.Multiply);
                        break;
                    case "lerp":
                        writer.Write((byte)Instruction.Lerp);
                        break;
                    case "clamp":
                    case "|":
                        writer.Write((byte)Instruction.Clamp);
                        break;
                    case "mod":
                        writer.Write((byte)Instruction.Modulo);
                        break;
                    case "sqrt":
                        writer.Write((byte)Instruction.SquareRoot);
                        break;
                    case "power":
                    case "pow":
                        writer.Write((byte)Instruction.Power);
                        break;
                    case "ceil":
                        writer.Write((byte)Instruction.Ceiling);
                        break;
                    case "floor":
                        writer.Write((byte)Instruction.Floor);
                        break;
                    case "range":
                        writer.Write((byte)Instruction.RangeStart);
                        if (parts.Length != 3)
                        {
                            throw new PineException("Cog compile error: Invalid range syntax \"{0}\"", s);
                        }
                        if (!double.TryParse(parts[1], out a) || !double.TryParse(parts[2], out b))
                        {
                            throw new PineException("Cog compile error: Bad range values \"{0}\"", parts[1]);
                        }
                        writer.Write(a);
                        writer.Write(b);
                        break;
                    case "end":
                        writer.Write((byte)Instruction.RangeEnd);
                        break;
                    case "sin":
                        writer.Write((byte)Instruction.Sin);
                        break;
                    case "cos":
                        writer.Write((byte)Instruction.Cos);
                        break;
                    case "tan":
                        writer.Write((byte)Instruction.Tan);
                        break;
                    case "atan":
                        writer.Write((byte)Instruction.Atan);
                        break;
                    case "abs":
                        writer.Write((byte)Instruction.Abs);
                        break;
                    case "rand":
                    case "random":
                        writer.Write((byte)Instruction.Rand);
                        break;
                    case "copy":
                    case "clone":
                        writer.Write((byte)Instruction.Copy);
                        break;
                    case "<":
                    case "lt":
                        writer.Write((byte)Instruction.LT);
                        break;
                    case ">":
                    case "gt":
                        writer.Write((byte)Instruction.GT);
                        break;
                    case "<=":
                    case "le":
                        writer.Write((byte)Instruction.LE);
                        break;
                    case ">=":
                    case "ge":
                        writer.Write((byte)Instruction.GE);
                        break;
                    case "pop":
                        if (parts.Length != 2)
                        {
                            throw new PineException("Cog compile error: Invalid pop syntax \"{0}\"", s);
                        }
                        else if (parts[1] == "out")
                        {
                            writer.Write((byte)Instruction.OutS);
                            break;
                        }
                        writer.Write((byte)Instruction.Pop);
                        
                        reg = GetRegisterIndex(parts[1]);                        
                        if (reg < 0)
                        {
                            throw new PineException("Cog compile error: Bad pop register \"{0}\"", parts[1]);
                        }
                        writer.Write(reg);
                        break;
                    case "zero":
                        writer.Write((byte)Instruction.Zero);
                        if (parts.Length != 2)
                        {
                            throw new PineException("Cog compile error: Invalid zero syntax \"{0}\"", s);
                        }
                        reg = GetRegisterIndex(parts[1]);
                        if (reg < 0)
                        {
                            throw new PineException("Cog compile error: Bad zero register \"{0}\"", parts[1]);
                        }
                        writer.Write(reg);
                        break;
                    case "fire":
                        writer.Write((byte)Instruction.Fire);
                        break;
                    case "jnz":
                        writer.Write((byte)Instruction.JumpNotZero);
                        if (parts.Length != 2)
                        {
                            throw new PineException("Cog compile error: Invalid jnz syntax \"{0}\"", s);
                        }
                        lbl1 = parts[1];
                        if (!labelCalls.Contains(lbl1))
                        {
                            labelCalls.Add(lbl1);
                        }
                        writer.Write(labelCalls.IndexOf(lbl1));
                        break;
                    case "jmp":
                        writer.Write((byte)Instruction.Jump);
                        if (parts.Length != 2)
                        {
                            throw new PineException("Cog compile error: Invalid jmp syntax \"{0}\"", s);
                        }
                        lbl1 = parts[1];
                        if (!labelCalls.Contains(lbl1))
                        {
                            labelCalls.Add(lbl1);
                        }
                        writer.Write(labelCalls.IndexOf(lbl1));
                        break;
                    case "not":
                        writer.Write((byte)Instruction.Not);
                        break;
                    case "jmpx":
                        writer.Write((byte)Instruction.JumpAlternate);
                        if (parts.Length != 3)
                        {
                            throw new PineException("Cog compile error: Invalid jmpx syntax \"{0}\"", s);
                        }
                        lbl1 = parts[1]; // Nonzero case
                        lbl2 = parts[2]; // Zero case
                        if (!labelCalls.Contains(lbl1))
                        {
                            labelCalls.Add(lbl1);
                        }
                        if (!labelCalls.Contains(lbl2))
                        {
                            labelCalls.Add(lbl2);
                        }
                        writer.Write(labelCalls.IndexOf(lbl1));
                        writer.Write(labelCalls.IndexOf(lbl2));
                        break;
                    case "call":
                        if (parts.Length != 2)
                        {
                            throw new PineException("Cog compile error: Invalid call syntax \"{0}\"", s);
                        }
                        writer.Write((byte)Instruction.Call);
                        lbl1 = parts[1];
                        if (!labelCalls.Contains(lbl1))
                        {
                            labelCalls.Add(lbl1);
                        }
                        writer.Write(labelCalls.IndexOf(lbl1));
                        break;
                    case "ret":
                        writer.Write((byte)Instruction.Return);
                        break;
                    case "eq":
                        writer.Write((byte)Instruction.Equal);
                        break;
                    case "ne":
                        writer.Write((byte)Instruction.NotEqual);
                        break;
                    case "sum":
                        writer.Write((byte)Instruction.Sum);
                        break;
                    case "prd":
                        writer.Write((byte)Instruction.Product);
                        break;
                    case "ilerp":
                        writer.Write((byte)Instruction.InvLerp);
                        break;
                    case "out":
                        reg = GetRegisterIndex(parts[1]);
                        if (parts.Length != 2)
                        {
                            throw new PineException("Cog compile error: Invalid push syntax \"{0}\"", s);
                        }
                        if (reg > -1)
                        {
                            writer.Write((byte)Instruction.OutR);
                            writer.Write(reg);
                        }
                        else if (double.TryParse(parts[1], out a))
                        {
                            writer.Write((byte)Instruction.OutC);
                            writer.Write(a);
                        }
                        else
                        {
                            throw new PineException("Cog compile error: Bad out value \"{0}\"", parts[1]);
                        }
                        break;
                    default:
                        break;
                }
            }
            writer.Close();
            if (labelCalls.Count > labelDefs.Count)
            {
                var lstMissing = labelCalls.Where(str => !labelDefs.ContainsKey(str)).ToArray();
                string missing = "";
                int count = lstMissing.Count();
                for (int i = 0; i < count; i++)
                {
                    missing += lstMissing[i];
                    if (i < count - 1)
                    {
                        missing += ", ";
                    }
                }
                throw new PineException("Cog compile error: Label definitions missing for the following references: {0}", missing);
            }
            int[] labels = new int[labelDefs.Count];
            foreach(KeyValuePair<string, int> labelPair in labelDefs)
            {
                int labelIndex = labelCalls.IndexOf(labelPair.Key);
                if (labelIndex > -1)
                {
                    labels[labelIndex] = labelPair.Value;
                }
            }
            return new CogBytecode(ms.ToArray(), labels);
        }

        private static int GetRegisterIndex(string regName)
        {
            CogRegister reg;
            if (Enum.TryParse<CogRegister>(regName, true, out reg)) // Get register by value
            {                
                string name = Enum.GetName(typeof(CogRegister), (byte)reg); // Get register by name
                if (string.IsNullOrEmpty(name))
                {
                    return -1;
                }
                else if (name.ToLower() != regName.ToLower())
                {
                    return -1;
                }
                return (int)reg;
            }
            else
            {
                return -1;
            }
        }
    }
}
