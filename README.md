# WordApprox

WordApprox is a Question answering bot. It finds the best matched answer from a set of questions and answer fed to the Algorithm.

## TSV (Tab Separated File)

A TSV file is required to feed to the algorithm that has question and answers pre-filled following the below mentioned **Rules**:

- Each line should have these columns (Question Answer)
- Multiple Question may have same answer
- Multiple Answers must not have same question
- Example:

```
Question	Answer
Hi	Hello.
How are you?	I am good.
What is your name?	WordApprox.
Hey!	Hello.
```

## Sample Program.cs: [Replace <Path_to_tsv_file> with actual TSV file path]

```
using System;
using System.Collections.Generic;
using System.Diagnostics;
using WordApprox_Core.Models;
using WordApprox_Core.Services.Core.Classifier;
using WordApprox_Core.Services.QuestionBase;
using static WordApprox_Core.Utilities.FAQ_Service_Utility;

namespace ImplWordApprox
{
    class Program
    {
        static void Main(string[] args)
        {
            HashSet<UnmappedQuestionAnswerModel> rawData = GetQuestionAnswerMap("<Path_to_tsv_file>");
            QuestionBaseModel qBase = new QuestionBaseModel("wordApx_TestBase");
            DamerauLevensteinClassifier classifier = new DamerauLevensteinClassifier();
            QuestionBaseService qBaseService = new QuestionBaseService(qBase, classifier, rawData);

            while (true)
            {
                Console.WriteLine("\n\nAsk me a question: ");
                string query = Console.ReadLine();
                Stopwatch stp1 = new Stopwatch();
                stp1.Start();
                var task = qBaseService.GetFAQAnswerAsync(query: query, _scoreThreshold: 0.5F, _top: 3);
                task.Wait();
                List<FAQAnswer> anss = task.Result;
                stp1.Stop();
                Console.WriteLine($"Time  (Async): {stp1.ElapsedMilliseconds} anss: {anss.Count}");

                stp1 = new Stopwatch();
                stp1.Start();
                List<FAQAnswer> anss2 = qBaseService.GetFAQAnswer(query: query, _scoreThreshold: 0.5F, _top: 3);
                stp1.Stop();
                Console.WriteLine($"Time (Non-Async): {stp1.ElapsedMilliseconds} anss: {anss2.Count}");

                if (anss.Count == 0)
                {
                    Console.WriteLine("Sorry don't know the answer (Async)!");
                }

                foreach (var ans in anss)
                {
                    Console.WriteLine("=============Async==============");
                    Console.WriteLine(ans);
                    Console.WriteLine("===========================");
                }

                Console.WriteLine("\n\n\n");

                if (anss2.Count == 0)
                {
                    Console.WriteLine("Sorry don't know the answer (Non-Async)!");
                }

                foreach (var ans in anss2)
                {
                    Console.WriteLine("=============NON-Async==============");
                    Console.WriteLine(ans);
                    Console.WriteLine("===========================");
                }
            }
        }
    }
}
```

---
