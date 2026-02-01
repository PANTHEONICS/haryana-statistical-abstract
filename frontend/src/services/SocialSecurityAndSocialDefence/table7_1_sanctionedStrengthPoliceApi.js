import { BaseApiService } from '../BaseApiService';

class Table7_1SanctionedStrengthPoliceApiService extends BaseApiService {
  constructor() {
    super('/v1/SSD/Table7_1/SanctionedStrengthPolice');
  }
  async getByYear(year) {
    return this.request(`/year/${year}`);
  }
}

export default new Table7_1SanctionedStrengthPoliceApiService();
