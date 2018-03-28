using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;

namespace Stream
{
    class Streamer
    {
        public int BufferSize { get; set; } = 1024;
        public byte[] Hash { get; set; }

        public Streamer(int bufferSize)
        {
            this.BufferSize = bufferSize;
        }

        public void WriteBlock(string outputPath, byte[] block)
        {
            using (FileStream stream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
            {
                stream.Write(block, 0, block.Length);
            }
        }

        public void Encode(string sourcePath, string outputPath)
        {
            long bytesReaded = 0;
            long blockAmount = 0;

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            try
            {
                using (FileStream stream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read))
                {                   
                    while (stream.Length != bytesReaded)
                    {
                        int availibleBytesCount = Convert.ToInt32((stream.Length - bytesReaded > BufferSize) ? BufferSize : stream.Length - bytesReaded);
                        byte[] buffer = new byte[availibleBytesCount];
                        stream.Read(buffer, 0, availibleBytesCount);
                        bytesReaded += availibleBytesCount;

                        Console.WriteLine("Count bytes read: " + availibleBytesCount);

                        byte[] block = new byte[buffer.Length + (Hash == null ? 0 : Hash.Length)];
                        buffer.CopyTo(block, 0);
                        if (Hash != null)
                            Hash.CopyTo(block, buffer.Length);

                        Hash = SHA512.Create().ComputeHash(block);
                        WriteBlock(outputPath + blockAmount++ + ".sm", block);
                    }
                }
                Console.WriteLine("Encode complete! Amount of blocks: " + blockAmount);
            }
            catch (Exception e)
            {
                Console.WriteLine("Encode not completed =((( " + "Error: " + e.Message);
            }
        }

        public void Decode(string sourcePath, string outputPath)
        {
            try
            {
                string[] fileNames = Directory.GetFiles(sourcePath);
                if (fileNames.Length == 0)
                    return;
                using (FileStream stream = File.Create(outputPath))
                {
                    byte[] buffer = ReadBytes(fileNames[0]);
                    stream.Write(buffer, 0, buffer.Length);
                    int writedBytes = 0;
                    for (long i = 1; i < fileNames.Length; i++)
                    {
                        byte[] bufferHash = SHA512.Create().ComputeHash(buffer);
                        buffer = ReadBytes(fileNames[i]);
                        if (buffer.Length < 65)
                        {
                            throw new System.ArgumentException("Source is incorrect!");
                        }

                        byte[] hash = new byte[64];
                        Array.Copy(buffer, buffer.Length - 64, hash, 0, 64);
                        if (!bufferHash.SequenceEqual(hash))
                        {
                            throw new System.ArgumentException("Source is incorrect!");
                        }

                        byte[] streamPart = new byte[buffer.Length - 64];
                        Array.Copy(buffer, 0, streamPart, 0, buffer.Length - 64);
                        stream.Write(streamPart, 0, streamPart.Length);
                        writedBytes += streamPart.Length;
                    }
                }
                Console.WriteLine("Decode complete!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Decode not completed =((( " + "Error: " + e.Message);
            }
        }

        public byte[] ReadBytes(string path)
        {
            byte[] buffer;
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                buffer = new byte[stream.Length];
                stream.Read(buffer, 0, Convert.ToInt32(stream.Length));
            }
            return buffer;
        }
    }


}
