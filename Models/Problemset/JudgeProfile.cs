using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semicolon.OnlineJudge.Models.Problemset
{
    public class JudgeProfile
    {
        public string TestDatas { get; set; }

        public double MemoryLimit { get; set; }

        public double TimeLimit { get; set; }

        public List<TestData> GetTestDatas()
        {
            if (!string.IsNullOrWhiteSpace(TestDatas))
            {
                return System.Text.Json.JsonSerializer.Deserialize<List<TestData>>(TestDatas);
            }

            return new List<TestData>();
        }

        public void SetTestDatas(List<TestData> testDatas)
        {
            TestDatas = System.Text.Json.JsonSerializer.Serialize(testDatas);
        }
    }
}
