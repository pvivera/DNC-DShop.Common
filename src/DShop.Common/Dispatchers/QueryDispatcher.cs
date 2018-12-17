using System.Threading.Tasks;
using DShop.Common.Handlers;
using DShop.Common.Types;
using Microsoft.Extensions.DependencyInjection;

namespace DShop.Common.Dispatchers
{
    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IServiceScope _context;

        public QueryDispatcher(IServiceScope context)
        {
            _context = context;
        }

        public async Task<TResult> QueryAsync<TResult>(IQuery<TResult> query)
        {
            var handlerType = typeof(IQueryHandler<,>)
                .MakeGenericType(query.GetType(), typeof(TResult));

            dynamic handler = _context.ServiceProvider.GetService(handlerType);

            return await handler.HandleAsync((dynamic)query);
        }
    }
}