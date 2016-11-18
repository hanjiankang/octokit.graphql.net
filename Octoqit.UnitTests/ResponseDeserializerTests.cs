﻿using System;
using System.Linq;
using LinqToGraphQL;
using LinqToGraphQL.Builders;
using LinqToGraphQL.Deserializers;
using Xunit;

namespace Octoqit.UnitTests
{
    public class ResponseDeserializerTests
    {
        [Fact]
        public void RepositoryOwner_Repository_Query()
        {
            var query = new RootQuery()
                .RepositoryOwner("grokys")
                .Repository("VisualStudio")
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    Owner = x.Owner.Select(o => new
                    {
                        o.Login
                    }),
                    x.IsFork,
                    x.IsPrivate,
                });
            string data = @"{
  ""data"": {
    ""repositoryOwner"": {
      ""repository"": {
        ""id"": ""1234"",
        ""name"": ""LinqToGraphQL"",
        ""owner"": {
          ""login"": ""grokys""
        },
        ""isFork"": false,
        ""isPrivate"": false
      }
    }
  }
}";

            var operation = new QueryBuilder().Build(query.Expression);
            var expectedType = query.GetType().GetGenericArguments()[0];
            dynamic result = new ResponseDeserializer().Deserialize(operation, data);

            Assert.IsType(expectedType, result);
            Assert.Equal("1234", result.Id);
            Assert.Equal("LinqToGraphQL", result.Name);
            Assert.Equal("grokys", Enumerable.Single(result.Owner).Login);
            Assert.Equal(false, result.IsFork);
            Assert.Equal(false, result.IsPrivate);
        }

        [Fact]
        public void Viewer_Email()
        {
            var query = new RootQuery().Viewer.Select(x => new { x.Email });
            string data = @"{
  ""data"": {
    ""viewer"": {
      ""email"": ""grokys@gmail.com""
    }
  }
}";
            var operation = new QueryBuilder().Build(query.Expression);
            var expectedType = query.GetType().GetGenericArguments()[0];
            dynamic result = new ResponseDeserializer().Deserialize(operation, data);

            Assert.Equal("grokys@gmail.com", result.Email);
        }

        [Fact]
        public void Viewer_Login_Email()
        {
            var query = new RootQuery().Viewer.Select(x => new { x.Login, x.Email });
            string data = @"{
  ""data"": {
    ""viewer"": {
      ""login"": ""grokys"",
      ""email"": ""grokys@gmail.com""
    }
  }
}";
            var operation = new QueryBuilder().Build(query.Expression);
            var expectedType = query.GetType().GetGenericArguments()[0];
            dynamic result = new ResponseDeserializer().Deserialize(operation, data);

            Assert.Equal("grokys", result.Login);
            Assert.Equal("grokys@gmail.com", result.Email);
        }
    }
}