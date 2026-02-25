# Efficient Large Dataset Processing Cheat Sheet

## Concepts: Big O Notation (O Notation)

O notation (Big O notation) is a way to describe how the running time or memory use of an algorithm grows as the size of the input increases.

- **O(1):** Constant time—doesn’t get slower as input grows.
- **O(n):** Linear time—gets slower in direct proportion to input size.
- **O(n²):** Quadratic time—gets much slower as input grows (like nested loops).

It helps you compare how efficient different algorithms are, especially for large inputs.

---

## Concepts: Multithreading, Parallel Processing, Streaming, Async, I/O vs CPU-bound

**Multithreading**
- Running multiple threads in the same program to do several things at once.
- Useful for tasks that can be split up and run independently (e.g., processing many files at once).
- Not used here because we need to keep output order and the main work is I/O-bound, not CPU-bound.
- **Pseudocode:**
  ```csharp
  // Start multiple threads to process files in parallel
  foreach (var file in files)
  {
      StartNewThread(() => ProcessFile(file));
  }
  WaitForAllThreadsToFinish();
  ```

**Parallel Processing**
- Running multiple operations at the same time, often across multiple CPU cores.
- Good for speeding up CPU-heavy tasks (like calculations or image processing).
- Not needed here since our main bottleneck is reading/writing files, not heavy computation.
- **Pseudocode:**
  ```csharp
  // Parallel processing of a CPU-bound task
  Parallel.ForEach(items, item =>
  {
      Compute(item);
  });
  ```

**Streaming**
- Processing data one piece at a time (e.g., line-by-line), instead of loading everything into memory.
- Used here for reading input files efficiently, especially with large data.
- **Pseudocode:**
  ```csharp
  using (var reader = new StreamReader(file))
  {
      string? line;
      while ((line = reader.ReadLine()) != null)
      {
          Process(line);
      }
  }
  ```

**Async (Asynchronous Programming)**
- Allows a program to start a task (like reading a file) and do other work while waiting for it to finish.
- Great for I/O-bound tasks to keep programs responsive.
- Used here to avoid blocking while reading/writing files.
- **Pseudocode:**
  ```csharp
  // Asynchronously read and process lines
  using (var reader = new StreamReader(file))
  {
      string? line;
      while ((line = await reader.ReadLineAsync()) != null)
      {
          await ProcessAsync(line);
      }
  }
  ```

**I/O-bound vs CPU-bound**
- **I/O-bound:** The program spends most time waiting for input/output (like disk or network).
- **CPU-bound:** The program spends most time doing calculations.
- This project is I/O-bound (reading/writing files), so async and streaming are more useful than multithreading or parallel processing.

---

## 1. External Sorting (Chunked Sort and Merge)

**When to use:**
- Input files are not sorted and the dataset is too large to fit in memory.

**Benefits:**
- Scales to very large datasets (beyond available RAM).
- Deterministic, sorted output regardless of input order.
- Memory efficient: only a chunk and a few records per temp file are in memory at any time.
- Robust: works for any unsorted input, no assumptions needed.

**How it works:**
1. **Chunking Phase:**
   - Read a chunk of records that fits in memory.
   - Sort the chunk in memory.
   - Write the sorted chunk to a temporary file.
   - Repeat until all input is processed, producing multiple sorted temp files.

2. **Merge Phase:**
   - Open all sorted temp files.
   - Use a min-heap (priority queue) to merge the smallest record from each file.
   - Write merged, sorted records to the final output file.
   - Delete temp files after merging.

**High-level C# pseudocode:**
```csharp
// Chunking phase
while (!endOfInput)
{
    var chunk = ReadNextChunk(); // fits in memory
    chunk.Sort();
    WriteChunkToTempFile(chunk);
}

// Merge phase
Open all temp files;
Initialize min-heap with first record from each file;
while (heap not empty)
{
    Pop smallest record, write to output;
    Read next record from that file, add to heap;
}
Delete temp files;
```

---

## 2. Streaming Merge (If Input Files Are Sorted)

**When to use:**
- Both input files are already sorted by the key (e.g., `Id`).

**Benefits:**
- Highly memory efficient: only one record from each input file is in memory at a time.
- Fast: no need to sort; just merge, which is O(n) time.
- Simple implementation if input is already sorted.
- Deterministic output: always sorted and reproducible.

**How it works:**
- Open both files and read the first record from each.
- Compare the current records from each file.
- Write the smaller record to the output and advance that file's reader.
- Repeat until both files are fully processed.

**High-level C# pseudocode:**
```csharp
using (var reader1 = new StreamReader(file1))
using (var reader2 = new StreamReader(file2))
using (var writer = new StreamWriter(outputFile))
{
    var rec1 = ReadNext(reader1);
    var rec2 = ReadNext(reader2);
    while (rec1 != null || rec2 != null)
    {
        if (rec2 == null || (rec1 != null && rec1.Id.CompareTo(rec2.Id) <= 0))
        {
            writer.WriteLine(JsonSerialize(rec1));
            rec1 = ReadNext(reader1);
        }
        else
        {
            writer.WriteLine(JsonSerialize(rec2));
            rec2 = ReadNext(reader2);
        }
    }
}
```

---

**Summary:**
- Use external sorting for unsorted, very large datasets.
- Use streaming merge for already sorted input—fastest and most memory efficient.
- Both approaches keep memory usage low and scale to large data volumes.
