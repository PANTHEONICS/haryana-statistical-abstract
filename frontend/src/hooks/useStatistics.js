import { useMemo } from 'react';

/**
 * Reusable Statistics Calculation Hook
 * Calculates statistics from data using provided calculation functions
 * 
 * @param {Array} data - Data array to calculate statistics from
 * @param {object} calculations - Object with calculation functions
 * @param {Array} dependencies - Additional dependencies for useMemo
 * @returns {object} - Calculated statistics object
 * 
 * @example
 * const stats = useStatistics(data, {
 *   totalRecords: (data) => data.length,
 *   latestValue: (data) => data.sort((a, b) => b.year - a.year)[0]?.value || 0,
 *   avgValue: (data) => data.reduce((sum, d) => sum + d.value, 0) / data.length
 * })
 */
export function useStatistics(data, calculations = {}, dependencies = []) {
  const stats = useMemo(() => {
    if (!data || !Array.isArray(data) || data.length === 0) {
      // Return default values for empty data
      const defaultStats = {};
      Object.keys(calculations).forEach(key => {
        defaultStats[key] = null;
      });
      return defaultStats;
    }

    const result = {};
    
    // Execute each calculation function
    Object.keys(calculations).forEach(key => {
      try {
        const calculation = calculations[key];
        if (typeof calculation === 'function') {
          result[key] = calculation(data);
        } else {
          result[key] = calculation;
        }
      } catch (error) {
        console.error(`Error calculating statistic "${key}":`, error);
        result[key] = null;
      }
    });

    return result;
  }, [data, ...dependencies]);

  return stats;
}

export default useStatistics;
