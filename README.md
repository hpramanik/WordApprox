# WordApprox

WordApprox is a frequently asked question answering algorithm. It finds the best matched answer from a set of questions and answer fed to the Algorithm.

## TSV (Tab Separated File)

A TSV file is required to feed to the algorithm that has question and answers pre-filled following the below mentioned **Rules**:

- Columns (Question Answer Source MetaInfo)
- Source and MetaInfo are optional fields
- MetaInfo: Specifies the column that is required for filtration or answers
- Multiple Question may have same answer
- Multiple Answers must not have same question

- Example TSV file:

```
Question	Answer	Source	MetaInfo
Hiya	Hi!	wordApx_Test	wordApx:chitchat
Good morning	Hi!	wordApx_Test	wordApx:chitchat
Hi	Hi!	wordApx_Test	wordApx:chitchat
Hello	Hi!	wordApx_Test	wordApx:chitchat
Heya	Hi!	wordApx_Test	wordApx:chitchat
Hi there!	Hi!	wordApx_Test	wordApx:chitchat
Good evening	Evening!	wordApx_Test	wordApx:chitchat
What's your age?	Still at development phrase..	wordApx_Test	wordApx:chitchat
Are you young?	Still at development phrase..	wordApx_Test	wordApx:chitchat
When were you born?	Still at development phrase..	wordApx_Test	wordApx:chitchat
What age are you?	Still at development phrase..	wordApx_Test	wordApx:chitchat
Are you old?	Still at development phrase..	wordApx_Test	wordApx:chitchat
How old are you?	Still at development phrase..	wordApx_Test	wordApx:chitchat
How long ago were you born?	Still at development phrase..	wordApx_Test	wordApx:chitchat
Ask me anything	I'm a much better answerer than asker.	wordApx_Test	wordApx:chitchat
Ask me a question	I'm a much better answerer than asker.	wordApx_Test	wordApx:chitchat
Can you ask me a question?	I'm a much better answerer than asker.	wordApx_Test	wordApx:chitchat
Ask me something	I'm a much better answerer than asker.	wordApx_Test	wordApx:chitchat
What do you want to know about me?	I'm a much better answerer than asker.	wordApx_Test	wordApx:chitchat

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
                var task = qBaseService.GetFAQAnswerAsync(query: query, _scoreThreshold: 0.5F, _top: 3, metaInfo: "wordApx:chitchat");
                task.Wait();
                List<FAQAnswer> anss = task.Result;
                stp1.Stop();
                Console.WriteLine($"Time  (Async): {stp1.ElapsedMilliseconds} anss: {anss.Count}");

                stp1 = new Stopwatch();
                stp1.Start();
                List<FAQAnswer> anss2 = qBaseService.GetFAQAnswer(query: query, _scoreThreshold: 0.5F, _top: 3, metaInfo: "wordApx:chitchat");
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
