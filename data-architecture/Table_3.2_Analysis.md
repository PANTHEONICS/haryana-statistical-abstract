# Data Architecture Analysis: Table 3.2
## Growth of Population in Haryana (1901-2011 Census)

**Document:** Statistical Abstract of Haryana 2023-24  
**Source:** Census of India, 2011, Administrative Atlas  
**Analysis Date:** January 2025  
**Data Architect:** Auto

---

## Executive Summary

Table 3.2 contains historical demographic data tracking population growth in Haryana state over 110 years (1901-2011). This time-series dataset requires careful architectural consideration for storage, querying, and analysis.

### Key Characteristics
- **Time Span:** 11 decennial census records (1901-2011)
- **Data Granularity:** State-level, decennial intervals
- **Dimensions:** Temporal (Year), Demographic (Population metrics)
- **Data Type:** Historical census data (reference data with analytical value)

---

## 1. Data Structure Analysis

### 1.1 Source Data Schema

| Column | Description | Data Type | Constraints | Notes |
|--------|-------------|-----------|-------------|-------|
| Year | Census year | INTEGER | PRIMARY KEY, CHECK (Year >= 1901 AND Year <= 2011), UNIQUE | Decennial years only |
| Population | Total population count | BIGINT | NOT NULL, CHECK (Population > 0) | Current format: Indian numbering (e.g., "46,23,064") |
| Variation_in_Population | Change from previous census | INTEGER | NULLABLE (first record) | Negative values indicate decrease |
| Decennial_Percentage_Increase | Growth rate percentage | DECIMAL(5,2) | NULLABLE (first record) | Negative values allowed |
| Male | Male population count | BIGINT | NOT NULL, CHECK (Male > 0) | Current format: Indian numbering |
| Female | Female population count | BIGINT | NOT NULL, CHECK (Female > 0) | Current format: Indian numbering |
| Sex_Ratio | Females per 1000 males | SMALLINT | NOT NULL, CHECK (Sex_Ratio >= 0) | Typically 800-1000 range |

### 1.2 Data Quality Observations

**Formatting Issues:**
- Numbers use Indian numbering system with commas (e.g., "46,23,064" = 4,623,064)
- Special characters in source () may indicate encoding issues
- Negative variation shown with "()" prefix

**Data Integrity Checks:**
- ✅ Population = Male + Female (must validate)
- ✅ Variation = Current_Population - Previous_Population
- ✅ Decennial_Percentage_Increase = (Variation / Previous_Population) * 100
- ✅ Sex_Ratio should approximate (Female / Male) * 1000

---

## 2. Data Model Design

### 2.1 Conceptual Data Model

```
┌─────────────────────────────────────────────────────────────┐
│                    Census_Population_Table                   │
├─────────────────────────────────────────────────────────────┤
│ PK: Year (INTEGER)                                          │
│                                                              │
│ Attributes:                                                  │
│   - Population (BIGINT)                                      │
│   - Variation_in_Population (INTEGER)                        │
│   - Decennial_Percentage_Increase (DECIMAL)                  │
│   - Male_Population (BIGINT)                                 │
│   - Female_Population (BIGINT)                               │
│   - Sex_Ratio (SMALLINT)                                     │
│                                                              │
│ Metadata:                                                    │
│   - Source (VARCHAR)                                         │
│   - Last_Updated (TIMESTAMP)                                 │
│   - Data_Quality_Score (DECIMAL)                             │
└─────────────────────────────────────────────────────────────┘
```

### 2.2 Relational Database Schema

#### Option 1: Normalized Schema (Recommended for Multiple Tables)

```sql
-- Main population table
CREATE TABLE census_population (
    census_year INTEGER PRIMARY KEY,
    total_population BIGINT NOT NULL CHECK (total_population > 0),
    variation_in_population INTEGER,
    decennial_percentage_increase DECIMAL(5,2),
    male_population BIGINT NOT NULL CHECK (male_population > 0),
    female_population BIGINT NOT NULL CHECK (female_population > 0),
    sex_ratio SMALLINT NOT NULL CHECK (sex_ratio >= 0),
    
    -- Data quality constraints
    CONSTRAINT chk_population_sum CHECK (total_population = male_population + female_population),
    CONSTRAINT chk_sex_ratio_accuracy CHECK (
        ABS(sex_ratio - ROUND((female_population::DECIMAL / male_population) * 1000)) <= 2
    ),
    
    -- Metadata
    source_document VARCHAR(255) DEFAULT 'Statistical Abstract of Haryana 2023-24',
    source_table VARCHAR(50) DEFAULT 'Table 3.2',
    last_updated TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Index for temporal queries
CREATE INDEX idx_census_year ON census_population(census_year);

-- Index for trend analysis
CREATE INDEX idx_population_trend ON census_population(census_year, total_population);
```

#### Option 2: Star Schema (For Analytics/Data Warehouse)

```sql
-- Dimension Table: Census Year
CREATE TABLE dim_census_year (
    census_year INTEGER PRIMARY KEY,
    decade VARCHAR(20), -- e.g., "1900s", "1910s"
    century VARCHAR(20), -- e.g., "20th Century"
    period_label VARCHAR(50), -- e.g., "Pre-Independence", "Post-Independence"
    is_milestone BOOLEAN DEFAULT FALSE
);

-- Fact Table: Population Metrics
CREATE TABLE fact_population_census (
    fact_id SERIAL PRIMARY KEY,
    census_year INTEGER REFERENCES dim_census_year(census_year),
    total_population BIGINT NOT NULL,
    male_population BIGINT NOT NULL,
    female_population BIGINT NOT NULL,
    sex_ratio SMALLINT NOT NULL,
    variation_from_previous INTEGER,
    percentage_change DECIMAL(5,2),
    
    -- Calculated metrics
    population_growth_rate DECIMAL(10,6),
    male_female_ratio DECIMAL(5,3),
    
    load_timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Indexes for OLAP queries
CREATE INDEX idx_fact_year ON fact_population_census(census_year);
CREATE INDEX idx_fact_population ON fact_population_census(total_population);
```

### 2.3 NoSQL Schema (Document Store - MongoDB Example)

```javascript
{
  "_id": ObjectId,
  "census_year": 2011,
  "population": {
    "total": 25351462,
    "male": 13494734,
    "female": 11856728
  },
  "demographics": {
    "sex_ratio": 879,
    "variation_from_previous": 4206898,
    "decennial_percentage_increase": 19.90
  },
  "metadata": {
    "source": "Statistical Abstract of Haryana 2023-24",
    "table": "Table 3.2",
    "source_reference": "Census of India, 2011, Administrative Atlas",
    "data_quality": {
      "validated": true,
      "validation_date": ISODate("2025-01-XX"),
      "checks_passed": ["population_sum", "sex_ratio_accuracy"]
    },
    "created_at": ISODate("2025-01-XX"),
    "updated_at": ISODate("2025-01-XX")
  },
  "derived_metrics": {
    "annual_growth_rate": 1.83,
    "growth_acceleration": -8.53, // Change in growth rate from previous decade
    "population_density": null // Would need area data
  }
}
```

---

## 3. Data Types and Storage Recommendations

### 3.1 Numeric Type Selection

| Field | Recommended Type | Rationale |
|-------|------------------|-----------|
| Year | INTEGER | Years are integers, no need for DATE |
| Population | BIGINT | Values exceed INTEGER range (max 2.5 billion) |
| Variation | INTEGER | Can be negative, values within INTEGER range |
| Percentage | DECIMAL(5,2) | Precise to 2 decimals, allows for -99.99 to 999.99 |
| Sex_Ratio | SMALLINT | Values typically 800-1000, SMALLINT sufficient |

### 3.2 Storage Considerations

- **Estimated Row Size:** ~80 bytes per record
- **Total Table Size:** ~11 rows × 80 bytes = 880 bytes (minimal)
- **With Indexes:** ~2-3 KB total
- **Storage Strategy:** No partitioning needed for 11 rows

---

## 4. Data Relationships and Dependencies

### 4.1 Temporal Relationship
- **Type:** Linear time series (strictly sequential)
- **Granularity:** Decennial (10-year intervals)
- **Missing Data:** None in source (complete series 1901-2011)

### 4.2 Calculated Fields Dependencies

```sql
-- Computed columns (can be materialized or calculated on-the-fly)
ALTER TABLE census_population ADD COLUMN population_growth_rate 
    GENERATED ALWAYS AS (
        CASE 
            WHEN LAG(total_population) OVER (ORDER BY census_year) IS NOT NULL
            THEN ROUND(
                ((total_population - LAG(total_population) OVER (ORDER BY census_year))::DECIMAL / 
                 LAG(total_population) OVER (ORDER BY census_year)) * 100, 
                2
            )
            ELSE NULL
        END
    ) STORED;
```

### 4.3 Referential Integrity

**Potential Foreign Key Relationships:**
- Links to administrative boundaries (state-level data)
- May link to other census tables (e.g., literacy, economic data) via year
- Source document reference (for data lineage)

---

## 5. Data Transformation Requirements

### 5.1 ETL Pipeline

**Extract:**
- Source: PDF document (Table_3.2.pdf)
- Format: Text extraction with table parsing
- Challenges: Indian numbering format, encoding issues

**Transform:**
```python
def transform_census_data(raw_data):
    """
    Transform extracted PDF data to normalized format
    """
    transformations = {
        'convert_indian_numbering': convert_to_integer,
        'calculate_derived_fields': calculate_variation_and_percentage,
        'validate_constraints': validate_data_quality,
        'normalize_encoding': fix_encoding_issues
    }
    return apply_transformations(raw_data, transformations)
```

**Load:**
- Target: Relational database (PostgreSQL/MySQL) or Data Warehouse
- Strategy: UPSERT (update if exists, insert if new)
- Frequency: One-time initial load, periodic updates as new data available

### 5.2 Data Cleaning Rules

1. **Number Format Conversion:**
   - Input: "46,23,064" (Indian numbering)
   - Output: 4623064 (Standard integer)

2. **Percentage Parsing:**
   - Handle negative percentages: "()9.70" → -9.70
   - Normalize to DECIMAL(5,2)

3. **Encoding Cleanup:**
   - Replace "" with appropriate Unicode characters
   - Standardize special characters

4. **Data Validation:**
   - Verify Population = Male + Female
   - Recalculate and validate Sex_Ratio
   - Validate temporal sequence (no gaps, no duplicates)

---

## 6. Query Patterns and Performance

### 6.1 Common Query Patterns

```sql
-- 1. Population trend over time
SELECT census_year, total_population 
FROM census_population 
ORDER BY census_year;

-- 2. Growth rate analysis
SELECT 
    census_year,
    decennial_percentage_increase,
    LAG(decennial_percentage_increase) OVER (ORDER BY census_year) as prev_growth
FROM census_population
WHERE decennial_percentage_increase IS NOT NULL;

-- 3. Sex ratio trend
SELECT census_year, sex_ratio 
FROM census_population 
ORDER BY census_year;

-- 4. Decade-wise comparison
SELECT 
    CASE 
        WHEN census_year < 1947 THEN 'Pre-Independence'
        WHEN census_year < 2000 THEN 'Post-Independence (20th Century)'
        ELSE '21st Century'
    END as period,
    AVG(decennial_percentage_increase) as avg_growth_rate,
    SUM(total_population) as total_population
FROM census_population
WHERE decennial_percentage_increase IS NOT NULL
GROUP BY period;

-- 5. Population projection (simple linear projection)
SELECT 
    census_year + 10 as projected_year,
    total_population * (1 + (decennial_percentage_increase / 100)) as projected_population
FROM census_population
WHERE census_year = 2011;
```

### 6.2 Performance Considerations

- **Table Size:** Extremely small (11 rows) - no performance concerns
- **Indexing Strategy:** Index on `census_year` for temporal queries
- **Query Optimization:** All queries will be sub-millisecond
- **Caching:** Data rarely changes - excellent candidate for application-level caching

---

## 7. Data Quality and Governance

### 7.1 Data Quality Metrics

| Metric | Target | Measurement |
|--------|--------|-------------|
| Completeness | 100% | All 11 decennial records present |
| Accuracy | >99% | Population sum = Male + Female |
| Consistency | 100% | Sex ratio matches calculated value (±2) |
| Validity | 100% | All years within valid range, all values > 0 |
| Timeliness | N/A | Historical data (static) |

### 7.2 Data Quality Rules

```sql
-- Data quality validation view
CREATE VIEW vw_data_quality_checks AS
SELECT 
    census_year,
    CASE 
        WHEN total_population = male_population + female_population 
        THEN 'PASS' ELSE 'FAIL' 
    END as population_sum_check,
    CASE 
        WHEN ABS(sex_ratio - ROUND((female_population::DECIMAL / male_population) * 1000)) <= 2 
        THEN 'PASS' ELSE 'FAIL' 
    END as sex_ratio_check,
    CASE 
        WHEN variation_in_population = 
            (total_population - LAG(total_population) OVER (ORDER BY census_year))
        THEN 'PASS' ELSE 'FAIL' 
    END as variation_check
FROM census_population;
```

### 7.3 Data Lineage

```
Source Document: Statistical Abstract of Haryana 2023-24
  └─> Table 3.2: Growth of population in Haryana: 1901-2011 Census
       └─> Original Source: Census of India, 2011, Administrative Atlas
            └─> Data Transformation: PDF Text Extraction → CSV → Database
                 └─> Target Tables: census_population, fact_population_census
```

---

## 8. Integration and API Design

### 8.1 REST API Endpoints

```
GET /api/v1/census/population
  - Query parameters: year (optional), start_year, end_year
  - Returns: List of population records

GET /api/v1/census/population/{year}
  - Returns: Single population record for specific year

GET /api/v1/census/population/trends
  - Query parameters: metric (population|growth|sex_ratio)
  - Returns: Trend data with calculated metrics

GET /api/v1/census/population/stats
  - Returns: Summary statistics (min, max, avg, median)
```

### 8.2 Data Export Formats

- **CSV:** For spreadsheet analysis
- **JSON:** For API consumption
- **Excel:** For business users
- **Parquet:** For analytical workloads

---

## 9. Security and Access Control

### 9.1 Data Classification

- **Sensitivity Level:** Public (census data is publicly available)
- **Confidentiality:** Low (aggregated state-level data, no PII)
- **Integrity:** Medium-High (statistical accuracy is important)
- **Availability:** Medium (reference data, not critical-path)

### 9.2 Access Control

```sql
-- Read-only access for analysts
CREATE ROLE census_reader;
GRANT SELECT ON census_population TO census_reader;

-- Full access for data administrators
CREATE ROLE census_admin;
GRANT SELECT, INSERT, UPDATE ON census_population TO census_admin;
```

---

## 10. Recommendations

### 10.1 Immediate Actions

1. ✅ **Data Extraction:** Complete PDF text extraction and validation
2. ✅ **Schema Design:** Implement normalized relational schema
3. ⚠️ **Data Cleaning:** Resolve encoding issues and number format conversion
4. ⚠️ **Validation:** Implement data quality checks before load

### 10.2 Short-term Enhancements

1. **Data Enrichment:**
   - Link to geographical boundaries (state GIS data)
   - Add period classifications (pre/post-independence)
   - Calculate additional derived metrics

2. **Data Quality Dashboard:**
   - Monitor data quality metrics
   - Track data lineage
   - Alert on anomalies

### 10.3 Long-term Considerations

1. **Data Expansion:**
   - Integrate with other census tables (literacy, economy, etc.)
   - Add district-level granularity if available
   - Project future population trends

2. **Analytical Capabilities:**
   - Build time-series forecasting models
   - Create comparative analysis with other states
   - Develop interactive dashboards

3. **Data Governance:**
   - Establish data dictionary
   - Document business rules
   - Create data quality SLA

---

## 11. Technical Specifications

### 11.1 Technology Stack Recommendations

| Layer | Technology | Rationale |
|-------|-----------|-----------|
| **Database** | PostgreSQL 14+ | Strong support for numeric types, constraints, window functions |
| **ETL** | Python (pandas, sqlalchemy) | Excellent PDF parsing, data transformation libraries |
| **API** | FastAPI / Flask | Lightweight, fast for small datasets |
| **Analytics** | Python (pandas, numpy) or R | Statistical analysis and visualization |

### 11.2 Implementation Checklist

- [ ] Create database schema
- [ ] Develop ETL script for PDF extraction
- [ ] Implement data transformation logic
- [ ] Set up data quality validation
- [ ] Load initial data
- [ ] Create indexes and views
- [ ] Develop API endpoints
- [ ] Write unit tests for data validation
- [ ] Document data dictionary
- [ ] Create data quality dashboard

---

## 12. Appendix

### 12.1 Sample Data (Normalized)

```csv
Year,Total_Population,Variation,Percentage_Increase,Male_Population,Female_Population,Sex_Ratio
1901,4623064,NULL,NULL,2476390,2146674,867
1911,4174677,-448387,-9.70,2274909,1899768,835
1921,4255892,81215,1.95,2307985,1947907,844
1931,4559917,304025,7.14,2473228,2086689,844
1941,5272829,712912,15.63,2821783,2451046,869
1951,5673597,400768,7.60,3031612,2641985,871
1961,7590524,1916927,33.79,4062787,3527737,868
1971,10036431,2445907,32.22,5377044,4659387,867
1981,12922119,2885688,28.75,6909679,6012440,870
1991,16463648,3541529,27.41,8827474,7636174,865
2001,21144564,4680916,28.43,11363953,9780611,861
2011,25351462,4206898,19.90,13494734,11856728,879
```

### 12.2 Key Insights from Data

1. **Population Growth:**
   - Lowest growth: 1911 (-9.70%) - likely due to migration or data issues
   - Highest growth: 1961 (33.79%) - post-partition recovery period
   - Recent trend: Declining growth rate (19.90% in 2011 vs 28.43% in 2001)

2. **Sex Ratio:**
   - Range: 835-879 (below 1000 indicates more males than females)
   - Lowest: 835 (1911), 844 (1921, 1931)
   - Recent improvement: 879 in 2011 (highest in series)

3. **Demographic Patterns:**
   - Consistent male majority throughout period
   - Post-independence period shows highest population growth

---

**Document Version:** 1.0  
**Last Updated:** January 2025  
**Next Review:** As needed when new census data becomes available
