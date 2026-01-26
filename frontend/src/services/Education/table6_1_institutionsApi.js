import { BaseApiService } from '../BaseApiService';

/**
 * Table 6.1 Institutions API Service
 * Department: Education
 * Extends BaseApiService with institution-specific methods
 */
class Table6_1InstitutionsApiService extends BaseApiService {
  constructor() {
    super('/v1/Education/Table6_1/Institutions');
  }

  /**
   * Get institution record by type
   * @param {string} institutionType - Institution type name
   * @returns {Promise} Institution record
   */
  async getByType(institutionType) {
    return this.request(`/type/${encodeURIComponent(institutionType)}`);
  }
}

export default new Table6_1InstitutionsApiService();
