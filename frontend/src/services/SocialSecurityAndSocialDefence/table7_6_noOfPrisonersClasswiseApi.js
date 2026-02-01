import { BaseApiService } from '../BaseApiService';

class Table7_6NoOfPrisonersClasswiseApiService extends BaseApiService {
  constructor() {
    super('/v1/SSD/Table7_6/NoOfPrisonersClasswise');
  }
  async getByYear(year) {
    return this.request(`/year/${encodeURIComponent(year)}`);
  }
}

export default new Table7_6NoOfPrisonersClasswiseApiService();
