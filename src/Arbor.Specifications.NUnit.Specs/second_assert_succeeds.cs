using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Arbor.Specifications.NUnit.Specs
{
    [Ignore("Self test")]
    public class second_assert_succeeds : ContextSpecification
    {
        public static Exception _exception = null;

        private When of = async () =>
        {
            await Task.Delay(20);
        };

        private Then the_assert_1_should_fail = async () => Assert.IsNotNull(_exception);

        public static bool HasRunSecond { get; set; }

        private Then the_assert_2_should_succeed = async () =>
        {
            HasRunSecond = true;
        };
    }
}
