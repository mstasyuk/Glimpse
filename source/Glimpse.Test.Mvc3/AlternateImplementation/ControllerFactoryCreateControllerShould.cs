﻿using System;
using Glimpse.Core.Extensibility;
using Glimpse.Core;
using Glimpse.Mvc3.AlternateImplementation;
using Glimpse.Test.Mvc3.Tester;
using Moq;
using Xunit;

namespace Glimpse.Test.Mvc3.AlternateImplementation
{
    public class ControllerFactoryCreateControllerShould:IDisposable
    {
        private ControllerFactoryCreateControllerTester tester;
        public ControllerFactoryCreateControllerTester Tester
        {
            get { return tester ?? (tester = ControllerFactoryCreateControllerTester.Create()); }
            set { tester = value; }
        }

        public void Dispose()
        {
            Tester = null;
        }

        [Fact]
        public void SetMethodToImplement()
        {
            var result = Tester.MethodToImplement;

            Assert.Equal("CreateController", result.Name);
        }

        [Fact]
        public void ThrowWithNullRuntimePolicyStrategy()
        {
            Assert.Throws<ArgumentNullException>(()=>new ControllerFactory.CreateController(null, new Mock<IMessageBroker>().Object));
        }

        [Fact]
        public void ThrowWithNullMessageBroker()
        {
            Assert.Throws<ArgumentNullException>(() => new ControllerFactory.CreateController(()=>RuntimePolicy.On, null));
        }

        [Fact]
        public void ProceedImmediatlyIfRuntimePolicyOff()
        {
            Tester.RuntimePolicyStrategy = ()=> RuntimePolicy.Off;

            Tester.NewImplementation(Tester.ContextMock.Object);

            Tester.ContextMock.Verify(c=>c.Proceed());
            Tester.MessageBrokerMock.Verify(mb => mb.Publish(It.IsAny<ControllerFactory.CreateController.Message>()), Times.Never());
        }

        [Fact]
        public void PublishMessageIfRuntimePolicyOn()
        {
            Tester.NewImplementation(Tester.ContextMock.Object);

            Tester.MessageBrokerMock.Verify(mb=>mb.Publish(It.IsAny<ControllerFactory.CreateController.Message>()));
        }
    }
}