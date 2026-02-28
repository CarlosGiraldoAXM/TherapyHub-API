using Microsoft.EntityFrameworkCore;
using TherapuHubAPI.Models;
using TherapuHubAPI.Repositorio.IRepositorio;

namespace TherapuHubAPI.Repositorio;

public interface IUnitOfWork : IDisposable
{
    ITipoEventoRepositorio EventTypes { get; }
    IEventosRepositorio Events { get; }
    IEventRecurrenceRepositorio EventRecurrences { get; }
    IEventoUsuariosRepositorio EventoUsuarios { get; }
    ICompanyChatsRepositorio CompanyChats { get; }
    IChatMessagesRepositorio ChatMessages { get; }
    IMessageReadsRepositorio MessageReads { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly ContextDB _context;
    private Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction? _transaction;

    public ITipoEventoRepositorio EventTypes { get; private set; }
    public IEventosRepositorio Events { get; private set; }
    public IEventRecurrenceRepositorio EventRecurrences { get; private set; }
    public IEventoUsuariosRepositorio EventoUsuarios { get; private set; }
    public ICompanyChatsRepositorio CompanyChats { get; private set; }
    public IChatMessagesRepositorio ChatMessages { get; private set; }
    public IMessageReadsRepositorio MessageReads { get; private set; }

    public UnitOfWork(ContextDB context)
    {
        _context = context;
        EventTypes = new TipoEventoRepositorio(_context);
        Events = new EventosRepositorio(_context);
        EventRecurrences = new EventRecurrenceRepositorio(_context);
        EventoUsuarios = new EventoUsuariosRepositorio(_context);
        CompanyChats = new CompanyChatsRepositorio(_context);
        ChatMessages = new ChatMessagesRepositorio(_context);
        MessageReads = new MessageReadsRepositorio(_context);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
