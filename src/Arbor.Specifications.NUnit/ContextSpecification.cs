using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NUnit.Framework;

namespace Arbor.Specifications.NUnit
{
    [DebuggerStepThrough]
    [TestFixture]
    public abstract class ContextSpecification
    {
        public delegate Task When();

        public delegate Task Cleanup();

        public delegate Task Given();

        public delegate Task Then();

        protected Exception Exception;

        public IEnumerator GetEnumerator()
        {
            return GetObservations().GetEnumerator();
        }

        [OneTimeSetUp]
        public async Task TestFixtureSetUpAsync()
        {
            InitializeContext();

            await InvokeGivenAsync();

            await InvokeWhenAsync();
        }

        private void InitializeContext()
        {
            Type t = GetType();
        }

        [OneTimeTearDown]
        public Task TestFixtureTearDownAsync()
        {
            return InvokeCleanupAsync();
        }

        private async Task<string> InvokeGivenAsync()
        {
            Type type = GetType();

            FieldInfo[] givenFields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic |
                                                     BindingFlags.FlattenHierarchy)
                .Where(fieldInfo => fieldInfo.FieldType == typeof(Given))
                .ToArray();

            if (givenFields.Length == 0)
            {
                return "";
            }

            if (givenFields.Length > 1)
            {
                throw new InvalidOperationException($"There are more than one field of type {nameof(Given)}");
            }

            FieldInfo givenField = givenFields.Single();
            if (givenField.GetValue(this) is Given givenDelegateAsync)
            {
                Exception = await Catch.ExceptionAsync(() => givenDelegateAsync.Invoke());
                return givenField.Name;
            }

            return "";
        }

        private async Task InvokeWhenAsync()
        {
            if (Exception != null)
            {
                return;
            }

            Type currentType = GetType();

            FieldInfo[] whenFields = currentType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic |
                                                           BindingFlags.FlattenHierarchy)
                .Where(fieldInfo => fieldInfo.FieldType == typeof(When))
                .ToArray();

            if (whenFields.Length > 1)
            {
                throw new InvalidOperationException($"There are more than one field of type {nameof(When)}");
            }

            if (whenFields.Length == 0)
            {
                return;
            }

            if (whenFields.Single().GetValue(this) is When whenDelegateAsync)
            {
                Exception = await Catch.ExceptionAsync(() => whenDelegateAsync.Invoke());
            }
        }

        private async Task InvokeCleanupAsync()
        {
            try
            {
                Type currentType = GetType();

                FieldInfo[] cleanupFields = currentType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic |
                                                                  BindingFlags.FlattenHierarchy)
                    .Where(fieldInfo => fieldInfo.FieldType == typeof(Cleanup))
                    .ToArray();

                if (cleanupFields.Length > 0)
                {
                    var cleanupExceptions = new List<Exception>(cleanupFields.Length);

                    foreach (Cleanup cleanupDelegateAsync in cleanupFields
                        .Select(field => field.GetValue(this) as Cleanup).Where(cleanup => cleanup != null))
                    {
                        try
                        {
                            await cleanupDelegateAsync.Invoke();
                        }
                        catch (Exception ex)
                        {
                            cleanupExceptions.Add(ex);
                        }
                    }

                    if (cleanupExceptions.Count > 0)
                    {
                        throw new AggregateException(cleanupExceptions);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async Task RunObservationsAsync()
        {
            IEnumerable<TestCaseData> thenDelegates = GetObservations().OfType<TestCaseData>();

            foreach (TestCaseData thenDelegate in thenDelegates)
            {
                if (thenDelegate.OriginalArguments.FirstOrDefault() is Then thenDelegateAsync)
                {
                    await ObservationsAsync(thenDelegateAsync);
                }
            }
        }

        public IEnumerable GetObservations()
        {
            Type currentType = GetType();

            string categoryName = "Uncategorized";
            string description = string.Empty;

            IEnumerable<CategoryAttribute> categoryAttributes =
                currentType.GetTypeInfo().GetCustomAttributes(typeof(CategoryAttribute), true)
                    .OfType<CategoryAttribute>()
                    .ToArray();

            IEnumerable<SubjectAttribute> subjectAttributes =
                currentType.GetTypeInfo().GetCustomAttributes(typeof(SubjectAttribute), true).OfType<SubjectAttribute>()
                    .ToArray();

            if (categoryAttributes.Any())
            {
                CategoryAttribute categoryAttribute = categoryAttributes.First();
                categoryName = categoryAttribute.Name;
            }

            if (subjectAttributes.Any())
            {
                //SubjectAttribute subjectAttribute = subjectAttributes.First();
                //description = subjectAttribute.Subject;
            }

            FieldInfo[] fieldInfos = currentType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic |
                                                           BindingFlags.FlattenHierarchy);

            FieldInfo[] itFieldInfos = fieldInfos.Where(field => field.FieldType == typeof(Then)).ToArray();

            foreach (FieldInfo thenFieldInfo in itFieldInfos)
            {
                var data = new TestCaseData(thenFieldInfo.GetValue(this));

                string caseDescription = (string.IsNullOrWhiteSpace(description) ? thenFieldInfo.Name : description)
                    .NormalizeTestName().EnsureStartsWithPrefix();
                data.SetDescription(caseDescription);
                data.SetName(thenFieldInfo.Name.NormalizeTestName().EnsureStartsWithPrefix() + " [" +
                             thenFieldInfo.Name + "]");
                data.SetCategory(categoryName);

                yield return data;
            }
        }

        [Test]
        [SpecificationSource(nameof(GetObservations))]
        public async Task ObservationsAsync([NotNull] Then thenDelegateAsync)
        {
            if (thenDelegateAsync == null)
            {
                throw new ArgumentNullException(nameof(thenDelegateAsync));
            }

            if (thenDelegateAsync == null)
            {
                throw new ArgumentNullException(nameof(thenDelegateAsync));
            }

            //if (Exception != null)
            //{
            //    throw Exception;
            //}

            try
            {
                await thenDelegateAsync();
            }
            catch (Exception ex)
            {
                Exception = ex;
                throw;
            }
        }
    }
}
