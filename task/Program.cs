using System;
using System.Text;

interface ITextProcessor
{
    string ProcessText();
    int CountWords();
    bool ContainsWord(string word);
}

class Document : ITextProcessor
{
    private string title = "";
    private string author = "";
    private string text = "";
    private DateTime createdAt;

    public string Title
    {
        get { return title; }
    }

    public string Author
    {
        get { return author; }
    }

    public string Text
    {
        get { return text; }
    }

    public DateTime CreatedAt
    {
        get { return createdAt; }
    }

    public Document()
    {
        createdAt = DateTime.Now;
    }

    public Document(string title, string author, string text)
    {
        SetDocumentData(title, author, text);
    }

    protected void SetDocumentData(string title, string author, string text)
    {
        this.title = title;
        this.author = author;
        this.text = text;
        createdAt = DateTime.Now;
    }

    public virtual string GetInfo()
    {
        return $"Title: {Title}, Author: {Author}, Date: {CreatedAt}";
    }

    public string ProcessText()
    {
        return Text.Trim();
    }

    public int CountWords()
    {
        string[] words = GetWords(Text);
        int count = 0;

        for (int i = 0; i < words.Length; i++)
        {
            if (words[i] != "")
            {
                count++;
            }
        }

        return count;
    }

    public bool ContainsWord(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
        {
            return false;
        }

        string[] words = GetWords(Text);
        string searchWord = CleanWord(word).ToLower();

        for (int i = 0; i < words.Length; i++)
        {
            if (words[i].ToLower() == searchWord)
            {
                return true;
            }
        }

        return false;
    }

    public static string[] GetWords(string text)
    {
        char[] separators =
        {
            ' ', '\n', '\r', '\t',
            '.', ',', '!', '?', ':', ';',
            '"', '\'', '(', ')', '[', ']'
        };

        string[] parts = text.Split(separators);
        string[] words = new string[parts.Length];

        for (int i = 0; i < parts.Length; i++)
        {
            words[i] = CleanWord(parts[i]);
        }

        return words;
    }

    private static string CleanWord(string word)
    {
        char[] punctuation =
        {
            '.', ',', '!', '?', ':', ';',
            '"', '\'', '(', ')', '[', ']'
        };

        return word.Trim(punctuation);
    }
}

class Article : Document
{
    public string Category { get; private set; }
    public int ReadTime { get; private set; }

    public Article(
        string title,
        string author,
        string text,
        string category,
        int readTime)
    {
        SetDocumentData(title, author, text);
        Category = category;
        ReadTime = readTime;
    }

    public override string GetInfo()
    {
        return $"Article: {Title}, Author: {Author}, " +
               $"Category: {Category}, Read time: {ReadTime} min.";
    }
}

class Report : Document
{
    public string Department { get; private set; }
    public int PageCount { get; private set; }

    public Report(
        string title,
        string author,
        string text,
        string department,
        int pageCount)
    {
        SetDocumentData(title, author, text);
        Department = department;
        PageCount = pageCount;
    }

    public override string GetInfo()
    {
        return $"Report: {Title}, Author: {Author}, " +
               $"Department: {Department}, Pages: {PageCount}";
    }
}

record DocumentStatistics(
    int WordCount,
    int CharacterCount,
    string LongestWord,
    int UniqueWordCount);

record DocumentSummary(
    string Title,
    string Author,
    string DocumentType,
    int WordCount);

class ReportGenerator
{
    public DocumentStatistics Analyze(Document document)
    {
        string[] words = Document.GetWords(document.Text);

        int wordCount = 0;
        int uniqueWordCount = 0;
        string longestWord = "";

        for (int i = 0; i < words.Length; i++)
        {
            if (words[i] != "")
            {
                wordCount++;

                if (words[i].Length > longestWord.Length)
                {
                    longestWord = words[i];
                }

                bool alreadyFound = false;

                for (int j = 0; j < i; j++)
                {
                    if (words[j] != "" &&
                        words[j].ToLower() == words[i].ToLower())
                    {
                        alreadyFound = true;
                    }
                }

                if (!alreadyFound)
                {
                    uniqueWordCount++;
                }
            }
        }

        return new DocumentStatistics(
            wordCount,
            document.Text.Length,
            longestWord,
            uniqueWordCount);
    }

    public string Generate(Document[] documents, int documentCount)
    {
        StringBuilder builder = new StringBuilder();
        int totalWords = 0;
        int totalCharacters = 0;

        builder.AppendLine("DOCUMENT REPORT");
        builder.AppendLine("----------------");

        for (int i = 0; i < documentCount; i++)
        {
            Document document = documents[i];
            DocumentStatistics statistics = Analyze(document);

            builder.AppendLine(document.GetInfo());
            builder.AppendLine("Words: " + statistics.WordCount);
            builder.AppendLine("Characters: " + statistics.CharacterCount);
            builder.AppendLine("Longest word: " + statistics.LongestWord);
            builder.AppendLine("Unique words: " + statistics.UniqueWordCount);
            builder.AppendLine();

            totalWords += statistics.WordCount;
            totalCharacters += statistics.CharacterCount;
        }

        builder.AppendLine("Total documents: " + documentCount);
        builder.AppendLine("Total words: " + totalWords);
        builder.AppendLine("Total characters: " + totalCharacters);

        return builder.ToString();
    }
}

class Program
{
    private static Document[] documents = new Document[0];
    private static int documentCount = 0;

    static void Main()
    {
        bool running = true;

        for (; running;)
        {
            Console.WriteLine();
            Console.WriteLine("DOCUMENT PROCESSING SYSTEM");
            Console.WriteLine("1. Add article");
            Console.WriteLine("2. Add report");
            Console.WriteLine("3. Show all documents");
            Console.WriteLine("4. Search documents by word");
            Console.WriteLine("5. Show document statistics");
            Console.WriteLine("6. Generate general report");
            Console.WriteLine("7. Show DocumentSummary example");
            Console.WriteLine("0. Exit");
            Console.Write("Choose an action: ");

            string choice = Console.ReadLine() ?? "";

            if (choice == "1")
            {
                AddArticle();
            }
            else if (choice == "2")
            {
                AddReport();
            }
            else if (choice == "3")
            {
                ShowDocuments();
            }
            else if (choice == "4")
            {
                SearchWord();
            }
            else if (choice == "5")
            {
                ShowStatistics();
            }
            else if (choice == "6")
            {
                GenerateReport();
            }
            else if (choice == "7")
            {
                ShowSummaryExample();
            }
            else if (choice == "0")
            {
                running = false;
            }
            else
            {
                Console.WriteLine("Invalid menu option.");
            }
        }
    }

    private static string ReadText(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine() ?? "";
    }

    private static int ReadNumber(string prompt)
    {
        int number;

        for (;;)
        {
            Console.Write(prompt);

            if (int.TryParse(Console.ReadLine(), out number) && number >= 0)
            {
                return number;
            }

            Console.WriteLine("Enter a non-negative whole number.");
        }
    }

    private static void AddDocument(Document document)
    {
        Document[] newDocuments = new Document[documentCount + 1];

        for (int i = 0; i < documentCount; i++)
        {
            newDocuments[i] = documents[i];
        }

        newDocuments[documentCount] = document;
        documents = newDocuments;
        documentCount++;
    }

    private static void AddArticle()
    {
        string title = ReadText("Enter article title: ");
        string author = ReadText("Enter author: ");
        string category = ReadText("Enter category: ");
        int readTime = ReadNumber("Enter reading time in minutes: ");
        string text = ReadText("Enter text: ");

        Article article = new Article(
            title, author, text, category, readTime);

        AddDocument(article);
        Console.WriteLine("Document created successfully!");
    }

    private static void AddReport()
    {
        string title = ReadText("Enter report title: ");
        string author = ReadText("Enter author: ");
        string department = ReadText("Enter department: ");
        int pageCount = ReadNumber("Enter page count: ");
        string text = ReadText("Enter text: ");

        Report report = new Report(
            title, author, text, department, pageCount);

        AddDocument(report);
        Console.WriteLine("Document created successfully!");
    }

    private static void ShowDocuments()
    {
        if (documentCount == 0)
        {
            Console.WriteLine("There are no documents yet.");
            return;
        }

        for (int i = 0; i < documentCount; i++)
        {
            Console.WriteLine((i + 1) + ". " + documents[i].GetInfo());
        }
    }

    private static void SearchWord()
    {
        string word = ReadText("Enter a word to search: ");
        bool found = false;

        for (int i = 0; i < documentCount; i++)
        {
            if (documents[i].ContainsWord(word))
            {
                Console.WriteLine();
                Console.WriteLine("Document found:");
                Console.WriteLine(documents[i].Title);
                Console.WriteLine("Author: " + documents[i].Author);
                Console.WriteLine(
                    "Word count: " + documents[i].CountWords());

                found = true;
            }
        }

        if (!found)
        {
            Console.WriteLine("No documents containing that word were found.");
        }
    }

    private static void ShowStatistics()
    {
        if (documentCount == 0)
        {
            Console.WriteLine("There are no documents yet.");
            return;
        }

        ShowDocuments();
        int number = ReadNumber("Enter document number: ") - 1;

        if (number < 0 || number >= documentCount)
        {
            Console.WriteLine("Invalid document number.");
            return;
        }

        ReportGenerator generator = new ReportGenerator();
        DocumentStatistics statistics = generator.Analyze(documents[number]);

        Console.WriteLine("Words: " + statistics.WordCount);
        Console.WriteLine("Characters: " + statistics.CharacterCount);
        Console.WriteLine("Longest word: " + statistics.LongestWord);
        Console.WriteLine("Unique words: " + statistics.UniqueWordCount);
    }

    private static void GenerateReport()
    {
        ReportGenerator generator = new ReportGenerator();
        string report = generator.Generate(documents, documentCount);
        Console.WriteLine(report);
    }

    private static void ShowSummaryExample()
    {
        if (documentCount == 0)
        {
            Console.WriteLine("Add a document first.");
            return;
        }

        Document document = documents[0];
        string documentType = "Document";

        if (document is Article)
        {
            documentType = "Article";
        }
        else if (document is Report)
        {
            documentType = "Report";
        }

        DocumentSummary summary = new DocumentSummary(
            document.Title,
            document.Author,
            documentType,
            document.CountWords());

        DocumentSummary changedSummary =
            summary with { Author = "New author" };

        Console.WriteLine("Original: " + summary);
        Console.WriteLine("Copy: " + changedSummary);
    }
}