using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
                return JsonSerializer.Deserialize<TestData>(ExampleData);
            }

            return new TestData();
        }

        public void SetExampleData(TestData exampleData)
        {
            ExampleData = JsonSerializer.Serialize(exampleData);
        }


        public JudgeProfile GetJudgeProfile()
        {
            if (!string.IsNullOrWhiteSpace(JudgeProfile))
            {
                return JsonSerializer.Deserialize<JudgeProfile>(JudgeProfile);
            }

            return new JudgeProfile();
        }

        public void SetJudgeProfile(JudgeProfile judgeProfile)
        {
            JudgeProfile = JsonSerializer.Serialize(judgeProfile);
        }

        public PassRate GetPassRate()
        {
            if (!string.IsNullOrWhiteSpace(PassRate))
            {
                return JsonSerializer.Deserialize<PassRate>(PassRate);
            }

            return new PassRate();
        }

        public void SetPassRate(PassRate passRate)
        {
            PassRate = JsonSerializer.Serialize(passRate);
        }
    }
}
