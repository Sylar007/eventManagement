using System.Text.RegularExpressions;
using Celebratix.Common.Exceptions;

namespace Celebratix.Common.Helpers;

public static class RandomStringsHelper
{
    public static string RandomString(int length, string set = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789")
    {
        var stringChars = new char[length];
        var random = new Random(Guid.NewGuid().GetHashCode());

        for (var i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = set[random.Next(set.Length)];
        }

        return new string(stringChars);
    }

    public const string SlugSet = "abcdefghijklmnopqrstuvwxyz0123456789-";
    public static string RandomSlug(int length)
    {
        return RandomString(length, "abcdefghjkmnpqrstuvwxyz23456789");
    }

    public static void VerifySlug(string slug)
    {
        if (slug.Length < 3)
            throw new SlugInvalidFormatException("Slug must be at least 3 characters long, was " + slug.Length);
        if (slug.Length > 50)
            throw new SlugInvalidFormatException("Slug must be at most 50 characters long, was " + slug.Length);
        if (Regex.IsMatch(slug, @"^\d+$")) // This is so that we can distinguish between a slug and an event id
            throw new SlugInvalidFormatException("Slug cannot be a number, it must contain at least one letter or dash");
        for (var i = 0; i < slug.Length; i++)
        {
            if (!SlugSet.Contains(slug[i]))
                throw new SlugInvalidFormatException("Slug contains invalid character '" + slug[i] + "'");
        }
    }
}
