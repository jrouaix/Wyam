﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Wyam.Common.Documents;
using Wyam.Testing;
using Wyam.Testing.Documents;
using Wyam.Testing.Execution;

namespace Wyam.Handlebars.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.Self | ParallelScope.Children)]
    public class HandlebarsFixture : BaseFixture
    {
        public class ExecuteTests : HandlebarsFixture
        {
            [Test]
            public void RendersHandlebars()
            {
                // Given
                var data = new { /* Wrong case expected working */ title = "My new post", body = "This is my first post!" };

                string input =
@"<div class=""entry"">
  <h1>{{metadata.data.Title}}</h1>
  <div class=""body"">
    {{metadata.data.body}}
  </div>
</div>";

                string output =
@"<div class=""entry"">
  <h1>My new post</h1>
  <div class=""body"">
    This is my first post!
  </div>
</div>";

                TestExecutionContext context = new TestExecutionContext();
                TestDocument document = new TestDocument(input, new Dictionary<string, object> { { "data", data } });
                var handlebars = new Handlebars();

                // When
                IList<IDocument> results = handlebars.Execute(new[] { document }, context).ToList();  // Make sure to materialize the result list

                // Then
                Assert.That(results.Select(x => x.Content), Is.EquivalentTo(new[] { output }));
            }

            [Test]
            public void RendersHandlebarsJsonHelper()
            {
                // Given
                var data = new { title = "My new post", body = "This is my first post!" };

                string input = @"{{json}}";

                string output =
@"{
  ""metadata"": {
    ""data"": {
      ""title"": ""My new post"",
      ""body"": ""This is my first post!""
    }
  }
}";

                TestExecutionContext context = new TestExecutionContext();
                TestDocument document = new TestDocument(input, new Dictionary<string, object> { { "data", data } });
                var handlebars = new Handlebars();

                // When
                IList<IDocument> results = handlebars.Execute(new[] { document }, context).ToList();  // Make sure to materialize the result list

                // Then
                Assert.That(results.Select(x => x.Content), Is.EquivalentTo(new[] { output }));
            }

            [Test]
            public void RendersHandlebarsJsonHelperWithErrors()
            {
                // Given
                var data = new { title = "This will show errors", Method = GetType().GetMethod(nameof(RendersHandlebarsJsonHelperWithErrors)) };

                string input = @"{{json metadata 'true'}}";

                string output =
@"{
  ""data"": {
    ""title"": ""This will show errors"",
    ""Method"": {}
  }
}
Serialisation errors :
- data.Method : Operation is not supported on this platform.";

                TestExecutionContext context = new TestExecutionContext();
                TestDocument document = new TestDocument(input, new Dictionary<string, object> { { "data", data } });
                var handlebars = new Handlebars();

                // When
                IList<IDocument> results = handlebars.Execute(new[] { document }, context).ToList();  // Make sure to materialize the result list

                // Then
                Assert.That(results.Select(x => x.Content), Is.EquivalentTo(new[] { output }));
            }

            [Test]
            public void RendersHandlebarsYamlHelper()
            {
                // Given
                var data = new { title = "My new post", body = "This is my first post!" };

                string input = @"{{yaml}}";

                string output = @"metadata:
  data:
    title: My new post
    body: This is my first post!
";

                TestExecutionContext context = new TestExecutionContext();
                TestDocument document = new TestDocument(input, new Dictionary<string, object> { { "data", data } });
                var handlebars = new Handlebars();

                // When
                IList<IDocument> results = handlebars.Execute(new[] { document }, context).ToList();  // Make sure to materialize the result list

                // Then
                Assert.That(results.Select(x => x.Content), Is.EquivalentTo(new[] { output }));
            }

            [Test]
            public void RendersHandlebarsYamlHelperWithErrors()
            {
                // Given
                var data = new { title = "This will show errors", Method = GetType().GetMethod(nameof(RendersHandlebarsJsonHelperWithErrors)) };

                string input = @"{{yaml metadata 'true'}}";

                string output =
@"data:
  title: This will show errors
  Method: {}

Serialisation errors :
- data.Method : Operation is not supported on this platform.";

                TestExecutionContext context = new TestExecutionContext();
                TestDocument document = new TestDocument(input, new Dictionary<string, object> { { "data", data } });
                var handlebars = new Handlebars();

                // When
                IList<IDocument> results = handlebars.Execute(new[] { document }, context).ToList();  // Make sure to materialize the result list

                // Then
                Assert.That(results.Select(x => x.Content), Is.EquivalentTo(new[] { output }));
            }
        }
    }
}