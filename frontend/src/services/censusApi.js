import { BaseApiService } from './BaseApiService';

/**
 * Census Population API Service
 * Extends BaseApiService with census-specific methods
 */
class CensusApiService extends BaseApiService {
  constructor() {
    super('/v1/CensusPopulation');
  }

  /**
   * Get census population record by year
   * @param {number} year - Census year
   * @returns {Promise} Census population record
   */
  async getByYear(year) {
    return this.request(`/year/${year}`);
  }

  /**
   * Get census population records by year range
   * @param {number} startYear - Start year
   * @param {number} endYear - End year
   * @returns {Promise} List of census population records
   */
  async getByRange(startYear, endYear) {
    return this.request(`/range?startYear=${startYear}&endYear=${endYear}`);
  }
}

export default new CensusApiService();
