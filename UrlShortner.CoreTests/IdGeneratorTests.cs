using System.Linq;
using FluentAssertions;
using UrlShortner.Core;
using Xunit;

namespace UrlShortner.CoreTests
{
    public class IdGeneratorTests
    {
        [Fact]
        public void GenerateId_Returns_String_Length_Equal_To_7()
        {
            for (var i = 0; i < 100000; i++)
            {
                IdGenerator.GenerateId().Length.Should().Be(7);
            }
        }

        [Fact]
        public void GenerateId_Returns_String_That_Has_More_Than_2_Different_Chars()
        {
            for (var i = 0; i < 100000; i++)
            {
                var id = IdGenerator.GenerateId();
                var groups = id.ToCharArray().GroupBy(c => c);

                groups.Count().Should().BeGreaterThan(2);
            }
        }

        [Fact]
        public void GenerateId_Returns_String_With_Char_Not_Repeated_More_Than_Twice()
        {
            for (var i = 0; i < 100000; i++)
            {
                var id = IdGenerator.GenerateId();
                var groups = id.ToCharArray().GroupBy(c => c);

                groups.Count(g => g.Count() > 2).Should().Be(0);
            }
        }
    }
}