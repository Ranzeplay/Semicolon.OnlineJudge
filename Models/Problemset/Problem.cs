using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Semicolon.OnlineJudge.Models.Problemset
{
    public class Problem
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string AuthorId { get; set; }

        public string Description { get; set; }

        public DateTime PublishTime { get; set; }

        public string ExampleData { get; set; }

        public string JudgeProfile { get; set; }

        public string PassRate { get; set; }

        public TestData GetExampleData()
        {
            if (!string.IsNullOrWhiteSpace(ExampleData))
            {
                return System.Text.Json.JsonSerializer.Deserialize<TestData>(ExampleData);
            }

            return new TestData();
        }

        public void SetExampleData(TestData exampleData)
        {
            ExampleData = System.Text.Json.JsonSerializer.Serialize(exampleData);
        }


        public JudgeProfile GetJudgeProfile()
        {
            if (!string.IsNullOrWhiteSpace(JudgeProfile))
            {
                return System.Text.Json.JsonSerializer.Deserialize<JudgeProfile>(JudgeProfile);
            }

            return new JudgeProfile();
        }

        public void SetJudgeProfile(JudgeProfile judgeProfile)
        {
            JudgeProfile = System.Text.Json.JsonSerializer.Serialize(judgeProfile);
        }

        public PassRate GetPassRate()
        {
            if (!string.IsNullOrWhiteSpace(PassRate))
            {
                return System.Text.Json.JsonSerializer.Deserialize<PassRate>(PassRate);
            }

            return new PassRate();
        }

        public void SetPassRate(PassRate passRate)
        {
            PassRate = System.Text.Json.JsonSerializer.Serialize(passRate);
        }
    }
}
