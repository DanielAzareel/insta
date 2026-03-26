using System.Text.Json;
using InstaAudit.Api.Models;

namespace InstaAudit.Api.Services;

public class InstagramAnalyzerService : IInstagramAnalyzerService
{
    public async Task<AnalysisResult> AnalyzeAsync(Stream followersJsonStream, Stream followingJsonStream, CancellationToken cancellationToken)
    {
        using var followersDoc = await JsonDocument.ParseAsync(followersJsonStream, cancellationToken: cancellationToken);
        using var followingDoc = await JsonDocument.ParseAsync(followingJsonStream, cancellationToken: cancellationToken);

        var followers = ParseFollowers(followersDoc.RootElement);
        var following = ParseFollowing(followingDoc.RootElement);

        var notFollowingBack = following.Except(followers, StringComparer.OrdinalIgnoreCase).OrderBy(x => x).ToList();
        var fans = followers.Except(following, StringComparer.OrdinalIgnoreCase).OrderBy(x => x).ToList();
        var mutuals = followers.Intersect(following, StringComparer.OrdinalIgnoreCase).OrderBy(x => x).ToList();

        return new AnalysisResult
        {
            FollowersCount = followers.Count,
            FollowingCount = following.Count,
            NotFollowingBack = notFollowingBack,
            Fans = fans,
            Mutuals = mutuals
        };
    }

    private static HashSet<string> ParseFollowers(JsonElement root)
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (root.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in root.EnumerateArray())
            {
                AddUsername(result, item);
            }
        }
        else if (root.TryGetProperty("relationships_followers", out var followersElement) && followersElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in followersElement.EnumerateArray())
            {
                AddUsername(result, item);
            }
        }

        return NormalizeSet(result);
    }

    private static HashSet<string> ParseFollowing(JsonElement root)
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("relationships_following", out var followingElement) && followingElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in followingElement.EnumerateArray())
            {
                AddUsername(result, item);
            }
        }
        else if (root.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in root.EnumerateArray())
            {
                AddUsername(result, item);
            }
        }

        return NormalizeSet(result);
    }

    private static void AddUsername(HashSet<string> set, JsonElement item)
    {
        if (item.TryGetProperty("string_list_data", out var stringListData)
            && stringListData.ValueKind == JsonValueKind.Array
            && stringListData.GetArrayLength() > 0)
        {
            var first = stringListData[0];
            if (first.TryGetProperty("value", out var valueElement))
            {
                var username = valueElement.GetString();
                if (!string.IsNullOrWhiteSpace(username))
                {
                    set.Add(username);
                    return;
                }
            }
        }

        if (item.TryGetProperty("title", out var titleElement))
        {
            var title = titleElement.GetString();
            if (!string.IsNullOrWhiteSpace(title))
            {
                set.Add(title);
            }
        }
    }

    private static HashSet<string> NormalizeSet(HashSet<string> usernames)
    {
        return usernames
            .Select(x => x.Trim().ToLowerInvariant())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }
}
