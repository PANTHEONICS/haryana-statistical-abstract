/**
 * Mock Data: Table 3.2 - Growth of Population in Haryana (1901-2011 Census)
 * Source: Statistical Abstract of Haryana 2023-24
 * 
 * JavaScript data for frontend/API usage
 */

export const censusPopulationData = [
  {
    year: 1901,
    total_population: 4623064,
    variation_in_population: null,
    decennial_percentage_increase: null,
    male_population: 2476390,
    female_population: 2146674,
    sex_ratio: 867,
    source_document: 'Statistical Abstract of Haryana 2023-24',
    source_table: 'Table 3.2',
    source_reference: 'Census of India, 2011, Administrative Atlas'
  },
  {
    year: 1911,
    total_population: 4174677,
    variation_in_population: -448387,
    decennial_percentage_increase: -9.70,
    male_population: 2274909,
    female_population: 1899768,
    sex_ratio: 835,
    source_document: 'Statistical Abstract of Haryana 2023-24',
    source_table: 'Table 3.2',
    source_reference: 'Census of India, 2011, Administrative Atlas'
  },
  {
    year: 1921,
    total_population: 4255892,
    variation_in_population: 81215,
    decennial_percentage_increase: 1.95,
    male_population: 2307985,
    female_population: 1947907,
    sex_ratio: 844,
    source_document: 'Statistical Abstract of Haryana 2023-24',
    source_table: 'Table 3.2',
    source_reference: 'Census of India, 2011, Administrative Atlas'
  },
  {
    year: 1931,
    total_population: 4559917,
    variation_in_population: 304025,
    decennial_percentage_increase: 7.14,
    male_population: 2473228,
    female_population: 2086689,
    sex_ratio: 844,
    source_document: 'Statistical Abstract of Haryana 2023-24',
    source_table: 'Table 3.2',
    source_reference: 'Census of India, 2011, Administrative Atlas'
  },
  {
    year: 1941,
    total_population: 5272829,
    variation_in_population: 712912,
    decennial_percentage_increase: 15.63,
    male_population: 2821783,
    female_population: 2451046,
    sex_ratio: 869,
    source_document: 'Statistical Abstract of Haryana 2023-24',
    source_table: 'Table 3.2',
    source_reference: 'Census of India, 2011, Administrative Atlas'
  },
  {
    year: 1951,
    total_population: 5673597,
    variation_in_population: 400768,
    decennial_percentage_increase: 7.60,
    male_population: 3031612,
    female_population: 2641985,
    sex_ratio: 871,
    source_document: 'Statistical Abstract of Haryana 2023-24',
    source_table: 'Table 3.2',
    source_reference: 'Census of India, 2011, Administrative Atlas'
  },
  {
    year: 1961,
    total_population: 7590524,
    variation_in_population: 1916927,
    decennial_percentage_increase: 33.79,
    male_population: 4062787,
    female_population: 3527737,
    sex_ratio: 868,
    source_document: 'Statistical Abstract of Haryana 2023-24',
    source_table: 'Table 3.2',
    source_reference: 'Census of India, 2011, Administrative Atlas'
  },
  {
    year: 1971,
    total_population: 10036431,
    variation_in_population: 2445907,
    decennial_percentage_increase: 32.22,
    male_population: 5377044,
    female_population: 4659387,
    sex_ratio: 867,
    source_document: 'Statistical Abstract of Haryana 2023-24',
    source_table: 'Table 3.2',
    source_reference: 'Census of India, 2011, Administrative Atlas'
  },
  {
    year: 1981,
    total_population: 12922119,
    variation_in_population: 2885688,
    decennial_percentage_increase: 28.75,
    male_population: 6909679,
    female_population: 6012440,
    sex_ratio: 870,
    source_document: 'Statistical Abstract of Haryana 2023-24',
    source_table: 'Table 3.2',
    source_reference: 'Census of India, 2011, Administrative Atlas'
  },
  {
    year: 1991,
    total_population: 16463648,
    variation_in_population: 3541529,
    decennial_percentage_increase: 27.41,
    male_population: 8827474,
    female_population: 7636174,
    sex_ratio: 865,
    source_document: 'Statistical Abstract of Haryana 2023-24',
    source_table: 'Table 3.2',
    source_reference: 'Census of India, 2011, Administrative Atlas'
  },
  {
    year: 2001,
    total_population: 21144564,
    variation_in_population: 4680916,
    decennial_percentage_increase: 28.43,
    male_population: 11363953,
    female_population: 9780611,
    sex_ratio: 861,
    source_document: 'Statistical Abstract of Haryana 2023-24',
    source_table: 'Table 3.2',
    source_reference: 'Census of India, 2011, Administrative Atlas'
  },
  {
    year: 2011,
    total_population: 25351462,
    variation_in_population: 4206898,
    decennial_percentage_increase: 19.90,
    male_population: 13494734,
    female_population: 11856728,
    sex_ratio: 879,
    source_document: 'Statistical Abstract of Haryana 2023-24',
    source_table: 'Table 3.2',
    source_reference: 'Census of India, 2011, Administrative Atlas'
  }
];

// Helper functions for data access
export const getCensusDataByYear = (year) => {
  return censusPopulationData.find(record => record.year === year);
};

export const getCensusDataRange = (startYear, endYear) => {
  return censusPopulationData.filter(record => record.year >= startYear && record.year <= endYear);
};

export const getAllYears = () => {
  return censusPopulationData.map(record => record.year);
};
