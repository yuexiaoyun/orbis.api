using Orbis.Api.Extensions;

namespace Orbis.Api.Controllers
{
    public class UsersController : CrudController<User, UserT>
    {
        protected override void OnCreating(UserT dto)
        {
            dto.Password = dto.Password.Md5Hash();
        }
    }
}