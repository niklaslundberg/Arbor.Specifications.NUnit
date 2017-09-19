using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Arbor.Specifications.NUnit.Specs
{
    public class when_using_base_class : CustomSpecificationBase
    {
        static Stack<string> call_stack = new Stack<string>();

        private Given setup_has_been_run = async () =>
        {
            call_stack.Push(nameof(setup_has_been_run));
            await Task.Delay(20);
        };

        private When running_when_with_custom_base_class = async () =>
        {
            call_stack.Push(nameof(running_when_with_custom_base_class));
            await Task.Delay(20);
        };

        private Then it_should_invoke_then = async () =>
        {
            Assert.AreEqual(call_stack.Pop(), nameof(running_when_with_custom_base_class));
            Assert.AreEqual(call_stack.Pop(), nameof(setup_has_been_run));
            Console.WriteLine("Should invoke console writeline");
        };
    }
}
