using NUnit.Framework;

namespace Arbor.Specifications.NUnit.Specs
{
    public class when_a_specification_is_uncategorized : ContextSpecification
    {
        private Then should_be_run_successfully = async () => Assert.IsTrue(true);
    }
}
