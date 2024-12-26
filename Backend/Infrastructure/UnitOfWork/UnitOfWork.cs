using Backend.Core.Interfaces.Repositories;
using Backend.Core.Interfaces.Services;

namespace Backend.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    public IPartner Partners { get; set; }
    public IPolicy Policies { get; set; }

    public UnitOfWork(IPartner Partners, IPolicy Policies)
    {
        this.Partners = Partners;
        this.Policies = Policies;
    }
}
