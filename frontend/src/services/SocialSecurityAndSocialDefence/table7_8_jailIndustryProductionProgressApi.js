import { BaseApiService } from '../BaseApiService';

class Table7_8JailIndustryProductionProgressApiService extends BaseApiService {
  constructor() {
    super('/v1/SSD/Table7_8/JailIndustryProductionProgress');
  }
  async getByYear(year) {
    return this.request(`/year/${encodeURIComponent(year)}`);
  }
}

export default new Table7_8JailIndustryProductionProgressApiService();
