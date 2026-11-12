using System.ComponentModel.DataAnnotations;

namespace LivvaPlatform.API.PolicyManagement.Interfaces.REST.Resources;

/// <summary>Data required to request the renewal of a policy. New renewals always start as <c>pending</c>.</summary>
public record CreateRenewalResource([Range(1, long.MaxValue, ErrorMessage = "RequiredPolicyId")] long PolicyId);
