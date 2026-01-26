# Mock Data: Table 3.2 - Census Population Data

This directory contains mock data files for **Table 3.2: Growth of Population in Haryana (1901-2011 Census)** extracted from the Statistical Abstract of Haryana 2023-24.

## Files

| File | Format | Description |
|------|--------|-------------|
| `table_3.2_census_population.csv` | CSV | Comma-separated values format for spreadsheet import |
| `table_3.2_census_population.json` | JSON | JSON format for API/application consumption |
| `table_3.2_census_population.sql` | SQL | SQL INSERT statements for database seeding |
| `table_3.2_census_population.ts` | TypeScript | TypeScript module with type definitions and helper functions |
| `table_3.2_census_population.js` | JavaScript | JavaScript module for frontend usage |

## Data Structure

Each record contains the following fields:

- **year** (INTEGER): Census year (1901-2011, decennial)
- **total_population** (BIGINT): Total population count
- **variation_in_population** (INTEGER | NULL): Change from previous census
- **decennial_percentage_increase** (DECIMAL | NULL): Growth rate percentage
- **male_population** (BIGINT): Male population count
- **female_population** (BIGINT): Female population count
- **sex_ratio** (SMALLINT): Females per 1000 males

## Usage Examples

### CSV Import
```python
import pandas as pd
df = pd.read_csv('mock-data/table_3.2_census_population.csv')
```

### JSON API Response
```javascript
import data from './table_3.2_census_population.json';
// Use data array in your API or application
```

### SQL Database Seeding
```sql
-- Run the SQL file to insert data into your database
\i mock-data/table_3.2_census_population.sql
```

### TypeScript/JavaScript Import
```typescript
import { censusPopulationData, getCensusDataByYear } from './table_3.2_census_population';

// Get data for a specific year
const year2011 = getCensusDataByYear(2011);
console.log(year2011); // { year: 2011, total_population: 25351462, ... }
```

### JavaScript ES6 Import
```javascript
import { censusPopulationData, getCensusDataRange } from './table_3.2_census_population.js';

// Get data for a date range
const recentData = getCensusDataRange(2000, 2011);
console.log(recentData); // Array of records from 2001 and 2011
```

## Data Summary

- **Total Records:** 12 (1901-2011, decennial years)
- **Time Period:** 110 years
- **Population Range:** 4,174,677 (1911) to 25,351,462 (2011)
- **Source:** Statistical Abstract of Haryana 2023-24, Table 3.2
- **Original Source:** Census of India, 2011, Administrative Atlas

## Data Quality Notes

- First record (1901) has `NULL` values for variation and percentage increase (no previous data)
- All records validated: `total_population = male_population + female_population`
- Sex ratio values validated against calculated values
- Data is complete (no missing years in the 1901-2011 decennial series)

## Related Documentation

For detailed data architecture analysis, refer to:
- `../data-architecture/Table_3.2_Analysis.md` - Comprehensive architecture documentation
- `../data-architecture/Table_3.2_Executive_Summary.md` - Executive summary

---

**Note:** This is historical census data from the Statistical Abstract. For the most recent census data, refer to official Census of India publications.
