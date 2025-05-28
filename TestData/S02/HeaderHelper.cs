internal static class HeaderHelper
{
    internal static IEnumerable<AcceptLanguagePart>? ParseAcceptLanguage(string? headerValue)
    {
        if (string.IsNullOrWhiteSpace(headerValue))
        {
            return null;
        }

        var parts = headerValue.Split(',')
            .Select(x => x.Trim().Split(';'))
            .Select(x =>
            {
                if (x.Length != 2)
                {
                    return new AcceptLanguagePart(x.First());
                }
                var value = float.TryParse(x[1].Split('=').Last(), out var i) ? i : 0;
                return new AcceptLanguagePart(x.First(), value);
            })
            .ToList();

        if (!parts.Any())
        {
            return null;
        }
        return parts.AsEnumerable();
    }

    internal record AcceptLanguagePart(string Language, float Value = 1f);
}