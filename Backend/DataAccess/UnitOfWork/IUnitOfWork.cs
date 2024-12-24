using Backend.DataAccess.Repositories;

namespace Backend.DataAccess.UnitOfWork;

public interface IUnitOfWork
{
    IPartner Partners { get; }
    IPolicy Policies { get; }
}
