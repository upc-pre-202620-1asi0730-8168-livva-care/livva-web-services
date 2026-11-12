namespace LivvaPlatform.API.PolicyManagement.Interfaces.REST.Resources;

/// <summary>A policy document returned by the API.</summary>
public record PolicyDocumentResource(long Id, long PolicyId, string DocumentType, string Name, string? FileUrl, DateTimeOffset? CreatedAt, DateTimeOffset? UpdatedAt);
