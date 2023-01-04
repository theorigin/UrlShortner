using System;
using System.Collections.Generic;
using FluentAssertions;
using LiteDB;
using Moq;
using UrlShortner.DataAccess;
using Xunit;

namespace UrlShortner.DataAccessTests
{
    public class UrlRepositoryTests
    {
        private readonly Mock<ILiteDatabase> _mockLiteDatabase;
        private readonly Mock<ILiteCollection<BsonDocument>> _mockLiteCollection;

        public UrlRepositoryTests()
        {
            _mockLiteDatabase = new Mock<ILiteDatabase>();
            _mockLiteCollection = new Mock<ILiteCollection<BsonDocument>>();
            _mockLiteDatabase.Setup(x => x.GetCollection("Url", It.IsAny<BsonAutoId>())).Returns(_mockLiteCollection.Object);
        }

        [Fact]
        public void Constructor_Throws_ArgumentNullException()
        {
            Action action = () => new UrlRepository(null!);

            action.Should()
                .Throw<ArgumentNullException>()
                .WithParameterName("liteDatabase");
        }

        [Fact]
        public void GetById_Returns_Null_When_Not_Found()
        {
            var returnRecord = new BsonDocument();
            _mockLiteCollection.Setup(x => x.FindById(It.IsAny<BsonValue>())).Returns(returnRecord);

            new UrlRepository(_mockLiteDatabase.Object)
                .GetById("8RPzSuQ")
                .Should()
                .BeNull();
        }

        [Fact]
        public void GetById_Returns_Correct_Record()
        {
            const string id = "idToFind";
            var expectedValue = Guid.NewGuid().ToString();
            var returnRecord = new BsonDocument(new Dictionary<string, BsonValue> { { "value", expectedValue } });
            
            _mockLiteCollection.Setup(x => x.FindById(id)).Returns(returnRecord);
            
            new UrlRepository(_mockLiteDatabase.Object)
                .GetById(id)
                .Should()
                .Be(expectedValue);
        }

        [Fact]
        public void Save_Inserts_New_Record()
        {
            new UrlRepository(_mockLiteDatabase.Object)
                .Save("123", "https://testurl.com");

            // check that the correct params were passed
        }
    }
}