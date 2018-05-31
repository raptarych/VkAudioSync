﻿using System.IO;
using System.Linq;

namespace VkAudioSync
{
    public static class StringExtensions
    {
        public static string CleanFileName(this string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }
    }
}
