using Backend.DataAccess.Repositories;

namespace Backend.DataAccess.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    public IPartner Partners { get; set; }

    public UnitOfWork(IPartner Partners)
    {
        this.Partners = Partners;
    }
}
