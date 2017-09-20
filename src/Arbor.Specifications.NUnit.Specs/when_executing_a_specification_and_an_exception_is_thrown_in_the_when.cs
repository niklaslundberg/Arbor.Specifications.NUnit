using System;
using NUnit.Framework;

namespace Arbor.Specifications.NUnit.Specs
{
    [TestFixture]
    [Subject("Context Specification")]
    public class when_executing_a_specification_and_an_exception_is_thrown_in_the_when : ContextSpecification
    {
        private static SubjectUnderTest _subjectUnderTest;
        private static Exception _exception;
        private static bool _establishHasRun;

        private Given context = async () => _subjectUnderTest = new SubjectUnderTest();

        private When of = async () =>
            _exception = await Catch.ExceptionAsync(async () => await _subjectUnderTest.TestFixtureSetUpAsync());

        private Then should_call_the_establish = async () => Assert.IsTrue(_establishHasRun);

        private Then should_bubble_the_exception_from_the_when = async () =>
            Assert.AreEqual("when", _subjectUnderTest.CaughtException.Message);

        private class SubjectUnderTest : ContextSpecification
        {
            public Exception CaughtException => Exception;
            private Given context = async () => _establishHasRun = true;
            private When of = async () => throw new Exception("when");
        }
    }
}
