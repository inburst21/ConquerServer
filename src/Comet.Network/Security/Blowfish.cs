// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Network - Blowfish.cs
// Description:
// 
// Creator: FELIPEVIEIRAVENDRAMI [FELIPE VIEIRA VENDRAMINI]
// 
// Developed by:
// Felipe Vieira Vendramini <felipevendramini@live.com>
// 
// Programming today is a race between software engineers striving to build bigger and better
// idiot-proof programs, and the Universe trying to produce bigger and better idiots.
// So far, the Universe is winning.
// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#region References

using System;
using System.Runtime.InteropServices;

#endregion

namespace Comet.Network.Security
{
    public sealed unsafe class Blowfish : ICipher
    {
        // Global Configuration Variable Declarations:
        public static byte[] InitialKey;

        // Global & Local-Scope Constant Declarations:
        private const int BF_ROUNDS = 16;
        public const int BF_BLOCK_SIZE = BF_ROUNDS / 2;
        private const int BF_P_SIZE = BF_ROUNDS + 2;
        private const int BF_KEY_SIZE = BF_P_SIZE * 4;
        private const int BF_S_SIZE = BF_ROUNDS * BF_ROUNDS;
        private const int BF_S_AMOUNT = 4;

        [DllImport("libeay32.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void BF_set_key(IntPtr _key, int len, byte[] data);

        [DllImport("libeay32.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void BF_cfb64_encrypt(byte[] in_, byte[] out_, int length, IntPtr schedule, byte[] ivec, ref int num, int enc);

        [DllImport("libeay32.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void BF_cfb64_encrypt(byte[] in_, byte* out_, int length, IntPtr schedule, byte[] ivec, ref int num, int enc);

        [StructLayout(LayoutKind.Sequential)]
        struct bf_key_st
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 18)]
            public UInt32[] P;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public UInt32[] S;
        }

        private IntPtr _key;
        private byte[] _encryptIv;
        private byte[] _decryptIv;
        private int _encryptNum;
        private int _decryptNum;

        public Blowfish()
        {
            _encryptIv = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            _decryptIv = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            bf_key_st key = new bf_key_st();
            key.P = new UInt32[16 + 2];
            key.S = new UInt32[4 * 256];
            _key = Marshal.AllocHGlobal(key.P.Length * sizeof(UInt32) + key.S.Length * sizeof(UInt32));
            Marshal.StructureToPtr(key, _key, false);
            _encryptNum = 0;
            _decryptNum = 0;
            GenerateKeys(new object[] {InitialKey});
        }

        ~Blowfish()
        {
            Marshal.FreeHGlobal(_key);
        }

        public void GenerateKeys(object[] seeds)
        {
            byte[] key = (byte[]) seeds[0];
            _encryptNum = 0;
            _decryptNum = 0;
            BF_set_key(_key, key.Length, key);
        }

        public bool SetDecryptionIV(byte[] iv)
        {
            Buffer.BlockCopy(iv, 0, _decryptIv, 0, 8);
            return true;
        }

        public bool SetEncryptionIV(byte[] iv)
        {
            Buffer.BlockCopy(iv, 0, _encryptIv, 0, 8);
            return true;
        }

        public void Decrypt(Span<byte> src, Span<byte> dst)
        {
            byte[] source = src.ToArray();
            byte[] destiny = new byte[src.Length];
            BF_cfb64_encrypt(source, destiny, source.Length, _key, _decryptIv, ref _decryptNum, 0);
            destiny.CopyTo(dst);
        }

        public void Encrypt(Span<byte> src, Span<byte> dst)
        {
            byte[] source = src.ToArray();
            byte[] destiny = new byte[src.Length];
            BF_cfb64_encrypt(source, destiny, source.Length, _key, _encryptIv, ref _encryptNum, 1);
            destiny.CopyTo(dst);
        }
    }
}