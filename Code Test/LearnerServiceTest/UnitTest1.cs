using Ncfe.CodeTest;

namespace LearnerServiceTest
{
    public class UnitTest1
    {
        [Fact]
        public void Test_GetLearnerFromArchived()
        {
            // Arrange
            LearnerService testClass = new LearnerService();

            // Act
            Learner learner = testClass.GetLearner(123, true);

            // Assert
            Assert.NotNull(learner);
            Assert.Equal(123, learner.Id);
            Assert.Equal("Archived Learner name", learner.Name);
        }

        [Fact]
        public void Test_GetLearner()
        {
            // Arrange
            LearnerService testClass = new LearnerService();

            // Act
            Learner learner = testClass.GetLearner(456, false);

            // Assert
            Assert.NotNull(learner);
            Assert.Equal(456, learner.Id);
            Assert.Equal("Regular Learner name", learner.Name);
        }
    }
}