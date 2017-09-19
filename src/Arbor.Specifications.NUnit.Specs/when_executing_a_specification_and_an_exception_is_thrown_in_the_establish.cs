using System;
using NUnit.Framework;

namespace Arbor.Specifications.NUnit.Specs
{
    [Subject("Context Specification")]
    public class when_executing_a_specification_and_an_exception_is_thrown_in_the_establish : ContextSpecification
    {
        private static SubjectUnderTest _subjectUnderTest;
        private static Exception _exception;

        private Given context = async () => _subjectUnderTest = new SubjectUnderTest();

        private When of = async () =>
            _exception = await Catch.ExceptionAsync(async () => await _subjectUnderTest.TestFixtureSetUpAsync());

        private Then should_bubble_the_exception_from_the_establish = async () =>
            Assert.AreEqual("context", _subjectUnderTest?.CaughtException?.Message);

        private class SubjectUnderTest : ContextSpecification
        {
            public Exception CaughtException => Exception.InnerException;
            private Given context = () => throw new Exception("context");
            private When of = () => throw new Exception("because");
        }
    }
}
