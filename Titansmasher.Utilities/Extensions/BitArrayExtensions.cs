using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Titansmasher.Extensions
{
    public static class BitArrayExtensions
    {
        #region Privates

        private static TStruct ToStruct<TStruct>(BitArray bitArray, int maxSize)
        {
            if (bitArray.Length > 32)
                throw new ArgumentException($"Argument length shall be at most {32} bits.", nameof(bitArray));

            TStruct[] array = new TStruct[1];
            bitArray.CopyTo(array, 0);
            return array[0];
        }

        private static BitArray FromStruct<TStruct>(TStruct value)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, value);
                return new BitArray(ms.ToArray());
            }
        }

        #endregion Privates

        #region Publics

        #region ConvertTo

        public static short ToShort(this BitArray bitArray)
            => ToStruct<short>(bitArray, 16);

        public static ushort ToUShort(this BitArray bitArray)
            => ToStruct<ushort>(bitArray, 16);

        public static int ToInt(this BitArray bitArray)
            => ToStruct<int>(bitArray, 32);

        public static uint ToUint(this BitArray bitArray)
            => ToStruct<uint>(bitArray, 32);

        public static long ToLong(this BitArray bitArray)
            => ToStruct<long>(bitArray, 64);

        public static ulong ToUlong(this BitArray bitArray)
            => ToStruct<ulong>(bitArray, 64);

        #endregion ConvertTo

        #region ConvertFrom

        public static BitArray ToBitArray(this short value)
            => FromStruct(value);

        public static BitArray ToBitArray(this ushort value)
            => FromStruct(value);

        public static BitArray ToBitArray(this int value)
            => FromStruct(value);

        public static BitArray ToBitArray(this uint value)
            => FromStruct(value);

        public static BitArray ToBitArray(this long value)
            => FromStruct(value);

        public static BitArray ToBitArray(this ulong value)
            => FromStruct(value);

        #endregion ConvertFrom

        #endregion Publics
    }
}