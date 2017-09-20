using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NUnit.Framework;

namespace Arbor.Specifications.NUnit.Specs
{
    public class failing_assertions_in_failing_single_assert : ContextSpecification
    {
        private static SubjectUnderTest _subjectUnderTest;
        private static Exception _exception;

        private Given a_nested_subject = async () => _subjectUnderTest = new SubjectUnderTest();

        private When assertion_fails_in_two_asserts = async () =>
            _exception = await Catch.ExceptionAsync(async () =>
            {
                await _subjectUnderTest.TestFixtureSetUpAsync();
                await _subjectUnderTest.RunObservationsAsync();
            });

        private Then it_should_have_an_assertion_exception_being_caught = async () =>
        {
            Console.WriteLine(_exception);
            Assert.IsInstanceOf<AssertionException>(_exception);
        };

        [Ignore("nested")]
        private class SubjectUnderTest : ContextSpecification
        {
            public static Exception _exception = null;

            private When of = async () =>
            {
                await Task.Delay(20);
            };

            private Then the_assert_should_fail = async () => Assert.IsNotNull(_exception);
        }
    }
}
