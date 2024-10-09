using System;

namespace WebsiteBanDoAnVaThucUong
{
    public interface IServiceCollection
    {
        object AddAuthentication(Action<object> value);
        void AddAuthorization(Action<object> value);
        void AddDbContext<T>(Func<object, object> value);
    }
}