using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace GhostBackupParser.DI;

public sealed class TypeRegistrar(IServiceCollection builder) : ITypeRegistrar
{
  public static TypeRegistrar Create()
  {
    var serviceCollection = new ServiceCollection();
    return new TypeRegistrar(serviceCollection);
  }

  [RequiresDynamicCode("IServiceCollection requires dynamic code :(")]
  public ITypeResolver Build()
  {
    return new TypeResolver(builder.BuildServiceProvider());
  }

  [RequiresDynamicCode("IServiceCollection requires dynamic code :(")]
  public void Register(
    Type service,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicFields)] Type implementation)
  {
    _ = builder.AddSingleton(service, implementation);
  }

  public void RegisterInstance(Type service, object implementation)
  {
    builder.AddSingleton(service, implementation);
  }

  public void RegisterLazy(Type service, Func<object> func)
  {
    ArgumentNullException.ThrowIfNull(func);
    builder.AddSingleton(service, (provider) => func());
  }
}
public sealed class TypeResolver(IServiceProvider? provider) : ITypeResolver, IDisposable
{
  private readonly IServiceProvider _provider = provider ?? throw new ArgumentNullException(nameof(provider));

  public object? Resolve(Type? type) => type is null 
      ? null 
      : _provider.GetService(type);

  public void Dispose()
  {
    if (_provider is IDisposable disposable)
    {
      disposable.Dispose();
    }
  }
}