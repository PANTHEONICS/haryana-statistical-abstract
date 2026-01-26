import { BaseApiService } from '../BaseApiService';

/**
 * Table 3.2 Census Population API Service
 * Department: Area & Population
 * Extends BaseApiService with census-specific methods
 */
class Table3_2CensusPopulationApiService extends BaseApiService {
  constructor() {
    super('/v1/AP/Table3_2/CensusPopulation');
  }

  /**
   * Get census population record by year
   * @param {number} year - Census year
   * @returns {Promise} Census population record
   */
  async getByYear(year) {
    return this.request(`/${year}`);
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

export default new Table3_2CensusPopulationApiService();
