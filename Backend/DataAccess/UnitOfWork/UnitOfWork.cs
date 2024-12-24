using Backend.DataAccess.Repositories;

namespace Backend.DataAccess.UnitOfWork;

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
