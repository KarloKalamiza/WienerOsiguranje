using Backend.Core.Interfaces.Repositories;

namespace Backend.Core.Interfaces.Services;

public interface IUnitOfWork
{
    IPartner Partners { get; }
    IPolicy Policies { get; }
}
