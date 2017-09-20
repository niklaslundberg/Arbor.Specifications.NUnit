using System;
using NUnit.Framework;

namespace Arbor.Specifications.NUnit.Specs
{
    [Subject("Context Specification")]
    public class when_executing_a_specification_and_an_exception_is_thrown_in_the_given : ContextSpecification
    {
        private static SubjectUnderTest _subjectUnderTest;
        private static Exception _exception;

        private static bool whenInSubectHasRun;

        private Given context = async () => _subjectUnderTest = new SubjectUnderTest();

        private When of = async () =>
            _exception = await Catch.ExceptionAsync(async () => await _subjectUnderTest.TestFixtureSetUpAsync());

        private Then should_bubble_the_exception_from_the_when = async () =>
            Assert.AreEqual("given", _subjectUnderTest?.CaughtException?.Message);

        private Then it_should_not_have_run_when = async () =>
            Assert.IsFalse(whenInSubectHasRun);

        private class SubjectUnderTest : ContextSpecification
        {
            public Exception CaughtException => Exception;
            private Given context = () => throw new Exception("given");
            private When of = () =>
            {
                whenInSubectHasRun = true;
                throw new Exception("when");
            };
        }
    }
}
