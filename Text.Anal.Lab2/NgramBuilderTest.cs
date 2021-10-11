using System.Linq;
using FluentAssertions;
using Moq.AutoMock;
using TextAnalLab2;
using Xunit;

namespace Text.Anal.Lab2
{
    public class NgramBuilderTest
    {
        private AutoMocker _mocker = new();

        [Fact(DisplayName = "проверят правильность построения ngram для 4х")]
        public void TestNgram4()
        {
            // Arrange
            var service = _mocker.CreateInstance<NGramBuilder>();
            var data = new[] { "мама", "мыла", "раму", "долго", "долго" };
            // Act
            var result = service.BuildNGrams(data, 4);

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(2);
            result.Should().OnlyContain(x => x.Count() == 4);
        }

        [Fact(DisplayName = "проверят правильность построения ngram для 3х")]
        public void TestNgram3()
        {
            // Arrange
            var service = _mocker.CreateInstance<NGramBuilder>();
            var data = new[] { "мама", "мыла", "раму", "долго", "долго" };
            // Act
            var result = service.BuildNGrams(data, 3);

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(3);
            result.Should().OnlyContain(x => x.Count() == 3);
        }

        [Fact(DisplayName = "Проверяет полное построение gramm")]
        public void TestNgramBuilder()
        {
            // Arrange
            var service = _mocker.CreateInstance<NGramBuilder>();
            var data = new[] { "мама", "мыла", "раму", "долго", "долго" };
            // Act
            var result = Enumerable.Range(2, 3).AsParallel().Select(
                x => service.BuildNGrams(data, x)).OrderBy(x => x.First().Count()).ToArray();

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(3);
            result.First().Should().OnlyContain(x => x.Count() == 2);
            result.Last().Should().OnlyContain(x => x.Count() == 4);
        }
    }
}