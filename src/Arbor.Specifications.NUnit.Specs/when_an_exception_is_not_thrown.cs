using System;
using System.Text;
using NUnit.Framework;

namespace Arbor.Specifications.NUnit.Specs
{
    public class when_an_exception_is_not_thrown : ContextSpecification
    {
        private static Exception _exception;

        private When of = async () =>
            _exception = await Catch.ExceptionAsync(async () => new StringBuilder("nothing wrong here"));

        private Then should_catch_the_exception = async () => Assert.IsNull(_exception);
    }
}
