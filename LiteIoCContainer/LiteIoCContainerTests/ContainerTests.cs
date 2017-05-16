using LiteIoCContainer;
using LiteIoCContainerTests.TestObjects;
using System;
using Xunit;

namespace LiteIoCContainerTests
{
    public class ContainerTests
    {
        private readonly Container _container;

        public ContainerTests()
        {
            _container = new Container();
        }

        [Fact]
        public void Container_ResolveNonRegisteredType_InvalidOperationExceptionThrow()
        {
            Assert.Throws<InvalidOperationException>(() => _container.Resolve<SimpleObject>());
        }

        [Fact]
        public void Container_ResolveRegisteredConcreteType_RegisteredTypeReturned()
        {
            _container.Register<SimpleObject>(typeof(SimpleObject));

            var obj = _container.Resolve<SimpleObject>();

            Assert.Equal(typeof(SimpleObject), obj.GetType());
        }

        [Fact]
        public void Container_ResolveRegisteredAbstractType_RegisteredTypeReturned()
        {
            _container.Register<ISimple>(typeof(SimpleObject));

            var obj = _container.Resolve<ISimple>();

            Assert.Equal(typeof(SimpleObject), obj.GetType());
        }

        [Fact]
        public void Container_RegisterTypeTwice_InvalidOperationExceptionThrown()
        {
            _container.Register<SimpleObject>(typeof(SimpleObject));

            Assert.Throws<InvalidOperationException>(() => _container.Register<SimpleObject>(typeof(SimpleObject)));
        }

        [Fact]
        public void Container_RegisterInterfaceAsTheResolution_ArgumentExceptionThrown()
        {
            Assert.Throws<ArgumentException>(() => _container.Register<ISimple>(typeof(ISimple)));
        }

        [Fact]
        public void Container_RegisterAbstractClassAsTheResolution_ArgumentExceptionThrown()
        {
            Assert.Throws<ArgumentException>(() => _container.Register<ISimple>(typeof(SimpleAbstract)));
        }

        [Fact]
        public void Container_ResolveTypeTwice_DifferentInstancesOfType()
        {
            _container.Register<ISimple>(typeof(SimpleObject));

            var simple1 = _container.Resolve<ISimple>();
            var simple2 = _container.Resolve<ISimple>();

            Assert.NotEqual(simple1, simple2);
        }

        [Fact]
        public void Container_RegisterNonMatchingTypes_ArgumentExceptionThrow()
        {
            Assert.Throws<ArgumentException>(() => _container.Register<CompositeObject>(typeof(SimpleObject)));
        }

        [Fact]
        public void Container_RegisterTypeAsItsBaseClassesInterface_ResolveSuccessfully()
        {
            _container.Register<IBase>(typeof(SimpleObject));

            var simple = _container.Resolve<IBase>();

            Assert.Equal(typeof(SimpleObject), simple.GetType());
        }

        [Fact]
        public void Container_RegisterSameTypeWithAbstractAndConreteType_ResolveBothAsSameType()
        {
            _container.Register<SimpleObject>(typeof(SimpleObject));
            _container.Register<ISimple>((typeof(SimpleObject)));

            var simple1 = _container.Resolve<SimpleObject>();
            var simple2 = _container.Resolve<ISimple>();

            Assert.Equal(simple1.GetType(), simple2.GetType());
        }

        [Fact]
        public void Container_ResolveObjectThatConstructorCannotBeResolved_InvalidOperationExceptionThrown()
        {
            _container.Register<CompositeObject>(typeof(CompositeObject));

            Assert.Throws<InvalidOperationException>(() => _container.Resolve<CompositeObject>());
        }

        [Fact]
        public void Container_RegisterObjectWithConstructorArgs_ArgsNotNull()
        {
            _container.Register<ISimple>(typeof(SimpleObject));
            _container.Register<CompositeObject>(typeof(CompositeObject));

            var composite = _container.Resolve<CompositeObject>();

            Assert.NotNull(composite.Simple);
        }

        [Fact]
        public void Container_TypeWithMultipleConstructorsResolved_ResolvesOnlyViableOne()
        {
            _container.Register<MultiConstructorObject>(typeof(MultiConstructorObject));

            var multiConstructorObj = _container.Resolve<MultiConstructorObject>();

            Assert.True(multiConstructorObj.wasDefaultConstructorUsed);
        }

        [Fact]
        public void Container_TypeWithMultipleConstructorsResolved_ResolvesTheOneWithMoreArguments()
        {
            _container.Register<MultiConstructorObject>(typeof(MultiConstructorObject));
            _container.Register<ISimple>(typeof(SimpleObject));

            var multiConstructorObj = _container.Resolve<MultiConstructorObject>();

            Assert.False(multiConstructorObj.wasDefaultConstructorUsed);
        }

        [Fact]
        public void Container_ResolveRegisteredInstanceTwice_BothInstanceAreTheSameReference()
        {
            _container.RegisterInstance<ISimple>(new SimpleObject());

            var simple1 = _container.Resolve<ISimple>();
            var simple2 = _container.Resolve<ISimple>();

            Assert.True(ReferenceEquals(simple1, simple2));
        }
    }
}
