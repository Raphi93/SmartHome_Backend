using SmartHome_Backend_NoSQL.Service;

namespace WetterdatenTest
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {

            WeatherstationMongoDB sut = new WeatherstationMongoDB();
            double answer = 22f;
            var result = sut.CheckSunDur("24.2.2023");
            Assert.Equal(answer, result);
        }
    }
}