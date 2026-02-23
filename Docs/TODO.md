# CustomerConsolidationFn Project To-Do List

## Goal
- Consolidate two NDJSON customer record files (Salesforce exports) from `/Data/`.
- Stream, filter, normalize, and output consolidated NDJSON and summary.

---

## To-Do Steps

### 1. Data Preparation
- [x] Ensure `GenerateCustomerData.cs` script is available in `/Data/`.
- [x] Run the script to generate `output1.ndjson` and `output2.ndjson`.
- [x] Verify files are present in `/Data/`.

### 2. Domain Models
- [ ] Create `CustomerRecord` model in `/Domain/`.
- [ ] Create `NormalizedCustomerRecord` model in `/Domain/`.
- [ ] Implement static country mapping dictionary in `/Domain/`.

### 3. Service Logic
- [ ] Implement streaming NDJSON reader in `/Services/`.
- [ ] Filter records by `lastModifiedUtc >= cutoffDate`.
- [ ] Normalize country name to ISO2 code, preserve original name.
- [ ] Handle unmapped countries (use "??", count occurrences).
- [ ] Write consolidated NDJSON output to `/Data/consolidated.ndjson`.
- [ ] Track and summarize records read/written and unmapped count.

### 4. Azure Function Entrypoint
- [ ] Implement HTTP-triggered function in `/Functions/ConsolidateFunction.cs`.
- [ ] Parse `cutoffDate` from request.
- [ ] Call service logic and return summary in HTTP response.

### 5. Project Structure & Clean-up
- [x] Ensure files are organized per assignment structure.
- [x] Exclude data generator script from main project compilation.
- [ ] Document unmapped country handling in code comments.

### 6. Final Review
- [ ] Test function with generated data.
- [ ] Review code for clarity, maintainability, and determinism.
- [ ] Prepare for interviewer walkthrough.

---

**Note:** No tests or cloud resources required. All file IO is local and streaming.
