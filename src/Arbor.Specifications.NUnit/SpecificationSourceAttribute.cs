using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace Arbor.Specifications.NUnit
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class SpecificationSourceAttribute : NUnitAttribute, ITestBuilder, IImplyFixture
    {
        private readonly NUnitTestCaseBuilder _builder = new NUnitTestCaseBuilder();

        public SpecificationSourceAttribute(string sourceName)
        {
            SourceName = sourceName;
        }

        public string SourceName { get; }

        public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test suite)
        {
            if (suite is null)
            {
                yield break;
            }

            Type type = suite.TypeInfo?.Type;

            string givenPrefix = "";
            string whenName = null;

            if (type != null)
            {
                FieldInfo[] givenFields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                    .Where(field => field.FieldType == typeof(ContextSpecification.Given)).ToArray();

                if (givenFields.Length == 1)
                {
                    givenPrefix = "Given " + givenFields.Single().Name.NormalizeTestName() + ", ";
                }

                FieldInfo[] whenFields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                    .Where(field => field.FieldType == typeof(ContextSpecification.When)).ToArray();

                if (whenFields.Length == 1)
                {
                    whenName = whenFields.Single().Name.EnsureStartsWithPrefix("when ");
                }
            }

            string[] classNameTokens = suite.FullName.Split('.', '+');
            string shortName = classNameTokens[classNameTokens.Length - 1];
            string testMemberName = (whenName ?? shortName).NormalizeTestName();
            suite.FullName = $"[{suite.Name}] {givenPrefix}{testMemberName}";

            foreach (TestCaseParameters testCaseParameters in GetTestCasesFor(method, suite, testMemberName))
            {
                TestMethod buildTestMethod = _builder.BuildTestMethod(method, suite, testCaseParameters);

                yield return buildTestMethod;
            }
        }

        private IEnumerable<TestCaseParameters> GetTestCasesFor(IMethodInfo method, Test suite, string testMemberName)
        {
            var list = new List<TestCaseParameters>();
            try
            {
                IEnumerable testCaseSource = GetTestCaseSource(method, suite, testMemberName);
                if (testCaseSource != null)
                {
                    foreach (object obj in testCaseSource)
                    {
                        var testCaseData = obj as TestCaseParameters;
                        list.Add(testCaseData);
                    }
                }
            }
            catch (Exception ex)
            {
                list.Clear();
                list.Add(new TestCaseParameters(ex));
            }
            return list;
        }

        private IEnumerable GetTestCaseSource(IMethodInfo method, Test suite, string testMemberName)
        {
            Type type = suite.TypeInfo.Type;
            object specification = Reflect.Construct(type);

            MemberInfo[] member = type.GetMember(SourceName,
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            if (member.Length == 1)
            {
                MemberInfo memberInfo = member[0];
                var methodInfo = memberInfo as MethodInfo;
                if (methodInfo != null)
                {
                    return (IEnumerable)methodInfo.Invoke(specification, null);
                }
            }

            return null;
        }
    }
}
