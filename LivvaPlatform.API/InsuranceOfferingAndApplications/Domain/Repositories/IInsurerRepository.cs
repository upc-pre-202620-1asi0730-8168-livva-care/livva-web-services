using LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Model.Aggregates;
using LivvaPlatform.API.Shared.Domain.Repositories;

namespace LivvaPlatform.API.InsuranceOfferingAndApplications.Domain.Repositories;

/// <summary>
///     Repository contract for the Insurer aggregate.
/// </summary>
public interface IInsurerRepository : IBaseRepository<Insurer>;
