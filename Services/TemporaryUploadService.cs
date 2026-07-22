using System.Collections.Concurrent;

namespace GestaoAcordos.Services;

public sealed class TemporaryUploadService
{
    private readonly ConcurrentDictionary<Guid, TemporaryUpload> _uploads = new();
    private static readonly TimeSpan Lifetime = TimeSpan.FromHours(2);

    public Guid Store(string fileName, string contentType, byte[] content)
    {
        CleanupExpired();
        var token = Guid.NewGuid();
        _uploads[token] = new TemporaryUpload(
            token,
            Path.GetFileName(fileName),
            string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType,
            content,
            DateTimeOffset.UtcNow);
        return token;
    }

    public bool TryGet(Guid token, out TemporaryUpload upload)
    {
        CleanupExpired();
        return _uploads.TryGetValue(token, out upload!);
    }

    public bool TryTake(Guid token, out TemporaryUpload upload)
    {
        CleanupExpired();
        return _uploads.TryRemove(token, out upload!);
    }

    public void Remove(Guid token) => _uploads.TryRemove(token, out _);

    private void CleanupExpired()
    {
        var limit = DateTimeOffset.UtcNow - Lifetime;
        foreach (var item in _uploads)
        {
            if (item.Value.CreatedAt < limit)
                _uploads.TryRemove(item.Key, out _);
        }
    }
}

public sealed record TemporaryUpload(
    Guid Token,
    string FileName,
    string ContentType,
    byte[] Content,
    DateTimeOffset CreatedAt);
