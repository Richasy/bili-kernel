// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.
// 算法来自：https://github.com/SocialSisterYi/bilibili-API-collect/blob/master/docs/misc/bvid_desc.md

using System.Numerics;

namespace Richasy.BiliKernel;

/// <summary>
/// AV / BV 号转换器.
/// </summary>
public static class AvBvConverter
{
    private static readonly BigInteger XOR_CODE = BigInteger.Parse("23442827791579");
    private static readonly BigInteger MASK_CODE = BigInteger.Parse("2251799813685247");
    private static readonly BigInteger MAX_AID = BigInteger.One << 51;
    private static readonly BigInteger BASE = 58;

    private const string DATA = "FcwAPNKTMug3GV5Lj7EJnHpWsx4tb8haYeviqBz6rkCy12mUSDQX9RdoZf";

    /// <summary>
    /// AV 号转换为 BV 号.
    /// </summary>
    /// <param name="aid"></param>
    /// <returns></returns>
    public static string Av2Bv(long aid)
    {
        var bytes = new char[] { 'B', 'V', '1', '0', '0', '0', '0', '0', '0', '0', '0', '0' };
        var bvIndex = bytes.Length - 1;
        var tmp = (MAX_AID | new BigInteger(aid)) ^ XOR_CODE;

        while (tmp > 0)
        {
            bytes[bvIndex] = DATA[(int)(tmp % BASE)];
            tmp /= BASE;
            bvIndex--;
        }

        // Swap positions
        (bytes[3], bytes[9]) = (bytes[9], bytes[3]);
        (bytes[4], bytes[7]) = (bytes[7], bytes[4]);

        return new string(bytes);
    }

    /// <summary>
    /// BV 号转换为 AV 号.
    /// </summary>
    /// <param name="bvid"></param>
    /// <returns></returns>
    public static long Bv2Av(string bvid)
    {
        var bvidArr = bvid.ToCharArray();

        // Swap positions back
        (bvidArr[3], bvidArr[9]) = (bvidArr[9], bvidArr[3]);
        (bvidArr[4], bvidArr[7]) = (bvidArr[7], bvidArr[4]);

        var remainingChars = new string([.. bvidArr.Skip(3)]);
        BigInteger tmp = 0;

        foreach (var c in remainingChars)
        {
            tmp = (tmp * BASE) + DATA.IndexOf(c);
        }

        return (long)((tmp & MASK_CODE) ^ XOR_CODE);
    }
}
