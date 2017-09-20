using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Arbor.Specifications.NUnit.Specs
{
    [Ignore("Self test")]
    public class single_assert_fails : ContextSpecification
    {
        public static Exception _exception = null;

        private When of = async () =>
        {
            await Task.Delay(20);
        };

        private Then the_assert_should_fail = async () => Assert.IsNotNull(_exception);
    }
}
