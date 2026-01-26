-- SQL INSERT statements for Table 3.2: Growth of Population in Haryana (1901-2011 Census)
-- Source: Statistical Abstract of Haryana 2023-24
-- Table: Census Population Data

INSERT INTO census_population (
    census_year,
    total_population,
    variation_in_population,
    decennial_percentage_increase,
    male_population,
    female_population,
    sex_ratio,
    source_document,
    source_table
) VALUES
    (1901, 4623064, NULL, NULL, 2476390, 2146674, 867, 'Statistical Abstract of Haryana 2023-24', 'Table 3.2'),
    (1911, 4174677, -448387, -9.70, 2274909, 1899768, 835, 'Statistical Abstract of Haryana 2023-24', 'Table 3.2'),
    (1921, 4255892, 81215, 1.95, 2307985, 1947907, 844, 'Statistical Abstract of Haryana 2023-24', 'Table 3.2'),
    (1931, 4559917, 304025, 7.14, 2473228, 2086689, 844, 'Statistical Abstract of Haryana 2023-24', 'Table 3.2'),
    (1941, 5272829, 712912, 15.63, 2821783, 2451046, 869, 'Statistical Abstract of Haryana 2023-24', 'Table 3.2'),
    (1951, 5673597, 400768, 7.60, 3031612, 2641985, 871, 'Statistical Abstract of Haryana 2023-24', 'Table 3.2'),
    (1961, 7590524, 1916927, 33.79, 4062787, 3527737, 868, 'Statistical Abstract of Haryana 2023-24', 'Table 3.2'),
    (1971, 10036431, 2445907, 32.22, 5377044, 4659387, 867, 'Statistical Abstract of Haryana 2023-24', 'Table 3.2'),
    (1981, 12922119, 2885688, 28.75, 6909679, 6012440, 870, 'Statistical Abstract of Haryana 2023-24', 'Table 3.2'),
    (1991, 16463648, 3541529, 27.41, 8827474, 7636174, 865, 'Statistical Abstract of Haryana 2023-24', 'Table 3.2'),
    (2001, 21144564, 4680916, 28.43, 11363953, 9780611, 861, 'Statistical Abstract of Haryana 2023-24', 'Table 3.2'),
    (2011, 25351462, 4206898, 19.90, 13494734, 11856728, 879, 'Statistical Abstract of Haryana 2023-24', 'Table 3.2');
