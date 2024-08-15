using AutoFixture.Kernel;

namespace Celebratix.Tests.SpecimenBuilders;

public abstract class AbstractSpecimenBuilder<T> : ISpecimenBuilder
{
    public object? Create(object request, ISpecimenContext context) =>
        request is Type type && type == typeof(T)
            ? CreateDefault(context)
            : new NoSpecimen();

    protected abstract T CreateDefault(ISpecimenContext context);
}