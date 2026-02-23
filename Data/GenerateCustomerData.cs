using System.Text;
using System.Text.Json;

var count = GetArg("--count", 300_000);

Console.WriteLine($"Generating {count:N0} records per source...");

await WriteFile("Data/output1.ndjson", Generate("OUTPUT1", count));
await WriteFile("Data/output2.ndjson", Generate("OUTPUT2", count));

Console.WriteLine("Done.");


// =====================
// Generator
// =====================

List<Record> Generate(string source, int amount)
{
    var rand = new Random();
    var list = new List<Record>(amount);

    for (int i = 0; i < amount; i++)
    {
        list.Add(new Record(
            Source: source,
            Id: Guid.NewGuid().ToString("N"),
            Name: RandomName(rand),
            Email: RandomEmail(rand),
            Country: RandomCountry(rand),
            LastModifiedUtc: DateTime.UtcNow
                .AddDays(-rand.Next(0, 30))
                .AddMinutes(-rand.Next(0, 1440))
        ));
    }

    Shuffle(list, rand);
    return list;
}


// =====================
// File Writer
// =====================

async Task WriteFile(string path, List<Record> records)
{
    Console.WriteLine($"Writing {path}");

    await using var stream = File.Create(path);
    await using var writer = new StreamWriter(stream, Encoding.UTF8);

    foreach (var r in records)
    {
        var json = JsonSerializer.Serialize(r);
        await writer.WriteLineAsync(json);
    }
}


// =====================
// Helpers
// =====================

string RandomName(Random rand)
{
    var first = new[] { "Anna", "Lars", "Mikkel", "Sofia", "Jonas", "Emma", "Oliver", "Freja" };
    var last = new[] { "Jensen", "Nielsen", "Hansen", "Pedersen", "Andersen", "Madsen" };

    return $"{Pick(first, rand)} {Pick(last, rand)}";
}

string RandomEmail(Random rand)
{
    var domains = new[] { "example.com", "corp.local", "sales.org" };
    return $"user{rand.Next(1, 1_000_000)}@{Pick(domains, rand)}";
}

string RandomCountry(Random rand)
{
    var countries = new[]
    {
        "Denmark",
        "Sweden",
        "Norway",
        "Germany",
        "France",
        "United Kingdom",
        "United States",
        "Canada",
        "Netherlands",
        "Spain",
        "Italy",
        "Poland",
        "Australia",
        "Japan",
        "Brazil"
    };

    return Pick(countries, rand);
}

T Pick<T>(T[] items, Random rand) => items[rand.Next(items.Length)];

void Shuffle<T>(IList<T> list, Random rand)
{
    for (int i = list.Count - 1; i > 0; i--)
    {
        int j = rand.Next(i + 1);
        (list[i], list[j]) = (list[j], list[i]);
    }
}

int GetArg(string name, int defaultValue)
{
    var argsArr = Args.ToArray();
    var index = Array.IndexOf(argsArr, name);
    if (index >= 0 && index < argsArr.Length - 1)
        return int.Parse(argsArr[index + 1]);

    return defaultValue;
}


// =====================
// Model
// =====================

record Record(
    string Source,
    string Id,
    string Name,
    string Email,
    string Country,
    DateTime LastModifiedUtc
);