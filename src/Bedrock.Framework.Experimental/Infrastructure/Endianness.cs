﻿using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Bedrock.Framework.Experimental.Infrastructure
{
    public static class Endianness
    {
        public static unsafe T FromNetworkOrder<T>(T value) where T : unmanaged
        {
            if (BitConverter.IsLittleEndian) return Reverse(value);
            return value;
        }

        public static unsafe T ToNetworkOrder<T>(T value) where T : unmanaged
        {
            if (BitConverter.IsLittleEndian) return Reverse(value);
            return value;
        }

        public static unsafe T Reverse<T>(T value) where T : unmanaged
        {
            if (sizeof(T) == 1)
            {
                return value;
            }
            else if (sizeof(T) == 2)
            {
                var val = BinaryPrimitives.ReverseEndianness(Unsafe.As<T, ushort>(ref value));
                return Unsafe.As<ushort, T>(ref val);
            }
            else if (sizeof(T) == 4)
            {
                var val = BinaryPrimitives.ReverseEndianness(Unsafe.As<T, uint>(ref value));
                return Unsafe.As<uint, T>(ref val);
            }
            else if (sizeof(T) == 8)
            {
                var val = BinaryPrimitives.ReverseEndianness(Unsafe.As<T, ulong>(ref value));
                return Unsafe.As<ulong, T>(ref val);
            }
            else if (sizeof(T) < 512)
            {
                var len = sizeof(T);
                var val = stackalloc byte[len];
                Unsafe.Write(val, value);
                int to = len >> 1, dest = len - 1;
                for (var i = 0; i < to; i++)
                {
                    var tmp = val[i];
                    val[i] = val[dest];
                    val[dest--] = tmp;
                }
                return Unsafe.Read<T>(val);
            }
            else
            {
                var len = sizeof(T);
                var val = new byte[len];

                fixed (void* valPointer = val)
                {
                    Unsafe.Write(valPointer, value);
                    int to = len >> 1, dest = len - 1;
                    for (var i = 0; i < to; i++)
                    {
                        var tmp = val[i];
                        val[i] = val[dest];
                        val[dest--] = tmp;
                    }

                    return Unsafe.Read<T>(valPointer);
                }
            }
        }
    }
}
