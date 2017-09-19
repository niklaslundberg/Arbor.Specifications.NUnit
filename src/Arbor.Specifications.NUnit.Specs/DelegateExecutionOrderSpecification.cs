using System.Collections;
using NUnit.Framework;

namespace Arbor.Specifications.NUnit.Specs
{
    [Subject("Context Specification")]
    public class DelegateExecutionOrderSpecification : ContextSpecification
    {
        private static readonly Stack _executionStack = new Stack();

        private Given an_async_given_delegate_exists = async () => _executionStack.Push("given");

        private When calling_when_async_delegate = async () => _executionStack.Push("when");

        private Then should_execute_the_when_delegate_successfully =
            async () => Assert.AreEqual(_executionStack.ToArray()[0], "when");

        private Then it_should_execute_the_given_delegate_successfully =
            async () => Assert.AreEqual(_executionStack.ToArray()[_executionStack.Count - 1], "given");

        private Then it_should_execute_the_then_delegate_successfully = async () => Assert.IsTrue(true);
    }
}
