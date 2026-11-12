
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Aggregates;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.Commands;
using LivvaPlatform.API.ClaimsAndIndemnities.Domain.Model.ValueObjects;

namespace LivvaPlatform.API.Tests.ClaimsAndIndemnities.Domain;

/// <summary>
///     Unit tests for Vehicle Claims and Life Indemnities domain aggregates.
/// </summary>
public class ClaimsAndIndemnitiesDomainTests
{
    private static CreateVehicleClaimCommand VehicleClaimCommand() =>
        new(
            1,
            10,
            new DateOnly(2026, 1, 15),
            " Av. Javier Prado, San Isidro ",
            "Vehicle accident on the main road.",
            5000);

    private static CreateIndemnityRequestCommand IndemnityCommand() =>
        new(
            1,
            20,
            new DateOnly(2026, 1, 15),
            "Life insurance indemnity request.",
            50000);

    [Fact]
    public void VehicleClaim_ShouldStartReportedWithInitialHistory()
    {
        var claim = new VehicleClaim(VehicleClaimCommand());

        Assert.Equal("Av. Javier Prado, San Isidro", claim.IncidentLocation);
        Assert.Equal(VehicleClaimStatus.REPORTED, claim.Status);
        Assert.Single(claim.StatusHistories);
        Assert.Equal(
            VehicleClaimStatus.REPORTED,
            claim.StatusHistories.First().Status);
        Assert.Null(claim.ResolvedAt);
    }

    [Fact]
    public void VehicleClaim_Cancel_ShouldCloseClaimAndRecordHistory()
    {
        var claim = new VehicleClaim(VehicleClaimCommand());

        claim.ChangeStatus(VehicleClaimStatus.CANCELLED);

        Assert.Equal(VehicleClaimStatus.CANCELLED, claim.Status);
        Assert.True(claim.IsClosed);
        Assert.NotNull(claim.ResolvedAt);
        Assert.Equal(2, claim.StatusHistories.Count);
        Assert.Throws<InvalidOperationException>(() => claim.ChangeStatus(VehicleClaimStatus.UNDER_REVIEW));
    }

    [Fact]
    public void VehicleClaim_CancelAfterApproval_ShouldThrow()
    {
        var claim = new VehicleClaim(VehicleClaimCommand());
        claim.ChangeStatus(VehicleClaimStatus.APPROVED, "Aprobado por la aseguradora.");

        Assert.Throws<InvalidOperationException>(() => claim.ChangeStatus(VehicleClaimStatus.CANCELLED));
    }

    [Fact]
    public void VehicleClaim_WithInvalidIdentifiers_ShouldThrow()
    {
        var command = VehicleClaimCommand() with { PolicyId = 0 };

        Assert.Throws<ArgumentException>(() => new VehicleClaim(command));
    }

    [Fact]
    public void VehicleClaim_WithFutureIncidentDate_ShouldThrow()
    {
        var command = VehicleClaimCommand() with
        {
            IncidentDate = DateOnly.MaxValue
        };

        Assert.Throws<ArgumentException>(() => new VehicleClaim(command));
    }

    [Fact]
    public void VehicleClaim_ChangeStatus_ShouldRecordHistory()
    {
        var claim = new VehicleClaim(VehicleClaimCommand());

        claim.ChangeStatus(
            VehicleClaimStatus.UNDER_REVIEW,
            "Documentation is being reviewed.");

        Assert.Equal(VehicleClaimStatus.UNDER_REVIEW, claim.Status);
        Assert.Equal(2, claim.StatusHistories.Count);
        Assert.Equal(
            "Documentation is being reviewed.",
            claim.StatusHistories.Last().Comment);
    }

    [Fact]
    public void VehicleClaim_WhenResolved_ShouldSetResolvedAt()
    {
        var claim = new VehicleClaim(VehicleClaimCommand());

        claim.ChangeStatus(
            VehicleClaimStatus.RESOLVED,
            "Claim successfully resolved.");

        Assert.Equal(VehicleClaimStatus.RESOLVED, claim.Status);
        Assert.NotNull(claim.ResolvedAt);
    }

    [Fact]
    public void VehicleClaim_WhenClosed_ShouldNotAllowAnotherStatusChange()
    {
        var claim = new VehicleClaim(VehicleClaimCommand());

        claim.ChangeStatus(
            VehicleClaimStatus.REJECTED,
            "Claim rejected.");

        Assert.Throws<InvalidOperationException>(() =>
            claim.ChangeStatus(
                VehicleClaimStatus.UNDER_REVIEW,
                "Try to reopen claim."));
    }

    [Fact]
    public void IndemnityRequest_ShouldStartSubmittedWithInitialHistory()
    {
        var request = new IndemnityRequest(IndemnityCommand());

        Assert.Equal(IndemnityRequestStatus.SUBMITTED, request.Status);
        Assert.Equal(1, request.PolicyId);
        Assert.Equal(20, request.BeneficiaryId);
        Assert.Single(request.StatusHistories);
        Assert.Null(request.ResolvedAt);
    }

    [Fact]
    public void IndemnityRequest_WithInvalidIdentifiers_ShouldThrow()
    {
        var command = IndemnityCommand() with { BeneficiaryId = 0 };

        Assert.Throws<ArgumentException>(() =>
            new IndemnityRequest(command));
    }

    [Fact]
    public void IndemnityRequest_WithFutureDate_ShouldThrow()
    {
        var command = IndemnityCommand() with
        {
            RequestDate = DateOnly.MaxValue
        };

        Assert.Throws<ArgumentException>(() =>
            new IndemnityRequest(command));
    }

    [Fact]
    public void IndemnityRequest_ChangeStatus_ShouldRecordHistory()
    {
        var request = new IndemnityRequest(IndemnityCommand());

        request.ChangeStatus(
            IndemnityRequestStatus.UNDER_REVIEW,
            "Documents are being evaluated.");

        Assert.Equal(
            IndemnityRequestStatus.UNDER_REVIEW,
            request.Status);

        Assert.Equal(2, request.StatusHistories.Count);
        Assert.Equal(
            "Documents are being evaluated.",
            request.StatusHistories.Last().Comment);
    }

    [Fact]
    public void IndemnityRequest_WhenPaid_ShouldSetResolvedAt()
    {
        var request = new IndemnityRequest(IndemnityCommand());

        request.ChangeStatus(
            IndemnityRequestStatus.PAID,
            "Indemnity payment completed.");

        Assert.Equal(IndemnityRequestStatus.PAID, request.Status);
        Assert.NotNull(request.ResolvedAt);
    }

    [Fact]
    public void IndemnityRequest_WhenClosed_ShouldNotAllowAnotherStatusChange()
    {
        var request = new IndemnityRequest(IndemnityCommand());

        request.ChangeStatus(
            IndemnityRequestStatus.REJECTED,
            "Request rejected.");

        Assert.Throws<InvalidOperationException>(() =>
            request.ChangeStatus(
                IndemnityRequestStatus.UNDER_REVIEW,
                "Try to reopen request."));
    }
}
