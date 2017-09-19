using System;
using NUnit.Framework;

namespace Arbor.Specifications.NUnit.Specs
{
    public class when_catching_an_exception : ContextSpecification
    {
        private static Exception _exception;

        private When of = async () =>
            _exception = await Catch.ExceptionAsync(async () => { throw new Exception("oh nos!"); });

        private Then should_catch_the_exception = async () => Assert.IsNotNull(_exception);
    }
}
