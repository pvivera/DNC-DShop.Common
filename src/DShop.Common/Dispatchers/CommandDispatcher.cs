using System.Threading.Tasks;
using DShop.Common.Handlers;
using DShop.Common.Messages;
using DShop.Common.RabbitMq;
using Microsoft.Extensions.DependencyInjection;

namespace DShop.Common.Dispatchers
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceScope _context;

        public CommandDispatcher(IServiceScope context)
        {
            _context = context;
        }

        public async Task SendAsync<T>(T command) where T : ICommand
            => await _context.ServiceProvider.GetService<ICommandHandler<T>>().HandleAsync(command, CorrelationContext.Empty);
    }
}