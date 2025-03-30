using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorDashboard.Models;

namespace BlazorDashboard.DataRetrieval
{
	public class IssuesGenerator
	{
		public async Task<IEnumerable<Issue>> GetIssues(DateTime sprintStartDate)
		{
			int sprintLengthDays = 14;
			int totalIssues = rand.Next(40, 51); // 40–50 issues

			List<Issue> issueList = new List<Issue>();
			int issueId = 0;

			int closeByDay7 = (int)(totalIssues * 0.45);
			int closeByDay14 = (int)(totalIssues * rand.Next(80, 91) / 100.0);
			int closeBetween8And14 = closeByDay14 - closeByDay7;

			// Create indices to enforce close schedule
			var allIndices = Enumerable.Range(0, totalIssues).ToList();
			var shuffled = allIndices.OrderBy(_ => rand.Next()).ToList();

			var toCloseByDay7 = new HashSet<int>(shuffled.Take(closeByDay7));
			var toCloseByDay14 = new HashSet<int>(shuffled.Skip(closeByDay7).Take(closeBetween8And14));

			for (int i = 0; i < totalIssues; i++)
			{
				Issue issue = new Issue();
				issue.Id = ++issueId;
				issue.Title = _dummyTitle.Substring(rand.Next(5, _dummyTitle.Length)) + issue.Id;

				// Create between day 0–4
				issue.CreatedOn = sprintStartDate.AddDays(rand.Next(0, 5));

				if (toCloseByDay7.Contains(i))
				{
					// Close between creation date and day 7
					int min = (issue.CreatedOn - sprintStartDate).Days;
					int closeDay = rand.Next(min, Math.Min(7, sprintLengthDays) + 1);
					issue.ClosedOn = sprintStartDate.AddDays(closeDay);
				}
				else if (toCloseByDay14.Contains(i))
				{
					// Close between day 8–14
					int min = Math.Max(8, (issue.CreatedOn - sprintStartDate).Days + 1);
					int closeDay = rand.Next(min, sprintLengthDays + 1);
					issue.ClosedOn = sprintStartDate.AddDays(closeDay);
				}
				// Else leave issue open

				int type = rand.Next(0, 3);
				issue.Type = (IssueType)type;
				issue.Labels = new List<string>
		{
			_issueTypes[type],
			"team " + rand.Next(1, 3),
			"priority " + rand.Next(1, 7),
			_componentList[rand.Next(0, _componentList.Length)],
			rand.Next(0, 6) == 0 ? "appearance" : "functionality",
			issue.IsOpen ? "open" : "closed"
		};

				if (issue.Type == IssueType.Bug)
				{
					int sev = rand.Next(0, 3);
					issue.Severity = (IssueSeverity)sev;
					issue.Labels.Add(_severities[sev]);
				}

				issue.Description = "<p><strong>Lorem ipsum</strong>... description omitted.</p>";

				issueList.Add(issue);
			}

			return issueList;
		}




		private static Random rand = new Random();
		private static string _dummyTitle = "Issue lorem ipsum dolor sit amet, consectetur adipiscing elit.";
		private static string[] _issueTypes = { "bug", "feature", "enhancement" };
		private static string[] _severities = { "low", "medium", "high" };
		private static string[] _componentList = { "grid", "button", "window", "chart", "textbox", "numeric textbox", "dropdownlist", "calendar" };
	}
}
