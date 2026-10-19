using System;
using System.Security.Cryptography;

namespace SIM.Sitecore9Installer.Security
{
  /// <summary>
  /// Generates Solr admin passwords that are safe to embed in
  /// https://user:pass@host/solr connection strings and SIF concat() command lines.
  /// Letters and digits only. Characters that look alike (0/O, 1/l/I) are left out.
  /// </summary>
  public static class SolrPasswordGenerator
  {
    private const string Upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
    private const string Lower = "abcdefghijkmnopqrstuvwxyz";
    private const string Digits = "23456789";
    private const string All = Upper + Lower + Digits;

    public static string Generate(int length = 8)
    {
      if (length < 3)
      {
        throw new ArgumentOutOfRangeException(nameof(length), "Length must be at least 3.");
      }

      var chars = new char[length];
      using (var rng = RandomNumberGenerator.Create())
      {
        // Guarantee at least one of each class
        chars[0] = Pick(rng, Upper);
        chars[1] = Pick(rng, Lower);
        chars[2] = Pick(rng, Digits);

        for (int i = 3; i < length; i++)
        {
          chars[i] = Pick(rng, All);
        }

        // Fisher-Yates shuffle so the guaranteed characters aren't always first
        for (int i = length - 1; i > 0; i--)
        {
          int j = NextInt(rng, i + 1);
          char tmp = chars[i];
          chars[i] = chars[j];
          chars[j] = tmp;
        }
      }

      return new string(chars);
    }

    private static char Pick(RandomNumberGenerator rng, string set)
    {
      return set[NextInt(rng, set.Length)];
    }

    /// <summary>
    /// Returns an unbiased random int in [0, maxExclusive).
    /// Values that would cause modulo bias are rejected and redrawn.
    /// (RandomNumberGenerator.GetInt32 isn't available on .NET Framework 4.8.)
    /// </summary>
    private static int NextInt(RandomNumberGenerator rng, int maxExclusive)
    {
      var buffer = new byte[4];
      uint max = (uint)maxExclusive;
      uint limit = uint.MaxValue - (uint.MaxValue % max);
      uint value;
      do
      {
        rng.GetBytes(buffer);
        value = BitConverter.ToUInt32(buffer, 0);
      } while (value >= limit);

      return (int)(value % max);
    }
  }
}
