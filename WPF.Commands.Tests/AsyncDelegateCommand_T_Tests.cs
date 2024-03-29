﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WPF.Commands.Tests
{
    /// <summary>
    /// Tests for <see cref="AsyncDelegateCommand{T}"/>.
    /// </summary>
    [TestClass]
    public class AsyncDelegateCommand_T_Tests
    {
        // Positive tests
        [TestMethod]
        public void Execute_ExecutesFunction()
        {
            var funcMock = new Mock<Func<object, Task>>();

            var asyncDelegate = new AsyncDelegateCommand<object>(funcMock.Object);
            asyncDelegate.Execute(null);

            // Waiting for async execution to end
            Thread.Sleep(100);

            Assert.AreEqual(1, funcMock.Invocations.Count);
        }

        [TestMethod]
        public void Execute_CanExecuteFunction_NoOptions_ReturnsTrueAsCanExecute()
        {
            var funcMock = new Mock<Func<object, Task>>();
            var asyncDelegate = new AsyncDelegateCommand<object>(funcMock.Object, canExecute: _ => true);

            Assert.IsTrue(asyncDelegate.CanExecute());
        }

        [TestMethod]
        public void Execute_CanExecuteFunction_NoOptions_ReturnsFalseAsCanExecute()
        {
            var funcMock = new Mock<Func<object, Task>>();
            var asyncDelegate = new AsyncDelegateCommand<object>(funcMock.Object, canExecute: _ => false);

            Assert.IsFalse(asyncDelegate.CanExecute(null));
        }

        [TestMethod]
        public void Execute_ExecuteOnce_ReturnsTrueBeforeExecution()
        {
            var funcMock = new Mock<Func<object, Task>>();
            var asyncDelegate = new AsyncDelegateCommand<object>(funcMock.Object, executeOnce: true);

            Assert.IsTrue(asyncDelegate.CanExecute());
        }

        [TestMethod]
        public void Execute_ExecuteOnce_ReturnsFalseAfterExecution()
        {
            var funcMock = new Mock<Func<object, Task>>();
            var asyncDelegate = new AsyncDelegateCommand<object>(funcMock.Object, executeOnce: true);
            asyncDelegate.Execute();

            Assert.IsFalse(asyncDelegate.CanExecute());
        }

        // Negative tests
        // Async command does not return exceptions to the caller (UI) because of sync nature of ICommand
        [TestMethod]
        public void Execute_FunctionThrow_ExecutesFunction_ErrorSwallen()
        {
            var asyncDelegate = new AsyncDelegateCommand<object>(_ => throw new Exception());
            asyncDelegate.Execute();
        }

        [TestMethod]
        public void Execute_Execute_WrongParameterType_InvalidOperationException()
        {
            var funcMock = new Mock<Func<int, Task>>();
            var asyncDelegate = new AsyncDelegateCommand<int>(funcMock.Object,
                exceptionHandler: exception =>
            {
                Assert.IsInstanceOfType(exception, typeof(InvalidOperationException));
                return true;
            });

            asyncDelegate.Execute("test");
        }

        [TestMethod]
        public void Execute_Execute_WrongParameterType_InvalidOperationException_Global()
        {
            DelegateCommandBase.GlobalExceptionHandler = exception =>
            {
                Assert.IsInstanceOfType(exception, typeof(InvalidOperationException));
                return true;
            };

            var funcMock = new Mock<Func<int, Task>>();
            var asyncDelegate = new AsyncDelegateCommand<int>(funcMock.Object);
            asyncDelegate.Execute("test");
        }

        [TestMethod]
        public void Execute_IntParameter_NullParameterType_ErrorSwallen()
        {
            var funcMock = new Mock<Func<int, Task>>();
            var asyncDelegate = new AsyncDelegateCommand<int>(funcMock.Object);
            asyncDelegate.Execute(null);
        }

        [TestMethod]
        public async Task Execute_IntParameter_NullableParameterType_NoError()
        {
            var funcMock = new Mock<Func<int?, Task>>();
            var wasError = false;
            var asyncDelegate = new AsyncDelegateCommand<int?>(funcMock.Object, exceptionHandler: exception =>
            {
                wasError = true;
                return false;
            });
            asyncDelegate.Execute(null);

            Assert.IsFalse(wasError);
        }

        [TestMethod]
        public void Execute_ObjParameter_NullParameterType_NoError()
        {
            var funcMock = new Mock<Func<object, Task>>();
            var asyncDelegate = new AsyncDelegateCommand<object>(funcMock.Object);
            asyncDelegate.Execute(null);
        }

        // CanExecute is not async - so error is not swallen
        [TestMethod]
        public void Execute_CanExecuteThrow_ErrorNotSwallen()
        {
            var funcMock = new Mock<Func<object, Task>>();
            var asyncDelegate = new AsyncDelegateCommand<object>(funcMock.Object, canExecute: _ => throw new Exception());
            Assert.ThrowsException<Exception>(() => asyncDelegate.CanExecute());
        }
    }
}
